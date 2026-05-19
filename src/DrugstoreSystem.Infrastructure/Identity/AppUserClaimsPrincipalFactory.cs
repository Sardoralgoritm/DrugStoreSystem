using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DrugstoreSystem.Infrastructure.Identity;

public class AppUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<AppUser, IdentityRole<int>>
{
    public AppUserClaimsPrincipalFactory(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        if (user.PharmacyId.HasValue)
            identity.AddClaim(new Claim("PharmacyId", user.PharmacyId.Value.ToString()));
        return identity;
    }
}
