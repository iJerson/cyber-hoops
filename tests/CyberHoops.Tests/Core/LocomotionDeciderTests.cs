using CyberHoops.Core.Character;
using Xunit;

namespace CyberHoops.Tests.Core;

public class LocomotionDeciderTests
{
    private static readonly LocomotionThresholds T = new(IdleMaxSpeed: 0.02, WalkMaxSpeed: 0.5, StopMinSpeed: 0.15);

    private static LocomotionSnapshot S(
        double speed = 0.0, bool hasBall = false, bool protecting = false, bool decelerating = false) =>
        new(speed, hasBall, protecting, decelerating);

    [Theory]
    [InlineData(0.0, LocomotionStates.Idle)]
    [InlineData(0.3, LocomotionStates.Walk)]
    [InlineData(0.8, LocomotionStates.Run)]
    public void NoBall_SpeedStates(double speed, string expected)
    {
        Assert.Equal(expected, LocomotionDecider.Decide(S(speed), T));
    }

    [Theory]
    [InlineData(0.0, LocomotionStates.DribbleIdle)]
    [InlineData(0.3, LocomotionStates.DribbleWalk)]
    [InlineData(0.8, LocomotionStates.DribbleRun)]
    public void Ball_SpeedStates(double speed, string expected)
    {
        Assert.Equal(expected, LocomotionDecider.Decide(S(speed, hasBall: true), T));
    }

    [Fact]
    public void Protecting_WithBall_Wins()
    {
        Assert.Equal(LocomotionStates.Protect, LocomotionDecider.Decide(S(0.8, hasBall: true, protecting: true), T));
    }

    [Fact]
    public void Protecting_WithoutBall_Ignored()
    {
        Assert.Equal(LocomotionStates.Run, LocomotionDecider.Decide(S(0.8, protecting: true), T));
    }

    [Fact]
    public void Decelerating_AboveStopSpeed_Stops()
    {
        Assert.Equal(LocomotionStates.Stop, LocomotionDecider.Decide(S(0.4, hasBall: true, decelerating: true), T));
    }

    [Fact]
    public void Decelerating_BelowStopSpeed_FallsThrough()
    {
        Assert.Equal(LocomotionStates.DribbleIdle, LocomotionDecider.Decide(S(0.01, hasBall: true, decelerating: true), T));
    }

    [Fact]
    public void Protect_BeatsStop()
    {
        Assert.Equal(LocomotionStates.Protect, LocomotionDecider.Decide(S(0.4, hasBall: true, protecting: true, decelerating: true), T));
    }

    [Fact]
    public void BoundaryValues_AreInclusiveLow()
    {
        Assert.Equal(LocomotionStates.Idle, LocomotionDecider.Decide(S(0.02), T));
        Assert.Equal(LocomotionStates.Walk, LocomotionDecider.Decide(S(0.5), T));
    }
}
