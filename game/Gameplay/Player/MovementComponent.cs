using CyberHoops.Core.Locomotion;
using CyberHoops.Core.StateMachine;
using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Moves a <see cref="CharacterBody3D"/> from movement intent supplied by any
/// <see cref="IMovementInputSource"/> (human now, AI in M4). All tuning comes
/// from <see cref="MovementStats"/>; locomotion state (Idle/Move/Sprint) is
/// managed by the shared Core state machine. Runs on the physics tick for
/// deterministic behaviour.
/// </summary>
[GlobalClass]
public partial class MovementComponent : Node
{
    [Export] public CharacterBody3D? Body { get; set; }
    [Export] public MovementStats? Stats { get; set; }

    /// <summary>Node implementing <see cref="IMovementInputSource"/> that drives this component.</summary>
    [Export] public Node? InputSource { get; set; }

    private readonly StateMachine _stateMachine = new();
    private readonly GaitClock _gaitClock = new();
    private IMovementInputSource? _input;
    private LocomotionState? _idle;
    private LocomotionState? _move;
    private LocomotionState? _sprint;

    public string? CurrentStateName => _stateMachine.Current?.Name;

    /// <summary>Master animation clock: stride cycle phase in [0,1), advanced by distance travelled.</summary>
    public float GaitPhase => (float)_gaitClock.Phase;

    /// <summary>Current horizontal speed, m/s.</summary>
    public float Speed { get; private set; }

    /// <summary>Horizontal speed normalized against sprint speed, in [0,1].</summary>
    public float NormalizedSpeed => Stats is null ? 0f : Mathf.Clamp(Speed / Stats.SprintSpeed, 0f, 1f);

    public override void _Ready()
    {
        if (Body is null || Stats is null)
        {
            GD.PushError($"{nameof(MovementComponent)} requires {nameof(Body)} and {nameof(Stats)}.");
            SetPhysicsProcess(false);
            return;
        }

        _input = InputSource as IMovementInputSource;
        if (_input is null)
        {
            GD.PushError($"{nameof(InputSource)} must implement {nameof(IMovementInputSource)}.");
            SetPhysicsProcess(false);
            return;
        }

        _idle = new LocomotionState(LocomotionStateNames.Idle, () => 0f);
        _move = new LocomotionState(LocomotionStateNames.Move, () => Stats.MoveSpeed);
        _sprint = new LocomotionState(LocomotionStateNames.Sprint, () => Stats.SprintSpeed);
        _stateMachine.AddState(_idle);
        _stateMachine.AddState(_move);
        _stateMachine.AddState(_sprint);
        _stateMachine.TransitionTo(LocomotionStateNames.Idle);
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = _input!.GetMoveDirection();
        UpdateLocomotionState(direction);
        _stateMachine.Update(delta);

        var current = (LocomotionState)_stateMachine.Current!;
        var targetVelocity = new Vector3(direction.X, 0f, direction.Y) * current.MaxSpeed;

        var body = Body!;
        var stats = Stats!;
        var rate = targetVelocity.LengthSquared() > body.Velocity.LengthSquared()
            ? stats.Acceleration
            : stats.Deceleration;
        var horizontal = new Vector3(body.Velocity.X, 0f, body.Velocity.Z)
            .MoveToward(targetVelocity, rate * (float)delta);
        body.Velocity = new Vector3(horizontal.X, body.Velocity.Y, horizontal.Z);

        FaceMovementDirection(body, stats, horizontal, delta);
        body.MoveAndSlide();

        Speed = new Vector3(body.Velocity.X, 0f, body.Velocity.Z).Length();
        _gaitClock.Advance(Speed * delta, stats.StrideFrequency);
    }

    private void UpdateLocomotionState(Vector2 direction)
    {
        var stats = Stats!;
        if (direction.Length() < stats.IdleInputThreshold)
        {
            _stateMachine.TransitionTo(LocomotionStateNames.Idle);
        }
        else if (_input!.IsSprintRequested())
        {
            _stateMachine.TransitionTo(LocomotionStateNames.Sprint);
        }
        else
        {
            _stateMachine.TransitionTo(LocomotionStateNames.Move);
        }
    }

    private static void FaceMovementDirection(CharacterBody3D body, MovementStats stats, Vector3 horizontalVelocity, double delta)
    {
        if (horizontalVelocity.LengthSquared() < 0.0001f)
        {
            return;
        }

        var targetYaw = Mathf.Atan2(-horizontalVelocity.X, -horizontalVelocity.Z);
        var rotation = body.Rotation;
        rotation.Y = Mathf.LerpAngle(rotation.Y, targetYaw, Mathf.Clamp(stats.TurnSpeed * (float)delta, 0f, 1f));
        body.Rotation = rotation;
    }
}
