using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.Video;
#endif

using ZXing;
using ZXing.Common;

namespace MesaTangible.Vision
{
    public struct DiscoDetectado
    {
        public string qrId;
        public Vector2 posicion;
        public float rotacion;
        public float confianza;
    }

    public class QRDetector : MonoBehaviour
    {
        public static QRDetector Instancia { get; private set; }

        [Header("Configuración de Cámara")]
        [SerializeField] private int indiceCamara = 0;
        [SerializeField] private Texture2D texturaScanner;

        [Header("Área de Escaneo")]
        [SerializeField] private Rect areaEscaneo = new Rect(0, 0, 512, 512);
        [SerializeField] private Color colorDebug = Color.green;

        [Header("Detección")]
        [SerializeField] private float intervaloDeteccion = 0.1f;
        [SerializeField] private bool usaGPU = true;

        private BarcodeReader lectorBarras;
        private Texture2D textura webcamTexture;
        private Color32[] pixeles;
        private bool escaneoActivo;
        private float tiempoUltimoEscaneo;

        public event System.Action<DiscoDetectado> OnDiscoDetectado;
        public event System.Action<string> OnDiscoPerdido;

        private List<DiscoDetectado> discosAnteriores;

        void Awake()
        {
            if (Instancia == null)
            {
                Instancia = this;
                discosAnteriores = new List<DiscoDetectado>();
                InicializarLector();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InicializarLector()
        {
            lectorBarras = new BarcodeReader();
            lectorBarras.Options = new DecodingOptions
            {
                TryHarder = true,
                PureBarcode = false,
                PossibleFormats = new[] { BarcodeFormat.QR_CODE }
            };

            #if UNITY_EDITOR || UNITY_STANDALONE
            if (SystemInfo.supportsGyroscope)
            {
                lectorBarras.Options.TryHarder = true;
            }
            #endif
        }

        void Start()
        {
            IniciarCamara();
        }

        public void IniciarCamara()
        {
            StopCamara();

            #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            webcamTexture = new Texture2D(512, 512, TextureFormat.RGB24, false);
            pixeles = new Color32[512 * 512];
            #endif

            StartCoroutine(InicializarWebcamCR());
        }

        private IEnumerator InicializarWebcamCR()
        {
            #if UNITY_2017_2_OR_NEWER
            yield return new WaitForEndOfFrame();
            #endif

            #if UNITY_EDITOR || UNITY_STANDALONE
            var dispositivos = WebCamTexture.devices;
            if (dispositivos.Length > 0)
            {
                var dispositivo = dispositivos[indiceCamara % dispositivos.Length];
                Debug.Log($"Iniciando cámara: {dispositivo.name}");

                webCamTexture = new WebCamTexture(dispositivo.name, 512, 512, 30);
                webCamTexture.Play();
                escaneoActivo = true;
            }
            else
            {
                Debug.LogError("No se encontró webcam");
            }
            #elif UNITY_ANDROID || UNITY_IOS
            escaneoActivo = true;
            #endif
        }

        public void StopCamara()
        {
            escaneoActivo = false;
            if (webCamTexture != null)
            {
                if (webCamTexture is WebCamTexture)
                {
                    ((WebCamTexture)webCamTexture).Stop();
                }
                Destroy(webCamTexture);
                webCamTexture = null;
            }
        }

        void Update()
        {
            if (!escaneoActivo) return;
            if (webCamTexture == null) return;
            if (webCamTexture.width <= 16 || webCamTexture.height <= 16) return;

            if (Time.time - tiempoUltimoEscaneo < intervaloDeteccion) return;
            tiempoUltimoEscaneo = Time.time;

            EscucharFrame();
        }

        private void EscucharFrame()
        {
            if (webCamTexture == null) return;

            try
            {
                var width = webCamTexture.width;
                var height = webCamTexture.height;

                if (pixeles == null || pixeles.Length != width * height)
                {
                    pixeles = new Color32[width * height];
                }

                webCamTexture.GetPixels32(pixeles);

                var resultado = lectorBarras.Decode(pixeles, width, height);

                if (resultado != null)
                {
                    ProcesarDeteccion(resultado);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Error en escaneo: {e.Message}");
            }
        }

        private void ProcesarDeteccion(BarcodeResult resultado)
        {
            var texto = resultado.Text;
            if (string.IsNullOrEmpty(texto)) return;

            var puntos = resultado.ResultPoints;
            if (puntos == null || puntos.Length < 4) return;

            float centroX = 0, centroY = 0;
            foreach (var punto in puntos)
            {
                centroX += punto.X;
                centroY += punto.Y;
            }
            centroX /= puntos.Length;
            centroY /= puntos.Length;

            var disco = new DiscoDetectado
            {
                qrId = texto,
                posicion = new Vector2(centroX, centroY),
                rotacion = 0f,
                confianza = 1f
            };

            var discoAnterior = discosAnteriores.FirstOrDefault(d => d.qrId == texto);
            if (discoAnterior.posicion != Vector2.zero)
            {
                disco.posicion = Vector2.Lerp(discoAnterior.posicion, disco.posicion, 0.5f);
            }

            var discoExistente = discosAnteriores.FirstOrDefault(d => d.qrId == texto);
            if (discoExistente.qrId == null)
            {
                discosAnteriores.Add(disco);
                OnDiscoDetectado?.Invoke(disco);
                Debug.Log($"Disco detectado: {texto} en posición {disco.posicion}");
            }
            else
            {
                var index = discosAnteriores.FindIndex(d => d.qrId == texto);
                discosAnteriores[index] = disco;
            }
        }

        public List<DiscoDetectado> ObtenerDiscosDetectados()
        {
            return new List<DiscoDetectado>(discosAnteriores);
        }

        public void RemoverDisco(string qrId)
        {
            var disco = discosAnteriores.FirstOrDefault(d => d.qrId == qrId);
            if (disco.qrId != null)
            {
                discosAnteriores.Remove(disco);
                OnDiscoPerdido?.Invoke(qrId);
                Debug.Log($"Disco perdido: {qrId}");
            }
        }

        public bool EstaDiscoPresente(string qrId)
        {
            return discosAnteriores.Any(d => d.qrId == qrId);
        }

        public Texture2D ObtenerTexturaCamara()
        {
            return webCamTexture;
        }

        void OnDestroy()
        {
            StopCamara();
            if (Instancia == this)
            {
                Instancia = null;
            }
        }

        #if UNITY_EDITOR
        void OnGUI()
        {
            var estilo = new GUIStyle(GUI.skin.label);
            estilo.normal.textColor = colorDebug;

            GUILayout.Label($"Discos detectados: {discosAnteriores.Count}", estilo);
            foreach (var disco in discosAnteriores)
            {
                GUILayout.Label($"  {disco.qrId}: {disco.posicion}", estilo);
            }

            if (webCamTexture != null)
            {
                GUI.DrawTexture(new Rect(10, 150, 200, 200), webCamTexture, ScaleMode.ScaleToFit);
            }
        }
        #endif
    }
}