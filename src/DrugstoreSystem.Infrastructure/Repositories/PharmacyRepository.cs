using DrugstoreSystem.Application.Interfaces;
using DrugstoreSystem.Domain.Entities;
using DrugstoreSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugstoreSystem.Infrastructure.Repositories;

public class PharmacyRepository : IPharmacyRepository
{
    private readonly DrugstoreDbContext _db;

    public PharmacyRepository(DrugstoreDbContext db) => _db = db;

    public async Task<IReadOnlyList<Pharmacy>> GetAllAsync(CancellationToken ct = default)
        => await _db.Pharmacies.OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

    public async Task<Pharmacy?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Pharmacies.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Pharmacy> AddAsync(Pharmacy pharmacy, CancellationToken ct = default)
    {
        _db.Pharmacies.Add(pharmacy);
        await _db.SaveChangesAsync(ct);
        return pharmacy;
    }

    public async Task UpdateAsync(Pharmacy pharmacy, CancellationToken ct = default)
    {
        _db.Pharmacies.Update(pharmacy);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var pharmacy = await _db.Pharmacies.FindAsync([id], ct);
        if (pharmacy is not null)
        {
            _db.Pharmacies.Remove(pharmacy);
            await _db.SaveChangesAsync(ct);
        }
    }
}
