using CyberHoops.Core.Dribble;
using Xunit;

namespace CyberHoops.Tests.Core;

public class BounceClockTests
{
    private const double Gravity = 9.8;

    [Fact]
    public void AdvanceByGait_LocksToGait()
    {
        var clock = new BounceClock();

        clock.AdvanceByGait(0.25, 1.0); // quarter gait cycle, 1 bounce per cycle

        Assert.Equal(0.25, clock.Phase, 10);
    }

    [Fact]
    public void AdvanceByGait_HalfRate_TwoGaitCyclesPerBounce()
    {
        var clock = new BounceClock();

        clock.AdvanceByGait(1.0, 0.5);

        Assert.Equal(0.5, clock.Phase, 10);
    }

    [Fact]
    public void AdvanceByGait_ZeroGaitDelta_HoldsPhase()
    {
        var clock = new BounceClock();
        clock.AdvanceByGait(0.3, 1.0);

        clock.AdvanceByGait(0.0, 1.0);

        Assert.Equal(0.3, clock.Phase, 10);
        Assert.False(clock.ContactThisTick);
    }

    [Fact]
    public void Contact_ReportedOnWrap()
    {
        var clock = new BounceClock();
        clock.AdvanceByGait(0.9, 1.0);
        Assert.False(clock.ContactThisTick);

        clock.AdvanceByGait(0.2, 1.0);

        Assert.True(clock.ContactThisTick);
        Assert.Equal(0.1, clock.Phase, 10);
    }

    [Fact]
    public void AdvanceByTime_MatchesPhysicalPeriodScaledByTempo()
    {
        var clock = new BounceClock();
        var period = DribbleCycle.PeriodFor(0.9, Gravity);

        // With tempo 2x, half the physical period completes a full cycle.
        clock.AdvanceByTime(period / 2.0, 0.9, Gravity, 2.0);

        Assert.True(clock.ContactThisTick);
        Assert.Equal(0.0, clock.Phase, 8);
    }

    [Fact]
    public void HeightFor_UsesSharedParabola()
    {
        var clock = new BounceClock();
        clock.AdvanceByGait(0.5, 1.0);

        Assert.Equal(0.9, clock.HeightFor(0.9), 10);
    }

    [Fact]
    public void ModeSwitch_PhaseIsContinuous()
    {
        var clock = new BounceClock();
        clock.AdvanceByGait(0.4, 1.0);

        clock.AdvanceByTime(0.001, 0.9, Gravity, 1.0); // tiny time step

        Assert.InRange(clock.Phase, 0.4, 0.41); // no jump on mode change
    }

    [Fact]
    public void InvalidInputs_Throw()
    {
        var clock = new BounceClock();
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.AdvanceByGait(-0.1, 1.0));
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.AdvanceByGait(0.1, 0.0));
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.AdvanceByTime(-0.1, 0.9, Gravity, 1.0));
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.AdvanceByTime(0.1, 0.9, Gravity, 0.0));
    }
}
