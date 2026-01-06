using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ApiResponse.Poc.Models;

/// <summary>
/// Represent error response
/// </summary>
public sealed class ErrorResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Message { get; set; }

    public object Errors { get; set; }

    public ErrorCode ErrorCode { get; set; } = ErrorCode.Unknown;

    public ResponseException Exception { get; set; }

    public ErrorResponse()
    {
    }

    public ErrorResponse(BaseException ex)
    {
        Message = ex.Message;
        Errors = ex.Errors;
        ErrorCode = ex.ErrorCode;
    }

    public ErrorResponse(ModelStateDictionary modelState)
    {
        ErrorCode = ErrorCode.ValidationFailed;
        Errors = modelState.Keys
            .SelectMany(key => modelState[key].Errors
                .Select(x => new ValidationError(key, x.ErrorMessage)))
            .ToList();
    }
}

public sealed class ResponseException
{
    public string Message { get; set; }
    public string Details { get; set; }
    public string StackTrace { get; set; }
}