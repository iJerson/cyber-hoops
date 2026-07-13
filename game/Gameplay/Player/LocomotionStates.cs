using CyberHoops.Core.StateMachine;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Locomotion state names shared by the movement component and, later, AI.
/// </summary>
public static class LocomotionStateNames
{
    public const string Idle = "Idle";
    public const string Move = "Move";
    public const string Sprint = "Sprint";
}

/// <summary>
/// A locomotion state contributes exactly one thing: the top speed the body
/// may reach while the state is active. Engine-free so it stays unit-testable.
/// </summary>
public sealed class LocomotionState : IState
{
    private readonly Func<float> _getMaxSpeed;

    public string Name { get; }

    /// <summary>Top speed for this state, in metres per second.</summary>
    public float MaxSpeed => _getMaxSpeed();

    public LocomotionState(string name, Func<float> getMaxSpeed)
    {
        Name = name;
        _getMaxSpeed = getMaxSpeed;
    }

    public void Enter()
    {
    }

    public void Update(double delta)
    {
    }

    public void Exit()
    {
    }
}
