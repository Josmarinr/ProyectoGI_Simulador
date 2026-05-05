# AGENTS.md - Mesa Tangible de Redes

## Contexto del Proyecto

Este proyecto es un sistema educativo desarrollado para la materia de **Grupo de Investigación** de la universidad. El objetivo es crear una aplicación interactiva para una mesa tangible que permita a los estudiantes aprender conceptos de redes de computadores mediante la manipulación física de discos con códigos QR.

## Hardware Disponible

- Mesa interactiva con pantalla y multicámaras
- Licencia de Unity disponible
- 6-10 discos con códigos QR para pruebas

## Estado Actual

**Fase**: Pre-alpha 0.1 ✓ COMPLETADA

### Completado ✓

| Componente | Archivo | Estado |
|-----------|---------|--------|
| TagDatabase | Core/TagDatabase.cs | ✓ Listo |
| NetworkManager | Core/NetworkManager.cs | ✓ Listo |
| QRDetector | Vision/QRDetector.cs | ✓ Listo |
| DiskController | Aplicacion/DiskController.cs | ✓ Listo |
| ActividadConstruyeTopologia | Aplicacion/ActividadConstruyeTopologia.cs | ✓ Listo |
| TopologyView | UI/TopologyView.cs | ✓ Listo |

### Pendiente ☑

| # | Actividad | Prioridad | Estado |
|---|----------|----------|---------|
| 1 | Actividad "Encuentra el Fallo" | Alta | Por hacer |
| 2 | Tabla de Enrutamiento Tangible | Alta | Por hacer |
| 3 | Simulación de Mejor Ruta | Media | Por hacer |
| 4 | Enrutamiento Estático | Media | Por hacer |
| 5 | Protocolos Dinámicos (RIP/OSPF) | Baja | Por hacer |

## Tecnologías

- Unity 2022.3+ (C#)
- ZXing.Net para detección de QR
- OpenCV (opcional para tracking avanzado)

## Estructura del Código

```
Assets/Scripts/
├── Core/
│   ├── TagDatabase.cs      # Base de datos de discos
│   └── NetworkManager.cs   # Gestión de red
├── Vision/
│   └── QRDetector.cs      # Detección QR
├── Aplicacion/
│   ├── DiskController.cs
│   └── ActividadConstruyeTopologia.cs
└── UI/
    └── TopologyView.cs
```

## Convenciones

### Namespaces
- `MesaTangible.Redes` - Código core
- `MesaTangible.Vision` - Detección
- `MesaTangible.Aplicacion` - Lógica de actividades
- `MesaTangible.UI` - Interfaz

### Patrones
- Singleton para managers principales (Instancia)
- Eventos para comunicación (OnNodoAgregado, OnConexionAgregada, etc.)
- Componentes MonoBehaviour con inicialización en Awake()

### Discos Definidos
- Router (ROUTER) - rojo
- Switch (SWITCH) - azul
- PC (PC-A, PC-B, PC-C) - verde
- Enlace (ENLACE) - gris
- Red Destino (RED-1) - amarillo
- Métrica (METRICA) - cian
- Interfaz (INTERFAZ-G0, INTERFAZ-G1) - magenta
- RIP (RIP) - naranja
- OSPF (OSPF) - violeta
- Fallo (FALLO-1, FALLO-2) - negro

## Siguiente Paso

Continuar con el desarrollo de las actividades pendientes, priorizando:

1. **Actividad "Encuentra el Fallo"** - Troubleshooting simulado
2. **Tabla de Enrutamiento Tangible** - Traducir configuración física a tabla routing
3. **Simulación de Mejor Ruta** - Algoritmos shortest path

---

## Por Hacer (TODO)

### Alta Prioridad

- [ ] **Actividad "Encuentra el Fallo"** (`Aplicacion/ActividadEncuentraElFallo.cs`)
  - Generar escenarios con fallos invisibles
  - Validar solución del estudiante
  - Mostrar "ping final exitoso" al resolver

- [ ] **Tabla de Enrutamiento Tangible** (`Aplicacion/TablaEnrutamientoTangible.cs`)
  - Discos: Red destino, Métrica, Interfaz de salida
  - Mostrar tabla de routing en pantalla
  - Simular llegada de paquetes

### Media Prioridad

- [ ] **Simulación de Mejor Ruta** (`Aplicacion/SimulacionMejorRuta.cs`)
  - Múltiples rutas hacia la misma red
  - Algoritmo de lowest cost path
  - Longest prefix match

- [ ] **Enrutamiento Estático** (`Aplicacion/EnrutamientoEstatico.cs`)
  - Discos: ip route, destino, máscara, next-hop
  - Validar configuración
  - Simular tráfico

### Baja Prioridad

- [ ] **Protocolos Dinámicos** (`Aplicacion/ProtocolosDinamicos.cs`)
  - Disco RIP (conteo de saltos)
  - Disco OSPF (costo)
  - Simular convergencia

## notas

- El código usa ZXing.Net desde NuGet
- La detección QR funciona vía webcam estándar
- La posición se calcula normalizando coordenadas de pantalla
- Topología se detecta automáticamente por proximidad