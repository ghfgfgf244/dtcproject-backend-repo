using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace dtc.API.Middlewares
{
    public class ExceptionResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Detailed { get; set; } = string.Empty;
    }

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict detected.");
                await WriteResponseAsync(context, HttpStatusCode.Conflict,
                    "The resource was modified by another user. Please reload and try again.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access detected.");
                await WriteResponseAsync(context, HttpStatusCode.Unauthorized,
                    "Unauthorized.", ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Requested resource was not found.");
                await WriteResponseAsync(context, HttpStatusCode.NotFound,
                    "The requested resource was not found.", ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request payload.");
                await WriteResponseAsync(context, HttpStatusCode.BadRequest,
                    "The request is invalid.", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule conflict detected.");
                await WriteResponseAsync(context, HttpStatusCode.Conflict,
                    "The requested operation conflicts with the current resource state.", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during the request.");
                await WriteResponseAsync(context, HttpStatusCode.InternalServerError,
                    "Internal Server Error occurred. Please contact the administrator.", ex.Message);
            }
        }

        private static Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode,
            string message, string detailed = "")
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ExceptionResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                Detailed = detailed
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
