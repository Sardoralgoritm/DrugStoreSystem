using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Application.Interfaces;
using DrugstoreSystem.Application.Requests;
using DrugstoreSystem.Domain.Entities;
using DrugstoreSystem.Domain.Exceptions;
using DrugstoreSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DrugstoreSystem.Infrastructure.Services;

public class PharmacyService : IPharmacyService
{
    private readonly IPharmacyRepository _repo;
    private readonly UserManager<AppUser> _userManager;

    public PharmacyService(IPharmacyRepository repo, UserManager<AppUser> userManager)
    {
        _repo = repo;
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<PharmacyDto>> GetAllAsync(CancellationToken ct = default)
    {
        var pharmacies = await _repo.GetAllAsync(ct);
        return pharmacies.Select(ToDto).ToList();
    }

    public async Task<PharmacyDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var pharmacy = await _repo.GetByIdAsync(id, ct);
        return pharmacy is null ? null : ToDto(pharmacy);
    }

    public async Task<PharmacyDto> CreateAsync(CreatePharmacyRequest request, CancellationToken ct = default)
    {
        var pharmacy = new Pharmacy(
            request.Name, request.Address,
            request.Latitude, request.Longitude,
            request.Phone, request.WorkingHours);

        await _repo.AddAsync(pharmacy, ct);

        // Create pharmacist account
        var user = new AppUser
        {
            UserName = request.PharmacistEmail,
            Email = request.PharmacistEmail,
            EmailConfirmed = true,
            PharmacyId = pharmacy.Id
        };

        var result = await _userManager.CreateAsync(user, request.PharmacistPassword);
        if (!result.Succeeded)
        {
            await _repo.DeleteAsync(pharmacy.Id, ct);
            throw new DomainException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, "Pharmacist");
        return ToDto(pharmacy);
    }

    public async Task UpdateAsync(int id, UpdatePharmacyRequest request, CancellationToken ct = default)
    {
        var pharmacy = await _repo.GetByIdAsync(id, ct)
            ?? throw new DomainException($"Pharmacy {id} not found.");

        pharmacy.Update(request.Name, request.Address,
            request.Latitude, request.Longitude,
            request.Phone, request.WorkingHours);

        await _repo.UpdateAsync(pharmacy, ct);
    }

    public async Task SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var pharmacy = await _repo.GetByIdAsync(id, ct)
            ?? throw new DomainException($"Pharmacy {id} not found.");

        if (isActive) pharmacy.Activate(); else pharmacy.Deactivate();
        await _repo.UpdateAsync(pharmacy, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
        => await _repo.DeleteAsync(id, ct);

    private static PharmacyDto ToDto(Pharmacy p) => new(
        p.Id, p.Name, p.Address, p.Latitude, p.Longitude,
        p.Phone, p.WorkingHours, p.IsActive, p.IsVerified, p.CreatedAt);
}
