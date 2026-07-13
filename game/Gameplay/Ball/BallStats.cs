using Godot;

namespace CyberHoops.Gameplay.Ball;

/// <summary>
/// Data-driven ball physics and dribble tuning. Designers tune the .tres;
/// no values live in code.
/// </summary>
[GlobalClass]
public partial class BallStats : Resource
{
    /// <summary>Ball radius in metres (regulation ≈ 0.12).</summary>
    [Export] public float Radius { get; set; } = 0.12f;

    /// <summary>Ball mass in kilograms.</summary>
    [Export] public float Mass { get; set; } = 0.62f;

    /// <summary>Physics material bounciness, 0–1.</summary>
    [Export(PropertyHint.Range, "0,1")] public float Bounciness { get; set; } = 0.8f;

    /// <summary>Physics material friction, 0–1.</summary>
    [Export(PropertyHint.Range, "0,1")] public float Friction { get; set; } = 0.6f;

    /// <summary>Peak dribble bounce height while standing, in metres.</summary>
    [Export] public float IdleDribbleHeight { get; set; } = 0.9f;

    /// <summary>Peak dribble bounce height while moving, in metres.</summary>
    [Export] public float MovingDribbleHeight { get; set; } = 0.55f;

    /// <summary>How fast the dribbled ball tracks the anchor horizontally, per second.</summary>
    [Export] public float DribbleFollowSpeed { get; set; } = 12.0f;
}
