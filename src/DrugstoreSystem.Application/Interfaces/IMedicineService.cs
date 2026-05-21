using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Requests;
using DrugstoreSystem.Domain.Entities;

namespace DrugstoreSystem.Application.Interfaces;

public interface IMedicineService
{
    Task<Medicine?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<MedicineAutocompleteDto>> AutocompleteAsync(string query, CancellationToken ct = default);
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
    Task<MedicineAutocompleteDto> CreateAsync(CreateMedicineRequest request, CancellationToken ct = default);
}
