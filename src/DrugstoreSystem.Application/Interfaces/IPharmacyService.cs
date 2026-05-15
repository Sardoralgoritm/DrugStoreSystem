using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Requests;

namespace DrugstoreSystem.Application.Interfaces;

public interface IPharmacyService
{
    Task<IReadOnlyList<PharmacyDto>> GetAllAsync(CancellationToken ct = default);
    Task<PharmacyDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PharmacyDto> CreateAsync(CreatePharmacyRequest request, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdatePharmacyRequest request, CancellationToken ct = default);
    Task SetActiveAsync(int id, bool isActive, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
