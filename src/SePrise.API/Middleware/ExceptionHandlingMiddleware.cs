namespace SePrise.API.Middleware;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SePrise.Domain.Exceptions;

/// <summary>
/// Middleware global para manejo centralizado de excepciones.
/// Mapea excepciones de dominio a respuestas HTTP apropiadas.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Inicializa nuevo ExceptionHandlingMiddleware.
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invoca el middleware.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Mapea excepciones a respuestas HTTP.
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            error = exception.Message,
            type = exception.GetType().Name
        };

        switch (exception)
        {
            case DomainException domainEx:
                // Excepciones de dominio (TurnoException, AtencionException, PacienteException, etc.)
                response.StatusCode = StatusCodes.Status409Conflict;
                break;

            case ArgumentNullException or ArgumentException:
                // Argumentos inválidos
                response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case KeyNotFoundException:
                // Recurso no encontrado
                response.StatusCode = StatusCodes.Status404NotFound;
                break;

            default:
                // Excepciones genéricas
                response.StatusCode = StatusCodes.Status500InternalServerError;
                errorResponse = new { error = "Error interno del servidor", type = "InternalServerError" };
                break;
        }

        return response.WriteAsJsonAsync(errorResponse);
    }
}
