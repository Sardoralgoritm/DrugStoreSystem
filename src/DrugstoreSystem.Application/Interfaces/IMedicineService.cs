using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Requests;

namespace DrugstoreSystem.Application.Interfaces;

public interface IMedicineService
{
    Task<IReadOnlyList<MedicineAutocompleteDto>> AutocompleteAsync(string query, CancellationToken ct = default);
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
    Task<MedicineAutocompleteDto> CreateAsync(CreateMedicineRequest request, CancellationToken ct = default);
}
