using DrugstoreSystem.Application.Algorithms;
using FluentAssertions;

namespace DrugstoreSystem.UnitTests;

public class HaversineCalculatorTests
{
    [Fact]
    public void Distance_SamePoint_ReturnsZero()
    {
        var d = HaversineCalculator.Distance(41.2995, 69.2401, 41.2995, 69.2401);
        d.Should().BeApproximately(0.0, 0.01);
    }

    [Fact]
    public void Distance_TashkentToSamarkand_IsApprox266Km()
    {
        var d = HaversineCalculator.Distance(41.2995, 69.2401, 39.6542, 66.9597);
        d.Should().BeApproximately(265.8, 2.0);
    }

    [Fact]
    public void Distance_TashkentToFergana_IsApprox238Km()
    {
        var d = HaversineCalculator.Distance(41.2995, 69.2401, 40.3764, 71.7971);
        d.Should().BeApproximately(238.3, 2.0);
    }

    [Fact]
    public void Distance_TashkentToMoscow_IsApprox2791Km()
    {
        var d = HaversineCalculator.Distance(41.2995, 69.2401, 55.7558, 37.6173);
        d.Should().BeApproximately(2791.0, 10.0);
    }

    [Fact]
    public void Distance_AntipodalPoints_IsApproxHalfCircumference()
    {
        var d = HaversineCalculator.Distance(0.0, 0.0, 0.0, 180.0);
        d.Should().BeApproximately(20015.0, 50.0);
    }
}
