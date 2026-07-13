namespace CyberHoops.Core.Character;

/// <summary>Locomotion state names (animation spec sections A–E, H).</summary>
public static class LocomotionStates
{
    public const string Idle = "Idle";
    public const string Walk = "Walk";
    public const string Run = "Run";
    public const string DribbleIdle = "DribbleIdle";
    public const string DribbleWalk = "DribbleWalk";
    public const string DribbleRun = "DribbleRun";
    public const string Protect = "Protect";
    public const string Stop = "Stop";
}

/// <summary>Everything the locomotion decision needs, pre-digested to plain values.</summary>
public readonly record struct LocomotionSnapshot(
    double NormalizedSpeed,
    bool HasBall,
    bool Protecting,
    bool Decelerating);

/// <summary>Speed boundaries for the thin speed states. Values come from a resource.</summary>
public readonly record struct LocomotionThresholds(
    double IdleMaxSpeed,
    double WalkMaxSpeed,
    double StopMinSpeed);

/// <summary>
/// Pure locomotion-state decision (spec: Player-State-Machine.md, locomotion
/// rows). Sprint is not a separate state — Run's amplitude scales with
/// normalized speed. Protect hysteresis is resolved upstream (DribbleComponent)
/// so this function stays stateless and deterministic.
/// </summary>
public static class LocomotionDecider
{
    public static string Decide(in LocomotionSnapshot s, in LocomotionThresholds t)
    {
        if (s.HasBall && s.Protecting)
        {
            return LocomotionStates.Protect;
        }

        if (s.Decelerating && s.NormalizedSpeed > t.StopMinSpeed)
        {
            return LocomotionStates.Stop;
        }

        if (s.NormalizedSpeed <= t.IdleMaxSpeed)
        {
            return s.HasBall ? LocomotionStates.DribbleIdle : LocomotionStates.Idle;
        }

        if (s.NormalizedSpeed <= t.WalkMaxSpeed)
        {
            return s.HasBall ? LocomotionStates.DribbleWalk : LocomotionStates.Walk;
        }

        return s.HasBall ? LocomotionStates.DribbleRun : LocomotionStates.Run;
    }
}
