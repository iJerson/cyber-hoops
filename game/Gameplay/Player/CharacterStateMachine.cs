using CyberHoops.Core.Character;
using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Per-character locomotion state (animation spec A–E, H) evaluated every
/// physics tick from live gameplay facts. Decision logic is the engine-free
/// <see cref="LocomotionDecider"/>; this node gathers the snapshot and tracks
/// state time. Purely observational — gameplay never waits on it.
/// </summary>
[GlobalClass]
public partial class CharacterStateMachine : Node
{
    [Export] public MovementComponent? Movement { get; set; }
    [Export] public PossessionComponent? Possession { get; set; }
    [Export] public DribbleComponent? Dribble { get; set; }
    [Export] public CharacterStateStats? Stats { get; set; }

    private float _previousSpeed;

    /// <summary>Current locomotion state name (see <see cref="LocomotionStates"/>).</summary>
    public string CurrentState { get; private set; } = LocomotionStates.Idle;

    /// <summary>Seconds spent in the current state.</summary>
    public double TimeInState { get; private set; }

    public override void _Ready()
    {
        if (Movement is null || Possession is null || Dribble is null || Stats is null)
        {
            GD.PushError($"{nameof(CharacterStateMachine)} is missing required exports.");
            SetPhysicsProcess(false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var stats = Stats!;
        var speed = Movement!.Speed;
        var deceleration = (_previousSpeed - speed) / (float)delta;
        _previousSpeed = speed;

        var snapshot = new LocomotionSnapshot(
            NormalizedSpeed: Movement.NormalizedSpeed,
            HasBall: Possession!.HasBall,
            Protecting: Dribble!.IsProtecting,
            Decelerating: deceleration > stats.StopDeceleration);

        var thresholds = new LocomotionThresholds(stats.IdleMaxSpeed, stats.WalkMaxSpeed, stats.StopMinSpeed);
        var next = LocomotionDecider.Decide(snapshot, thresholds);

        if (next == CurrentState)
        {
            TimeInState += delta;
        }
        else
        {
            CurrentState = next;
            TimeInState = 0.0;
        }
    }
}
