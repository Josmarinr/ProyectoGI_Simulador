# Plan de Proyecto: Mesa Tangible de Redes

## Sistema Educativo Interactivo para Aprendizaje de Redes de Computadores mediante Interfaces Tangibles

---

**[UNIVERSIDAD]**

**[CARRERA]**

---

**Estudiante:** [NOMBRE DEL ESTUDIANTE]

**Director:** [NOMBRE DEL DIRECTOR]

**Materia:** [MATERIA - Grupo de Investigación / Grupo de Trabajo]

**Fecha:** [FECHA]

---

# 1. Introducción

La enseñanza de redes de computadores en entornos universitarios tradicionalmente se ha basado en simuladores software o equipos físicos costosos que requieren configuraciones específicas. Las mesas tangibles ofrecen una alternativa innovadora que combina el aprendizaje kinestésico con la simulación digital, permitiendo a los estudiantes manipular objetos físicos que representan componentes de red y observar su comportamiento en tiempo real.

Este proyecto propone desarrollar una aplicación interactiva para una mesa tangible existente, capaz de detectar y rastrear discos con códigos QR que representan componentes de red (routers, switches, PCs, enlaces), y simular comportamientos de enrutamiento mediante interacción física con estos objetos.

## 1.1. Antecedentes

Las interfaces tangibles de usuario (TUI) han demostrado ser efectivas en educación, especialmente en conceptos abstractos que pueden ser difíciles de visualizar. Aplicaciones similares incluyen:

- **Touchcodes** (Matsushita et al.): Utilizan RFID para detectar objetos en superficies interactivas
- **EasyTangibleTable** (IreshSampath): Framework Unity para aplicaciones tangibles mediante TUIO
- **AR Marker Tracking** (Unity ARFoundation): Detección de marcadores QR y ArUco para seguimiento de posición y rotación

Este proyecto aprovechará estas tecnologías para crear un sistema tangible especializado en educación de redes.

# 2. Objetivos

## 2.1. Objetivo General

Desarrollar un prototipo de aplicación interactiva para mesa tangible que permita a los estudiantes aprender conceptos de redes de computadores mediante la manipulación física de discos que representan componentes de red, visualizando en tiempo real el comportamiento de la topología, tablas de enrutamiento y protocolos de enrutamiento.

## 2.2. Objetivos Específicos

### Grupo de Trabajo

1. Implementar sistema de detección de discos mediante códigos QR utilizando las cámaras de la mesa tangible
2. Desarrollar algoritmo de rastreo de posición (X,Y) y rotación de cada disco en la superficie
3. Crear sistema de generación automática de topología de red basada en la posición de los discos
4. Implementar actividad "Construye la Topología": construir topologías físicas (bus, árbol, malla, estrella)
5. Implementar actividad "Encuentra el Fallo": diagnosticar y resolver problemas de conectividad simulados

### Grupo de Investigación

6. Desarrollar sistema de "Tabla de Enrutamiento Tangible" que traduzca configuración física a entradas de routing table
7. Implementar simulación de selección de mejor ruta mediante métricas y longest prefix match
8. Crear simulación de enrutamiento estático con validación de configuración
9. Implementar simulación de protocolos dinámicos (RIP, OSPF) con visualización de convergencia

### Propuesta de Grado

10. Documentar la arquitectura del sistema y las estrategias de usabilidad empleadas
11. Plantear anteproyecto para estudio de usabilidad en entornos educativos

---

# 3. Justificación

## 3.1. Relevancia Académica

- **Aprendizaje activo**: Los estudiantes no solo observan, sino que manipulan directamente la simulación
- **Retroalimentación inmediata**: El sistema muestra resultados instantáneos al cambiar configuraciones
- **Reducción de costos**: Elimina necesidad de equipamiento físico de redes (routers, switches) para prácticas básicas
- **Portabilidad**: Un solo sistema puede ser utilizado por múltiples estudiantes simultáneamente
- **Escalabilidad**: Nuevas actividades pueden agregarse mediante software

## 3.2. Viabilidad Técnica

