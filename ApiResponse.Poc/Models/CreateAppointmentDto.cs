namespace ApiResponse.Poc.Models;

public sealed record CreateAppointmentDto
{
    public DateTime Date { get; init; }
    public string CustomerName { get; init; }
}

