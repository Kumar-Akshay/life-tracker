namespace LifeTrackerPro.Shared.DTOs;

public record TransactionDto
{
    public Guid Id { get; init; }
    public DateOnly Date { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string? CategoryTag { get; init; }
    public bool IsRecurring { get; init; }
    public string CurrencyCode { get; init; } = "INR";
}

public class CreateTransactionRequestDto
{
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? CategoryTag { get; set; }
    public bool IsRecurring { get; set; }
    public string CurrencyCode { get; set; } = "INR";
}

public class UpdateTransactionRequestDto
{
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? CategoryTag { get; set; }
    public bool IsRecurring { get; set; }
    public string CurrencyCode { get; set; } = "INR";
}
