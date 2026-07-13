using Godot;
using CyberHoops.Gameplay.Player;

namespace CyberHoops.Gameplay.AI;

/// <summary>
/// Movement intent written by <see cref="AIBrainComponent"/> and read by the
/// same <see cref="MovementComponent"/> humans use — the movement code never
/// knows an AI is driving.
/// </summary>
[GlobalClass]
public partial class AIInputComponent : Node, IMovementInputSource
{
    /// <summary>World-space ground-plane direction the brain wants to move, length ≤ 1.</summary>
    public Vector2 DesiredDirection { get; set; } = Vector2.Zero;

    public bool SprintRequested { get; set; }

    public Vector2 GetMoveDirection() => DesiredDirection;

    public bool IsSprintRequested() => SprintRequested;
}
