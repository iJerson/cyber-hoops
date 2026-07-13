using Godot;
using CyberHoopsBall = CyberHoops.Gameplay.Ball.Ball;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Gives a player possession of the ball. A pickup <see cref="Area3D"/>
/// detects the free ball and starts a dribble at the anchor; dribble height
/// follows the locomotion state. Exposes a release API for future
/// shoot/pass/steal mechanics.
/// </summary>
[GlobalClass]
public partial class PossessionComponent : Node
{
    /// <summary>Score slot for this player: 0 = human, 1 = AI opponent.</summary>
    [Export] public int TeamId { get; set; }

    [Export] public Area3D? PickupArea { get; set; }

    /// <summary>Where the dribble happens, e.g. beside the player's hand.</summary>
    [Export] public Node3D? DribbleAnchor { get; set; }

    /// <summary>Used to match dribble height to Idle vs Move/Sprint.</summary>
    [Export] public MovementComponent? Movement { get; set; }

    private CyberHoopsBall? _ball;

    public bool HasBall => _ball is not null;

    public override void _Ready()
    {
        if (PickupArea is null || DribbleAnchor is null || Movement is null)
        {
            GD.PushError($"{nameof(PossessionComponent)} requires {nameof(PickupArea)}, {nameof(DribbleAnchor)} and {nameof(Movement)}.");
            return;
        }

        PickupArea.BodyEntered += OnBodyEntered;
        AddToGroup("possession");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_ball?.Stats is not { } stats)
        {
            return;
        }

        var isIdle = Movement!.CurrentStateName == LocomotionStateNames.Idle;
        _ball.SetDribbleHeight(isIdle ? stats.IdleDribbleHeight : stats.MovingDribbleHeight);
    }

    /// <summary>Glues the held ball to the dribble anchor (dunk carry, shot wind-up).</summary>
    public void HoldBall() => _ball?.Hold(DribbleAnchor!);

    /// <summary>Releases the ball with the given velocity and gives up possession.</summary>
    public CyberHoopsBall? ReleaseBall(Vector3 velocity)
    {
        var ball = _ball;
        _ball = null;
        if (ball is not null)
        {
            ball.LastShooterTeam = TeamId;
            ball.Release(velocity);
        }

        return ball;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (HasBall || body is not CyberHoopsBall ball || ball.CurrentStateName != CyberHoopsBall.FreeState)
        {
            return;
        }

        _ball = ball;
        ball.StartDribble(DribbleAnchor!);
    }
}
