using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Exceptions;
using System.ComponentModel.DataAnnotations;

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
            catch (Exception ex) when (!context.Response.HasStarted)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Exception occurred: {Type}", exception.GetType().Name);

            var problem = exception switch
            {
                BadRequestException e => CreateProblem(400, "bad-request", e.Message),
                NotFoundException e => CreateProblem(404, "not-found", e.Message),
                ValidationException e => CreateProblem(422, "validation-error", e.Message),
                DbUpdateConcurrencyException e => CreateProblem(409, "conflict", e.Message),
                _ => CreateProblem(500, "internal-error", "Unexpected error occurred.")
            };

            context.Response.StatusCode = problem.Status!.Value;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }

        private static ProblemDetails CreateProblem(int status, string code, string detail)
        {
            return new ProblemDetails
            {
                Status = status,
                Type = code, // not standart solution
                Title = ReasonPhrases.GetReasonPhrase(status),
                Detail = detail
            };
        }
    }
}
