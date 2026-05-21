namespace DrugstoreSystem.Application.DTOs;

public record MedicineAutocompleteDto(
    int Id,
    string Name,
    string? GenericName,
    string? DosageForm
);

public record CategoryDto(int Id, string Name);

public record MedicineCandidateDto(
    int Id,
    string Name,
    string? GenericName,
    string? NameRu,
    string? DosageForm,
    string? Manufacturer,
    string? Description,
    string? CategoryName,
    double Score
);

public record PharmacyResultDto(
    int PharmacyId,
    string PharmacyName,
    string Address,
    double Latitude,
    double Longitude,
    string? Phone,
    string? WorkingHours,
    decimal Price,
    int Quantity,
    int InventoryId
);

public record RankedPharmacyDto(
    int PharmacyId,
    string PharmacyName,
    string Address,
    string? Phone,
    string? WorkingHours,
    decimal Price,
    int Quantity,
    double? DistanceKm,
    string MapsUrl
);

public record SearchResultDto(
    MedicineCandidateDto Medicine,
    IReadOnlyList<PharmacyResultDto> RawPharmacies,
    IReadOnlyList<RankedPharmacyDto> Pharmacies
);
