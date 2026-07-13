namespace CyberHoops.Core.Dribble;

/// <summary>
/// Bounce phase source for a live dribble. Two modes, one continuous phase:
/// while moving the phase advances with the gait clock (bounce locked to
/// footsteps — the sync invariant); at rest it advances with time at the
/// gravity-derived period times an arcade tempo multiplier.
/// Contact is at phase 0.0/1.0, apex at 0.5 (same convention as
/// <see cref="DribbleCycle"/>).
/// </summary>
public sealed class BounceClock
{
    private double _phase;

    public double Phase => _phase;

    /// <summary>True when the last Advance crossed a floor contact (phase wrap).</summary>
    public bool ContactThisTick { get; private set; }

    /// <summary>Advance locked to the gait: bounce cycles per gait cycle.</summary>
    public double AdvanceByGait(double gaitPhaseDelta, double bouncesPerGaitCycle)
    {
        if (gaitPhaseDelta < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(gaitPhaseDelta), "Gait delta must be non-negative.");
        }

        if (bouncesPerGaitCycle <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(bouncesPerGaitCycle), "Bounce rate must be positive.");
        }

        return Step(gaitPhaseDelta * bouncesPerGaitCycle);
    }

    /// <summary>Advance by time at the physical bounce period for the given apex height, scaled by tempo.</summary>
    public double AdvanceByTime(double delta, double apexHeight, double gravity, double tempoMultiplier)
    {
        if (delta < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(delta), "Delta must be non-negative.");
        }

        if (tempoMultiplier <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(tempoMultiplier), "Tempo multiplier must be positive.");
        }

        var period = DribbleCycle.PeriodFor(apexHeight, gravity) / tempoMultiplier;
        return Step(delta / period);
    }

    /// <summary>Ball height above floor contact for the current phase.</summary>
    public double HeightFor(double apexHeight) => DribbleCycle.HeightAt(_phase, apexHeight);

    public void Reset()
    {
        _phase = 0.0;
        ContactThisTick = false;
    }

    private double Step(double phaseDelta)
    {
        var next = _phase + phaseDelta;
        ContactThisTick = next >= 1.0;
        _phase = next % 1.0;
        return _phase;
    }
}
