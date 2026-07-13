using CyberHoops.Core.Shooting;
using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Shoots the possessed ball on a solved ballistic arc toward the hoop's
/// shot-target node. Arc math lives in the engine-free
/// <see cref="ShotArcSolver"/> so AI shots reuse it in M4.
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
    [Export] public ShootStats? Stats { get; set; }

    private float _gravity;

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
        if (ReadPlayerInput && Input.IsActionJustPressed(ShootAction))
        {
            TryShoot();
        }
    }

    /// <summary>Shoots at the hoop if this player currently possesses the ball. Returns true when a shot was released.</summary>
    public bool TryShoot()
    {
        if (!Possession!.HasBall)
        {
            return false;
        }

        if (GetTree().GetFirstNodeInGroup(ShotTargetGroup) is not Node3D target)
        {
            GD.PushWarning($"No node in group '{ShotTargetGroup}' — cannot shoot.");
            return false;
        }

        return Shoot(target.GlobalPosition);
    }

    private bool Shoot(Vector3 targetPosition)
    {
        var releasePosition = Body!.GlobalPosition + Vector3.Up * Stats!.ReleaseHeight;

        var toTarget = targetPosition - releasePosition;
        var groundDelta = new Vector3(toTarget.X, 0f, toTarget.Z);
        var groundDistance = groundDelta.Length();
        if (groundDistance < 0.01f)
        {
            return false;
        }

        var solution = ShotArcSolver.Solve(groundDistance, toTarget.Y, Stats.ArcClearance, _gravity);
        var velocity = groundDelta.Normalized() * (float)solution.HorizontalSpeed
                       + Vector3.Up * (float)solution.VerticalSpeed;

        var ball = Possession!.ReleaseBall(velocity);
        if (ball is null)
        {
            return false;
        }

        ball.GlobalPosition = releasePosition;
        return true;
    }
}
