namespace ApiResponse.Poc.Models;

/// <summary>
/// Represent validation details
/// </summary>
public sealed record ValidationError(string Field, string Message);
