using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Data-driven thresholds for the character locomotion state machine.
/// </summary>
[GlobalClass]
public partial class CharacterStateStats : Resource
{
    /// <summary>Normalized speed at or below which the character is idle.</summary>
    [Export] public float IdleMaxSpeed { get; set; } = 0.02f;

    /// <summary>Normalized speed boundary between walk and run poses.</summary>
    [Export] public float WalkMaxSpeed { get; set; } = 0.5f;

    /// <summary>Deceleration (m/s²) beyond which the Stop state plays.</summary>
    [Export] public float StopDeceleration { get; set; } = 14.0f;

    /// <summary>Normalized speed below which Stop hands over to idle.</summary>
    [Export] public float StopMinSpeed { get; set; } = 0.15f;
}
