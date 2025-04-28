using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace MiniHotel.API.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhanded exception occured. More details: {Message}", exception.Message);

            context.Response.ContentType = "application/problem+json";

            var (statusCode, typeUri, title, message) = exception switch
            {
                BadRequestException e => ((int)HttpStatusCode.BadRequest, "Bad Request", "Invalid request", e.Message),
                KeyNotFoundException e => ((int)HttpStatusCode.NotFound, "Not Found", "Resource not found", e.Message),
                ValidationException e => (422, "Validation Error", "Validation failed", e.Message),
                DbUpdateConcurrencyException e => (409, "Conflict", "Resource conflict", e.Message),
                _ => (500, "Server Error", "Internal Server Error", "Unexpected error occurred.")
            };

            context.Response.StatusCode = statusCode;

            var problem = new
            {
                Status = statusCode,
                Type = typeUri,
                Title = title,
                Message = message,
            };

            var jsonResponse = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
