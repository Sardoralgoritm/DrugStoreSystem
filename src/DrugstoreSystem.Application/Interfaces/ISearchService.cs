using DrugstoreSystem.Application.DTOs;

namespace DrugstoreSystem.Application.Interfaces;

public interface ISearchService
{
    Task<IReadOnlyList<SearchResultDto>> SearchAsync(string query, CancellationToken ct = default);
}
