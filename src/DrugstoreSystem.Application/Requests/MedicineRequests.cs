using DrugstoreSystem.Domain.Enums;

namespace DrugstoreSystem.Application.Requests;

public record CreateMedicineRequest(
    string Name,
    string? GenericName,
    string? NameRu,
    DosageForm? DosageForm,
    int? CategoryId,
    string? Manufacturer,
    string? Description,
    int CreatedByPharmacyId,
    IReadOnlyList<string> Synonyms
);

public record AddToInventoryRequest(int MedicineId, decimal Price, int Quantity);
