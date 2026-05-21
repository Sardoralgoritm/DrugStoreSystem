using DrugstoreSystem.Application.DTOs;

namespace DrugstoreSystem.Application.Interfaces;

public interface ISearchRepository
{
    Task<IReadOnlyList<MedicineAutocompleteDto>> AutocompleteAsync(string query, CancellationToken ct = default);
    Task<IReadOnlyList<MedicineCandidateDto>> FindMedicinesAsync(string query, CancellationToken ct = default);
}
