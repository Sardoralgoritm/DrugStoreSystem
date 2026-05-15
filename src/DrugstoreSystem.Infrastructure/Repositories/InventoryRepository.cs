using DrugstoreSystem.Application.Interfaces;
using DrugstoreSystem.Domain.Entities;
using DrugstoreSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugstoreSystem.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly DrugstoreDbContext _db;

    public InventoryRepository(DrugstoreDbContext db) => _db = db;

    public async Task<IReadOnlyList<PharmacyMedicine>> GetByPharmacyAsync(int pharmacyId, CancellationToken ct = default)
        => await _db.PharmacyMedicines
            .Include(pm => pm.Medicine)
            .Where(pm => pm.PharmacyId == pharmacyId)
            .OrderBy(pm => pm.Medicine.Name)
            .ToListAsync(ct);

    public async Task<PharmacyMedicine?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.PharmacyMedicines
            .Include(pm => pm.Medicine)
            .FirstOrDefaultAsync(pm => pm.Id == id, ct);

    public async Task UpdateAsync(PharmacyMedicine item, CancellationToken ct = default)
    {
        _db.PharmacyMedicines.Update(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var item = await _db.PharmacyMedicines.FindAsync([id], ct);
        if (item is not null)
        {
            _db.PharmacyMedicines.Remove(item);
            await _db.SaveChangesAsync(ct);
        }
    }
}
