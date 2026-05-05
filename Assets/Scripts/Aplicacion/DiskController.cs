using UnityEngine;
using MesaTangible.Redes;

namespace MesaTangible.Aplicacion
{
    public class DiskController : MonoBehaviour
    {
        [Header("Identificación")]
        [SerializeField] private string qrId;
        [SerializeField] private DiscoTipo tipoDisco;
        [SerializeField] private string nombreMostrar;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer iconoRenderer;
        [SerializeField] private Color colorBase = Color.white;

        [Header("Estado")]
        [SerializeField] private bool estaActivo;
        [SerializeField] private Vector3 posicionMesa;
        [SerializeField] private float rotacionZ;

        private bool estaEnMesa;
        private float tiempoUltimaActualizacion;

        public string QrId => qrId;
        public DiscoTipo TipoDisco => tipoDisco;
        public string NombreMostrar => nombreMostrar;
        public bool EstaActivo => estaActivo;
        public bool EstaEnMesa => estaEnMesa;
        public Vector3 PosicionMesa => posicionMesa;

        public event System.Action<DiskController> OnCambioEstado;
        public event System.Action<Vector3, float> OnActualizarPosicion;

        void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Inicializar(string qrId, DiscoTipo tipo, string nombre, Color color)
        {
            this.qrId = qrId;
            this.tipoDisco = tipo;
            this.nombreMostrar = nombre;
            this.colorBase = color;
            this.estaActivo = true;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }

            gameObject.name = $"Disco_{nombre}_{qrId}";
        }

        public void ActualizarPosicion(Vector3 nuevaPosicion, float rotacion)
        {
            posicionMesa = nuevaPosicion;
            transform.position = nuevaPosicion;
            rotacionZ = rotacion;
            transform.rotation = Quaternion.Euler(0, 0, rotacion);

            if (!estaEnMesa)
            {
                estaEnMesa = true;
                OnCambioEstado?.Invoke(this);
            }

            tiempoUltimaActualizacion = Time.time;
            OnActualizarPosicion?.Invoke(nuevaPosicion, rotacion);
        }

        public void RemoverDeMesa()
        {
            if (estaEnMesa)
            {
                estaEnMesa = false;
                OnCambioEstado?.Invoke(this);
            }
        }

        public void SetActivo(bool activo)
        {
            if (estaActivo != activo)
            {
                estaActivo = activo;
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = activo;
                }
                if (iconoRenderer != null)
                {
                    iconoRenderer.enabled = activo;
                }
                OnCambioEstado?.Invoke(this);
            }
        }

        public void SetColor(Color color)
        {
            colorBase = color;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }

        public void MostrarFeedback(bool correcto)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = correcto ? Color.green : Color.red;
                CancelInvoke(nameof(RestaurarColor));
                Invoke(nameof(RestaurarColor), 0.5f);
            }
        }

        private void RestaurarColor()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = colorBase;
            }
        }

        public Vector3 ObtenerPosicion()
        {
            return estaEnMesa ? posicionMesa : transform.position;
        }

        public float ObtenerRotacion()
        {
            return rotacionZ;
        }

        public bool EstaActualizado(float timeout = 1f)
        {
            return Time.time - tiempoUltimaActualizacion < timeout;
        }

        public DiscoTipo GetTipo()
        {
            return tipoDisco;
        }

        public bool EsNodo()
        {
            return tipoDisco == DiscoTipo.Router || 
                   tipoDisco == DiscoTipo.Switch || 
                   tipoDisco == DiscoTipo.PC;
        }

        public bool EsHerramienta()
        {
            return tipoDisco == DiscoTipo.RedDestino ||
                   tipoDisco == DiscoTipo.Metrica ||
                   tipoDisco == DiscoTipo.Interfaz ||
                   tipoDisco == DiscoTipo.RIP ||
                   tipoDisco == DiscoTipo.OSPF;
        }

        public bool EsFallo()
        {
            return tipoDisco == DiscoTipo.Fallo;
        }

        void OnDestroy()
        {
            RemoverDeMesa();
        }
    }
}