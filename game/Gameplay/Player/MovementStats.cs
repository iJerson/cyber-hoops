using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Data-driven locomotion tuning. All movement values live here so nothing is
/// hardcoded in components; designers tune the .tres, not code.
/// </summary>
[GlobalClass]
public partial class MovementStats : Resource
{
    /// <summary>Top speed while walking, in metres per second.</summary>
    [Export] public float MoveSpeed { get; set; } = 5.0f;

    /// <summary>Top speed while sprinting, in metres per second.</summary>
    [Export] public float SprintSpeed { get; set; } = 8.0f;

    /// <summary>How quickly the player reaches target speed, in m/s².</summary>
    [Export] public float Acceleration { get; set; } = 30.0f;

    /// <summary>How quickly the player stops when input ceases, in m/s².</summary>
    [Export] public float Deceleration { get; set; } = 40.0f;

    /// <summary>Turn rate towards the movement direction, in radians per second.</summary>
    [Export] public float TurnSpeed { get; set; } = 12.0f;

    /// <summary>Input magnitude below which the player is considered idle.</summary>
    [Export] public float IdleInputThreshold { get; set; } = 0.1f;
}
