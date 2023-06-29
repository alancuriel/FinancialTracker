using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinancialTracker.Api;

public class Account
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public List<Guid> Transactions { get; set; } = new();

}
