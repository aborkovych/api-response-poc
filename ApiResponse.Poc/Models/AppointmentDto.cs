namespace ApiResponse.Poc.Models;

public sealed record AppointmentDto
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public string CustomerName { get; init; }
}