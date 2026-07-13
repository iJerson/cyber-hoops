using Godot;

namespace CyberHoops.Presentation.Camera;

/// <summary>
/// Arcade follow camera: trails the player with damped movement and aims at a
/// blend of player and hoop so the basket stays framed. All tuning in
/// <see cref="CameraStats"/>.
/// </summary>
[GlobalClass]
public partial class CameraFollowComponent : Camera3D
{
    [Export] public Node3D? Target { get; set; }
    [Export] public CameraStats? Stats { get; set; }
    [Export] public StringName HoopGroup { get; set; } = "shot_target";

    private Node3D? _hoop;

    public override void _Ready()
    {
        if (Target is null || Stats is null)
        {
            GD.PushError($"{nameof(CameraFollowComponent)} requires {nameof(Target)} and {nameof(Stats)}.");
            SetPhysicsProcess(false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        _hoop ??= GetTree().GetFirstNodeInGroup(HoopGroup) as Node3D;
        if (_hoop is null)
        {
            return;
        }

        var stats = Stats!;
        var anchor = Target!.GlobalPosition with { Y = 0f };
        anchor.X *= stats.LateralTracking;

        var desired = anchor + stats.FollowOffset;
        var weight = Mathf.Clamp(stats.FollowSpeed * (float)delta, 0f, 1f);
        GlobalPosition = GlobalPosition.Lerp(desired, weight);

        var aim = Target.GlobalPosition.Lerp(_hoop.GlobalPosition, stats.LookAtHoopBlend);
        LookAt(aim, Vector3.Up);
    }
}
