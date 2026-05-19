using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Helpers;
using DrugstoreSystem.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DrugstoreSystem.Infrastructure.Persistence.Search;

public class SearchRepository : ISearchRepository
{
    private readonly DrugstoreDbContext _db;

    public SearchRepository(DrugstoreDbContext db) => _db = db;

    public async Task<IReadOnlyList<MedicineAutocompleteDto>> AutocompleteAsync(string query, CancellationToken ct = default)
    {
        var q = query.Trim().ToLower();
        if (q.Length < 2) return [];

        var pattern = $"%{q}%";

        var medicines = await _db.Medicines
            .Where(m =>
                EF.Functions.ILike(m.Name, pattern) ||
                (m.GenericName != null && EF.Functions.ILike(m.GenericName, pattern)))
            .OrderBy(m => m.Name)
            .Take(10)
            .ToListAsync(ct);

        return medicines
            .Select(m => new MedicineAutocompleteDto(m.Id, m.Name, m.GenericName, m.DosageForm.ToUzbek()))
            .ToList();
    }
}
