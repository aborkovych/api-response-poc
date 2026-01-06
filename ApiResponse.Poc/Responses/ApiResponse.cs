using System.Diagnostics;
using ApiResponse.Poc.Models;

namespace ApiResponse.Poc.Responses;

/// <summary>
/// Api response with ErrorResponse
/// </summary>
/// <typeparam name="T">Type of response data</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the response is successful
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Only present when Success = true
    /// </summary>
    public T Data { get; init; }

    /// <summary>
    /// Only present when Success = false
    /// </summary>
    public ErrorResponse Error { get; init; }

    /// <summary>
    /// Meta information about the response
    /// </summary>
    public MetaInfo Meta { get; init; }

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
            Errors = new EntityDto<TEntityId>(id)
        };
        return Failure(errorResponse);
    }
}

public sealed class MetaInfo
{
    /// <summary>
    /// Traceability for support/debugging
    /// </summary>
    public string TraceId { get; set; }
    /// <summary>
    /// Span ID for tracing
    /// </summary>
    public string SpanId { get; set; }
    /// <summary>
    ///  Timestamp of the response
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Pagination (optional; set only for list endpoints)
    /// </summary>
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