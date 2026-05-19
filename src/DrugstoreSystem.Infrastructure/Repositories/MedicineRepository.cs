using DrugstoreSystem.Application.Interfaces;
using DrugstoreSystem.Domain.Entities;
using DrugstoreSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugstoreSystem.Infrastructure.Repositories;

public class MedicineRepository : IMedicineRepository
{
    private readonly DrugstoreDbContext _db;

    public MedicineRepository(DrugstoreDbContext db) => _db = db;

    public async Task<Medicine> CreateAsync(Medicine medicine, CancellationToken ct = default)
    {
        _db.Medicines.Add(medicine);
        await _db.SaveChangesAsync(ct);
        return medicine;
    }

    public async Task AddSynonymAsync(MedicineSynonym synonym, CancellationToken ct = default)
    {
        _db.MedicineSynonyms.Add(synonym);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default)
        => await _db.Categories.OrderBy(c => c.Name).ToListAsync(ct);
}
