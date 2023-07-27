
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace FinancialTracker.Api.Model;

[CollectionName("users")]
public class User : MongoIdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<Guid> Accounts { get; set; } = new();
    public List<Guid> Categories { get; set; } = new();
}