namespace SePrise.Tests.Integration.Fixtures;

using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SePrise.Infrastructure.Persistence;

/// <summary>
/// Factory para pruebas de integración con SQLite in-memory persistente.
/// Mantiene la conexión abierta durante la vida del factory para evitar que
/// la base de datos se destruya entre operaciones.
/// </summary>
public class SePriseWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public SePriseWebApplicationFactory()
    {
        // Creamos una conexión a una base de datos SQLite en memoria
        _connection = new SqliteConnection("DataSource=:memory:");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Abrimos la conexión una sola vez para que la base de datos se mantenga viva
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            // Remover configuraciones previas de EF Core para evitar conflictos entre SqlServer y Sqlite
            var efServices = services.Where(s => 
                s.ServiceType.FullName != null && 
                s.ServiceType.FullName.StartsWith("Microsoft.EntityFrameworkCore")).ToList();

            foreach (var s in efServices)
            {
                services.Remove(s);
            }

            services.RemoveAll(typeof(System.Data.Common.DbConnection));

            // Agregar el DbContext configurado para usar la conexión SQLite en memoria
            services.AddDbContext<SePriseDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Construir un ServiceProvider intermedio para inicializar la BD
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<SePriseDbContext>();

            // Asegurarnos de que el esquema de la base de datos esté creado
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
