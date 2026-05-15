---
name: optimization-algorithm
description: Implementation guide for the Haversine pharmacy-ranking algorithm and SortMode toggle. Open before implementing or modifying HaversineCalculator, PharmacyRanker, or search result ranking.
---

# Optimization Algorithm — Implementation Guide

Full specification: [docs/optimization-algorithm.md](../../docs/optimization-algorithm.md)

---

## 1. Architecture Location

```
DrugstoreSystem.Application
  └── Algorithms/HaversineCalculator.cs   — pure static, no DI (unit-testable)
  └── Algorithms/PharmacyRanker.cs        — pure static, uses HaversineCalculator
  └── Services/SearchService.cs           — calls PharmacyRanker after search
```

Both `HaversineCalculator` and `PharmacyRanker` are **static classes** — no constructor injection, no interfaces. They contain pure functions (same input → same output, no side effects). This makes them trivially unit-testable.

---

## 2. HaversineCalculator — Final Implementation

```csharp
// DrugstoreSystem.Application/Algorithms/HaversineCalculator.cs
public static class HaversineCalculator
{
    private const double EarthRadiusKm = 6371.0;

    public static double Distance(double lat1, double lng1, double lat2, double lng2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLng = ToRadians(lng2 - lng1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))
              * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

        var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
        return EarthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
```

