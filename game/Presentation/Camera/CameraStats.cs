using Godot;

namespace CyberHoops.Presentation.Camera;

/// <summary>
/// Data-driven camera tuning. Designers tune the .tres; no values in code.
/// </summary>
[GlobalClass]
public partial class CameraStats : Resource
{
    /// <summary>Camera position offset from the follow target, in metres.</summary>
    [Export] public Vector3 FollowOffset { get; set; } = new(0f, 9f, 11f);

    /// <summary>How quickly the camera closes on its desired position, per second.</summary>
    [Export] public float FollowSpeed { get; set; } = 4.0f;

    /// <summary>0 = look at the player, 1 = look at the hoop; between blends the aim point.</summary>
    [Export(PropertyHint.Range, "0,1")] public float LookAtHoopBlend { get; set; } = 0.35f;

    /// <summary>Fraction of the player's ground position the camera tracks sideways (1 = full).</summary>
    [Export(PropertyHint.Range, "0,1")] public float LateralTracking { get; set; } = 0.6f;
}
