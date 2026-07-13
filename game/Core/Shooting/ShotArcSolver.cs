namespace CyberHoops.Core.Shooting;

/// <summary>
/// Launch velocity for a ballistic shot, split into horizontal speed along the
/// ground direction to the target and vertical speed.
/// </summary>
public readonly record struct ShotSolution(double HorizontalSpeed, double VerticalSpeed, double FlightTime);

/// <summary>
/// Solves launch velocity for a projectile that must clear an apex above both
/// endpoints and land on the target. Pure math — engine-free, deterministic,
/// shared by player shots now and AI shots in M4.
/// </summary>
public static class ShotArcSolver
{
    /// <summary>
    /// Solves the arc.
    /// </summary>
    /// <param name="horizontalDistance">Ground-plane distance to the target, metres. Must be &gt; 0.</param>
    /// <param name="verticalDelta">Target height minus release height, metres.</param>
    /// <param name="arcClearance">Apex height above the higher endpoint, metres. Must be &gt; 0.</param>
    /// <param name="gravity">Gravity magnitude, m/s². Must be &gt; 0.</param>
    public static ShotSolution Solve(double horizontalDistance, double verticalDelta, double arcClearance, double gravity)
    {
        if (horizontalDistance <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(horizontalDistance), "Horizontal distance must be positive.");
        }

        if (arcClearance <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(arcClearance), "Arc clearance must be positive.");
        }

        if (gravity <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(gravity), "Gravity must be positive.");
        }

        // Apex sits arcClearance above the higher of release point (y=0) and target (y=verticalDelta).
        var apex = Math.Max(0.0, verticalDelta) + arcClearance;
        var riseTime = Math.Sqrt(2.0 * apex / gravity);
        var fallTime = Math.Sqrt(2.0 * (apex - verticalDelta) / gravity);
        var flightTime = riseTime + fallTime;

        var verticalSpeed = gravity * riseTime;
        var horizontalSpeed = horizontalDistance / flightTime;

        return new ShotSolution(horizontalSpeed, verticalSpeed, flightTime);
    }
}
