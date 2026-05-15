namespace DrugstoreSystem.Application.Requests;

public record CreatePharmacyRequest(
    string Name,
    string Address,
    double Latitude,
    double Longitude,
    string? Phone,
    string? WorkingHours,
    string PharmacistEmail,
    string PharmacistPassword
);

public record UpdatePharmacyRequest(
    string Name,
    string Address,
    double Latitude,
    double Longitude,
    string? Phone,
    string? WorkingHours
);

public record CreatePharmacistRequest(
    string Email,
    string Password,
    int PharmacyId
);

public record UpdateInventoryItemRequest(decimal Price, int Quantity);
