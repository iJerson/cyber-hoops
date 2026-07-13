using CyberHoops.Core.Shooting;
using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Context-sensitive finishing shared by human and AI. One shoot action picks
/// the finish by ground distance to the rim: dunk inside DunkRange (lunge to
/// the rim and slam), layup inside LayupRange (low quick arc), otherwise a
/// jump shot on a solved ballistic arc. Arc math lives in the engine-free
/// <see cref="ShotArcSolver"/>; all tuning in <see cref="ShootStats"/>.
/// </summary>
[GlobalClass]
public partial class ShootingComponent : Node
{
    /// <summary>Group holding the hoop's target node (rim centre).</summary>
    [Export] public StringName ShotTargetGroup { get; set; } = "shot_target";

    [Export] public StringName ShootAction { get; set; } = "shoot";

    /// <summary>When true, polls the shoot input action. AI leaves this off and calls <see cref="TryShoot"/>.</summary>
    [Export] public bool ReadPlayerInput { get; set; } = true;

    [Export] public CharacterBody3D? Body { get; set; }
    [Export] public PossessionComponent? Possession { get; set; }

    /// <summary>Frozen during the dunk lunge so steering cannot fight the scripted motion.</summary>
    [Export] public MovementComponent? Movement { get; set; }

    [Export] public ShootStats? Stats { get; set; }

    /// <summary>Played on the dunk slam. Optional.</summary>
    [Export] public AudioStreamPlayer3D? DunkSound { get; set; }

    private float _gravity;
    private bool _dunking;
    private double _dunkElapsed;
    private Vector3 _dunkStart;
    private Vector3 _dunkLanding;
    private Vector3 _rimPosition;

    public bool IsDunking => _dunking;

    public override void _Ready()
    {
        if (Body is null || Possession is null || Stats is null)
        {
            GD.PushError($"{nameof(ShootingComponent)} requires {nameof(Body)}, {nameof(Possession)} and {nameof(Stats)}.");
            SetPhysicsProcess(false);
            return;
        }

        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_dunking)
        {
            UpdateDunk(delta);
            return;
        }

        if (ReadPlayerInput && Input.IsActionJustPressed(ShootAction))
        {
            TryShoot();
        }
    }

    /// <summary>Finishes by context if this player possesses the ball. Returns true when a finish started.</summary>
    public bool TryShoot()
    {
        if (_dunking || !Possession!.HasBall)
        {
            return false;
        }

        if (GetTree().GetFirstNodeInGroup(ShotTargetGroup) is not Node3D target)
        {
            GD.PushWarning($"No node in group '{ShotTargetGroup}' — cannot shoot.");
            return false;
        }

        var stats = Stats!;
        var groundDistance = GroundDistance(Body!.GlobalPosition, target.GlobalPosition);

        if (groundDistance <= stats.DunkRange)
        {
            StartDunk(target.GlobalPosition);
            return true;
        }

        var clearance = groundDistance <= stats.LayupRange ? stats.LayupArcClearance : stats.ArcClearance;
        return Shoot(target.GlobalPosition, clearance);
    }

    private bool Shoot(Vector3 targetPosition, float arcClearance)
    {
        var releasePosition = Body!.GlobalPosition + Vector3.Up * Stats!.ReleaseHeight;

        var toTarget = targetPosition - releasePosition;
        var groundDelta = new Vector3(toTarget.X, 0f, toTarget.Z);
        var groundDistance = groundDelta.Length();
        if (groundDistance < 0.01f)
        {
            return false;
        }

        var solution = ShotArcSolver.Solve(groundDistance, toTarget.Y, arcClearance, _gravity);
        var velocity = groundDelta.Normalized() * (float)solution.HorizontalSpeed
                       + Vector3.Up * (float)solution.VerticalSpeed;

        var ball = Possession!.ReleaseBall(velocity);
        if (ball is null)
        {
            return false;
        }

        ball.GlobalPosition = releasePosition;
        ball.LastShotGroundDistance = groundDistance;
        return true;
    }

    private void StartDunk(Vector3 rimPosition)
    {
        _dunking = true;
        _dunkElapsed = 0.0;
        _rimPosition = rimPosition;
        _dunkStart = Body!.GlobalPosition;

        // Land just in front of the rim, on the shooter's side.
        var back = (_dunkStart - rimPosition) with { Y = 0f };
        var landingOffset = back.LengthSquared() < 0.0001f ? Vector3.Back * 0.5f : back.Normalized() * 0.5f;
        _dunkLanding = (rimPosition + landingOffset) with { Y = _dunkStart.Y };

        Body.Velocity = Vector3.Zero;
        Movement?.SetPhysicsProcess(false);
        Possession!.HoldBall();
    }

    private void UpdateDunk(double delta)
    {
        var stats = Stats!;
        _dunkElapsed += delta;
        var t = Mathf.Clamp((float)(_dunkElapsed / stats.DunkDuration), 0f, 1f);

        var position = _dunkStart.Lerp(_dunkLanding, t);
        position.Y += Mathf.Sin(t * Mathf.Pi) * stats.DunkJumpHeight;
        Body!.GlobalPosition = position;

        if (t < 1f)
        {
            return;
        }

        var ball = Possession!.ReleaseBall(Vector3.Down * stats.DunkSlamSpeed);
        if (ball is not null)
        {
            ball.GlobalPosition = _rimPosition + Vector3.Up * 0.15f;
            ball.LastShotGroundDistance = GroundDistance(_dunkLanding, _rimPosition);
            DunkSound?.Play();
        }

        _dunking = false;
        Movement?.SetPhysicsProcess(true);
    }

    private static float GroundDistance(Vector3 a, Vector3 b) =>
        ((b - a) with { Y = 0f }).Length();
}
