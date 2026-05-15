using DrugstoreSystem.Domain.Entities;

namespace DrugstoreSystem.Application.Interfaces;

public interface IPharmacyRepository
{
    Task<IReadOnlyList<Pharmacy>> GetAllAsync(CancellationToken ct = default);
    Task<Pharmacy?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Pharmacy> AddAsync(Pharmacy pharmacy, CancellationToken ct = default);
    Task UpdateAsync(Pharmacy pharmacy, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
