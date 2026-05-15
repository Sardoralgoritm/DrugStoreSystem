namespace DrugstoreSystem.Application.Interfaces;

public interface ICurrentUser
{
    string? Email { get; }
    int? PharmacyId { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    bool IsPharmacist { get; }
}
