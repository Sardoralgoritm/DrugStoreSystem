using Microsoft.AspNetCore.Identity;

namespace DrugstoreSystem.Infrastructure.Identity;

public class AppUser : IdentityUser<int>
{
    public int? PharmacyId { get; set; }
}
