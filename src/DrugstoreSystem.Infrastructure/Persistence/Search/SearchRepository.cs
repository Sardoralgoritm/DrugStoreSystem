using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Helpers;
using DrugstoreSystem.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DrugstoreSystem.Infrastructure.Persistence.Search;

public class SearchRepository : ISearchRepository
{
    private readonly DrugstoreDbContext _db;

    public SearchRepository(DrugstoreDbContext db) => _db = db;

    public async Task<IReadOnlyList<MedicineAutocompleteDto>> AutocompleteAsync(string query, CancellationToken ct = default)
    {
        var q = query.Trim().ToLower();
        if (q.Length < 2) return [];

        var pattern = $"%{q}%";

        var medicines = await _db.Medicines
            .Where(m =>
                EF.Functions.ILike(m.Name, pattern) ||
                (m.GenericName != null && EF.Functions.ILike(m.GenericName, pattern)))
            .OrderBy(m => m.Name)
            .Take(10)
            .ToListAsync(ct);

        return medicines
            .Select(m => new MedicineAutocompleteDto(m.Id, m.Name, m.GenericName, m.DosageForm.ToUzbek(), m.Manufacturer))
            .ToList();
    }

    public async Task<IReadOnlyList<MedicineCandidateDto>> FindMedicinesAsync(string query, CancellationToken ct = default)
    {
        var q = query.Trim().ToLower();
        if (q.Length < 2) return [];

        var results = await _db.Database
            .SqlQuery<MedicineCandidateRaw>($"""
                SELECT
                    m.id,
                    m.name,
                    m.generic_name,
                    m.name_ru,
                    m.dosage_form,
                    m.manufacturer,
                    m.description,
                    c.name AS category_name,
                    CASE
                        WHEN LOWER(m.name) = {q}
                          OR LOWER(m.generic_name) = {q}
                          OR LOWER(m.name_ru) = {q}
                        THEN 1.0
                        WHEN LOWER(m.name) LIKE '%' || {q} || '%'
                          OR LOWER(m.generic_name) LIKE '%' || {q} || '%'
                          OR LOWER(m.name_ru) LIKE '%' || {q} || '%'
                        THEN 0.7
                        WHEN EXISTS (
                            SELECT 1 FROM medicine_synonyms s
                            WHERE s.medicine_id = m.id
                              AND LOWER(s.synonym) LIKE '%' || {q} || '%'
                        )
                        THEN 0.65
                        ELSE GREATEST(
                            similarity(m.name, {q}),
                            COALESCE(similarity(m.generic_name, {q}), 0.0),
                            COALESCE(similarity(m.name_ru, {q}), 0.0)
                        )
                    END AS score
                FROM medicines m
                LEFT JOIN categories c ON c.id = m.category_id
                WHERE
                    LOWER(m.name) LIKE '%' || {q} || '%'
                    OR LOWER(m.generic_name) LIKE '%' || {q} || '%'
                    OR LOWER(m.name_ru) LIKE '%' || {q} || '%'
                    OR EXISTS (
                        SELECT 1 FROM medicine_synonyms s
                        WHERE s.medicine_id = m.id
                          AND LOWER(s.synonym) LIKE '%' || {q} || '%'
                    )
                    OR similarity(m.name, {q}) > 0.3
                    OR similarity(m.generic_name, {q}) > 0.3
                    OR similarity(m.name_ru, {q}) > 0.3
                ORDER BY score DESC
                LIMIT 20
            """)
            .ToListAsync(ct);

        return results
            .Select(r => new MedicineCandidateDto(
                r.Id, r.Name, r.GenericName, r.NameRu,
                r.DosageForm, r.Manufacturer, r.Description,
                r.CategoryName, r.Score))
            .ToList();
    }

    private record MedicineCandidateRaw(
        int Id,
        string Name,
        string? GenericName,
        string? NameRu,
        string? DosageForm,
        string? Manufacturer,
        string? Description,
        string? CategoryName,
        double Score
    );
}
