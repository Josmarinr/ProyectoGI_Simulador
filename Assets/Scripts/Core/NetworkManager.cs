using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MesaTangible.Redes
{
    public enum EstadoNodo
    {
        Desconectado,
        Conectado,
        Fallo,
        AdministrativamenteDown
    }

    [System.Serializable]
    public class NodoRed
    {
        public string id;
        public string qrId;
        public DiscoTipo tipo;
        public string nombre;
        public Vector3 posicion;
        public Quaternion rotacion;
        public EstadoNodo estado;
        public string ip;
        public string mascara;
        public List<string> interfaces;
        public Dictionary<string, string> tablaRouting;

        public NodoRed(string id, string qrId, DiscoTipo tipo, string nombre)
        {
            this.id = id;
            this.qrId = qrId;
            this.tipo = tipo;
            this.nombre = nombre;
            this.estado = EstadoNodo.Conectado;
            this.tablaRouting = new Dictionary<string, string>();
            this.interfaces = new List<string>();
        }
    }

    [System.Serializable]
    public class Conexion
    {
        public string nodoOrigenId;
        public string nodoDestinoId;
        public string interfazOrigen;
        public string interfazDestino;
        public int costo;
        public bool activa;

        public Conexion(string origen, string destino, string ifaceOrigen, string ifaceDestino, int costo = 1)
        {
            this.nodoOrigenId = origen;
            this.nodoDestinoId = destino;
            this.interfazOrigen = ifaceOrigen;
            this.interfazDestino = ifaceDestino;
            this.costo = costo;
            this.activa = true;
        }
    }

    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instancia { get; private set; }

        [SerializeField]
        private Dictionary<string, NodoRed> nodos;

        [SerializeField]
        private List<Conexion> conexiones;

        [SerializeField]
        private Dictionary<string, List<string>> topologia;

        public event System.Action<NodoRed> OnNodoAgregado;
        public event System.Action<string> OnNodoRemovado;
        public event System.Action<Conexion> OnConexionAgregada;
        public event System.Action OnTopologiaActualizada;

        void Awake()
        {
            if (Instancia == null)
            {
                Instancia = this;
                nodos = new Dictionary<string, NodoRed>();
                conexiones = new List<Conexion>();
                topologia = new Dictionary<string, List<string>>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public string AgregarNodo(string qrId, Vector3 posicion, Quaternion rotacion)
        {
            if (TagDatabase.Instancia == null)
            {
                Debug.LogError("TagDatabase no encontrado");
                return null;
            }

            if (!TagDatabase.Instancia.TryGetDisco(qrId, out var disco))
            {
                Debug.LogWarning($"QR ID no reconocido: {qrId}");
                return null;
            }

            if (nodos.ContainsKey(qrId))
            {
                ActualizarPosicionNodo(qrId, posicion, rotacion);
                return qrId;
            }

            var nuevoNodo = new NodoRed(qrId, qrId, disco.tipo, disco.nombreMostrar)
            {
                posicion = posicion,
                rotacion = rotacion
            };

            if (disco.tipo == DiscoTipo.Router || disco.tipo == DiscoTipo.Switch)
            {
                nuevoNodo.interfaces = new List<string> { "G0/0", "G0/1", "G0/2", "G0/3" };
            }

            nodos.Add(qrId, nuevoNodo);
            topologia[qrId] = new List<string>();

            OnNodoAgregado?.Invoke(nuevoNodo);
            ActualizarTopologia();

            Debug.Log($"Nodo agregado: {nuevoNodo.nombre} ({qrId})");
            return qrId;
        }

        public void ActualizarPosicionNodo(string qrId, Vector3 posicion, Quaternion rotacion)
        {
            if (nodos.TryGetValue(qrId, out var nodo))
            {
                nodo.posicion = posicion;
                nodo.rotacion = rotacion;
            }
        }

        public void RemoverNodo(string qrId)
        {
            if (nodos.Remove(qrId))
            {
                conexiones.RemoveAll(c => c.nodoOrigenId == qrId || c.nodoDestinoId == qrId);

                topologia.Remove(qrId);
                foreach (var lista in topologia.Values)
                {
                    lista.Remove(qrId);
                }

                OnNodoRemovado?.Invoke(qrId);
                ActualizarTopologia();
                Debug.Log($"Nodo removido: {qrId}");
            }
        }

        public void AgregarConexion(string origen, string destino, string ifaceOrigen = "", string ifaceDestino = "", int costo = 1)
        {
            if (!nodos.ContainsKey(origen) || !nodos.ContainsKey(destino))
            {
                Debug.LogWarning("No se puede agregar conexión: nodo no encontrado");
                return;
            }

            if (ifaceOrigen == "") ifaceOrigen = "G0/0";
            if (ifaceDestino == "") ifaceDestino = ifaceOrigen;

            var conexion = new Conexion(origen, destino, ifaceOrigen, ifaceDestino, costo);
            conexiones.Add(conexion);

            if (!topologia[origen].Contains(destino))
                topologia[origen].Add(destino);
            if (!topologia[destino].Contains(origen))
                topologia[destino].Add(origen);

            OnConexionAgregada?.Invoke(conexion);
            ActualizarTopologia();

            Debug.Log($"Conexión agregada: {origen} <-> {destino}");
        }

        public void RemoverConexion(string origen, string destino)
        {
            var conexion = conexiones.FirstOrDefault(c => 
                (c.nodoOrigenId == origen && c.nodoDestinoId == destino) ||
                (c.nodoOrigenId == destino && c.nodoDestinoId == origen));

            if (conexion != null)
            {
                conexiones.Remove(conexion);
                topologia[origen].Remove(destino);
                topologia[destino].Remove(origen);
                ActualizarTopologia();
            }
        }

        private void ActualizarTopologia()
        {
            OnTopologiaActualizada?.Invoke();
        }

        public bool EstaConectado(string nodoId)
        {
            return topologia.TryGetValue(nodoId, out var vecinos) && vecinos.Count > 0;
        }

        public bool HayCamino(string origen, string destino)
        {
            if (origen == destino) return true;
            if (!topologia.ContainsKey(origen)) return false;

            var visitados = new HashSet<string>();
            var cola = new Queue<string>();
            cola.Enqueue(origen);

            while (cola.Count > 0)
            {
                var actual = cola.Dequeue();
                if (visitados.Contains(actual)) continue;
                visitados.Add(actual);

                if (topologia.TryGetValue(actual, out var vecinos))
                {
                    foreach (var vecino in vecinos)
                    {
                        if (vecino == destino) return true;
                        if (!visitados.Contains(vecino))
                            cola.Enqueue(vecino);
                    }
                }
            }

            return false;
        }

        public List<NodoRed> ObtenerNodos()
        {
            return nodos.Values.ToList();
        }

        public List<Conexion> ObtenerConexiones()
        {
            return new List<Conexion>(conexiones);
        }

        public Dictionary<string, List<string>> ObtenerTopologia()
        {
            return new Dictionary<string, List<string>>(topologia);
        }

        public NodoRed ObtenerNodo(string qrId)
        {
            return nodos.TryGetValue(qrId, out var nodo) ? nodo : null;
        }

        public void AgregarEntradaRouting(string nodoId, string destino, string siguienteSalto)
        {
            if (nodos.TryGetValue(nodoId, out var nodo))
            {
                nodo.tablaRouting[destino] = siguienteSalto;
            }
        }

        public string ObtenerSiguienteSalto(string nodoId, string destino)
        {
            if (nodos.TryGetValue(nodoId, out var nodo))
            {
                if (nodo.tablaRouting.TryGetValue(destino, out var salto))
                    return salto;
            }
            return null;
        }

        public void Limpiar()
        {
            nodos.Clear();
            conexiones.Clear();
            topologia.Clear();
            Debug.Log("NetworkManager limpiado");
        }
    }
}