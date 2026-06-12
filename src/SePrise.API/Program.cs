using SePrise.Infrastructure.DependencyInjection;
using SePrise.Application.DependencyInjection;
using SePrise.Infrastructure.Persistence;
using SePrise.API.Middleware;
using SePrise.Application.Services.Interfaces;
using SePrise.Application.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using SePrise.Application.DTOs.Paciente;
using SePrise.Application.DTOs.Turno;
using SePrise.Application.DTOs.Atencion;
using SePrise.Application.Validators.Paciente;
using SePrise.Application.Validators.Turno;
using SePrise.Application.Validators.Atencion;

var builder = WebApplication.CreateBuilder(args);
// 1. SERVICIOS DE APLICACIÓN (Application Layer)
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IAgendamientoService, AgendamientoService>();
builder.Services.AddScoped<IAcreditacionService, AcreditacionService>();
builder.Services.AddScoped<IAtencionService, AtencionService>();
builder.Services.AddScoped<IReportesService, ReportesService>();
// 2. REPOSITORIOS E INFRAESTRUCTURA
builder.Services.AddInfrastructureServices(builder.Configuration);
// 4. AUTOMAPPER (Mapping)
builder.Services.AddAutoMapper(cfg => {
    cfg.AddMaps(typeof(Program).Assembly);
});
// 5. FLUENTVALIDATION (Validación)
builder.Services.AddApplicationValidators();
// 6. CONTROLADORES
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// 7. SWAGGER (Documentación API - Opcional)
builder.Services.AddSwaggerGen();
// 8. CORS (Si es necesario para frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// CONSTRUIR APP
var app = builder.Build();
// MIDDLEWARE PIPELINE

// 1. EXCEPTION HANDLING (debe ir antes de routing)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. SWAGGER (solo en Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SePrise API v1");
    });
}

// 3. HTTPS Redirection
app.UseHttpsRedirection();

// 4. CORS
app.UseCors("AllowAll");

// 5. AUTHORIZATION (si tienes)
app.UseAuthorization();

// 6. ROUTING + CONTROLLERS
app.MapControllers();
// SEEDING (Datos de Prueba Básicos)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();
    
    // Asegurar que la BD existe
    context.Database.EnsureCreated();

    // Crear Especialidades si no hay
    if (!context.Especialidades.Any())
    {
        var esp1 = SePrise.Domain.Entities.Especialidad.Crear("Cardiología", "Tratamiento del corazón");
        var esp2 = SePrise.Domain.Entities.Especialidad.Crear("Pediatría", "Atención a niños");
        var esp3 = SePrise.Domain.Entities.Especialidad.Crear("Traumatología", "Lesiones óseas y musculares");
        context.Especialidades.AddRange(esp1, esp2, esp3);
        context.SaveChanges();

        // Crear Salas si no hay
        if (!context.Salas.Any())
        {
            var sala1 = SePrise.Domain.Entities.Sala.Crear("Consultorio 101", SePrise.Domain.Entities.TipoSala.Consultorio);
            var sala2 = SePrise.Domain.Entities.Sala.Crear("Consultorio 102", SePrise.Domain.Entities.TipoSala.Consultorio);
            var sala3 = SePrise.Domain.Entities.Sala.Crear("Consultorio 201", SePrise.Domain.Entities.TipoSala.Procedimientos);
            context.Salas.AddRange(sala1, sala2, sala3);
            context.SaveChanges();
        }

        // Crear Médicos si no hay
        if (!context.Medicos.Any())
        {
            var med1 = SePrise.Domain.Entities.Medico.Crear("MP-1010", "Juan", "Pérez");
            var med2 = SePrise.Domain.Entities.Medico.Crear("MP-2020", "María", "Gómez");
            var med3 = SePrise.Domain.Entities.Medico.Crear("MP-3030", "Carlos", "Ruiz");
            context.Medicos.AddRange(med1, med2, med3);
            context.SaveChanges();

            // Asociar Médicos con Especialidades
            var medEsp1 = SePrise.Domain.Entities.MedicoEspecialidad.Crear(med1.IdMedico, esp1.IdEspecialidad);
            var medEsp2 = SePrise.Domain.Entities.MedicoEspecialidad.Crear(med2.IdMedico, esp2.IdEspecialidad);
            var medEsp3 = SePrise.Domain.Entities.MedicoEspecialidad.Crear(med3.IdMedico, esp3.IdEspecialidad);
            context.MedicoEspecialidades.AddRange(medEsp1, medEsp2, medEsp3);
            context.SaveChanges();
        }
    }

    // Insertar nueva especialidad y 3 médicos extra (Oftalmología)
    if (!context.Especialidades.Any(e => e.Nombre == "Oftalmología"))
    {
        var espOftalmo = SePrise.Domain.Entities.Especialidad.Crear("Oftalmología", "Cuidado de los ojos y visión");
        context.Especialidades.Add(espOftalmo);
        context.SaveChanges();

        var med4 = SePrise.Domain.Entities.Medico.Crear("MP-4040", "Ana", "López");
        var med5 = SePrise.Domain.Entities.Medico.Crear("MP-5050", "Luis", "Martínez");
        var med6 = SePrise.Domain.Entities.Medico.Crear("MP-6060", "Elena", "Sánchez");
        context.Medicos.AddRange(med4, med5, med6);
        context.SaveChanges();

        var medEsp4 = SePrise.Domain.Entities.MedicoEspecialidad.Crear(med4.IdMedico, espOftalmo.IdEspecialidad);
        var medEsp5 = SePrise.Domain.Entities.MedicoEspecialidad.Crear(med5.IdMedico, espOftalmo.IdEspecialidad);
        var medEsp6 = SePrise.Domain.Entities.MedicoEspecialidad.Crear(med6.IdMedico, espOftalmo.IdEspecialidad);
        context.MedicoEspecialidades.AddRange(medEsp4, medEsp5, medEsp6);
        context.SaveChanges();
    }
}
// RUN
app.Run();

// Requerido para WebApplicationFactory en el proyecto de pruebas
public partial class Program { }


