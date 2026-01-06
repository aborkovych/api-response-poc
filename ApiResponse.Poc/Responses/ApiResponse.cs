using System.Diagnostics;
using ApiResponse.Poc.Models;

namespace ApiResponse.Poc.Responses;

/// <summary>
/// Api response with ErrorResponse
/// </summary>
/// <typeparam name="T">Type of response data</typeparam>
public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; set; }

    /// Only present when Success = true
    public T Data { get; set; }

    // Only present when Success = false
    public ErrorResponse Error { get; set; }

    // Always allowed, optional
    public MetaInfo Meta { get; set; }

    public static ApiResponse<T> Success(T data, MetaInfo meta = null) => new()
    {
        IsSuccess = true,
        Data = data,
        Meta = meta ?? new MetaInfo()
    };

    public static ApiResponse<T> Failure(ErrorResponse error, MetaInfo meta = null) => new()
    {
        IsSuccess = false,
        Error = error,
        Meta = meta ?? new MetaInfo()
    };
    
    public static ApiResponse<T> NotFound<TEntityId>(TEntityId id, string message = "Resource not found")
    {
        var errorResponse = new ErrorResponse
        {
            Message = message,
            ErrorCode = ErrorCode.EntityDoesNotExist,
            Errors = new MissingEntityDto<TEntityId>(id)
        };
        return Failure(errorResponse);
    }
}

public sealed class MetaInfo
{
    // Traceability for support/debugging
    public string TraceId { get; set; }
    public string SpanId { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    // Pagination (optional; set only for list endpoints)
    public PageMeta Page { get; set; }
    
    // other fields: async, resilience etc.
    public MetaInfo()
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            TraceId = activity.TraceId.ToString();
            SpanId = activity.SpanId.ToString();
        }
        Timestamp = DateTimeOffset.UtcNow;
    }
}

public sealed class PageMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int? TotalItems { get; set; }
    public int? TotalPages { get; set; }
}