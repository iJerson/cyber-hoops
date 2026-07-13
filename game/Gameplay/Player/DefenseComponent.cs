using Godot;
using CyberHoopsBall = CyberHoops.Gameplay.Ball.Ball;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Defensive actions shared by human and AI. One attempt does the right thing
/// by context: a ball dribbled by the opponent in range is knocked loose
/// (steal); a ball in flight in range is swatted away (block). Attempts are
/// gated by a cooldown; all tuning lives in <see cref="DefenseStats"/>.
/// </summary>
[GlobalClass]
public partial class DefenseComponent : Node
{
    [Export] public StringName DefendAction { get; set; } = "defend";

    /// <summary>When true, polls the defend input action. AI leaves this off and calls <see cref="TryDefend"/>.</summary>
    [Export] public bool ReadPlayerInput { get; set; } = true;

    [Export] public CharacterBody3D? Body { get; set; }

    /// <summary>Own possession — you cannot steal a ball you are holding.</summary>
    [Export] public PossessionComponent? Possession { get; set; }

    [Export] public DefenseStats? Stats { get; set; }

    /// <summary>Played on a successful steal or block. Optional.</summary>
    [Export] public AudioStreamPlayer3D? HitSound { get; set; }
    [Export] public StringName BallGroup { get; set; } = "ball";
    [Export] public StringName PossessionGroup { get; set; } = "possession";

    private double _cooldownRemaining;

    public bool IsOnCooldown => _cooldownRemaining > 0.0;

    public override void _Ready()
    {
        if (Body is null || Possession is null || Stats is null)
        {
            GD.PushError($"{nameof(DefenseComponent)} requires {nameof(Body)}, {nameof(Possession)} and {nameof(Stats)}.");
            SetPhysicsProcess(false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_cooldownRemaining > 0.0)
        {
            _cooldownRemaining -= delta;
        }

        if (ReadPlayerInput && Input.IsActionJustPressed(DefendAction))
        {
            TryDefend();
        }
    }

    /// <summary>Attempts a steal or block by context. Returns true when the ball was disturbed.</summary>
    public bool TryDefend()
    {
        if (IsOnCooldown || Possession!.HasBall)
        {
            return false;
        }

        if (GetTree().GetFirstNodeInGroup(BallGroup) is not CyberHoopsBall ball)
        {
            return false;
        }

        _cooldownRemaining = Stats!.CooldownSeconds;

        var disturbed = ball.CurrentStateName switch
        {
            CyberHoopsBall.DribblingState or CyberHoopsBall.HeldState => TrySteal(ball),
            CyberHoopsBall.FreeState => TryBlock(ball),
            _ => false,
        };

        if (disturbed)
        {
            HitSound?.Play();
        }

        return disturbed;
    }

    private bool TrySteal(CyberHoopsBall ball)
    {
        var stats = Stats!;
        if (GroundDistance(Body!.GlobalPosition, ball.GlobalPosition) > stats.StealRange)
        {
            return false;
        }

        var victim = FindHolder(ball);
        if (victim is null)
        {
            return false;
        }

        var away = (ball.GlobalPosition - Body.GlobalPosition) with { Y = 0f };
        var knockDirection = away.LengthSquared() < 0.0001f ? Vector3.Forward : away.Normalized();
        victim.ReleaseBall(knockDirection * stats.StealKnockSpeed + Vector3.Up * stats.StealPopSpeed);
        // Spawn the loose ball clear of both bodies so it never unfreezes inside a capsule.
        ball.GlobalPosition += knockDirection * 0.5f;
        ball.LastShooterTeam = -1;
        return true;
    }

    private bool TryBlock(CyberHoopsBall ball)
    {
        var stats = Stats!;
        if (ball.GlobalPosition.Y < stats.BlockMinBallHeight
            || Body!.GlobalPosition.DistanceTo(ball.GlobalPosition) > stats.BlockRange)
        {
            return false;
        }

        var away = ball.GlobalPosition - Body.GlobalPosition;
        var direction = away.LengthSquared() < 0.0001f ? Vector3.Forward : away.Normalized();
        ball.LinearVelocity = direction * stats.BlockDeflectSpeed;
        return true;
    }

    private PossessionComponent? FindHolder(CyberHoopsBall ball)
    {
        foreach (var node in GetTree().GetNodesInGroup(PossessionGroup))
        {
            if (node is PossessionComponent possession && possession.HasBall && possession != Possession)
            {
                return possession;
            }
        }

        return null;
    }

    private static float GroundDistance(Vector3 a, Vector3 b) =>
        ((b - a) with { Y = 0f }).Length();
}
