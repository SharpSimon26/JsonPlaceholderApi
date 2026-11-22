using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace JsonPlaceholder.Api.Exceptions;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Eccezione non gestita intercettata: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            ArgumentException or BadHttpRequestException => (StatusCodes.Status400BadRequest, "Richiesta non valida"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Risorsa non trovata"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Accesso non autorizzato"),
            _ => (StatusCodes.Status500InternalServerError, "Errore interno del server")
        };

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                // In produzione evita di esporre exception.Message per gli errori 500
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "Si è verificato un errore imprevisto."
                    : exception.Message
            }
        });
    }
}