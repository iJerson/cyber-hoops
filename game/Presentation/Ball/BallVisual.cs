using Godot;
using CyberHoopsBall = CyberHoops.Gameplay.Ball.Ball;

namespace CyberHoops.Presentation.Ball;

/// <summary>
/// Squash and stretch on the ball mesh. Reads the ball's physics/dribble state
/// every tick and deforms the visual only — never touches physics. Squash near
/// floor contact, stretch with fall speed; reserved for high-energy moments
/// (free flight and dribble), never while held.
/// </summary>
[GlobalClass]
public partial class BallVisual : MeshInstance3D
{
    /// <summary>Max vertical squash at floor contact, fraction (0.15 = 15%).</summary>
    [Export] public float SquashAmount { get; set; } = 0.15f;

    /// <summary>Max vertical stretch at peak fall speed, fraction.</summary>
    [Export] public float StretchAmount { get; set; } = 0.08f;

    /// <summary>Fall speed that produces full stretch, m/s.</summary>
    [Export] public float StretchAtSpeed { get; set; } = 8.0f;

    /// <summary>Ball height (in radii) below which contact squash applies.</summary>
    [Export] public float SquashBelowRadii { get; set; } = 1.6f;

    /// <summary>Deformation blend speed, per second.</summary>
    [Export] public float BlendSpeed { get; set; } = 14.0f;

    private CyberHoopsBall? _ball;
    private float _deform; // negative = squash, positive = stretch

    public override void _Ready()
    {
        _ball = GetParent() as CyberHoopsBall;
        if (_ball is null)
        {
            GD.PushError($"{nameof(BallVisual)} must be a direct child of the Ball.");
            SetPhysicsProcess(false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var ball = _ball!;
        var radius = ball.Stats?.Radius ?? 0.12f;
        float target;

        if (ball.CurrentStateName == CyberHoopsBall.HeldState)
        {
            target = 0f;
        }
        else
        {
            var heightAboveFloor = ball.GlobalPosition.Y - radius;
            var nearFloor = heightAboveFloor < radius * (SquashBelowRadii - 1f);
            if (nearFloor)
            {
                target = -SquashAmount;
            }
            else
            {
                var fallSpeed = ball.CurrentStateName == CyberHoopsBall.FreeState
                    ? Mathf.Max(0f, -ball.LinearVelocity.Y)
                    : 0f;
                target = StretchAmount * Mathf.Clamp(fallSpeed / StretchAtSpeed, 0f, 1f);
            }
        }

        _deform = Mathf.MoveToward(_deform, target, BlendSpeed * (float)delta);

        // Volume-ish preserving: vertical scale 1+d, horizontal 1-d/2.
        var vertical = 1f + _deform;
        var horizontal = 1f - _deform * 0.5f;
        Scale = new Vector3(horizontal, vertical, horizontal);
    }
}
