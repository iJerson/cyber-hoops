using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Data-driven shooting tuning. Designers tune the .tres; no values in code.
/// </summary>
[GlobalClass]
public partial class ShootStats : Resource
{
    /// <summary>How high the arc apex clears the higher of release point and rim, in metres.</summary>
    [Export] public float ArcClearance { get; set; } = 1.2f;

    /// <summary>Height above the player origin where the ball leaves the hands, in metres.</summary>
    [Export] public float ReleaseHeight { get; set; } = 2.1f;

    /// <summary>Inside this ground distance to the rim, shooting becomes a layup.</summary>
    [Export] public float LayupRange { get; set; } = 3.2f;

    /// <summary>Arc apex clearance for layups — lower and quicker than a jump shot, in metres.</summary>
    [Export] public float LayupArcClearance { get; set; } = 0.45f;

    /// <summary>Inside this ground distance to the rim, shooting becomes a dunk.</summary>
    [Export] public float DunkRange { get; set; } = 1.8f;

    /// <summary>Time of the dunk lunge from launch to slam, in seconds.</summary>
    [Export] public float DunkDuration { get; set; } = 0.35f;

    /// <summary>Peak extra height of the dunk lunge arc, in metres.</summary>
    [Export] public float DunkJumpHeight { get; set; } = 1.1f;

    /// <summary>Downward speed of the slammed ball, in m/s.</summary>
    [Export] public float DunkSlamSpeed { get; set; } = 8.0f;

    /// <summary>Pre-load crouch before the dunk lunge, seconds (arcade-short; motion reads as anticipation).</summary>
    [Export] public float DunkDipSeconds { get; set; } = 0.06f;

    /// <summary>Exponent on the lunge height curve; below 1 flattens the apex for extra hang time.</summary>
    [Export] public float DunkHangExponent { get; set; } = 0.7f;

    /// <summary>Pre-load dip before a jump shot or layup releases, seconds.</summary>
    [Export] public float ShotDipSeconds { get; set; } = 0.05f;

    /// <summary>Fraction of the dunk lunge at which the ball slams through the rim (near apex).</summary>
    [Export(PropertyHint.Range, "0.3,0.9")] public float DunkSlamAt { get; set; } = 0.55f;

    /// <summary>Landing absorption time after the dunk, seconds.</summary>
    [Export] public float DunkLandRecoverSeconds { get; set; } = 0.15f;
}
