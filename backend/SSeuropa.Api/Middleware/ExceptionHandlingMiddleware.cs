using System.Net;
using System.Text.Json;
using FluentValidation;
using SSeuropa.Application.Common;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await WriteErrorAsync(context, ex);
        }
    }

    private async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var (status, response) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.Fail("Validation failed.", ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))),
            ValidationDomainException vde => (HttpStatusCode.BadRequest, ApiResponse<object>.Fail(vde.Message, vde.Errors.ToDictionary(k => k.Key, v => v.Value))),
            NotFoundException nfe => (HttpStatusCode.NotFound, ApiResponse<object>.Fail(nfe.Message)),
            UnauthorizedAccessDomainException uae => (HttpStatusCode.Forbidden, ApiResponse<object>.Fail(uae.Message)),
            ConflictException ce => (HttpStatusCode.Conflict, ApiResponse<object>.Fail(ce.Message)),
            DomainException de => (HttpStatusCode.BadRequest, ApiResponse<object>.Fail(de.Message)),
            _ => (HttpStatusCode.InternalServerError, ApiResponse<object>.Fail("An unexpected error occurred."))
        };

        if ((int)status >= 500)
            logger.LogError(exception, "Unhandled exception");
        else
            logger.LogWarning(exception, "Handled domain/application exception");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
