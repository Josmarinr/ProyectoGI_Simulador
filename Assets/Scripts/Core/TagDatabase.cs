using System.Collections.Generic;
using UnityEngine;

namespace MesaTangible.Redes
{
    public enum DiscoTipo
    {
        Desconocido = 0,
        Router = 1,
        Switch = 2,
        PC = 3,
        Enlace = 4,
        RedDestino = 5,
        Metrica = 6,
        Interfaz = 7,
        RIP = 8,
        OSPF = 9,
        Fallo = 10
    }

    [System.Serializable]
    public class DiscoDefinition
    {
        public string qrId;
        public DiscoTipo tipo;
        public string nombreMostrar;
        public Color colorVisual;
        public Sprite icono;

        public DiscoDefinition(string qrId, DiscoTipo tipo, string nombreMostrar, Color colorVisual)
        {
            this.qrId = qrId;
            this.tipo = tipo;
            this.nombreMostrar = nombreMostrar;
            this.colorVisual = colorVisual;
        }
    }

    public class TagDatabase : MonoBehaviour
    {
        public static TagDatabase Instancia { get; private set; }

        [SerializeField]
        private List<DiscoDefinition> discoDefiniciones;

        private Dictionary<string, DiscoDefinition> lookupTable;

        void Awake()
        {
            if (Instancia == null)
            {
                Instancia = this;
                InicializarTabla();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InicializarTabla()
        {
            lookupTable = new Dictionary<string, DiscoDefinition>();

            if (discoDefiniciones == null || discoDefiniciones.Count == 0)
            {
                CargarDiscosDefault();
            }

            foreach (var disco in discoDefiniciones)
            {
                if (!lookupTable.ContainsKey(disco.qrId))
                {
                    lookupTable.Add(disco.qrId, disco);
                }
            }
        }

        private void CargarDiscosDefault()
        {
            discoDefiniciones = new List<DiscoDefinition>
            {
                new DiscoDefinition("ROUTER", DiscoTipo.Router, "Router", Color.red),
                new DiscoDefinition("SWITCH", DiscoTipo.Switch, "Switch", Color.blue),
                new DiscoDefinition("PC-A", DiscoTipo.PC, "PC A", Color.green),
                new DiscoDefinition("PC-B", DiscoTipo.PC, "PC B", Color.green),
                new DiscoDefinition("PC-C", DiscoTipo.PC, "PC C", Color.green),
                new DiscoDefinition("ENLACE", DiscoTipo.Enlace, "Enlace", Color.gray),
                new DiscoDefinition("RED-1", DiscoTipo.RedDestino, "Red 192.168.1.0/24", Color.yellow),
                new DiscoDefinition("METRICA", DiscoTipo.Metrica, "Metrica", Color.cyan),
                new DiscoDefinition("INTERFAZ-G0", DiscoTipo.Interfaz, "G0/0", Color.magenta),
                new DiscoDefinition("INTERFAZ-G1", DiscoTipo.Interfaz, "G0/1", Color.magenta),
                new DiscoDefinition("RIP", DiscoTipo.RIP, "RIP", new Color(1f, 0.5f, 0f)),
                new DiscoDefinition("OSPF", DiscoTipo.OSPF, "OSPF", new Color(0.5f, 0f, 1f)),
                new DiscoDefinition("FALLO-1", DiscoTipo.Fallo, "Fallo Cable", Color.black),
                new DiscoDefinition("FALLO-2", DiscoTipo.Fallo, "Fallo Config", Color.black)
            };
        }

        public bool TryGetDisco(string qrId, out DiscoDefinition disco)
        {
            qrId = qrId?.ToUpperInvariant().Trim() ?? "";
            return lookupTable.TryGetValue(qrId, out disco);
        }

        public DiscoTipo GetTipo(string qrId)
        {
            if (TryGetDisco(qrId, out var disco))
            {
                return disco.tipo;
            }
            return DiscoTipo.Desconocido;
        }

        public void AgregarDisco(string qrId, DiscoTipo tipo, string nombre, Color color)
        {
            var nuevoDisco = new DiscoDefinition(qrId, tipo, nombre, color);
            if (!lookupTable.ContainsKey(qrId))
            {
                lookupTable.Add(qrId, nuevoDisco);
                discoDefiniciones.Add(nuevoDisco);
            }
        }

        public List<DiscoDefinition> ObtenerTodos()
        {
            return new List<DiscoDefinition>(discoDefiniciones);
        }
    }
}