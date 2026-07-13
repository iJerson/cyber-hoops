using CyberHoops.Core.Dribble;
using Xunit;

namespace CyberHoops.Tests.Core;

public class DribbleCycleTests
{
    private const double Gravity = 9.8;

    [Fact]
    public void HeightAt_FloorContactAtCycleEnds()
    {
        Assert.Equal(0.0, DribbleCycle.HeightAt(0.0, 1.0));
        Assert.Equal(0.0, DribbleCycle.HeightAt(1.0, 1.0), 10);
    }

    [Fact]
    public void HeightAt_PeaksAtMidCycle()
    {
        Assert.Equal(0.9, DribbleCycle.HeightAt(0.5, 0.9), 10);
    }

    [Fact]
    public void PeriodFor_MatchesFreeFallTime()
    {
        // Drop from 1m under 9.8 m/s²: t = sqrt(2h/g) ≈ 0.4518s down, same up.
        var period = DribbleCycle.PeriodFor(1.0, Gravity);
        Assert.Equal(2.0 * Math.Sqrt(2.0 / Gravity), period, 10);
    }

    [Fact]
    public void PeriodFor_NonPositiveInputs_Throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DribbleCycle.PeriodFor(0.0, Gravity));
        Assert.Throws<ArgumentOutOfRangeException>(() => DribbleCycle.PeriodFor(1.0, 0.0));
    }

    [Fact]
    public void Advance_IsDeterministic()
    {
        var a = new DribbleCycle();
        var b = new DribbleCycle();
        for (var i = 0; i < 100; i++)
        {
            Assert.Equal(a.Advance(1.0 / 60.0, 0.8, Gravity), b.Advance(1.0 / 60.0, 0.8, Gravity));
        }

        Assert.Equal(a.Phase, b.Phase);
    }

    [Fact]
    public void Advance_WrapsPhase()
    {
        var cycle = new DribbleCycle();
        var period = DribbleCycle.PeriodFor(0.8, Gravity);

        cycle.Advance(period * 1.25, 0.8, Gravity);

        Assert.InRange(cycle.Phase, 0.0, 1.0);
        Assert.Equal(0.25, cycle.Phase, 10);
    }

    [Fact]
    public void Reset_ReturnsToFloorContact()
    {
        var cycle = new DribbleCycle();
        cycle.Advance(0.1, 0.8, Gravity);

        cycle.Reset();

        Assert.Equal(0.0, cycle.Phase);
    }
}
