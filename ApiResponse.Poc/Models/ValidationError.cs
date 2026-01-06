namespace ApiResponse.Poc.Models;

/// <summary>
/// Represent validation details
/// </summary>
public class ValidationError(string field, string message)
{
    public string Field { get; } = !string.IsNullOrEmpty(field) ? field : null;

    public string Message { get; } = message;
}