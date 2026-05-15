using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Interfaces;
using DrugstoreSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugstoreSystem.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly DrugstoreDbContext _db;

    public AdminService(DrugstoreDbContext db) => _db = db;

    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var totalPharmacies = await _db.Pharmacies.CountAsync(ct);
        var activePharmacies = await _db.Pharmacies.CountAsync(p => p.IsActive, ct);
        var totalMedicines = await _db.Medicines.CountAsync(ct);
        var totalInventoryItems = await _db.PharmacyMedicines.CountAsync(ct);

        return new AdminDashboardDto(totalPharmacies, activePharmacies, totalMedicines, totalInventoryItems);
    }
}