- **Hardware disponible**: Mesa con pantalla y multicámaras ya existente
- **Licencia Unity**: Disponible para desarrollo
- **Discos existentes**: 6-10 discos con códigos QR para pruebas iniciales
- **Tecnologías probadas**: AR Foundation, OpenCV, EasyTangibleTable son soluciones maduras

---

# 4. Alcance del Proyecto

## 4.1. Entregables por Fase

| Fase | Entregable | Descripción |
|------|------------|-------------|
| **GT-1** | Sistema de detección de discos | Lectura de códigos QR e identificación de tipo de componente |
| **GT-2** | Sistema de rastreo de posición | Seguimiento XY y rotación en tiempo real |
| **GT-3** | Visualizador de topología | Renderizado 2D/3D de la red basada en posición de discos |
| **GT-4** | Actividad "Construye la Topología" | Validación de construcción de topologías físicas |
| **GT-5** | Actividad "Encuentra el Fallo" | Sistema de troubleshooting simulado |
| **GI-1** | Tabla de Enrutamiento Tangible | Traducción de configuración física a tabla de routing |
| **GI-2** | Simulador de Mejor Ruta | Visualización de selección de ruta por métrica/prefijo |
| **GI-3** | Enrutamiento Estático | Simulación de rutas estáticas con validación |
| **GI-4** | Protocolos Dinámicos | Simulación de RIP y OSPF |
| **PG** | Documentación | Arquitectura y propuesta de anteproyecto |

## 4.2. Tipos de Discos a Utilizar

| ID QR | Disco | Función |
|-------|-------|----------|
| 1 | Router | Nodo de enrutamiento |
| 2 | Switch | Nodo de conmutación |
| 3-5 | PC-A, PC-B, PC-C | Hosts finales |
| 6 | Enlace | Conexión entre nodos |
| 7 | Red Destino | Prefijo de red para routing |
| 8 | Métrica | Costo o hop count |
| 9-10 | Interfaz | Interfaces de red (G0/0, G0/1) |
| 11 | RIP | Protocolo de enrutamiento RIP |
| 12 | OSPF | Protocolo de enrutamiento OSPF |
| 13-14 | Escenarios de fallo | Discos para Troubleshooting |

---

# 5. Metodología

## 5.1. Enfoque Metodológico

Metodología de desarrollo ágil adaptada para investigación académica:

1. **Investigación** (Semanas 1-2): Revisión de tecnologías detección QR, tracking, frameworks Unity para TUI
2. **Diseño** (Semana 3): Arquitectura del sistema, diseño de clases, definición de estados
3. **Desarrollo iterativo** (Semanas 4-12): Ciclos de desarrollo con pruebas funcionales
4. **Documentación** (Semanas 13-16): Redacción técnica y presentación de resultados

## 5.2. Tecnologías a Utilizar

