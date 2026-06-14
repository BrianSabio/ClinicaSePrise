# DOCUMENTO TÉCNICO PROFESIONAL — Sistema SePrise Circuito A

## 1. RESUMEN EJECUTIVO
### 1.1 Propósito y alcance
El sistema SePrise (Circuito A) es una plataforma integral para la gestión de consultorios externos, orientada a digitalizar y optimizar el flujo completo de atención médica: desde el registro de pacientes y el agendamiento de turnos, hasta la recepción (acreditación) y la atención clínica en consultorio. Este documento proporciona una visión exhaustiva de su arquitectura, código y decisiones de diseño.

### 1.2 Audiencia objetivo
Desarrolladores, arquitectos de software, ingenieros de QA y futuros mantenedores del sistema que necesiten comprender la base de código, la arquitectura y los conceptos de programación subyacentes.

### 1.3 Requisitos técnicos mínimos
- **Backend/Frontend:** .NET 10.0 SDK
- **Base de Datos:** SQL Server (LocalDB soportado nativamente)
- **Herramientas:** Visual Studio 2022 o IDE compatible.

### 1.4 Stack tecnológico
- **Plataforma:** .NET 10 (C# 12)
- **API:** ASP.NET Core Web API
- **Cliente UI:** Windows Forms (WinForms) moderno
- **ORM:** Entity Framework Core 10
- **Mapeo y Validación:** AutoMapper, FluentValidation
- **Exportación de Datos:** ClosedXML (Excel nativo)
- **Testing:** xUnit, Moq, WebApplicationFactory

---

## 2. DESCRIPCIÓN DEL SISTEMA
### 2.1 Visión general
SePrise divide el flujo clínico en responsabilidades claras, manejadas mediante una API RESTful escalable y consumidas por un cliente Windows Forms robusto que previene errores mediante una interfaz fluida y validaciones estrictas.

### 2.2 Objetivos funcionales
- Gestión integral de Pacientes, Médicos, Especialidades y Salas.
- Orquestación de Agendamiento de Turnos y Demanda Espontánea.
- Control de estado en tiempo real (Acreditación y Atención Médica).
- Generación de reportes de ocupación y atenciones.

### 2.3 Limitaciones y exclusiones
El Circuito A no incluye facturación avanzada, internaciones (hospitalización) ni módulos de farmacia. Está enfocado estrictamente en consultorios externos.

### 2.4 Casos de uso principales
1. Crear/Modificar perfil de paciente.
2. Agendar un turno con un médico en una fecha/hora.
3. Acreditar a un paciente que llega a la clínica.
4. Iniciar y finalizar la atención médica por parte del doctor.

---

## 3. ARQUITECTURA GENERAL
### 3.1 Principios arquitectónicos
El sistema está diseñado bajo **Clean Architecture** y principios tácticos de **Domain-Driven Design (DDD)**. Esto garantiza que la lógica de negocio pura (el Dominio) no tenga dependencias hacia afuera, logrando un sistema altamente testeable e independiente de frameworks de bases de datos o UI.

### 3.2 Capas de la solución
- **Domain:** Entidades, Value Objects, Agregados y Excepciones. Sin dependencias externas.
- **Application:** Casos de uso, DTOs, Validadores e Interfaces.
- **Infrastructure:** DbContext, Repositorios EF Core, Migraciones.
- **API:** Controladores REST, Middleware, inyección de dependencias.
- **WinForms (UI):** Consumo de API, navegación, vistas.

### 3.3 Diagrama de capas (ASCII)
```text
┌────────────────────────────────────────────────────────┐
│                   PRESENTATION LAYER                   │
│        [ SePrise.API ]       [ SePrise.WinForms ]      │
└─────────────────────────┬──────────────────────────────┘
                          │ (HTTP/REST)
┌─────────────────────────▼──────────────────────────────┐
│                   APPLICATION LAYER                    │
│    [ Servicios, DTOs, FluentValidation, AutoMapper ]   │
└─────────────────────────┬──────────────────────────────┘
                          │ (Usa interfaces)
┌─────────────────────────▼──────────────────────────────┐
│                      DOMAIN LAYER                      │
│ [ Entidades, Agregados, Enums, Excepciones, Reglas ]   │
└─────────────────────────▲──────────────────────────────┘
                          │ (Implementa interfaces)
┌─────────────────────────┴──────────────────────────────┐
│                  INFRASTRUCTURE LAYER                  │
│       [ EF Core, Repositorios, SQL Server, Migrations] │
└────────────────────────────────────────────────────────┘
```

### 3.4 Flujo de dependencias
La flecha de dependencia siempre apunta hacia adentro (hacia el Dominio). La Infraestructura y la API dependen de Application y Domain, pero Domain no depende de nadie.

### 3.5 Decisiones tecnológicas
Se eligió .NET 10 por su rendimiento y soporte LTS extendido. Clean Architecture asegura escalabilidad. EF Core permite modelado rico y migraciones seguras. WinForms se seleccionó para una adopción rápida en entornos de escritorio hospitalarios heredados.

---

## 4. CAPA DE DOMINIO (SePrise.Domain)
### 4.1 Propósito y responsabilidad
Es el corazón del software. Contiene toda la lógica de negocio, invariantes y reglas corporativas. Ninguna regla de validación de negocio puro debe escapar de esta capa.

### 4.2 Entidades de dominio
- **Paciente**: Gestiona la identidad del paciente. Validaciones: DNI único, Email obligatorio, longitud máxima de texto.
- **Médico**: Gestiona profesionales, matrícula (obligatoria) y relación N:N con especialidades.
- **Especialidad**: Define áreas médicas y la duración estándar de sus turnos.
- **Sala**: Define el espacio físico (Consultorio o Quirófano).
- **Medico_Especialidad**: Tabla asociativa limpia para EF Core.

### 4.3 Agregados raíz (DDD)
Un agregado es un clúster de entidades tratadas como una unidad para la mutación de datos.

#### 4.3.1 TurnoAggregate
Controla el ciclo de vida de un turno agendado.
**Métodos Clave**:
- `Crear()`: Valida fechas futuras e IDs correctos. Nace como *Reservado*.
- `ConfirmarTurno()`: Pasa a *Confirmado*.
- `CancelarTurno()`: Pasa a *Cancelado*.
- `ReprogramarTurno()`: Pasa a *Reprogramado* (se crea uno nuevo a nivel de servicio).

Ejemplo de código:
```csharp
public void ConfirmarTurno()
{
    if (Estado != EstadoTurno.Reservado)
        throw new TurnoException($"No se puede confirmar desde {Estado}");
    
    Estado = EstadoTurno.Confirmado;
    ActualizarFechaModificacion();
}
```

#### 4.3.2 AtencionAggregate
Controla la atención médica en la clínica una vez acreditado.
**Métodos Clave**:
- `CrearDesdeConfirmacion()`: Nace como *Acreditada*.
- `IniciarAtencion()`: Pasa a *EnProgreso*.
- `FinalizarAtencion(string notas)`: Pasa a *Finalizada*, guarda las notas.

### 4.4 Value Objects (Enums)
- **EstadoTurno**: Reservado, Confirmado, Atendido, Cancelado, NoAsistio, Reprogramado.
- **EstadoAtencion**: Acreditada, EnProgreso, Finalizada, Cancelada.
- **TipoSala**: Consultorio, Procedimientos.

### 4.5 Excepciones de dominio
`DomainException` base, con especializaciones como `TurnoException`, `PacienteException`, atrapadas globalmente por la API para retornar HTTP 409 Conflict o 400 Bad Request.

---

## 5. MÁQUINAS DE ESTADO (State Machines)
### 5.1 Estados de Turno

#### 5.1.1 Diagrama de transiciones (ASCII art)
```text
┌─────────────────────────────────────────────────┐
│              MÁQUINA DE ESTADOS - TURNO         │
├─────────────────────────────────────────────────┤
│                                                  │
│  [Reservado]                                    │
│      │                                          │
│      ├──→ [Confirmado] ──→ [Atendido] ✓        │
│      │        │                                 │
│      │        ├──→ [Cancelado] ✓               │
│      │        │                                 │
│      │        └──→ [Reprogramado] ✓ + nuevo   │
│      │                                          │
│      ├──→ [NoAsistio] ✓                        │
│      │                                          │
│      └──→ [Cancelado] ✓                        │
│                                                  │
└─────────────────────────────────────────────────┘
```

#### 5.1.2 Reglas de transición
- Solo un turno `Reservado` puede confirmarse (Acreditación).
- Un turno `Confirmado` no puede marcarse como `NoAsistio` (ya llegó el paciente).
- Estados con ✓ son terminales.

### 5.2 Estados de Atención

#### 5.2.1 Diagrama de transiciones (ASCII art)
```text
┌─────────────────────────────────────────────────┐
│             MÁQUINA DE ESTADOS - ATENCIÓN       │
├─────────────────────────────────────────────────┤
│                                                  │
│  [Acreditada]                                   │
│      │                                          │
│      ├──→ [EnProgreso] ──→ [Finalizada] ✓      │
│      │                                          │
│      └──→ [Cancelada] ✓                        │
│                                                  │
└─────────────────────────────────────────────────┘
```

#### 5.3 Coherencia Turno ↔ Atención
Cuando la `Atencion` pasa a `Finalizada`, dispara en cascada que el `Turno` pase a `Atendido`. Esto se orquesta en el `AtencionService`.

---

## 6. CAPA DE APLICACIÓN (SePrise.Application)
### 6.1 Propósito y responsabilidad
Orquestar los casos de uso, transaccionar con los repositorios y mapear datos hacia el exterior.

### 6.2 Servicios de aplicación
- **AgendamientoService**: Crea turnos, chequea solapamientos de horario del médico.
- **AcreditacionService**: **CRÍTICO**. Convierte un Turno en Atención de forma atómica.
- **AtencionService**: Maneja el trabajo del médico (iniciar, anotar, finalizar).
- **PacienteService**: ABM de pacientes.

### 6.3 DTOs y Mapeo
Aíslan las entidades de la base de datos de la red. `AutoMapper` centraliza la conversión `Entity -> DTO` y `CreateRequest -> Entity`.

### 6.4 Validadores
Se usa `FluentValidation` para inyectar reglas complejas (ej. edad mínima, regex de emails) antes de llegar al dominio.

---

## 7. CAPA DE INFRAESTRUCTURA (SePrise.Infrastructure)
### 7.1 Propósito y responsabilidad
Detalles de implementación técnica: base de datos, correos, archivos.

### 7.2 DbContext (EF Core 10)
`SePriseDbContext` utiliza configuraciones separadas (ej. `TurnoConfiguration`) implementando `IEntityTypeConfiguration<T>`.

### 7.3 Repositorios
Se implementa el **Repository Pattern** (`ITurnoRepository`, `IPacienteRepository`) para ocultar los `DbSet<T>` y encapsular las consultas LINQ pesadas (ej. buscar turnos de un médico en una fecha).

---

## 8. CAPA DE PRESENTACIÓN — API REST (SePrise.API)
### 8.1 Propósito
Exponer el sistema al exterior mediante HTTP/JSON.

### 8.2 Controllers
- Controladores delgados. Solo reciben la request HTTP, delegan a Application, y retornan `200 OK`, `201 Created` o errores.

- API RESTful: Construida en ASP.NET Core 10, expone endpoints HTTP consumibles. Incluye integración con Swagger para documentación automática.
- WinForms (Cliente): Aplicación de escritorio .NET 10 (con componentes personalizados de diseño "ModernControls" para la UX) que consume la API a través de HttpClient, usando serialización System.Net.Http.Json.

---

## 9. CAPA DE PRESENTACIÓN — FRONTEND (SePrise.WinForms)
### 9.1 Propósito
Interfaz de usuario de alto rendimiento para el personal administrativo y médico.

### 9.2 Arquitectura
Utiliza un `ApiClient` centralizado construido sobre `HttpClient` con serialización estricta (`UnmappedMemberHandling.Disallow`) para garantizar cohesión total de contratos con la API.

### 9.3 Formularios
Paneles modernos con arquitectura inyectada. 
- **CrearTurnoForm**: Validaciones visuales y dropdowns dinámicos.
- **AcreditacionForm**: Filtra los turnos que están únicamente en estado `Reservado`.
- **GenerarReporteForm**: Consulta en tiempo real de estadísticas. Integra `ClosedXML` para exportar el `DataGridView` directamente a Excel (`.xlsx`) sin requerir Microsoft Office instalado.

---

## 10. MODELO DE DATOS (DATABASE SCHEMA)
### 10.1 Entity Relationship Diagram (ASCII)
```text
┌──────────────┐         ┌──────────────────┐
│  PACIENTE    │─────────│ TURNO            │
│──────────────│  1:N    │──────────────────│
│ IdPaciente   │         │ IdTurno (PK)     │
│ DNI (unique) │         │ IdPaciente (FK)  │
│ Nombre       │         │ IdMedico (FK)    │
│ Apellido     │         │ IdEspecialidad   │
│ Email        │         │ IdSala (FK)      │
│ Telefono     │         │ FechaHoraInicio  │
│ ...          │         │ Estado (enum)    │
└──────────────┘         └──────────────────┘
       │                          │
       │                          │
       ├─────────────────────────┤
       │                          │
       │                    ┌──────────────────┐
       └────────────────────│ ATENCION         │
                1:N         │──────────────────│
                            │ IdAtencion (PK)  │
                            │ IdTurno (FK null)│
                            │ IdPaciente (FK)  │
                            │ IdMedico (FK)    │
                            │ Estado (enum)    │
                            │ Notas            │
                            └──────────────────┘
```

### 10.2 Restricciones de integridad
- DNI de Paciente: Único.
- Matrícula de Médico: Única.
- Cascadas: Bloqueadas para evitar borrados accidentales de historiales.

---

## 11. FLUJOS OPERATIVOS CRÍTICOS
### 11.1 Flujo: Acreditación (CASCADA CRÍTICA)
```text
Recepcionista → API
      │
      ├─ POST /api/atenciones/acreditar
      │
      ▼
   [AcreditacionService.AcreditarPacienteAsync]
      │
      ├─ Obtener Turno
      ├─ Validar: Estado == Reservado
      │
      ├─ TRANSACCIÓN ATÓMICA (SaveChangesAsync único)
      │   ├─ Turno.ConfirmarTurno() → Estado = Confirmado
      │   ├─ Crear Atencion (Estado = Acreditada)
      │   ├─ Repositorio.Add(Atencion)
      │
      ├─ SaveChangesAsync() commit a DB
      │
      ▼
   201 Created -> Paciente en sala de espera
```

---

## 12. VALIDACIONES Y REGLAS DE NEGOCIO
1. **Frontend**: Valida campos vacíos, longitudes, formato email. (Experiencia de usuario).
2. **API/Application**: `FluentValidation` valida DTOs antes de tocar la lógica.
3. **Dominio**: Factory methods (`.Crear()`) arrojan `ArgumentException` si se viola la pureza de los datos.

---

## 13. PATRONES Y PRINCIPIOS ARQUITECTÓNICOS
- **SOLID**: Inversión de dependencias generalizada. Principio de Responsabilidad Única (SRP) en todos los servicios.
- **Repository Pattern**: Abstracción del ORM.
- **Async/Await**: Todo I/O a base de datos es asíncrono para liberar hilos del servidor de Kestrel (escalabilidad).

---

## 14. TESTING
- **41 Integrations Tests** usando `WebApplicationFactory` con una base de datos en memoria (`InMemoryDb`) para validar controladores y repositorios simultáneamente sin mocking excesivo.
- Cobertura del 69.7%, concentrada en flujos críticos.

---

## 15. ENDPOINTS REST COMPLETO
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | /api/pacientes | Listar todos los pacientes |
| POST | /api/pacientes | Crear paciente |
| PUT | /api/pacientes/{id} | Actualizar paciente |
| GET | /api/turnos | Listar turnos (soporta filtros por query param) |
| POST | /api/turnos | Agendar turno |
| PATCH| /api/turnos/{id}/confirmar | Confirma turno (transición de estado) |
| POST | /api/atenciones/acreditar | Generar atención desde turno |
| PATCH| /api/atenciones/{id}/iniciar| Médico inicia atención |
| PATCH| /api/atenciones/{id}/finalizar| Médico finaliza y agrega notas clínicas |

*(La API expone un total de 24 endpoints debidamente documentados vía Swagger).*

---

## 16. DECISIONES TÉCNICAS JUSTIFICADAS
- **¿Por qué Clean Architecture?** Previene que un cambio en Windows Forms o SQL Server rompa las reglas de agendamiento.
- **¿Por qué Serialización Estricta en JSON?** (`UnmappedMemberHandling.Disallow`): Falla rápido. Si la API envía datos nuevos o cambia nombres, el frontend falla en desarrollo, previniendo bugs silenciosos en producción.
- **¿Por qué Seeding en Program.cs?** Facilita la evaluación académica y el despliegue "Zero Config" en entornos de pruebas, permitiendo una experiencia "Plug and Play".

---

## 17. OPTIMIZACIONES Y CONSIDERACIONES
- **Filtros en Repositorios**: En lugar de traer toda la tabla a memoria, se delegan los filtros pesados (ej. `Where(FechaHoraInicio)`) al motor SQL Server.
- **Transacciones Atómicas**: Acreditación utiliza un solo `SaveChangesAsync` para garantizar consistencia: si crear la atención falla, el turno no se marca como confirmado (evita estados zombis).

---

## 18. SEGURIDAD
- ORM (`EF Core`) previene ataques de **SQL Injection** usando consultas parametrizadas.
- Validación fuerte en la API detiene envíos maliciosos burlando el Frontend.

---

## 19. ESCALABILIDAD FUTURA
- La arquitectura permite reemplazar `SePrise.WinForms` por una aplicación `Angular/React` en la web simplemente conectándose a los mismos endpoints de la API, sin reescribir ni una línea de código de negocio.
- Puede añadirse autenticación JWT fácilmente registrando middleware en la API.

---

## 20. TROUBLESHOOTING Y FAQ
- **Error: "El archivo se ha bloqueado por SePrise.API"**: Ocurre si haces `dotnet run` mientras la API ya está corriendo en segundo plano. Usa `taskkill /F /IM SePrise.API.exe` (Windows) o cierra la terminal que lo originó.
- **Turnos no aparecen en Acreditación**: Asegúrate de que el turno esté en estado `Reservado`. Si está en `Confirmado`, ya fue acreditado.

---

## 21. CONCLUSIÓN Y REFERENCIAS
SePrise es un excelente ejemplo de software diseñado para durar. Combina patrones estructurales pesados con código C# moderno y limpio, garantizando su capacidad de evolución a futuro.
