using Godot;
using CyberHoopsBall = CyberHoops.Gameplay.Ball.Ball;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Ownership rules only: a pickup <see cref="Area3D"/> detects the free ball
/// and claims it; release/hold APIs serve shooting, dunks and steals. How the
/// possessed ball bounces is <see cref="DribbleComponent"/>'s job.
/// </summary>
[GlobalClass]
public partial class PossessionComponent : Node
{
    /// <summary>Score slot for this player: 0 = human, 1 = AI opponent.</summary>
    [Export] public int TeamId { get; set; }

    [Export] public Area3D? PickupArea { get; set; }

    /// <summary>Where the dribble happens; placed each tick by DribbleComponent.</summary>
    [Export] public Node3D? DribbleAnchor { get; set; }

    private CyberHoopsBall? _ball;

    public bool HasBall => _ball is not null;

    /// <summary>The possessed ball, if any. Read by DribbleComponent.</summary>
    public CyberHoopsBall? Ball => _ball;

    public override void _Ready()
    {
        if (PickupArea is null || DribbleAnchor is null)
        {
            GD.PushError($"{nameof(PossessionComponent)} requires {nameof(PickupArea)} and {nameof(DribbleAnchor)}.");
            return;
        }

        PickupArea.BodyEntered += OnBodyEntered;
        AddToGroup("possession");
    }

    /// <summary>Glues the held ball to an anchor (dunk carry, shot wind-up). Defaults to the dribble anchor.</summary>
    public void HoldBall(Node3D? anchor = null) => _ball?.Hold(anchor ?? DribbleAnchor!);

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
