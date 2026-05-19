using DrugstoreSystem.Domain.Entities;

namespace DrugstoreSystem.Application.Interfaces;

public interface IMedicineRepository
{
    Task<Medicine> CreateAsync(Medicine medicine, CancellationToken ct = default);
    Task AddSynonymAsync(MedicineSynonym synonym, CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default);
}
