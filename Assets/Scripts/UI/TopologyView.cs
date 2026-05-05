using UnityEngine;
using UnityEngine.UI;
using MesaTangible.Redes;
using MesaTangible.Aplicacion;
using System.Collections.Generic;

namespace MesaTangible.UI
{
    public class TopologyView : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private RectTransform panelNodos;
        [SerializeField] private RectTransform panelConexiones;
        [SerializeField] private Text textoEstado;

        [Header("Prefabs")]
        [SerializeField] private GameObject prefabNodoVisual;
        [SerializeField] private GameObject prefabLinea;

        [Header("Configuración")]
        [SerializeField] private float escalaVisualizacion = 50f;
        [SerializeField] private Color colorNodoActivo = Color.green;
        [SerializeField] private Color colorNodoInactivo = Color.red;
        [SerializeField] private Color colorLinea = Color.white;

        private Dictionary<string, GameObject> nodosVisuales;
        private List<GameObject> lineasVisuales;

        void Awake()
        {
            nodosVisuales = new Dictionary<string, GameObject>();
            lineasVisuales = new List<GameObject>();
        }

        void Start()
        {
            if (ActividadConstruyeTopologia.Instancia != null)
            {
                ActividadConstruyeTopologia.Instancia.OnTopologiaCambiada += HandleTopologiaCambiada;
            }

            if (NetworkManager.Instancia != null)
            {
                NetworkManager.Instancia.OnNodoAgregado += HandleNodoAgregado;
                NetworkManager.Instancia.OnNodoRemovado += HandleNodoRemovado;
                NetworkManager.Instancia.OnConexionAgregada += HandleConexionAgregada;
            }
        }

        private void HandleTopologiaCambiada(TopologiaInfo info)
        {
            if (textoEstado != null)
            {
                string estado = info.esValida ? "✓ " : "✗ ";
                textoEstado.text = $"{estado}{info.mensaje}\nTipo: {info.tipo}";
                textoEstado.color = info.esValida ? colorNodoActivo : colorNodoInactivo;
            }
        }

        private void HandleNodoAgregado(NodoRed nodo)
        {
            AgregarNodoVisual(nodo);
            ActualizarConexionesVisuales();
        }

        private void HandleNodoRemovado(string qrId)
        {
            if (nodosVisuales.TryGetValue(qrId, out var go))
            {
                Destroy(go);
                nodosVisuales.Remove(qrId);
            }
            ActualizarConexionesVisuales();
        }

        private void HandleConexionAgregada(Conexion conexion)
        {
            ActualizarConexionesVisuales();
        }

        private void AgregarNodoVisual(NodoRed nodo)
        {
            if (prefabNodoVisual == null) return;

            var go = Instantiate(prefabNodoVisual, panelNodos);
            go.name = $"Nodo_{nodo.id}";

            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(nodo.posicion.x, nodo.posicion.y) * escalaVisualizacion;

            var texto = go.GetComponentInChildren<Text>();
            if (texto != null)
            {
                texto.text = nodo.nombre;
            }

            nodosVisuales.Add(nodo.id, go);
        }

        private void ActualizarConexionesVisuales()
        {
            foreach (var linea in lineasVisuales)
            {
                Destroy(linea);
            }
            lineasVisuales.Clear();

            if (NetworkManager.Instancia == null) return;

            var conexiones = NetworkManager.Instancia.ObtenerConexiones();
            foreach (var conexion in conexiones)
            {
                if (nodosVisuales.TryGetValue(conexion.nodoOrigenId, out var nodoOrigen) &&
                    nodosVisuales.TryGetValue(conexion.nodoDestinoId, out var nodoDestino))
                {
                    CrearLineaVisual(nodoOrigen, nodoDestino);
                }
            }
        }

        private void CrearLineaVisual(GameObject origen, GameObject destino)
        {
            if (prefabLinea == null) return;

            var lineaGo = Instantiate(prefabLinea, panelConexiones);
            var rt = lineaGo.GetComponent<RectTransform>();
            var origenRt = origen.GetComponent<RectTransform>();
            var destinoRt = destino.GetComponent<RectTransform>();

            Vector2 puntoMedio = (origenRt.anchoredPosition + destinoRt.anchoredPosition) / 2;
            rt.anchoredPosition = puntoMedio;

            float longitud = Vector2.Distance(origenRt.anchoredPosition, destinoRt.anchoredPosition);
            rt.sizeDelta = new Vector2(longitud, rt.sizeDelta.y);

            float angulo = Mathf.Atan2(
                destinoRt.anchoredPosition.y - origenRt.anchoredPosition.y,
                destinoRt.anchoredPosition.x - origenRt.anchoredPosition.x
            ) * Mathf.Rad2Deg;
            rt.localRotation = Quaternion.Euler(0, 0, angulo - 90);

            lineasVisuales.Add(lineaGo);
        }

        public void Mostrar(bool mostrar)
        {
            gameObject.SetActive(mostrar);
        }

        public void Limpiar()
        {
            foreach (var go in nodosVisuales.Values)
            {
                Destroy(go);
            }
            nodosVisuales.Clear();

            foreach (var go in lineasVisuales)
            {
                Destroy(go);
            }
            lineasVisuales.Clear();
        }
    }
}