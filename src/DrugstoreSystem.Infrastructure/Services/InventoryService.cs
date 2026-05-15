using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Interfaces;
using DrugstoreSystem.Application.Requests;
using DrugstoreSystem.Domain.Entities;
using DrugstoreSystem.Domain.Exceptions;
using DrugstoreSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugstoreSystem.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repo;
    private readonly DrugstoreDbContext _db;

    public InventoryService(IInventoryRepository repo, DrugstoreDbContext db)
    {
        _repo = repo;
        _db = db;
    }

    public async Task<PharmacistDashboardDto> GetDashboardAsync(int pharmacyId, CancellationToken ct = default)
    {
        var pharmacy = await _db.Pharmacies.FindAsync([pharmacyId], ct)
            ?? throw new DomainException($"Pharmacy {pharmacyId} not found.");

        var totalMedicines = await _db.PharmacyMedicines
            .CountAsync(pm => pm.PharmacyId == pharmacyId, ct);

        var lowStockCount = await _db.PharmacyMedicines
            .CountAsync(pm => pm.PharmacyId == pharmacyId && pm.Quantity < 5, ct);

        var mostExpensive = await _db.PharmacyMedicines
            .Include(pm => pm.Medicine)
            .Where(pm => pm.PharmacyId == pharmacyId)
            .OrderByDescending(pm => pm.Price)
            .Take(10)
            .ToListAsync(ct);

        return new PharmacistDashboardDto(
            pharmacy.Name,
            totalMedicines,
            lowStockCount,
            mostExpensive.Select(ToDto).ToList());
    }

    public async Task<IReadOnlyList<InventoryItemDto>> GetInventoryAsync(int pharmacyId, CancellationToken ct = default)
    {
        var items = await _repo.GetByPharmacyAsync(pharmacyId, ct);
        return items.Select(ToDto).ToList();
    }

    public async Task UpdateItemAsync(int itemId, int pharmacyId, UpdateInventoryItemRequest request, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(itemId, ct)
            ?? throw new DomainException("Inventar elementi topilmadi.");

        if (item.PharmacyId != pharmacyId)
            throw new DomainException("Bu element sizning dorixonangizga tegishli emas.");

        item.UpdateStock(request.Price, request.Quantity);
        await _repo.UpdateAsync(item, ct);
    }

    public async Task RemoveItemAsync(int itemId, int pharmacyId, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(itemId, ct)
            ?? throw new DomainException("Inventar elementi topilmadi.");

        if (item.PharmacyId != pharmacyId)
            throw new DomainException("Bu element sizning dorixonangizga tegishli emas.");

        await _repo.DeleteAsync(itemId, ct);
    }

    private static InventoryItemDto ToDto(PharmacyMedicine pm) => new(
        pm.Id,
        pm.MedicineId,
        pm.Medicine.Name,
        pm.Medicine.GenericName,
        pm.Medicine.DosageForm?.ToString(),
        pm.Price,
        pm.Quantity,
        pm.UpdatedAt);
}
