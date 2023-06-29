using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace FinancialTracker.Api.Model;

[CollectionName("roles")]
public class Role : MongoIdentityRole<Guid>
{

}