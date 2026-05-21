using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Domain.Enums;

namespace DrugstoreSystem.Application.Interfaces;

public interface ISearchService
{
    Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        string query,
        double? userLat = null,
        double? userLng = null,
        SortMode sortMode = SortMode.Distance,
        CancellationToken ct = default);
}
