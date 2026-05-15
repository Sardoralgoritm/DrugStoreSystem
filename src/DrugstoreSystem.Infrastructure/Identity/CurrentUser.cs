using DrugstoreSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DrugstoreSystem.Infrastructure.Identity;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    public bool IsAdmin => User?.IsInRole("Admin") ?? false;
    public bool IsPharmacist => User?.IsInRole("Pharmacist") ?? false;

    public int? PharmacyId
    {
        get
        {
            var value = User?.FindFirstValue("PharmacyId");
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
