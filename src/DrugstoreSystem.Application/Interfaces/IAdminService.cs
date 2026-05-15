using DrugstoreSystem.Application.DTOs;

namespace DrugstoreSystem.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken ct = default);
}