| Componente | Tecnología | Justificación |
|-----------|-------------|----------------|
| Desarrollo principal | Unity 2022+ (C#) | Licencia disponible, robustez |
| Detección de markers | Unity AR Foundation / QR Detection asset | Soporte nativo para QR codes |
| Tracking de posición | OpenCV via Unity o ARFoundation | Precisión sub-centimétrica |
| Interfaz de usuario | Unity UI / uGUI | Integración nativa |
| Base de datos | JSON local | Simple, sin servidor |

## 5.3. Arquitectura del Sistema

```
+------------------------------------------------------------------+
|                         CAPA DE VISION                             |
|  [Cámaras de la mesa] -> [AR Marker Manager]                       |
|  - Detección de códigos QR                                       |
|  - Cálculo de posición (X,Y) y rotación                        |
+----------------------------+-----------------------------------+
                             v
+------------------------------------------------------------------+
|                         CAPA DE DATOS                            |
|  [TagDatabase]  <-  [NetworkManager]                           |
|  - Mapeo QR ID -> Tipo de disco                                |
|  - Estado de cada nodo (up/down, IP, máscara)                 |
|  - Conexiones entre nodos                                       |
+----------------------------+-----------------------------------+
                             v
+------------------------------------------------------------------+
|                      CAPA DE APLICACIÓN                         |
|  [TopologyView] [RoutingTableView] [ProtocolView]           |
|  - Visualización de topología en tiempo real                  |
|  - Simulación de tablas de enrutamiento                      |
|  - Ejecución de protocolos dinámicos                         |
+------------------------------------------------------------------+
```

---

# 6. Cronograma

## 6.1. Cronograma General (16 Semanas)

| Semanas | Fase | Entregables |
|--------|------|-------------|
| 1-2 | GT-Inicio | Proyecto Unity configurado, Cámaras funcionando, Detección QR básica |
| 3-4 | GT-Desarrollo | Tracking de posición (X,Y), Base de datos de discos |
| 5-6 | GT-Avanzado | "Construye la Topología" activo, Validación de conectividad |
| 7-8 | GI-Inicio | "Tabla de Enrutamiento Tangible" |
| 9-10 | GI-Desarrollo | "Mejor Ruta" + "Enrutamiento Estático" |
| 11-12 | GI-Avanzado | Protocolos dinámicos (RIP/OSPF) |
| 13-14 | PG-Doc | Documentación, Propuesta de anteproyecto |
| 15-16 | PG-Final | Revisión, Presentación |

## 6.2. Distribución de Horas Estimadas

| Actividad | Horas Estimadas |
|-----------|------------------|
| Investigación y documentación | 40 |
| Desarrollo de software | 80 |
| Pruebas y validación | 30 |
| Redacción de documentación | 30 |
| Presentación | 10 |
| **Total** | **190 horas** |

---

# 7. Resultados Esperados

## 7.1. Resultados Inmediatos

- Aplicación Unity funcional capaz de ejecutarse en la mesa tangible
- Sistema de detección y tracking de 6+ discos simultáneamente
- Mínimo 2 actividades educativas funcionando ("Construye la Topología", "Encuentra el Fallo")
- Simulador básico de tablas de enrutamiento

## 7.2. Resultados a Largo Plazo

- Plataforma expandible para nuevas actividades educativas
- Documentación técnica que permita replicación del sistema
- Datos para estudio de usabilidad en educación de redes
- Publicación académica potencial

---

# 8. Recursos Requeridos

## 8.1. Recursos Disponibles

| Recurso | Disponibilidad | Costo |
|--------|---------------|-------|
| Mesa interactiva | Disponible (propia) | $0 |
| Cámaras integradas | Disponible (mesa) | $0 |
| Licencia Unity | Disponible | $0 |
| Discos con QR | Disponible (6-10 uds) | $0 |
| Monitor/pantalla | Disponible (mesa) | $0 |

## 8.2. Recursos Adicionales Estimados

| Recurso | Justificación | Costo Estimado |
|--------|--------------|---------------|
| Discos adicionales | Para actividades GI | $20-30 |
| Impresión de códigos QR | Refuerzos | $10 |
| Materiales de construcción | Acrílico/madera | $20 |
| **Total estimado** | | **$50-60** |

---

# 9. Limitaciones y Riesgos

| Riesgo | Probabilidad | Impacto | Mitigación |
|-------|-------------|--------|------------|
| Detección QR con iluminación variable | Media | Alto | Calibrar cámara, usar iluminación constante |
| Latencia en tracking | Baja | Medio | Optimizar pipeline, reducir resolución si es necesario |
| Limitaciones de las cámaras de la mesa | Media | Alto | Pruebas tempranas, adaptación |
| Compatibilidad con SDK de la mesa | Baja | Alto | Usar cámara como webcam estándar |

---

# 10. Referencias

1. IreshSampath. (2026). unity-assets-easy-tangible-table. GitHub.
2. Unity Technologies. (2024). AR Marker Manager - AR Foundation Documentation.
3. normanderwan. (2024). ArucoUnity Documentation.
4. Olwal, A., et al. (2008). SurfaceFusion: Unobtrusive Tracking of Idle Objects on Interactive Surfaces. IEEE.
5. Automatico. (2015). Real-time object detection and tracking with Raspberry Pi. GitHub.
6. NilsSchnorr. (2025). HapticCollectionMediaPlayer. GitHub.

---

# 11. Aprobaciones

| Rol | Nombre | Firma | Fecha |
|-----|--------|-------|-------|
| Estudiante | | | |
| Director | | | |

---

*Documento generado para fines de aprobación del proyecto de [MATERIA]*

**Versión:** 1.0

**Fecha de elaboración:** [FECHA]