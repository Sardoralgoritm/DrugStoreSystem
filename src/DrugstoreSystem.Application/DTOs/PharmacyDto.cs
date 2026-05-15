namespace DrugstoreSystem.Application.DTOs;

public record PharmacyDto(
    int Id,
    string Name,
    string Address,
    double Latitude,
    double Longitude,
    string? Phone,
    string? WorkingHours,
    bool IsActive,
    bool IsVerified,
    DateTime CreatedAt
);

public record AdminDashboardDto(
    int TotalPharmacies,
    int ActivePharmacies,
    int TotalMedicines,
    int TotalInventoryItems
);

public record AppUserDto(int Id, string Email, string Role, int? PharmacyId);