Do not modify this. The formula is mathematically fixed — only the constant `EarthRadiusKm` could theoretically change (it won't).

---

## 3. PharmacyRanker — Final Implementation

```csharp
// DrugstoreSystem.Application/Algorithms/PharmacyRanker.cs
public static class PharmacyRanker
{
    public static IReadOnlyList<RankedPharmacyDto> Rank(
        IEnumerable<AvailablePharmacyDto> pharmacies,
        double? userLat,
        double? userLng,
        SortMode sortMode)
    {
        var ranked = pharmacies.Select(p => new RankedPharmacyDto(
            PharmacyId:   p.PharmacyId,
            PharmacyName: p.PharmacyName,
            Address:      p.Address,
            Phone:        p.Phone,
            WorkingHours: p.WorkingHours,
            Price:        p.Price,
            Quantity:     p.Quantity,
            DistanceKm:   (userLat.HasValue && userLng.HasValue)
                              ? Math.Round(HaversineCalculator.Distance(
                                    userLat.Value, userLng.Value,
                                    p.Latitude, p.Longitude), 1)
                              : (double?)null,
            MapsUrl:      $"https://www.google.com/maps/search/?api=1&query={p.Latitude},{p.Longitude}"
        ));

        return sortMode switch
        {
            SortMode.Price    => ranked.OrderBy(p => p.Price).ToList(),
            SortMode.Distance => ranked.OrderBy(p => p.DistanceKm ?? double.MaxValue).ToList(),
            _                 => throw new ArgumentOutOfRangeException(nameof(sortMode))
        };
    }
}
```

---

## 4. Geolocation JS Interop

```javascript
// wwwroot/js/geolocation.js
window.getLocation = () => new Promise((resolve, reject) => {
    if (!navigator.geolocation) {
        reject('not_supported');
        return;
    }
    navigator.geolocation.getCurrentPosition(
        pos => resolve({ lat: pos.coords.latitude, lng: pos.coords.longitude }),
        err => reject(err.message),
        { timeout: 5000, maximumAge: 60000 }
    );
});
```

```csharp
// SearchPage.razor (code-behind)
[Inject] IJSRuntime JS { get; set; } = default!;

private double? _userLat;
private double? _userLng;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (!firstRender) return;
    try
    {
        var loc = await JS.InvokeAsync<LocationResult>("getLocation");
        _userLat = loc.Lat;
        _userLng = loc.Lng;
        StateHasChanged();
    }
    catch
    {
        // Geolocation denied or unavailable — show manual input
        _showManualLocation = true;
        StateHasChanged();
    }
}

private record LocationResult(double Lat, double Lng);
```

---

## 5. Manual Location Override (Nominatim)

When user types a city name (e.g., "Farg'ona"), geocode it with Nominatim:

```csharp
// In SearchPage.razor
private async Task GeocodeCityAsync(string cityName)
{
    var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(cityName)}&format=json&limit=1";
    var results = await Http.GetFromJsonAsync<NominatimResult[]>(url);
    if (results?.Length > 0)
    {
        _userLat = double.Parse(results[0].Lat, CultureInfo.InvariantCulture);
        _userLng = double.Parse(results[0].Lon, CultureInfo.InvariantCulture);
    }
}

private record NominatimResult(string Lat, string Lon, string DisplayName);
```

Add `User-Agent` header to the HttpClient (Nominatim requirement):
```csharp
// Program.cs
builder.Services.AddHttpClient("nominatim", c =>
    c.DefaultRequestHeaders.UserAgent.ParseAdd("DrugstoreSystem/1.0"));
```

---

## 6. Sort Toggle in Blazor

```razor
@* SearchPage.razor *@
<MudToggleGroup T="SortMode" @bind-Value="_sortMode" SelectionMode="SelectionMode.SingleSelection"
                Color="Color.Primary" CheckMark>
    <MudToggleItem Value="SortMode.Distance">
        <MudIcon Icon="@Icons.Material.Filled.NearMe" Size="Size.Small" Class="mr-1" />
        Yaqinlik
    </MudToggleItem>
    <MudToggleItem Value="SortMode.Price">
        <MudIcon Icon="@Icons.Material.Filled.AttachMoney" Size="Size.Small" Class="mr-1" />
        Narx
    </MudToggleItem>
</MudToggleGroup>
```

When `_sortMode` changes:
- **Do NOT re-run the search query** (no DB call)
- Call `PharmacyRanker.Rank(existingPharmacies, _userLat, _userLng, _sortMode)` in-memory
- `StateHasChanged()` to re-render

---

## 7. Unit Test Structure

```csharp
// DrugstoreSystem.UnitTests/HaversineCalculatorTests.cs
public class HaversineCalculatorTests
{
    [Fact]
    public void Distance_SamePoint_ReturnsZero()
    {
        var d = HaversineCalculator.Distance(41.2995, 69.2401, 41.2995, 69.2401);
        d.Should().BeApproximately(0.0, 0.01);
    }

    [Fact]
    public void Distance_TashkentToSamarkand_IsApprox281Km()
    {
        var d = HaversineCalculator.Distance(41.2995, 69.2401, 39.6542, 66.9597);
        d.Should().BeApproximately(281.0, 2.0);
    }

    [Fact]
    public void Distance_TashkentToFergana_IsApprox234Km()
    {
        var d = HaversineCalculator.Distance(41.2995, 69.2401, 40.3764, 71.7971);
        d.Should().BeApproximately(234.0, 2.0);
    }

    [Fact]
    public void Distance_TashkentToMoscow_IsApprox3534Km()
    {
        var d = HaversineCalculator.Distance(41.2995, 69.2401, 55.7558, 37.6173);
        d.Should().BeApproximately(3534.0, 10.0);
    }

    [Fact]
    public void Distance_AntipodalPoints_IsApproxHalfCircumference()
    {
        var d = HaversineCalculator.Distance(0.0, 0.0, 0.0, 180.0);
        d.Should().BeApproximately(20015.0, 50.0);
    }
}
```

---

## 8. Common Mistakes to Avoid

| Mistake | Correct approach |
|---|---|
| Applying Haversine in Infrastructure (DB layer) | Haversine lives in Application — pure C# math, no DB |
| Injecting `HaversineCalculator` as a service | It's a `static class` — call directly: `HaversineCalculator.Distance(...)` |
| Re-running search query on sort toggle | Sort in-memory — no DB call needed |
| Forgetting `double.MaxValue` fallback when location is null | `DistanceKm ?? double.MaxValue` in `OrderBy` |
| Displaying 6 decimal places for distance | Always `Math.Round(d, 1)` — "1.8 km" not "1.834521 km" |
