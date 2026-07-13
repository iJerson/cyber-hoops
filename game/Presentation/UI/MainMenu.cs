using Godot;

namespace CyberHoops.Presentation.UI;

/// <summary>
/// Entry menu: start a match or quit.
/// </summary>
[GlobalClass]
public partial class MainMenu : Control
{
    [Export(PropertyHint.File, "*.tscn")] public string MatchScenePath { get; set; } = "res://game/Main.tscn";
    [Export] public Button? PlayButton { get; set; }
    [Export] public Button? QuitButton { get; set; }

    public override void _Ready()
    {
        if (PlayButton is null || QuitButton is null)
        {
            GD.PushError($"{nameof(MainMenu)} requires {nameof(PlayButton)} and {nameof(QuitButton)}.");
            return;
        }

        PlayButton.Pressed += () => GetTree().ChangeSceneToFile(MatchScenePath);
        QuitButton.Pressed += () => GetTree().Quit();
        PlayButton.GrabFocus();
    }
}
