using LifeTrackerPro.Domain.Common;

namespace LifeTrackerPro.Domain.Entities;

public class Transaction : BaseUserEntity
{
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? CategoryTag { get; set; }
    public bool IsRecurring { get; set; }
    public string CurrencyCode { get; set; } = "INR";
}
