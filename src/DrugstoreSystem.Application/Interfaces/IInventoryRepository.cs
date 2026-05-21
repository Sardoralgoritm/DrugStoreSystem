using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Domain.Entities;

namespace DrugstoreSystem.Application.Interfaces;

public interface IInventoryRepository
{
    Task<IReadOnlyList<PharmacyMedicine>> GetByPharmacyAsync(int pharmacyId, CancellationToken ct = default);
    Task<PharmacyMedicine?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PharmacyMedicine?> GetByPharmacyAndMedicineAsync(int pharmacyId, int medicineId, CancellationToken ct = default);
    Task AddAsync(PharmacyMedicine item, CancellationToken ct = default);
    Task UpdateAsync(PharmacyMedicine item, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<PharmacyResultDto>> GetAvailablePharmaciesAsync(int medicineId, CancellationToken ct = default);
}
