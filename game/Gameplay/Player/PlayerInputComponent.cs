using Godot;

namespace CyberHoops.Gameplay.Player;

/// <summary>
/// Reads the human player's input actions and exposes them as movement intent.
/// Action names are exported so bindings stay data-driven.
/// </summary>
[GlobalClass]
public partial class PlayerInputComponent : Node, IMovementInputSource
{
    [Export] public StringName MoveLeftAction { get; set; } = "move_left";
    [Export] public StringName MoveRightAction { get; set; } = "move_right";
    [Export] public StringName MoveUpAction { get; set; } = "move_up";
    [Export] public StringName MoveDownAction { get; set; } = "move_down";
    [Export] public StringName SprintAction { get; set; } = "sprint";

    public Vector2 GetMoveDirection() =>
        Input.GetVector(MoveLeftAction, MoveRightAction, MoveUpAction, MoveDownAction);

    public bool IsSprintRequested() => Input.IsActionPressed(SprintAction);
}
