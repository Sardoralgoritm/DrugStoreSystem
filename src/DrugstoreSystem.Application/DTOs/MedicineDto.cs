namespace DrugstoreSystem.Application.DTOs;

public record MedicineAutocompleteDto(
    int Id,
    string Name,
    string? GenericName,
    string? DosageForm
);

public record CategoryDto(int Id, string Name);
