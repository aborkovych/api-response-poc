using System.Text.Json;
using ApiResponse.Poc.Models;
using ApiResponse.Poc.Responses;

namespace ApiResponse.Poc;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions Options = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception has occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorResponse = new ErrorResponse
            {
                Message = "An unexpected error occurred.",
                ErrorCode = ErrorCode.Unknown,
                Exception = new ResponseException
                {
                    Message = ex.Message,
                    Details = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                }
            };

            var apiResponse = ApiResponse<object>.Failure(errorResponse, new MetaInfo
            {
                TraceId = context.TraceIdentifier
            });

            var json = JsonSerializer.Serialize(apiResponse, Options);
            await context.Response.WriteAsync(json);
        }
    }
}

