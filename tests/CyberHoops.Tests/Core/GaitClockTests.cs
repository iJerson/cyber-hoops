using CyberHoops.Core.Locomotion;
using Xunit;

namespace CyberHoops.Tests.Core;

public class GaitClockTests
{
    [Fact]
    public void Advance_AccumulatesByDistance()
    {
        var clock = new GaitClock();

        clock.Advance(0.5, 0.55);

        Assert.Equal(0.275, clock.Phase, 10);
    }

    [Fact]
    public void Advance_WrapsAtOne()
    {
        var clock = new GaitClock();

        clock.Advance(2.5, 1.0); // 2.5 cycles

        Assert.Equal(0.5, clock.Phase, 10);
    }

    [Fact]
    public void Advance_ZeroDistance_HoldsPhase()
    {
        var clock = new GaitClock();
        clock.Advance(0.3, 1.0);

        clock.Advance(0.0, 1.0);

        Assert.Equal(0.3, clock.Phase, 10);
    }

    [Fact]
    public void Advance_IsDeterministic()
    {
        var a = new GaitClock();
        var b = new GaitClock();
        for (var i = 0; i < 200; i++)
        {
            a.Advance(0.0833, 0.55);
            b.Advance(0.0833, 0.55);
        }

        Assert.Equal(a.Phase, b.Phase);
    }

    [Fact]
    public void Advance_NegativeDistance_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GaitClock().Advance(-0.1, 1.0));
    }

    [Fact]
    public void Advance_NonPositiveFrequency_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GaitClock().Advance(0.1, 0.0));
    }

    [Fact]
    public void Reset_ReturnsToLeftPlant()
    {
        var clock = new GaitClock();
        clock.Advance(0.7, 1.0);

        clock.Reset();

        Assert.Equal(0.0, clock.Phase);
    }
}
