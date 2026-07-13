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
}
