using CyberHoops.Core.Dribble;
using Godot;
using CyberHoopsBall = CyberHoops.Gameplay.Ball.Ball;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Owns the live dribble: bounce phase (locked to the gait clock while moving,
/// free-running at arcade tempo at rest), continuous apex height by speed and
/// defender pressure, and anchor placement per situation (front-outside at
/// rest, pushed ahead at speed, far hip in the protect stance). Feeds the ball
/// its bounce and the rig its pump. PossessionComponent keeps ownership rules;
/// this component only shapes the bounce.
/// </summary>
[GlobalClass]
public partial class DribbleComponent : Node
{
    [Export] public CharacterBody3D? Body { get; set; }
    [Export] public PossessionComponent? Possession { get; set; }
    [Export] public MovementComponent? Movement { get; set; }
    [Export] public Node3D? DribbleAnchor { get; set; }
    [Export] public DribbleStats? Stats { get; set; }
    [Export] public StringName OpponentGroup { get; set; } = "player";

    private readonly BounceClock _bounce = new();
    private float _gravity;
    private float _lastGaitPhase;
    private bool _protecting;
    private float _apex = -1f; // smoothed; -1 = not dribbling, re-seed on pickup

    /// <summary>Current bounce phase in [0,1); contact at 0.</summary>
    public float BouncePhase => (float)_bounce.Phase;

    /// <summary>True while the possessed ball is live-dribbling.</summary>
    public bool IsDribbling { get; private set; }

    /// <summary>Ball height as a fraction of the current apex, in [0,1]. For the arm pump.</summary>
    public float NormalizedBallHeight { get; private set; }

    /// <summary>True while the protect stance is active (defender near, with hysteresis).</summary>
    public bool IsProtecting => _protecting;

    /// <summary>+1 when the defender is on the body's right, -1 on the left. For the shield pose.</summary>
    public float ProtectSideSign { get; private set; } = 1f;

    public override void _Ready()
    {
        if (Body is null || Possession is null || Movement is null || DribbleAnchor is null || Stats is null)
        {
            GD.PushError($"{nameof(DribbleComponent)} is missing required exports.");
            SetPhysicsProcess(false);
            return;
        }

        if (Stats.ProtectExitRadius <= Stats.ProtectEnterRadius)
        {
            GD.PushError("ProtectExitRadius must exceed ProtectEnterRadius or the protect stance flickers.");
        }

        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();
        _lastGaitPhase = Movement.GaitPhase;
    }

    public override void _PhysicsProcess(double delta)
    {
        var ball = Possession!.Ball;
        if (ball is null || ball.CurrentStateName != CyberHoopsBall.DribblingState)
        {
            IsDribbling = false;
            NormalizedBallHeight = 0f;
            _protecting = false;
            _apex = -1f;
            _lastGaitPhase = Movement!.GaitPhase;
            return;
        }

        var stats = Stats!;
        var normalizedSpeed = Movement!.NormalizedSpeed;
        UpdateProtectState(stats);

        var apexTarget = _protecting
            ? stats.ProtectHeight
            : Mathf.Lerp(stats.IdleHeight, stats.MovingHeight, normalizedSpeed);

        if (!IsDribbling)
        {
            // Fresh pickup: start at floor contact with the target apex — no
            // stale phase, no mid-air teleport.
            _bounce.Reset();
            _apex = apexTarget;
            IsDribbling = true;
        }
        else
        {
            // Apex changes glide (state flips must not teleport the ball mid-flight).
            _apex = Mathf.MoveToward(_apex, apexTarget, stats.ApexBlendSpeed * (float)delta);
        }

        AdvanceBounce(delta, stats, _apex, normalizedSpeed);
        PlaceAnchor(delta, stats, normalizedSpeed);

        var height = (float)_bounce.HeightFor(_apex);
        ball.SetBounce(height, _bounce.ContactThisTick);
        NormalizedBallHeight = _apex <= 0f ? 0f : height / _apex;
    }

    private void AdvanceBounce(double delta, DribbleStats stats, float apex, float normalizedSpeed)
    {
        var gaitPhase = Movement!.GaitPhase;
        var gaitDelta = gaitPhase - _lastGaitPhase;
        if (gaitDelta < 0f)
        {
            gaitDelta += 1f;
        }

        _lastGaitPhase = gaitPhase;

        // Gait drives the bounce, but gravity sets a floor on the tempo: at a
        // creep the gait step would stretch the period toward infinity and the
        // ball would hover. Whichever advances the phase further wins.
        var tempo = stats.IdleTempoMultiplier * (_protecting ? stats.ProtectTempoMultiplier : 1f);
        var bouncesPerCycle = Mathf.Lerp(stats.WalkBouncesPerCycle, stats.RunBouncesPerCycle, normalizedSpeed);
        var gaitStep = gaitDelta * bouncesPerCycle;
        var timeStep = delta / (DribbleCycle.PeriodFor(apex, _gravity) / tempo);

        if (gaitStep >= timeStep)
        {
            _bounce.AdvanceByGait(gaitDelta, bouncesPerCycle);
        }
        else
        {
            _bounce.AdvanceByTime(delta, apex, _gravity, tempo);
        }
    }

    private void UpdateProtectState(DribbleStats stats)
    {
        var defender = GetTree().GetFirstNodeInGroup(OpponentGroup) as Node3D;
        if (defender is null || defender == Body)
        {
            _protecting = false;
            return;
        }

        var distance = ((defender.GlobalPosition - Body!.GlobalPosition) with { Y = 0f }).Length();
        if (_protecting)
        {
            _protecting = distance < stats.ProtectExitRadius;
        }
        else
        {
            _protecting = distance < stats.ProtectEnterRadius;
        }
    }

    /// <summary>Glides the anchor between idle / run-ahead / protect placements in body-local space.</summary>
    private void PlaceAnchor(double delta, DribbleStats stats, float normalizedSpeed)
    {
        Vector3 target;
        if (_protecting)
        {
            ProtectSideSign = DefenderSideSign();
            target = new Vector3(stats.ProtectAnchorOffset.X * -ProtectSideSign, 0f, -stats.ProtectAnchorOffset.Y);
        }
        else
        {
            // Forward is -Z in body space; blend from front-outside to pushed-ahead.
            var idle = new Vector3(stats.IdleAnchorOffset.X, 0f, -stats.IdleAnchorOffset.Y);
            var ahead = new Vector3(stats.IdleAnchorOffset.X * 0.5f, 0f, -stats.RunAheadDistance - stats.IdleAnchorOffset.Y);
            target = idle.Lerp(ahead, normalizedSpeed);
        }

        DribbleAnchor!.Position = DribbleAnchor.Position.MoveToward(target, stats.AnchorMoveSpeed * (float)delta);
    }

    /// <summary>+1 when the defender is on the body's right, -1 on the left.</summary>
    private float DefenderSideSign()
    {
        if (GetTree().GetFirstNodeInGroup(OpponentGroup) is not Node3D defender || defender == Body)
        {
            return 1f;
        }

        var local = Body!.GlobalTransform.AffineInverse() * defender.GlobalPosition;
        return local.X >= 0f ? 1f : -1f;
    }
}
