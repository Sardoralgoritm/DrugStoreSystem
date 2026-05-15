using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Requests;

namespace DrugstoreSystem.Application.Interfaces;

public interface IInventoryService
{
    Task<PharmacistDashboardDto> GetDashboardAsync(int pharmacyId, CancellationToken ct = default);
    Task<IReadOnlyList<InventoryItemDto>> GetInventoryAsync(int pharmacyId, CancellationToken ct = default);
    Task UpdateItemAsync(int itemId, int pharmacyId, UpdateInventoryItemRequest request, CancellationToken ct = default);
    Task RemoveItemAsync(int itemId, int pharmacyId, CancellationToken ct = default);
}
