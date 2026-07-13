using CyberHoops.Core.Dribble;
using CyberHoops.Core.StateMachine;
using Godot;

namespace CyberHoops.Gameplay.Ball;

/// <summary>
/// The game ball. Three modes managed by the shared Core state machine:
/// Free (full physics), Held (frozen, glued to an anchor) and Dribbling
/// (frozen, deterministic bounce beside an anchor). Physics values come from
/// <see cref="BallStats"/>.
/// </summary>
[GlobalClass]
public partial class Ball : RigidBody3D
{
    public const string FreeState = "Free";
    public const string HeldState = "Held";
    public const string DribblingState = "Dribbling";

    [Export] public BallStats? Stats { get; set; }

    /// <summary>Team index of the last shooter, or -1 when unattributed. Stamped on release.</summary>
    public int LastShooterTeam { get; set; } = -1;

    /// <summary>Ground distance from hoop at the moment of the last shot, for 2/3-point scoring.</summary>
    public float LastShotGroundDistance { get; set; }

    private readonly StateMachine _stateMachine = new();
    private readonly DribbleCycle _dribbleCycle = new();
    private Node3D? _anchor;
    private float _dribbleHeight;
    private float _gravity;

    public string? CurrentStateName => _stateMachine.Current?.Name;

    /// <summary>The node currently holding or dribbling the ball, if any.</summary>
    public Node3D? Anchor => _anchor;

    public override void _Ready()
    {
        if (Stats is null)
        {
            GD.PushError($"{nameof(Ball)} requires {nameof(Stats)}.");
            SetPhysicsProcess(false);
            return;
        }

        Mass = Stats.Mass;
        PhysicsMaterialOverride = new PhysicsMaterial
        {
            Bounce = Stats.Bounciness,
            Friction = Stats.Friction,
        };
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();
        _dribbleHeight = Stats.IdleDribbleHeight;

        _stateMachine.AddState(new BallModeState(FreeState, enter: () => Freeze = false));
        _stateMachine.AddState(new BallModeState(HeldState, enter: () => Freeze = true));
        _stateMachine.AddState(new BallModeState(DribblingState, enter: () =>
        {
            Freeze = true;
            _dribbleCycle.Reset();
        }));
        _stateMachine.TransitionTo(FreeState);
    }

    /// <summary>Glues the ball to the anchor with physics off (carry, shot wind-up).</summary>
    public void Hold(Node3D anchor)
    {
        _anchor = anchor;
        _stateMachine.TransitionTo(HeldState);
    }

    /// <summary>Starts the deterministic dribble bounce beside the anchor.</summary>
    public void StartDribble(Node3D anchor)
    {
        _anchor = anchor;
        _stateMachine.TransitionTo(DribblingState);
    }

    /// <summary>Sets the current dribble peak height (idle vs moving), in metres.</summary>
    public void SetDribbleHeight(float height) => _dribbleHeight = height;

    /// <summary>Releases the ball back to free physics with the given velocity (shot, pass, steal knock-away).</summary>
    public void Release(Vector3 velocity)
    {
        _anchor = null;
        _stateMachine.TransitionTo(FreeState);
        LinearVelocity = velocity;
    }

    /// <summary>Teleports the ball to a point as a fresh free ball (post-basket check ball).</summary>
    public void ResetAt(Vector3 position)
    {
        _anchor = null;
        LastShooterTeam = -1;
        _stateMachine.TransitionTo(FreeState);
        LinearVelocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
        GlobalPosition = position;
    }

    public override void _PhysicsProcess(double delta)
    {
        _stateMachine.Update(delta);

        switch (_stateMachine.Current?.Name)
        {
            case HeldState when _anchor is not null:
                GlobalPosition = _anchor.GlobalPosition;
                break;
            case DribblingState when _anchor is not null:
                UpdateDribble(delta);
                break;
        }
    }

    private void UpdateDribble(double delta)
    {
        var stats = Stats!;
        var height = (float)_dribbleCycle.Advance(delta, _dribbleHeight, _gravity);

        var anchorXz = _anchor!.GlobalPosition with { Y = 0f };
        var currentXz = GlobalPosition with { Y = 0f };
        var followedXz = currentXz.MoveToward(anchorXz, stats.DribbleFollowSpeed * (float)delta);

        GlobalPosition = new Vector3(followedXz.X, stats.Radius + height, followedXz.Z);
    }

    /// <summary>Minimal state for ball modes: per-tick work happens in <see cref="Ball._PhysicsProcess"/>.</summary>
    private sealed class BallModeState : IState
    {
        private readonly Action _enter;

        public string Name { get; }

        public BallModeState(string name, Action enter)
        {
            Name = name;
            _enter = enter;
        }

        public void Enter() => _enter();

        public void Update(double delta)
        {
        }

        public void Exit()
        {
        }
    }
}
