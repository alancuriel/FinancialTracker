namespace FinancialTracker.Api.Dtos;

public class GenericResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}