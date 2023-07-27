using MongoDB.Bson.Serialization.Attributes;

namespace FinancialTracker.Api;

public class Transaction
{
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid AccountId { get; set; } = Guid.Empty;
    public Guid CategoryId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; } = TransactionType.Regular;
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public bool Excluded { get; set; }
    public string Note { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
}
