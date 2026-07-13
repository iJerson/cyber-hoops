using Godot;

namespace CyberHoops.Presentation.UI;

/// <summary>
/// In-match pause menu. Esc (ui_cancel) toggles; keeps processing while the
/// tree is paused. Ignores pause requests once the match is over (the win
/// screen owns the paused state then).
/// </summary>
[GlobalClass]
public partial class PauseMenu : Control
{
    [Export(PropertyHint.File, "*.tscn")] public string MenuScenePath { get; set; } = "res://game/Presentation/UI/MainMenu.tscn";
    [Export] public Button? ResumeButton { get; set; }
    [Export] public Button? RestartButton { get; set; }
    [Export] public Button? MenuButton { get; set; }

    private bool _pausedByMenu;

    public override void _Ready()
    {
        if (ResumeButton is null || RestartButton is null || MenuButton is null)
        {
            GD.PushError($"{nameof(PauseMenu)} is missing button exports.");
            return;
        }

        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
        ResumeButton.Pressed += Resume;
        RestartButton.Pressed += () => LeaveTo(() => GetTree().ReloadCurrentScene());
        MenuButton.Pressed += () => LeaveTo(() => GetTree().ChangeSceneToFile(MenuScenePath));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_cancel"))
        {
            return;
        }

        if (_pausedByMenu)
        {
            Resume();
        }
        else if (!GetTree().Paused)
        {
            _pausedByMenu = true;
            GetTree().Paused = true;
            Visible = true;
            ResumeButton!.GrabFocus();
        }
    }

    private void Resume()
    {
        _pausedByMenu = false;
        GetTree().Paused = false;
        Visible = false;
    }

    private void LeaveTo(System.Action change)
    {
        _pausedByMenu = false;
        GetTree().Paused = false;
        change();
    }
}
