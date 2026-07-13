using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Data-driven steal/block tuning. Designers tune the .tres; no values in code.
/// </summary>
[GlobalClass]
public partial class DefenseStats : Resource
{
    /// <summary>Max distance to a dribbled ball for a steal to connect, in metres.</summary>
    [Export] public float StealRange { get; set; } = 1.2f;

    /// <summary>Horizontal speed of the knocked-loose ball after a steal, in m/s.</summary>
    [Export] public float StealKnockSpeed { get; set; } = 3.5f;

    /// <summary>Upward pop of the knocked-loose ball after a steal, in m/s.</summary>
    [Export] public float StealPopSpeed { get; set; } = 2.0f;

    /// <summary>Max distance to a ball in flight for a block to connect, in metres.</summary>
    [Export] public float BlockRange { get; set; } = 1.8f;

    /// <summary>Ball must be at least this high off the floor to be blockable, in metres.</summary>
    [Export] public float BlockMinBallHeight { get; set; } = 1.5f;

    /// <summary>Speed of the deflected ball after a block, in m/s.</summary>
    [Export] public float BlockDeflectSpeed { get; set; } = 6.0f;

    /// <summary>Seconds between defensive attempts.</summary>
    [Export] public float CooldownSeconds { get; set; } = 0.8f;
}
