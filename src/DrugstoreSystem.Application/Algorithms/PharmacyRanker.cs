using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Domain.Enums;

namespace DrugstoreSystem.Application.Algorithms;

public static class PharmacyRanker
{
    public static IReadOnlyList<RankedPharmacyDto> Rank(
        IEnumerable<PharmacyResultDto> pharmacies,
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
            MapsUrl: FormattableString.Invariant($"https://www.google.com/maps/search/?api=1&query={p.Latitude},{p.Longitude}")
        ));

        return sortMode switch
        {
            SortMode.Price    => ranked.OrderBy(p => p.Price).ToList(),
            SortMode.Distance => ranked.OrderBy(p => p.DistanceKm ?? double.MaxValue).ToList(),
            _                 => throw new ArgumentOutOfRangeException(nameof(sortMode))
        };
    }
}
