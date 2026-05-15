using DrugstoreSystem.Domain.Entities;

namespace DrugstoreSystem.Application.Interfaces;

public interface IInventoryRepository
{
    Task<IReadOnlyList<PharmacyMedicine>> GetByPharmacyAsync(int pharmacyId, CancellationToken ct = default);
    Task<PharmacyMedicine?> GetByIdAsync(int id, CancellationToken ct = default);
    Task UpdateAsync(PharmacyMedicine item, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
