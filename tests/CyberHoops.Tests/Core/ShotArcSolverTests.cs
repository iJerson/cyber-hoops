using CyberHoops.Core.Shooting;
using Xunit;

namespace CyberHoops.Tests.Core;

public class ShotArcSolverTests
{
    private const double Gravity = 9.8;

    private static double SimulateLandingHeight(ShotSolution s, double horizontalDistance, double gravity)
    {
        // Analytic position at the moment the projectile has covered the horizontal distance.
        var t = horizontalDistance / s.HorizontalSpeed;
        return s.VerticalSpeed * t - 0.5 * gravity * t * t;
    }

    [Theory]
    [InlineData(5.0, 1.0, 1.2)]   // typical jump shot: 5m out, rim 1m above release
    [InlineData(1.0, 1.5, 0.5)]   // close layup-ish arc
    [InlineData(10.0, 0.0, 2.0)]  // long flat-delta shot
    [InlineData(4.0, -0.5, 1.0)]  // shooting downhill (target below release)
    public void Solve_ProjectileLandsOnTarget(double distance, double verticalDelta, double clearance)
    {
        var solution = ShotArcSolver.Solve(distance, verticalDelta, clearance, Gravity);

        var landingHeight = SimulateLandingHeight(solution, distance, Gravity);

        Assert.Equal(verticalDelta, landingHeight, 6);
    }

    [Fact]
    public void Solve_ApexClearsHigherEndpointByClearance()
    {
        var solution = ShotArcSolver.Solve(5.0, 1.0, 1.2, Gravity);

        // Apex of v²/2g above release must equal max(0, delta) + clearance.
        var apex = solution.VerticalSpeed * solution.VerticalSpeed / (2.0 * Gravity);

        Assert.Equal(1.0 + 1.2, apex, 6);
    }

    [Fact]
    public void Solve_FlightTimeMatchesComponents()
    {
        var solution = ShotArcSolver.Solve(6.0, 0.8, 1.0, Gravity);

        Assert.Equal(6.0 / solution.HorizontalSpeed, solution.FlightTime, 6);
    }

    [Fact]
    public void Solve_IsDeterministic()
    {
        var a = ShotArcSolver.Solve(5.0, 1.0, 1.2, Gravity);
        var b = ShotArcSolver.Solve(5.0, 1.0, 1.2, Gravity);

        Assert.Equal(a, b);
    }

    [Theory]
    [InlineData(0.0, 1.0, 1.0)]
    [InlineData(-1.0, 1.0, 1.0)]
    [InlineData(5.0, 1.0, 0.0)]
    [InlineData(5.0, 1.0, -0.2)]
    public void Solve_InvalidInputs_Throw(double distance, double delta, double clearance)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ShotArcSolver.Solve(distance, delta, clearance, Gravity));
    }

    [Fact]
    public void Solve_ZeroGravity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ShotArcSolver.Solve(5.0, 1.0, 1.0, 0.0));
    }
}
