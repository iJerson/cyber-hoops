using Godot;

namespace CyberHoops.Gameplay.AI;

/// <summary>
/// Data-driven AI tuning. Designers tune the .tres; no values in code.
/// </summary>
[GlobalClass]
public partial class AIStats : Resource
{
    /// <summary>Distance from the hoop at which the AI takes the shot, in metres.</summary>
    [Export] public float ShootRange { get; set; } = 4.0f;

    /// <summary>How far in front of the ball-handler the AI positions when defending, in metres.</summary>
    [Export] public float DefendDistance { get; set; } = 1.2f;

    /// <summary>Distance to a steering target below which the AI stops pushing input, in metres.</summary>
    [Export] public float ArriveRadius { get; set; } = 0.3f;

    /// <summary>The AI sprints when its steering target is farther than this, in metres.</summary>
    [Export] public float SprintBeyondDistance { get; set; } = 4.0f;
}
