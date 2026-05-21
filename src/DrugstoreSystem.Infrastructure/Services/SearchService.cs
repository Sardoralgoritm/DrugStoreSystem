using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Interfaces;

namespace DrugstoreSystem.Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly ISearchRepository _searchRepo;
    private readonly IInventoryRepository _inventoryRepo;

    public SearchService(ISearchRepository searchRepo, IInventoryRepository inventoryRepo)
    {
        _searchRepo = searchRepo;
        _inventoryRepo = inventoryRepo;
    }

    public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return [];

        var medicines = await _searchRepo.FindMedicinesAsync(query, ct);
        if (medicines.Count == 0) return [];

        var results = new List<SearchResultDto>();
        foreach (var medicine in medicines)
        {
            var pharmacies = await _inventoryRepo.GetAvailablePharmaciesAsync(medicine.Id, ct);
            if (pharmacies.Count > 0)
                results.Add(new SearchResultDto(medicine, pharmacies));
        }

        return results;
    }
}
