using System.Collections.Generic;
using UnityEngine;
using MesaTangible.Redes;
using MesaTangible.Vision;
using System.Linq;

namespace MesaTangible.Aplicacion
{
    public enum TipoTopologia
    {
        Ninguna,
        Bus,
        Estrella,
        Anillo,
        Arbol,
        Malla,
        Mixta
    }

    [System.Serializable]
    public class TopologiaInfo
    {
        public TipoTopologia tipo;
        public int cantidadNodos;
        public bool esValida;
        public string mensaje;
    }

    public class ActividadConstruyeTopologia : MonoBehaviour
    {
        public static ActividadConstruyeTopologia Instancia { get; private set; }

        [Header("Configuración")]
        [SerializeField] private bool autoDetectar = true;
        [SerializeField] private float distanciaEnlace = 150f;

        [Header(" Referencias")]
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private QRDetector qrDetector;

        private Dictionary<string, DiskController> discosActivos;
        private TopologiaInfo topologiaActual;

        public TopologiaInfo TopologiaActual => topologiaActual;

        public event System.Action<TopologiaInfo> OnTopologiaCambiada;

        void Awake()
        {
            if (Instancia == null)
            {
                Instancia = this;
                discosActivos = new Dictionary<string, DiskController>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            if (networkManager == null)
                networkManager = NetworkManager.Instancia;
            if (qrDetector == null)
                qrDetector = QRDetector.Instancia;

            if (qrDetector != null)
            {
                qrDetector.OnDiscoDetectado += HandleDiscoDetectado;
                qrDetector.OnDiscoPerdido += HandleDiscoPerdido;
            }
        }

        private void HandleDiscoDetectado(DiscoDetectado disco)
        {
            if (TagDatabase.Instancia == null) return;

            string qrId = disco.qrId;
            if (discosActivos.ContainsKey(qrId)) return;

            if (TagDatabase.Instancia.TryGetDisco(qrId, out var discoDef))
            {
                var nuevoDisco = CrearDiskController(discoDef, disco);
                if (nuevoDisco != null)
                {
                    discosActivos.Add(qrId, nuevoDisco);
                    networkManager.AgregarNodo(qrId, disco.posicion, Quaternion.Euler(0, 0, disco.rotacion));

                    if (autoDetectar)
                    {
                        DetectarConexiones();
                    }
                }
            }
        }

        private void HandleDiscoPerdido(string qrId)
        {
            if (discosActivos.TryGetValue(qrId, out var disco))
            {
                disco.RemoverDeMesa();
                networkManager.RemoverNodo(qrId);
                discosActivos.Remove(qrId);

                if (autoDetectar)
                {
                    DetectarConexiones();
                }
            }
        }

        private DiskController CrearDiskController(DiscoDefinition discoDef, DiscoDetectado detectado)
        {
            var go = new GameObject($"Disco_{discoDef.nombreMostrar}");
            go.transform.parent = transform;

            var controller = go.AddComponent<DiskController>();
            controller.Inicializar(discoDef.qrId, discoDef.tipo, discoDef.nombreMostrar, discoDef.colorVisual);

            float x = detectado.posicion.x / Screen.width;
            float y = 1f - (detectado.posicion.y / Screen.height);

            var posicionMesa = new Vector3(
                (x - 0.5f) * 10f,
                (y - 0.5f) * 10f,
                0f
            );

            controller.ActualizarPosicion(posicionMesa, detectado.rotacion);

            return controller;
        }

        private void DetectarConexiones()
        {
            var nodos = networkManager.ObtenerNodos();
            int conexionesDetectadas = 0;

            for (int i = 0; i < nodos.Count; i++)
            {
                for (int j = i + 1; j < nodos.Count; j++)
                {
                    var nodoA = nodos[i];
                    var nodoB = nodos[j];

                    float distancia = Vector3.Distance(nodoA.posicion, nodoB.posicion);

                    if (distancia < distanciaEnlace)
                    {
                        var existente = networkManager.ObtenerConexiones()
                            .FirstOrDefault(c => 
                                (c.nodoOrigenId == nodoA.id && c.nodoDestinoId == nodoB.id) ||
                                (c.nodoOrigenId == nodoB.id && c.nodoDestinoId == nodoA.id));

                        if (existente == null)
                        {
                            networkManager.AgregarConexion(nodoA.id, nodoB.id);
                            conexionesDetectadas++;
                        }
                    }
                }
            }

            EvaluarTopologia();
        }

        private void EvaluarTopologia()
        {
            var nodos = networkManager.ObtenerNodos();
            var conexiones = networkManager.ObtenerConexiones();

            topologiaActual = new TopologiaInfo
            {
                cantidadNodos = nodos.Count,
                tipo = DeterminarTipoTopologia(nodos, conexiones)
            };

            topologiaActual.esValida = ValidarTopologia(topologiaActual, out topologiaActual.mensaje);

            OnTopologiaCambiada?.Invoke(topologiaActual);

            Debug.Log($"Topología detectada: {topologiaActual.tipo} - {topologiaActual.mensaje}");
        }

        private TipoTopologia DeterminarTipoTopologia(List<NodoRed> nodos, List<Conexion> conexiones)
        {
            if (nodos.Count == 0) return TipoTopologia.Ninguna;
            if (nodos.Count == 1) return TipoTopologia.Ninguna;

            if (conexiones.Count == 0) return TipoTopologia.Ninguna;

            int promedioGrados = conexiones.Count * 2 / nodos.Count;
            int maxGrados = nodos.Max(n => ObtenerGrado(n.id, conexiones));
            int minGrados = nodos.Min(n => ObtenerGrado(n.id, conexiones));

            bool esEstrella = maxGrados == nodos.Count - 1 && promedioGrados <= 1;
            bool esBus = maxGrados <= 2 && promedioGrados == 1 && nodos.Count > 2;
            bool esAnillo = minGrados == 2 && maxGrados == 2 && conexiones.Count == nodos.Count;
            bool esMalla = promedioGrados >= 2 && maxGrados >= 2;

            if (esMalla) return TipoTopologia.Malla;
            if (esAnillo) return TipoTopologia.Anillo;
            if (esBus) return TipoTopologia.Bus;
            if (esEstrella) return TipoTopologia.Estrella;

            return TipoTopologia.Mixta;
        }

        private int ObtenerGrado(string nodoId, List<Conexion> conexiones)
        {
            return conexiones.Count(c => c.nodoOrigenId == nodoId || c.nodoDestinoId == nodoId);
        }

        private bool ValidarTopologia(TopologiaInfo info, out string mensaje)
        {
            if (info.cantidadNodos < 2)
            {
                mensaje = "Se necesitan al menos 2 nodos";
                return false;
            }

            var nodos = networkManager.ObtenerNodos();

            foreach (var nodo in nodos)
            {
                if (!networkManager.EstaConectado(nodo.id))
                {
                    mensaje = $"Nodo {nodo.nombre} no está conectado";
                    return false;
                }
            }

            var conexiones = networkManager.ObtenerConexiones();
            if (conexiones.Count < nodos.Count - 1)
            {
                mensaje = "Faltan conexiones";
                return false;
            }

            mensaje = $"Topología válida ({info.tipo})";
            return true;
        }

        public List<DiskController> ObtenerDiscosActivos()
        {
            return new List<DiskController>(discosActivos.Values);
        }

        public void Reiniciar()
        {
            foreach (var disco in discosActivos.Values)
            {
                Destroy(disco.gameObject);
            }
            discosActivos.Clear();
            networkManager.Limpiar();
            topologiaActual = new TopologiaInfo();
        }
    }
}