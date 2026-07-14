using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Data-driven dribble behaviour: bounce heights, tempo, gait sync rates and
/// anchor placement per situation. Designers tune the .tres; no values in code.
/// </summary>
[GlobalClass]
public partial class DribbleStats : Resource
{
    /// <summary>Bounce apex while standing, metres.</summary>
    [Export] public float IdleHeight { get; set; } = 0.9f;

    /// <summary>Bounce apex at full speed, metres.</summary>
    [Export] public float MovingHeight { get; set; } = 0.55f;

    /// <summary>Bounce apex while protecting from a near defender, metres.</summary>
    [Export] public float ProtectHeight { get; set; } = 0.45f;

    /// <summary>Idle bounce tempo relative to physics (arcade energy at rest).</summary>
    [Export] public float IdleTempoMultiplier { get; set; } = 1.15f;

    /// <summary>Protect bounce tempo relative to the base tempo.</summary>
    [Export] public float ProtectTempoMultiplier { get; set; } = 1.5f;

    /// <summary>Bounces per gait cycle at walking speed.</summary>
    [Export] public float WalkBouncesPerCycle { get; set; } = 1.0f;

    /// <summary>Bounces per gait cycle at full sprint.</summary>
    [Export] public float RunBouncesPerCycle { get; set; } = 0.5f;

    /// <summary>Defender distance that enters the protect stance, metres.</summary>
    [Export] public float ProtectEnterRadius { get; set; } = 1.5f;

    /// <summary>Defender distance that exits the protect stance, metres (hysteresis).</summary>
    [Export] public float ProtectExitRadius { get; set; } = 2.0f;

    /// <summary>Anchor while standing: sideways and forward offsets, metres (front-outside of the dribble foot).</summary>
    [Export] public Vector2 IdleAnchorOffset { get; set; } = new(0.3f, 0.5f);

    /// <summary>How far ahead the ball is pushed at full speed, metres.</summary>
    [Export] public float RunAheadDistance { get; set; } = 0.85f;

    /// <summary>Protect anchor: sideways (away from defender) and backward offsets, metres.</summary>
    [Export] public Vector2 ProtectAnchorOffset { get; set; } = new(0.4f, -0.15f);

    /// <summary>How fast the anchor glides between placements, m/s.</summary>
    [Export] public float AnchorMoveSpeed { get; set; } = 6.0f;

    /// <summary>How fast the bounce apex height adapts to state changes, m/s (continuity — no mid-air teleports).</summary>
    [Export] public float ApexBlendSpeed { get; set; } = 2.5f;
}
