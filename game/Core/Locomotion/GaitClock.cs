namespace CyberHoops.Core.Locomotion;

/// <summary>
/// The master animation clock: gait phase advanced by distance travelled, not
/// time, so footsteps always match ground speed (no skating) and everything
/// synced to it (dribble bounce, arm swing) stays deterministic.
/// Phase convention: [0,1) per stride cycle — left foot plants at 0.0, right
/// foot at 0.5.
/// </summary>
public sealed class GaitClock
{
    private double _phase;

    /// <summary>Current phase in [0,1).</summary>
    public double Phase => _phase;

    /// <summary>
    /// Advances the clock by ground distance travelled this tick.
    /// </summary>
    /// <param name="distance">Horizontal distance moved, metres. Must be ≥ 0.</param>
    /// <param name="strideFrequency">Stride cycles per metre. Must be &gt; 0.</param>
    public double Advance(double distance, double strideFrequency)
    {
        if (distance < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(distance), "Distance must be non-negative.");
        }

        if (strideFrequency <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(strideFrequency), "Stride frequency must be positive.");
        }

        _phase = (_phase + distance * strideFrequency) % 1.0;
        return _phase;
    }

    /// <summary>Resets to left-foot plant.</summary>
    public void Reset() => _phase = 0.0;
}
