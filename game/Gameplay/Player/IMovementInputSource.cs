using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Strategy interface feeding movement intent to <see cref="MovementComponent"/>.
/// Implemented by <see cref="PlayerInputComponent"/> for humans and, in M4, by
/// the AI controller — the movement component never knows who is driving.
/// </summary>
public interface IMovementInputSource
{
    /// <summary>Desired movement direction on the ground plane, length ≤ 1.</summary>
    Vector2 GetMoveDirection();

    /// <summary>Whether sprint is currently requested.</summary>
    bool IsSprintRequested();
}
