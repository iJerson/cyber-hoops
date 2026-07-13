namespace CyberHoops.Core.AI;

/// <summary>
/// Everything the AI needs to know about the world this tick, pre-digested to
/// plain values so the decision stays engine-free and deterministic.
/// </summary>
public readonly record struct AIWorldSnapshot(
    bool BallIsFree,
    bool SelfHasBall,
    bool OpponentHasBall,
    double DistanceToHoop,
    double ShootRange);

/// <summary>
/// State names from the AI overview doc (Phase 1 finite state machine).
/// </summary>
public static class AIStateNames
{
    public const string Idle = "Idle";
    public const string ChaseBall = "ChaseBall";
    public const string Attack = "Attack";
    public const string Shoot = "Shoot";
    public const string Defend = "Defend";
}

/// <summary>
/// Pure decision function for the Phase 1 FSM: snapshot in, state name out.
/// No engine types, no randomness — unit-testable and identical every tick.
/// </summary>
public static class AIDecider
{
    public static string Decide(in AIWorldSnapshot snapshot)
    {
        if (snapshot.SelfHasBall)
        {
            return snapshot.DistanceToHoop <= snapshot.ShootRange
                ? AIStateNames.Shoot
                : AIStateNames.Attack;
        }

        if (snapshot.BallIsFree)
        {
            return AIStateNames.ChaseBall;
        }

        return snapshot.OpponentHasBall ? AIStateNames.Defend : AIStateNames.Idle;
    }
}
