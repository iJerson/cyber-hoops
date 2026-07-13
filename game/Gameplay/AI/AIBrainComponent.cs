using CyberHoops.Core.AI;
using Godot;
using CyberHoops.Gameplay.Player;
using CyberHoopsBall = CyberHoops.Gameplay.Ball.Ball;

namespace CyberHoops.Gameplay.AI;

/// <summary>
/// Phase 1 finite-state AI (see docs/03-ai/AI-Overview.md). Each physics tick:
/// digest the world into an <see cref="AIWorldSnapshot"/>, ask the engine-free
/// <see cref="AIDecider"/> for a state, then steer via
/// <see cref="AIInputComponent"/> and shoot via <see cref="ShootingComponent"/>.
/// Ball and opponent are found by group so the scene stays loosely coupled.
/// </summary>
[GlobalClass]
public partial class AIBrainComponent : Node
{
    [Export] public CharacterBody3D? Body { get; set; }
    [Export] public AIInputComponent? InputComponent { get; set; }
    [Export] public PossessionComponent? Possession { get; set; }
    [Export] public ShootingComponent? Shooting { get; set; }
    [Export] public DefenseComponent? Defense { get; set; }
    [Export] public AIStats? Stats { get; set; }
    [Export] public StringName BallGroup { get; set; } = "ball";
    [Export] public StringName OpponentGroup { get; set; } = "player";
    [Export] public StringName ShotTargetGroup { get; set; } = "shot_target";

    /// <summary>Current FSM state name, for debugging and future UI.</summary>
    public string CurrentState { get; private set; } = AIStateNames.Idle;

    public override void _Ready()
    {
        if (Body is null || InputComponent is null || Possession is null || Shooting is null || Stats is null)
        {
            GD.PushError($"{nameof(AIBrainComponent)} is missing required exports.");
            SetPhysicsProcess(false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var ball = GetTree().GetFirstNodeInGroup(BallGroup) as CyberHoopsBall;
        var opponent = GetTree().GetFirstNodeInGroup(OpponentGroup) as Node3D;
        var hoop = GetTree().GetFirstNodeInGroup(ShotTargetGroup) as Node3D;
        if (ball is null || hoop is null)
        {
            return;
        }

        var selfHasBall = Possession!.HasBall;
        var snapshot = new AIWorldSnapshot(
            BallIsFree: ball.CurrentStateName == CyberHoopsBall.FreeState,
            SelfHasBall: selfHasBall,
            OpponentHasBall: ball.CurrentStateName != CyberHoopsBall.FreeState && !selfHasBall,
            DistanceToHoop: GroundDistance(Body!.GlobalPosition, hoop.GlobalPosition),
            ShootRange: Stats!.ShootRange);

        CurrentState = AIDecider.Decide(snapshot);

        switch (CurrentState)
        {
            case AIStateNames.ChaseBall:
                SteerTowards(ball.GlobalPosition);
                break;
            case AIStateNames.Attack:
                SteerTowards(hoop.GlobalPosition);
                break;
            case AIStateNames.Shoot:
                Stop();
                Shooting!.TryShoot();
                break;
            case AIStateNames.Defend when opponent is not null:
                SteerTowards(DefendPosition(opponent.GlobalPosition, hoop.GlobalPosition));
                if (Defense?.Stats is { } defense
                    && GroundDistance(Body.GlobalPosition, ball.GlobalPosition) <= defense.StealRange)
                {
                    Defense.TryDefend();
                }

                break;
            default:
                Stop();
                break;
        }
    }

    /// <summary>Point between the ball-handler and the hoop, DefendDistance in front of the handler.</summary>
    private Vector3 DefendPosition(Vector3 opponentPosition, Vector3 hoopPosition)
    {
        var toHoop = (hoopPosition - opponentPosition) with { Y = 0f };
        return toHoop.LengthSquared() < 0.0001f
            ? opponentPosition
            : opponentPosition + toHoop.Normalized() * Stats!.DefendDistance;
    }

    private void SteerTowards(Vector3 target)
    {
        var delta = (target - Body!.GlobalPosition) with { Y = 0f };
        var distance = delta.Length();
        if (distance <= Stats!.ArriveRadius)
        {
            Stop();
            return;
        }

        var direction = delta / distance;
        InputComponent!.DesiredDirection = new Vector2(direction.X, direction.Z);
        InputComponent.SprintRequested = distance > Stats.SprintBeyondDistance;
    }

    private void Stop()
    {
        InputComponent!.DesiredDirection = Vector2.Zero;
        InputComponent.SprintRequested = false;
    }

    private static float GroundDistance(Vector3 a, Vector3 b) =>
        ((b - a) with { Y = 0f }).Length();
}
