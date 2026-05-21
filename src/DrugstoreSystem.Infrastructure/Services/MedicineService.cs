using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Interfaces;
using DrugstoreSystem.Application.Requests;
using DrugstoreSystem.Domain.Entities;

namespace DrugstoreSystem.Infrastructure.Services;

public class MedicineService : IMedicineService
{
    private readonly IMedicineRepository _repo;
    private readonly ISearchRepository _search;

    public MedicineService(IMedicineRepository repo, ISearchRepository search)
    {
        _repo = repo;
        _search = search;
    }

    public Task<Medicine?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repo.GetByIdAsync(id, ct);

    public Task<IReadOnlyList<MedicineAutocompleteDto>> AutocompleteAsync(string query, CancellationToken ct = default)
        => _search.AutocompleteAsync(query, ct);

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var categories = await _repo.GetCategoriesAsync(ct);
        return categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList();
    }

    public async Task<MedicineAutocompleteDto> CreateAsync(CreateMedicineRequest request, CancellationToken ct = default)
    {
        var medicine = new Medicine(
            request.Name, request.GenericName, request.NameRu,
            request.DosageForm, request.CategoryId,
            request.Manufacturer, request.Description,
            request.CreatedByPharmacyId);

        await _repo.CreateAsync(medicine, ct);

        foreach (var synonym in request.Synonyms.Where(s => !string.IsNullOrWhiteSpace(s)))
            await _repo.AddSynonymAsync(new MedicineSynonym(medicine.Id, synonym.Trim()), ct);

        return new MedicineAutocompleteDto(
            medicine.Id, medicine.Name, medicine.GenericName,
            medicine.DosageForm?.ToString());
    }
}
