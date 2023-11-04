using MongoDB.Bson.Serialization.Attributes;

namespace FinancialTracker.Api;

public class Category
{
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color {get; set;} = string.Empty;
}
