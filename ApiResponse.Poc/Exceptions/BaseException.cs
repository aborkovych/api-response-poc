namespace ApiResponse.Poc;

public class BaseException : Exception
{
    public int StatusCode { get; protected set; } = 400;

    public object Errors { get; protected set; }

    public ErrorCode ErrorCode { get; protected set; }

    public BaseException() { }

    public BaseException(string message, ErrorCode errorCode = ErrorCode.Unknown)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}