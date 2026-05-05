# Mesa Tangible de Redes

Sistema educativo para aprendizaje de redes de computadores mediante interfaces tangibles.

## Estado del Proyecto

**Versión:** Pre-alpha 0.1
**Última actualización:** Mayo 2026

---

## Avances (Entrega Inicial)

### Completado ✓

- [x] **TagDatabase** - Sistema de mapeo QR ID → Tipo de disco
  - 14 tipos de disco predefinidos (Router, Switch, PC, Enlace, Red Destino, Métrica, Interfaz, RIP, OSPF, Fallo)
  - extensible en runtime

- [x] **NetworkManager** - Gestión de nodos y conexiones
  - Agregar/remover nodos dinámicamente
  - Detección automática de conexiones por proximidad
  - Algoritmo de búsqueda de camino (BFS)
  - Sistema de tablas de enrutamiento básico

- [x] **QRDetector** - Detección de códigos QR vía webcam
  - usa ZXing.Net para decoding
  - Detección en tiempo real (configurable 0.1s - 1.0s)
  - Soporte para múltiples cámaras
  - Suavizado de posición (LERP)

- [x] **DiskController** - Componente de control por disco
  - Identificación de tipo y nombre
  - Tracking de posición y rotación
  - Estados activo/inactivo
  - Feedback visual

- [x] **ActividadConstruyeTopologia** - Actividad "Construye la Topología"
  - Detección automática de topología (Bus, Estrella, Anillo, Árbol, Malla, Mixta)
  - Validación de conectividad
  - Detección de conexiones por proximidad

- [x] **TopologyView** - Interfaz de visualización
  - Renderizado de nodos y conexiones
  - Indicador de estado (válido/inválido)

### Pendiente (Próximas Entregas)

- [ ] **Actividad "Encuentra el Fallo"** (Troubleshooting)
- [ ] **Tabla de Enrutamiento Tangible**
- [ ] **Simulación de Mejor Ruta**
- [ ] **Enrutamiento Estático**
- [ ] **Protocolos Dinámicos (RIP/OSPF)**

---

## Requisitos

- Unity 2022.3+ (LTS)
- .NET Framework 4.7.2+
- ZXing.Net ( paqueteNuGet)

## Instalación

1. Clonar repositorio
2. Abrir en Unity 2022.3+
3. Importar ZXing.Net:
   - Package Manager → Add package from NuGet
   - Buscar `ZXing.Net` e instalar

## Estructura

```
Assets/
└── Scripts/
    ├── Core/
    │   ├── TagDatabase.cs      # base de datos de discos
    │   └── NetworkManager.cs  # gestión de red
    ├── Vision/
    │   └── QRDetector.cs     # detección QR
    ├── Aplicacion/
    │   ├── DiskController.cs
    │   └── ActividadConstruyeTopologia.cs
    └── UI/
        └── TopologyView.cs
```

## Configuración Inicial

### 1. Escena

1. Crear nueva escena
2. Agregar GameObject "GameManager":
   - Adding TagDatabase
   - Adding NetworkManager
3. Agregar GameObject "Detector" con QRDetector
4. Agregar cámara (Orthographic, size 5)

### 2. Cámaras

En inspector de QRDetector:
- `indiceCamara`: 0 (primera cámara)
- `intervaloDeteccion`: 0.1 (10 FPS)

### 3. Discos

Discos predefinidos en TagDatabase:
| ID QR | Tipo | Color |
|-------|------|-------|
| ROUTER | Router | Rojo |
| SWITCH | Switch | Azul |
| PC-A/B/C | PC | Verde |
| ENLACE | Enlace | Gris |
| RED-1 | Red Destino | Amarillo |
| METRICA | Métrica | Cian |
| INTERFAZ-G0/G1 | Interfaz | Magenta |
| RIP | RIP | Naranja |
| OSPF | OSPF | Violeta |
| FALLO-1/2 | Fallo | Negro |

## Uso

1. Ejecutar escena en Unity
2. Webcam detecta códigos QR automáticamente
3. Mover discos sobre la mesa
4. Topología se actualiza en tiempo real
5. Ver console para debug

## Pruebas

### Ejecutar Tests en Unity

1. Window → Test Runner
2. Seleccionar pestaña "Play Mode"
3. Click "Run All" o ejecutar test específico

### Cobertura de Tests

| Suite | Casos | Descripción |
|-------|-------|-------------|
| TagDatabaseTests | 11 | Pruebas de mapeo QR → tipo disco |
| NetworkManagerTests | 20 | Pruebas de gestión de nodos y conexiones |

**Total: 31 unit tests**

### Casos de Prueba Principales

```csharp
// TagDatabase
GetTipo_QRIdValido_ReturnsRouter()
GetTipo_QRIdDesconocido_ReturnsDesconocido()
TryGetDisco_QRIdValido_ReturnsTrue()
ObtenerTodos_Returns14Discos()

// NetworkManager
AgregarNodo_QRIdValido_NodoAgregado()
HayCamino_NodosConectados_True()
HayCamino_NodosNoConectados_False()
AgregarConexion_DosNodos_ConexionCreada()
```

## API

```csharp
// Obtener tipo de disco
var tipo = TagDatabase.Instancia.GetTipo("ROUTER");

// Agregar nodo
networkManager.AgregarNodo("ROUTER", posicion, rotacion);

// Verificar conectividad
bool conectado = networkManager.HayCamino("ROUTER", "PC-A");
```

## Troubleshooting

- **Webcam no funciona**: Verificar que no esté en uso por otra app
- **QR no se detecta**: Mejorar iluminación, aumentar contraste del QR
- **Latencia alta**: Reducir `intervaloDeteccion` a 0.2s

---

## Créditos

- ZXing.Net
- EasyTangibleTable (IreshSampath)
- SurfaceFusion (Olwal et al.)