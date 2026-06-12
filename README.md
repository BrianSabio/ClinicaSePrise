# SePrise Circuito A — Sistema de Gestión de Consultorios Externos

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![Coverage](https://img.shields.io/badge/coverage-69.7%25-yellow)
![License](https://img.shields.io/badge/license-MIT-blue)
![Version](https://img.shields.io/badge/version-1.0.0-blue)

## 🎯 Tabla de Contenidos

- [Características](#características)
- [Requisitos Previos](#requisitos-previos)
- [Instalación](#instalación)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Uso](#uso)
- [Arquitectura](#arquitectura)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Troubleshooting](#troubleshooting)
- [Roadmap](#roadmap)
- [Licencia](#licencia)

## 📖 Descripción General

SePrise es un sistema de información moderno diseñado para digitalizar y orquestar el flujo integral de pacientes en consultorios externos. Está pensado para recepcionistas, administradores y personal médico que requieran agilidad en la acreditación y registro clínico, reduciendo drásticamente errores humanos mediante validaciones rígidas y transacciones atómicas.

Esta versión se enfoca exclusivamente en el **Circuito A** (Consultorios Externos). Cubre desde el alta de pacientes y la agenda de turnos, hasta la acreditación presencial y la finalización de la atención médica en consultorio. Excluye funcionalidades propias de internación, facturación avanzada y el **Circuito B** (Estudios Clínicos), los cuales forman parte del roadmap futuro.

## ✨ Características

### Backend API REST
- ✅ **24 endpoints REST** fully functional
- ✅ **Architecture**: Clean Architecture + Domain-Driven Design (DDD)
- ✅ **Database**: SQL Server / Entity Framework Core 8
- ✅ **Testing**: 41 integration tests (69.7% coverage)
- ✅ **Documentation**: Swagger/OpenAPI interactivas
- ✅ **State Machines**: Transiciones de estado estrictas para Turno y Atención

### Frontend Windows Forms
- ✅ Login rápido por DNI
- ✅ CRUD Pacientes (Create, Read, Update, Delete lógico)
- ✅ Gestión Turnos (agendar, confirmar, cancelar, reprogramar)
- ✅ Acreditación de Pacientes (recepción y derivación a sala)
- ✅ Gestión de Atenciones (exclusivo médico/consultorio)
- ✅ Responsive UI con validación visual inmediata
- ✅ Llamadas HTTP asíncronas para evitar bloqueos de interfaz

### Características Técnicas
- ✅ **Cascadas transaccionales**: Operaciones atómicas en un solo commit (ej: Confirmar turno + Iniciar atención).
- ✅ Validación en múltiples niveles (UI, FluentValidation, Entidades).
- ✅ Manejo de excepciones graceful (Exception Middleware en la API).
- ✅ Dependency Injection (IoC container nativo de .NET).
- ✅ AutoMapper para mapeo de DTOs.

## 🛠️ Requisitos Previos

### Software

| Componente | Versión | Enlace |
|-----------|---------|--------|
| .NET SDK | 8.0+ | [descargar](https://dotnet.microsoft.com/download) |
| SQL Server | 2019+ o LocalDB | [descargar](https://www.microsoft.com/sql-server/sql-server-downloads) |
| Visual Studio | 2022+ (opcional) | [descargar](https://visualstudio.microsoft.com/) |
| Git | Último | [descargar](https://git-scm.com/) |

### Hardware Mínimo

- CPU: Dual-core 2.0 GHz
- RAM: 4 GB
- Disco: 2 GB libres

## 📦 Instalación

### 1. Clonar repositorio

```bash
git clone https://github.com/tu-usuario/SePrise.git
cd SePrise
```

### 2. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 3. Configurar base de datos

La API viene configurada por defecto para usar LocalDB, lo cual requiere **cero configuración manual**. Si deseas usar un SQL Server dedicado, edita el archivo `src/SePrise.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SePrise_Dev;Trusted_Connection=True;Encrypt=False;"
  }
}
```

### 4. Compilar la Solución y Generar Base de Datos

El sistema está configurado para **crear y sembrar** la base de datos automáticamente en su primer arranque (`EnsureCreated`). Solo compila:

```bash
dotnet build
```

Output esperado:
```
Build succeeded. (0 errors, 0 warnings)
```

## 🚀 Uso

### Iniciar API REST (Backend)

```bash
cd src/SePrise.API
dotnet run
```

Output esperado:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5293
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**Verificar API funciona**: `http://localhost:5293/swagger`

### Iniciar Frontend Windows Forms

En otra terminal:

```bash
cd src/SePrise.WinForms
dotnet run
```

Se abrirá el **LoginForm**. Ingresa el DNI **12345678** (este es un "bypass" programado exclusivamente para pruebas que te permite ingresar al sistema como Administrador sin necesidad de configurar pacientes previamente en la base de datos).

### Flujo de Ejemplo Rápido

#### 1. **Crear Paciente** (Tab Pacientes)
Ingresa un DNI de 8 dígitos, nombre, apellido y un email válido. Haz click en Guardar.

#### 2. **Agendar Turno** (Tab Turnos)
Selecciona al paciente creado, elige Cardiología y al Dr. García. Elige fecha futura. Al guardar, el turno nacerá en estado **Reservado**.

#### 3. **Acreditar Paciente** (Tab Acreditación - Recepción)
Busca el turno de Juan Pérez. Haz click en Acreditar. El turno pasará automáticamente a **Confirmado** y aparecerá en la bandeja del médico en la sala de espera (**Acreditada**).

#### 4. **Atender Paciente** (Tab Atención - Consultorio)
Filtra por pacientes en estado "Acreditada". Selecciona a Juan Pérez, haz click en Iniciar (pasa a **EnProgreso**), redacta las notas médicas y haz click en Finalizar (pasa a **Finalizada** y el turno a **Atendido**).

## 📂 Estructura del Proyecto

```text
SePrise/
├── src/
│   ├── SePrise.Domain/               (Dominio puro - DDD)
│   │   ├── Entities/                 (Paciente, Medico, Especialidad)
│   │   ├── Aggregates/               (TurnoAggregate, AtencionAggregate)
│   │   └── Exceptions/               (DomainException)
│   │
│   ├── SePrise.Application/          (Casos de uso)
│   │   ├── DTOs/                     (Modelos de entrada/salida)
│   │   ├── Services/                 (Agendamiento, Acreditacion, Atencion)
│   │   └── Validators/               (FluentValidation)
│   │
│   ├── SePrise.Infrastructure/       (Persistencia)
│   │   ├── Persistence/              (SePriseDbContext, Configurations)
│   │   └── Repositories/             (Implementación EF Core)
│   │
│   ├── SePrise.API/                  (Presentación REST)
│   │   ├── Controllers/
│   │   └── Middleware/               (Manejo global de excepciones)
│   │
│   └── SePrise.WinForms/             (Presentación Desktop)
│       ├── Forms/                    (UI)
│       └── Services/                 (ApiClient centralizado)
│
├── tests/
│   └── SePrise.Tests/                (41 Integration Tests in-memory)
│
├── docs/
│   ├── DOCUMENTO_TECNICO_PROFESIONAL.md
│   └── MANUAL_USUARIO.md
│
├── documento_tecnico.txt             (Resumen técnico en raíz)
└── README.md                         (Este archivo)
```

## 🏛️ Arquitectura

### Patrones Aplicados

- **Clean Architecture**: Dependencias apuntan siempre al centro (Dominio). La capa `Domain` no tiene dependencias de ningún otro proyecto.
- **Domain-Driven Design (DDD)**: Los Agregados encapsulan la lógica de negocio mutando su propio estado.
- **Repository Pattern**: Abstracción del ORM (`ITurnoRepository`, `IPacienteRepository`).
- **Dependency Injection**: IoC container provisto por .NET para resolución transversal de interfaces.

**Para detalle técnico completo**: Ver [DOCUMENTO_TECNICO_PROFESIONAL.md](docs/DOCUMENTO_TECNICO_PROFESIONAL.md)

## 📡 API Documentation

### Swagger Interactive
Una vez que la API está corriendo localmente, visita `http://localhost:5293/swagger` para una interfaz interactiva donde puedes probar todos los endpoints y visualizar los esquemas JSON.

### Endpoints Disponibles
El sistema expone 24 endpoints. Resumen:
- **Pacientes**: 5 (POST, GET, PUT, DELETE)
- **Turnos**: 6 (POST, GET, PATCH)
- **Atenciones**: 8 (POST, GET, PATCH)
- **Especialidades**: 1 (GET)
- **Médicos**: 1 (GET)
- **Salas**: 1 (GET)
- **Reportes**: 4 (GET)

## 🧪 Testing

### Ejecutar Todos los Tests

Para ejecutar las pruebas de integración en memoria de forma limpia:

```bash
dotnet test
```

Output esperado:
```
Test Run Successful.
Total tests: 41
Passed: 41
Duration: ~15 seconds
```

## 🐛 Troubleshooting

### Error: "El archivo se ha bloqueado por SePrise.API"
Ocurre si haces `dotnet run` mientras la API ya está corriendo en segundo plano. 
**Solución**: Utiliza el Administrador de Tareas o cierra la terminal que lo originó (`taskkill /F /IM SePrise.API.exe`).

### Error: El Dropdown de Médicos aparece vacío
Asegúrate de haber seleccionado una **Especialidad** primero en el formulario, ya que los médicos se filtran dependientemente de la especialidad elegida.

## 🗺️ Roadmap

### ✅ Completado (v1.0)
- [x] Backend API REST y estructura Clean Architecture
- [x] Aplicación Desktop en Windows Forms con llamadas HTTP
- [x] Módulos de Pacientes, Turnos, Acreditación y Atención
- [x] Sembrado Automático de Base de Datos (Seeding)
- [x] Suite de Pruebas de Integración (41 tests)

### 🚧 Próximas Fases (v1.1+)
- [ ] **Autenticación JWT**: Habilitar roles de usuario para Recepción, Médico y Administrador.
- [ ] **Circuito B (Estudios Clínicos)**: Sumar registro de insumos y derivaciones.
- [ ] **Auditoría Histórica**: Quién cambió qué estado y a qué hora.
- [ ] **Reportes Avanzados**: Estadísticas de ocupación con gráficos y exportación a Excel.

## 📚 Documentación Adicional

| Documento | Descripción |
|---|---|
| [DOCUMENTO TÉCNICO](docs/DOCUMENTO_TECNICO_PROFESIONAL.md) | Arquitectura, componentes, diseño DDD, reglas y flujos críticos. |
| [MANUAL DE USUARIO](docs/MANUAL_USUARIO.md) | Guía paso a paso para el uso interactivo de la interfaz gráfica. |

## 👨‍💻 Contribución

### Cómo contribuir código
1. Realiza un Fork del repositorio.
2. Crea una rama (`git checkout -b feature/nueva-funcionalidad`).
3. Sube tus cambios (`git commit -am 'Agrega reportes PDF'`).
4. Haz push a la rama (`git push origin feature/nueva-funcionalidad`).
5. Abre un Pull Request detallando la arquitectura tocada.

## 📄 Licencia

Este proyecto se distribuye de manera académica y está bajo licencia **MIT**.

## 👥 Autores

- **Desarrolladores Principales**: Brian Ezequiel Sabio - Pedro Matias Bustamante
- **Institución**: IFTS 29 - Desarrollo de Sistemas de Información orientados a la gestión y apoyo a las decisiones.
- **Periodo**: Junio 2026

---

**Última actualización**: Junio 12, 2026  
**Versión**: 1.0.0  
**Estado**: Production Ready ✅
