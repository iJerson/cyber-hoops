namespace CyberHoops.Core.StateMachine;

/// <summary>
/// A single state in a <see cref="StateMachine"/>. Implementations hold no
/// engine references so they can be unit tested and reused by AI in M4.
/// </summary>
public interface IState
{
    /// <summary>Unique name used for transitions and diagnostics.</summary>
    string Name { get; }

    /// <summary>Called once when the state becomes active.</summary>
    void Enter();

    /// <summary>Called every tick while active. <paramref name="delta"/> is the fixed tick duration in seconds.</summary>
    void Update(double delta);

    /// <summary>Called once when the machine leaves this state.</summary>
    void Exit();
}
