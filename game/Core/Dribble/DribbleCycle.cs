namespace CyberHoops.Core.Dribble;

/// <summary>
/// Deterministic dribble bounce cycle. Pure math — no engine types — so the
/// rhythm is unit-testable and identical for player and AI.
/// The ball follows a normalized parabola: floor contact at phase 0 and 1,
/// peak height at phase 0.5.
/// </summary>
public sealed class DribbleCycle
{
    private double _phase;

    /// <summary>Current position in the bounce cycle, in [0, 1).</summary>
    public double Phase => _phase;

    /// <summary>
    /// Advances the cycle and returns the ball height above floor contact.
    /// </summary>
    /// <param name="delta">Tick duration in seconds.</param>
    /// <param name="bounceHeight">Peak height of the bounce, in metres.</param>
    /// <param name="gravity">Gravity magnitude, in m/s². Sets the cycle period like a real bounce.</param>
    public double Advance(double delta, double bounceHeight, double gravity)
    {
        var period = PeriodFor(bounceHeight, gravity);
        _phase = (_phase + delta / period) % 1.0;
        return HeightAt(_phase, bounceHeight);
    }

    /// <summary>Resets the cycle to floor contact.</summary>
    public void Reset() => _phase = 0.0;

    /// <summary>Time for one full bounce (up and back down), in seconds.</summary>
    public static double PeriodFor(double bounceHeight, double gravity)
    {
        if (bounceHeight <= 0.0 || gravity <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(bounceHeight), "Bounce height and gravity must be positive.");
        }

        return 2.0 * Math.Sqrt(2.0 * bounceHeight / gravity);
    }

    /// <summary>Height above floor at a given phase in [0, 1): 0 at the ends, peak at 0.5.</summary>
    public static double HeightAt(double phase, double bounceHeight) =>
        bounceHeight * 4.0 * phase * (1.0 - phase);
}
