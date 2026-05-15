# API Contracts — DrugstoreSystem

This document defines all service interfaces, DTOs, and request/response types in `DrugstoreSystem.Application`. These are the contracts that Web pages consume and Infrastructure implements.

---

## 1. Service Interfaces

### 1.1 `ISearchService`

```csharp
public interface ISearchService
{
    Task<IReadOnlyList<SearchResultDto>> SearchAsync(SearchRequest request, CancellationToken ct = default);
}
```

**Used by:** `Pages/Public/SearchPage.razor`

---

### 1.2 `IPharmacyService`

```csharp
public interface IPharmacyService
{
    Task<IReadOnlyList<PharmacyDto>> GetAllAsync(CancellationToken ct = default);
    Task<PharmacyDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PharmacyDto?> GetByPharmacistUserIdAsync(int userId, CancellationToken ct = default);
    Task<PharmacyDto> CreateAsync(CreatePharmacyRequest request, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdatePharmacyRequest request, CancellationToken ct = default);
    Task SetActiveAsync(int id, bool isActive, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
```

**Used by:** Admin pages + Pharmacist profile page.

---

### 1.3 `IMedicineService`

```csharp
public interface IMedicineService
{
    Task<IReadOnlyList<MedicineAutocompleteDto>> AutocompleteAsync(string query, CancellationToken ct = default);
    Task<MedicineDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<MedicineDto> CreateAsync(CreateMedicineRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
}
```

**Used by:** Pharmacist inventory "add medicine" form + Public medicine detail page.

---

### 1.4 `IInventoryService`

```csharp
public interface IInventoryService
{
    Task<IReadOnlyList<InventoryItemDto>> GetByPharmacyAsync(int pharmacyId, CancellationToken ct = default);
    Task UpsertAsync(int pharmacyId, UpsertInventoryRequest request, CancellationToken ct = default);
    Task RemoveAsync(int pharmacyId, int medicineId, CancellationToken ct = default);
    Task UpdateQuantityAsync(int pharmacyId, int medicineId, int quantity, CancellationToken ct = default);
}
```

**Used by:** Pharmacist inventory management pages.

---

### 1.5 `IAdminService`

```csharp
public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken ct = default);
    Task<AppUserDto> CreatePharmacistAccountAsync(CreatePharmacistRequest request, CancellationToken ct = default);
}
```

**Used by:** Admin dashboard + pharmacy creation flow (creates pharmacy + pharmacist account together).

---

## 2. Request Types

### 2.1 `SearchRequest`
```csharp
public record SearchRequest(
    string Query,
    double? UserLatitude,
    double? UserLongitude,
    SortMode SortMode = SortMode.Distance
);
```

### 2.2 `CreatePharmacyRequest`
```csharp
public record CreatePharmacyRequest(
    string Name,
    string Address,
    double Latitude,
    double Longitude,
    string? Phone,
    string? WorkingHours,
    string PharmacistEmail,    // account to create
    string PharmacistPassword
);
```

### 2.3 `UpdatePharmacyRequest`
```csharp
public record UpdatePharmacyRequest(
    string Name,
    string Address,
    double Latitude,
    double Longitude,
    string? Phone,
    string? WorkingHours
);
```

### 2.4 `CreateMedicineRequest`
```csharp
public record CreateMedicineRequest(
    string Name,
    string? GenericName,
    string? NameRu,
    string? DosageForm,
    int? CategoryId,
    string? Manufacturer,
    string? Description,
    IReadOnlyList<string> Synonyms,
    int CreatedByPharmacyId
);
```

### 2.5 `UpsertInventoryRequest`
```csharp
public record UpsertInventoryRequest(
    int MedicineId,
    decimal Price,
    int Quantity
);
```

### 2.6 `CreatePharmacistRequest`
```csharp
public record CreatePharmacistRequest(
    string Email,
    string Password,
    int PharmacyId
);
```

---

## 3. DTO Types

### 3.1 Search DTOs
```csharp
public record SearchResultDto(
    MedicineCandidateDto Medicine,
    IReadOnlyList<RankedPharmacyDto> Pharmacies
);

public record MedicineCandidateDto(
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

public record RankedPharmacyDto(
    int PharmacyId,
    string PharmacyName,
    string Address,
    string? Phone,
    string? WorkingHours,
    decimal Price,
    int Quantity,
    double? DistanceKm,
    string MapsUrl
);
```

### 3.2 Pharmacy DTOs
```csharp
public record PharmacyDto(
    int Id,
    string Name,
    string Address,
    double Latitude,
    double Longitude,
    string? Phone,
    string? WorkingHours,
    bool IsActive,
    bool IsVerified,
    DateTime CreatedAt
);
```

### 3.3 Medicine DTOs
```csharp
public record MedicineAutocompleteDto(int Id, string Name, string? GenericName, string? NameRu, string? DosageForm);

public record MedicineDetailDto(
    int Id,
    string Name,
    string? GenericName,
    string? NameRu,
    string? DosageForm,
    string? Manufacturer,
    string? Description,
    string? CategoryName,
    IReadOnlyList<string> Synonyms
);

public record MedicineDto(int Id, string Name, string? GenericName, string? NameRu);

public record CategoryDto(int Id, string Name);
```

### 3.4 Inventory DTO
```csharp
public record InventoryItemDto(
    int MedicineId,
    string MedicineName,
    string? GenericName,
    string? DosageForm,
    decimal Price,
    int Quantity,
    DateTime UpdatedAt
);
```

### 3.5 Admin DTOs
```csharp
public record AdminDashboardDto(
    int TotalPharmacies,
    int ActivePharmacies,
    int TotalMedicines,
    int TotalInventoryItems
);

public record AppUserDto(int Id, string Email, string Role, int? PharmacyId);
```

---

## 4. FluentValidation Validators

| Validator | Validates |
|---|---|
| `CreatePharmacyRequestValidator` | Name required; Lat in [-90,90]; Lng in [-180,180]; valid email |
| `UpdatePharmacyRequestValidator` | Same as Create (minus account fields) |
| `CreateMedicineRequestValidator` | Name required, length ≤ 200; synonyms each ≤ 200 |
| `UpsertInventoryRequestValidator` | Price > 0; Quantity ≥ 0 |
| `CreatePharmacistRequestValidator` | Valid email; password ≥ 8 chars |

---

## 5. Repository Interfaces

```csharp
public interface IPharmacyRepository
{
    Task<IReadOnlyList<Pharmacy>> GetAllAsync(CancellationToken ct = default);
    Task<Pharmacy?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Pharmacy> AddAsync(Pharmacy pharmacy, CancellationToken ct = default);
    Task UpdateAsync(Pharmacy pharmacy, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public interface IInventoryRepository
{
    Task<IReadOnlyList<PharmacyMedicine>> GetByPharmacyAsync(int pharmacyId, CancellationToken ct = default);
    Task<IReadOnlyList<AvailablePharmacyDto>> GetAvailablePharmaciesAsync(int medicineId, CancellationToken ct = default);
    Task UpsertAsync(PharmacyMedicine item, CancellationToken ct = default);
    Task RemoveAsync(int pharmacyId, int medicineId, CancellationToken ct = default);
}

public interface ISearchRepository
{
    Task<IReadOnlyList<MedicineCandidateDto>> FindMedicinesAsync(string query, CancellationToken ct = default);
    Task<IReadOnlyList<MedicineAutocompleteDto>> AutocompleteAsync(string query, CancellationToken ct = default);
}

public interface IMedicineRepository
{
    Task<Medicine?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Medicine> AddAsync(Medicine medicine, CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default);
}
```
