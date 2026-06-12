# MANUAL DE USUARIO — Sistema SePrise Circuito A

## 📖 Tabla de Contenidos

1. [Introducción](#1-introducción)
2. [Requisitos de Sistema](#2-requisitos-de-sistema)
3. [Instalación](#3-instalación)
4. [Primer Inicio](#4-primer-inicio)
5. [Interfaz Principal](#5-interfaz-principal)
6. [Módulos](#6-módulos)
   - [6.1 Gestión de Pacientes](#61-gestión-de-pacientes)
   - [6.2 Gestión de Turnos](#62-gestión-de-turnos)
   - [6.3 Acreditación de Pacientes](#63-acreditación-de-pacientes)
   - [6.4 Gestión de Atenciones](#64-gestión-de-atenciones)
7. [Casos de Uso Completos](#7-casos-de-uso-completos)
8. [Atajos y Tips](#8-atajos-y-tips)
9. [Troubleshooting](#9-troubleshooting)
10. [FAQ](#10-faq)
11. [Contacto y Soporte](#11-contacto-y-soporte)

---

## 1. Introducción

### ¿Qué es SePrise?

SePrise es un sistema de información diseñado para gestionar de manera integral el flujo de pacientes en consultorios externos de clínicas y centros médicos.

**Funcionalidades principales**:
- 📋 Gestión de pacientes (datos personales, contacto)
- 📅 Agendamiento de turnos (especialidades, médicos, horarios)
- ✅ Acreditación de pacientes (validación en recepción)
- 👨‍⚕️ Gestión de atenciones (flujo médico completo)
- 📊 Reportes y exportaciones (estadísticas)

### Módulos y Roles

| Módulo | Usuario Típico | Función |
|--------|---|---|
| **Pacientes** | Administrador, Recepcionista | Crear/actualizar datos de pacientes |
| **Turnos** | Recepcionista, Administrador | Agendar, confirmar, cancelar turnos |
| **Acreditación** | Recepcionista | Validar pacientes al llegar |
| **Atenciones** | Médico, Profesional | Registrar y finalizar atenciones |
| **Reportes** | Administrador | Consultar estadísticas y exportar |

---

## 2. Requisitos de Sistema

### Hardware Mínimo

| Componente | Especificación |
|-----------|---|
| Procesador | Intel/AMD dual-core 2.0 GHz+ |
| Memoria RAM | 4 GB mínimo (8 GB recomendado) |
| Espacio en disco | 2 GB libres mínimo |
| Monitor | Resolución 1024x768 mínimo |
| Red | Conexión TCP/IP local (LAN) |

### Software Requerido

| Software | Versión | Nota |
|----------|---------|------|
| Windows OS | Windows 7 SP1+ o Windows 10/11 | 64-bit recomendado |
| .NET Runtime | 8.0 o superior | Incluido en el sistema |
| SQL Server | 2019+ o LocalDB | Administrador configura |
| Navegador | Chrome/Edge/Firefox | Para acceder a reportes web (opcional) |

### Conexión a Servidor

- **IP del servidor API**: Configurado por administrador.
- **Puerto**: Típicamente 5293 (local) o asignado por red.
- **Conexión**: TCP/IP (intranet).

---

## 3. Instalación

### Paso 1: Obtener la Aplicación
El administrador de sistemas proveerá la carpeta o el instalador del sistema **SePrise**.

### Paso 2: Ejecutar el Sistema
1. Navegue a la carpeta de la aplicación `SePrise.WinForms`.
2. Haga doble clic en el archivo ejecutable `SePrise.WinForms.exe`.

### Paso 3: Configuración Inicial
La primera vez que abra la aplicación, verifique que la API central esté funcionando. Si es un entorno de red local, el administrador le proveerá la IP. Si es entorno de desarrollo, el servidor corre automáticamente en `localhost:5293`.

---

## 4. Primer Inicio

### Pantalla de Login

```text
┌─────────────────────────────────┐
│   SePrise — Inicio de Sesión    │
├─────────────────────────────────┤
│                                 │
│  Ingrese su DNI:                │
│  ┌──────────────────────────┐   │
│  │                          │   │
│  └──────────────────────────┘   │
│                                 │
│  [Ingresar]                     │
│                                 │
└─────────────────────────────────┘
```

**Acciones**:
1. **Ingresar DNI**: Número de 8 dígitos (sin guiones ni puntos).
2. **Click "Ingresar"**:
   - Si el DNI está registrado, accederá al sistema principal.
   - Si el sistema no conecta, contacte al soporte técnico.

---

## 5. Interfaz Principal

Una vez iniciada la sesión, verá la pantalla central:

```text
┌───────────────────────────────────────────────────────────┐
│   SePrise — Sistema de Gestión de Consultorios Externos   │
├───────────────────────────────────────────────────────────┤
│                                                           │
│  [Pacientes]  [Turnos]  [Acreditación]  [Atenciones]      │
│                                                           │
│  ┌──────────────────────────────────────────────────────┐ │
│  │                                                      │ │
│  │   [Contenido del módulo seleccionado]                │ │
│  │                                                      │ │
│  └──────────────────────────────────────────────────────┘ │
│                                                           │
│  Status bar: Conectado a SePrise API                      │
└───────────────────────────────────────────────────────────┘
```

### Elementos de la Pantalla
| Elemento | Descripción |
|---|---|
| **Botones Superiores** | Los 4 módulos principales del sistema. |
| **Área Central** | Muestra la información y formularios del módulo activo. |
| **Status Bar** | Estado de conexión en la parte inferior. |

### Navegación
Para cambiar de módulo simplemente haga click en el botón correspondiente. La información no se perderá al navegar entre pestañas.

---

## 6. Módulos

### 6.1 Gestión de Pacientes

**¿Cuándo usar?**: Crear nuevo paciente, actualizar datos o buscar historial.

```text
┌──────────────────────────────────────────────────┐
│  GESTIÓN DE PACIENTES                            │
├──────────────────────────────────────────────────┤
│  [➕ Nuevo Paciente]                            │
│  ┌──────────────────────────────────────────────┐│
│  │ DNI │ Nombre │ Apellido │ Email │ Teléfono   ││
│  ├──────────────────────────────────────────────┤│
│  │ 123 │ Juan   │ Pérez    │ j@... │ 555...     ││
│  │ 456 │ María  │ García   │ m@... │ 666...     ││
│  └──────────────────────────────────────────────┘│
└──────────────────────────────────────────────────┘
```

#### Crear Nuevo Paciente
1. Click en **"➕ Nuevo Paciente"**.
2. Complete el formulario emergente (DNI, Nombre, Email, etc.).
3. **Validaciones**: Los campos obligatorios se marcarán en rojo si faltan. El sistema verificará que el email tenga el formato correcto y que el DNI no exista previamente.
4. Click en **"Guardar"**.

#### Editar Paciente
1. Seleccione el paciente en la grilla.
2. Modifique sus datos.
3. Click en **Guardar Cambios**. El DNI no puede modificarse.

---

### 6.2 Gestión de Turnos

**¿Cuándo usar?**: Agendar un turno para un paciente, reprogramarlo o cancelarlo.

#### Agendar Nuevo Turno
1. Click en **"Crear Turno"**.
2. **Paciente**: Seleccione al paciente de la lista.
3. **Especialidad**: Elija (ej: Cardiología).
4. **Médico**: La lista se filtrará automáticamente según la especialidad elegida.
5. **Sala**: Elija el consultorio.
6. **Fecha y Hora**: Seleccione cuándo será la cita.
7. Click en **"Guardar"**. El turno nacerá en estado **Reservado**.

#### Cancelar Turno
Si el paciente llama para cancelar:
1. Seleccione el turno en la grilla.
2. Click en **"Cancelar Turno"**.

---

### 6.3 Acreditación de Pacientes

**¿Cuándo usar?**: Cuando el paciente llega presencialmente a la recepción de la clínica.

```text
┌──────────────────────────────────────────────────┐
│  ACREDITACIÓN DE PACIENTE (Recepción)            │
├──────────────────────────────────────────────────┤
│  Turnos de Hoy (Pendientes de Acreditación):     │
│  [ Dropdown con los turnos "Reservados" ]        │
│                                                  │
│  Modalidad de Pago: [ Obra Social / Particular ] │
│  [Acreditar Paciente]                            │
└──────────────────────────────────────────────────┘
```

#### Acreditar por Turno Previo
1. Despliegue la lista de turnos. **Solo aparecerán los turnos en estado Reservado** (pacientes que aún no han llegado).
2. Seleccione el turno del paciente que acaba de llegar.
3. Seleccione la Modalidad de Pago.
4. Click en **"Acreditar Paciente"**.
5. El turno pasará a **Confirmado** y el paciente desaparecerá de esta lista para aparecer en la lista de espera del Médico.

---

### 6.4 Gestión de Atenciones

**¿Cuándo usar?**: Es la pantalla exclusiva para el Médico dentro del consultorio.

```text
┌──────────────────────────────────────────────────┐
│  GESTIÓN DE ATENCIONES (Consultorio)             │
├──────────────────────────────────────────────────┤
│  Pacientes en Sala de Espera:                    │
│  [ Tabla con los pacientes "Acreditados" ]       │
│                                                  │
│  [Iniciar Atención]  [Finalizar Atención]        │
└──────────────────────────────────────────────────┘
```

#### Flujo del Médico
1. **Ver Sala de Espera**: La tabla mostrará todos los pacientes que ya fueron acreditados en recepción.
2. **Iniciar Atención**: Seleccione al paciente y haga click en "Iniciar Atención". El estado cambiará a **EnProgreso**.
3. **Atender y Finalizar**: Una vez terminada la consulta, el médico hará click en "Finalizar Atención". Esto liberará al paciente y cerrará el ciclo completo.

---

## 7. Casos de Uso Completos

### Caso 1: Nuevo Paciente → Turno → Atención (Flujo Completo)

1. **Recepción (Pacientes)**: La recepcionista registra a Roberto (DNI 99887766).
2. **Recepción (Turnos)**: Le agenda un turno con el Dr. García en Cardiología para mañana a las 14:00hs. El turno queda "Reservado".
3. **Recepción (Acreditación)**: Al día siguiente, Roberto llega. La recepcionista lo busca en la pestaña Acreditación y presiona "Acreditar". Roberto pasa a la Sala de Espera virtual.
4. **Consultorio (Atenciones)**: El Dr. García ve a Roberto en su pantalla. Presiona "Iniciar", lo atiende, y luego presiona "Finalizar". El sistema cierra el turno.

---

## 8. Atajos y Tips

### Tips de Eficiencia
1. **Autocompletado**: Al elegir la Especialidad en el formulario de Turnos, el sistema cargará automáticamente solo a los médicos que atiendan esa especialidad.
2. **Validaciones Tempranas**: No necesita preocuparse por escribir mal un correo; el sistema lo bloqueará antes de guardarlo en la base de datos.
3. **Filtros Automáticos**: La pantalla de Recepción (Acreditación) solo le mostrará la lista de personas que *deben* llegar, ocultando a los que ya llegaron o cancelaron.

---

## 9. Troubleshooting

### Problema: "No puedo conectar a la aplicación"
**Soluciones**:
1. Verifique con IT que la API de SePrise (backend) esté encendida.
2. Si está en desarrollo local, asegúrese de haber corrido `dotnet run` en el proyecto API antes de abrir WinForms.

### Problema: "Médico o Sala ocupada en ese horario"
**Causa**: Está intentando agendar un turno que se superpone (solapa) con otro turno ya existente para ese médico o esa sala.
**Solución**: Elija un horario 30 minutos más tarde o temprano, o cambie de profesional.

### Problema: "El paciente llegó pero no aparece en Acreditación"
**Causa**: Seguramente ya fue acreditado previamente, o el turno se agendó para un día/hora distinto.
**Solución**: Vaya a la pestaña "Turnos" y busque su nombre en la tabla general para verificar el estado de su cita.

---

## 10. FAQ

### P: ¿Puedo eliminar un paciente definitivamente?
**R**: El sistema usa un borrado lógico ("soft delete"). Al presionar eliminar, el paciente se marca como inactivo para que sus turnos médicos históricos no se pierdan (auditoría), pero ya no aparecerá en las búsquedas activas.

### P: ¿Qué pasa si acredito un paciente dos veces?
**R**: Es imposible. Una vez acreditado, el paciente desaparece de la lista de pendientes en la recepción y pasa a la vista del médico. 

### P: ¿Puedo atender un paciente sin acreditarlo en recepción?
**R**: No. El flujo de negocio estricto exige que el paciente valide su pago y asistencia en la recepción antes de aparecerle en la pantalla al doctor.

---

## 11. Contacto y Soporte

### Para Problemas Técnicos
**Administrador de Sistema (Soporte Nivel 1)**:
- Email: soporte-interno@seprise.com
- Horario: Lunes a Viernes, 8am - 6pm

### Reporte de Errores Críticos (Bugs)
Si la aplicación se cierra inesperadamente:
1. Anote qué pantalla estaba usando.
2. Contacte a IT para que revisen los *Logs* del servidor.

---

**Manual de Usuario — SePrise Circuito A v1.0**  
**Última actualización**: Junio 2026  
**Versión**: 1.0.0 Production Ready

