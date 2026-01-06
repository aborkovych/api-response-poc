using System.Text.Json;
using ApiResponse.Poc.Models;
using ApiResponse.Poc.Responses;
using Microsoft.AspNetCore.Diagnostics;

namespace ApiResponse.Poc;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception has occurred.");

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var errorResponse = new ErrorResponse
        {
            Message = "An unexpected error occurred.",
            ErrorCode = ErrorCode.Unknown,
            Exception = new ResponseException
            {
                Message = exception.Message,
                Details = exception.InnerException?.Message,
                StackTrace = exception.StackTrace
            }
        };

        var apiResponse = ApiResponse<object>.Failure(errorResponse, new MetaInfo
        {
            TraceId = httpContext.TraceIdentifier
        });

        var json = JsonSerializer.Serialize(apiResponse, Options);
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}

