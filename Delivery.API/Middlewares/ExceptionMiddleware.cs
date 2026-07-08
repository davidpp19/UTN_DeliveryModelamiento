using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Delivery.Modelos.Excepciones;

namespace Delivery.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                // Errores controlados de regla de negocio
                _logger.LogWarning(ex, "Business Rule Exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                // Errores no controlados
                _logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Message = message
            });

            return context.Response.WriteAsync(result);
        }
    }
}
