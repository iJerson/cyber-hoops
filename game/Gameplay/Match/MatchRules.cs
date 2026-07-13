using Godot;

namespace CyberHoops.Gameplay.Match;

/// <summary>
/// Data-driven match rules. Designers tune the .tres; no values in code.
/// </summary>
[GlobalClass]
public partial class MatchRules : Resource
{
    /// <summary>First team to reach this many points wins.</summary>
    [Export] public int TargetScore { get; set; } = 21;

    /// <summary>Points for a shot released inside the three-point distance.</summary>
    [Export] public int InsideShotPoints { get; set; } = 2;

    /// <summary>Points for a shot released beyond the three-point distance.</summary>
    [Export] public int OutsideShotPoints { get; set; } = 3;

    /// <summary>Ground distance from the hoop that separates inside from outside shots, in metres.</summary>
    [Export] public float ThreePointDistance { get; set; } = 6.75f;

    /// <summary>How far in front of the receiving player the ball is placed after a basket, in metres.</summary>
    [Export] public float CheckBallOffset { get; set; } = 1.0f;
}
