# Optimization Algorithm — DrugstoreSystem

This document specifies the pharmacy-ranking algorithm — the **second academic contribution** of this thesis. After the search algorithm returns matching medicines and their stocked pharmacies, this algorithm determines the order in which pharmacies are presented to the user.

**Implementation class:** `DrugstoreSystem.Application/Algorithms/HaversineCalculator.cs`
**Used by:** `DrugstoreSystem.Application/Services/SearchService.cs`
**Skill file:** [.claude/skills/optimization-algorithm/SKILL.md](../.claude/skills/optimization-algorithm/SKILL.md)

---

## 1. Problem Statement

Given:
- A set of pharmacies `P = {p₁, p₂, ..., pₙ}` that all stock a requested medicine
- The user's coordinates `(φ_u, λ_u)` (latitude, longitude)
- Each pharmacy `pᵢ` has coordinates `(φᵢ, λᵢ)`, price `priceᵢ`, and quantity `qtyᵢ`

**Pre-condition:** All `pᵢ` satisfy `qtyᵢ > 0` AND `is_active = true` (availability filter applied before this step).

**Output:** An ordered list of pharmacies — either by ascending geodesic distance to the user or by ascending price, based on `SortMode`.

---

## 2. SortMode Enum

```csharp
public enum SortMode
{
    Distance = 0,   // default — closest pharmacy first
    Price    = 1    // cheapest pharmacy first
}
```

The user selects `SortMode` via a toggle on the search page. The default is `Distance`.

---

## 3. Haversine Formula

The Haversine formula computes the **great-circle distance** (shortest path over the Earth's surface) between two points given their latitude and longitude in degrees.

### Mathematical definition

```
a = sin²(Δφ/2) + cos(φ₁) · cos(φ₂) · sin²(Δλ/2)
c = 2 · atan2(√a, √(1−a))
d = R · c
```

Where:
- `φ` = latitude in radians
- `λ` = longitude in radians
- `Δφ = φ₂ − φ₁`
- `Δλ = λ₂ − λ₁`
- `R = 6371 km` (mean Earth radius)
- `d` = distance in kilometres

### C# Implementation

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

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
```

This is a **pure static function** with no dependencies — directly unit-testable.

---

## 4. Ranking Logic

```csharp
// DrugstoreSystem.Application/Services/PharmacyRanker.cs
public static class PharmacyRanker
{
    public static IReadOnlyList<RankedPharmacyDto> Rank(
        IEnumerable<AvailablePharmacyDto> pharmacies,
        double? userLat,
        double? userLng,
        SortMode sortMode)
    {
        var withDistance = pharmacies.Select(p => new RankedPharmacyDto(
            PharmacyId:    p.PharmacyId,
            PharmacyName:  p.PharmacyName,
            Address:       p.Address,
            Phone:         p.Phone,
            WorkingHours:  p.WorkingHours,
            Price:         p.Price,
            Quantity:      p.Quantity,
            DistanceKm:    (userLat.HasValue && userLng.HasValue)
                               ? Math.Round(HaversineCalculator.Distance(userLat.Value, userLng.Value, p.Latitude, p.Longitude), 1)
                               : (double?)null,
            MapsUrl:       BuildMapsUrl(p.Latitude, p.Longitude)
        ));

        return sortMode switch
        {
            SortMode.Price    => withDistance.OrderBy(p => p.Price).ToList(),
            SortMode.Distance => withDistance
                                    .OrderBy(p => p.DistanceKm ?? double.MaxValue)
                                    .ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(sortMode))
        };
    }

    private static string BuildMapsUrl(double lat, double lng)
        => $"https://www.google.com/maps/search/?api=1&query={lat},{lng}";
}
```

**Fallback rule:** If `SortMode = Distance` but user location is unavailable, `DistanceKm = null` and pharmacies are sorted by `double.MaxValue` (all tied) — effectively unordered but not crashing. The UI shows "Masofa noma'lum" instead of a distance value.

---

## 5. Google Maps URL Format

```
https://www.google.com/maps/search/?api=1&query={latitude},{longitude}
```

Example for Tashkent city center:
```
https://www.google.com/maps/search/?api=1&query=41.2995,69.2401
```

This opens Google Maps in a new browser tab and shows a pin at the pharmacy's coordinates. No API key required for this redirect format.

---

## 6. User Location Flow

```
Page load
   │
   ├─► JS interop: navigator.geolocation.getCurrentPosition()
   │       │
   │       ├─► SUCCESS → store (lat, lng) in component state
   │       └─► DENIED/UNAVAILABLE → show manual location input field
   │
   └─► User can override at any time:
           Types city/address → geocode (Nominatim/OpenStreetMap free API)
           → update (lat, lng) in component state
           → re-run search with new coordinates
```

**JS interop call (in SearchPage.razor):**
```javascript
// wwwroot/js/geolocation.js
window.getLocation = () => new Promise((resolve, reject) => {
    if (!navigator.geolocation) { reject('not_supported'); return; }
    navigator.geolocation.getCurrentPosition(
        pos => resolve({ lat: pos.coords.latitude, lng: pos.coords.longitude }),
        err => reject(err.message),
        { timeout: 5000 }
    );
});
```

```csharp
// In SearchPage.razor code-behind
var loc = await JS.InvokeAsync<LocationResult>("getLocation");
UserLat = loc.Lat;
UserLng = loc.Lng;
```

---

## 7. Test Cases for HaversineCalculator

Unit tests in `DrugstoreSystem.UnitTests/HaversineCalculatorTests.cs`:

| Test | Input | Expected output |
|---|---|---|
| Same point | (41.2995, 69.2401) → (41.2995, 69.2401) | 0.0 km |
| Tashkent → Samarkand | (41.2995, 69.2401) → (39.6542, 66.9597) | ~281 km |
| Tashkent → Fergana | (41.2995, 69.2401) → (40.3764, 71.7971) | ~234 km |
| Tashkent → Moscow | (41.2995, 69.2401) → (55.7558, 37.6173) | ~3534 km |
| Antipodal points | (0, 0) → (0, 180) | ~20,015 km (half circumference) |

Tolerance: ±1 km for long distances, ±0.1 km for short distances.

---

## 8. Display Format

| Value | Display |
|---|---|
| `DistanceKm = 0.4` | "0.4 km" |
| `DistanceKm = 1.8` | "1.8 km" |
| `DistanceKm = 12.5` | "12.5 km" |
| `DistanceKm = null` | "Masofa noma'lum" |

Rounded to 1 decimal place (`Math.Round(d, 1)`).

---

## 9. Complexity Analysis (for thesis)

| Operation | Complexity |
|---|---|
| Haversine per pharmacy | O(1) — constant time |
| Ranking N pharmacies | O(N log N) — sort |
| Full algorithm (M medicines × N pharmacies each) | O(M × N log N) |

For the demo dataset (M ≤ 20, N ≤ 10): total ranking time < 1 ms. The search SQL query dominates latency.
