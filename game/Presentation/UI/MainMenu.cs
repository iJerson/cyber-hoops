using Godot;

namespace CyberHoops.Presentation.UI;

/// <summary>
/// Entry menu: start a match or quit.
/// </summary>
[GlobalClass]
public partial class MainMenu : Control
{
    [Export(PropertyHint.File, "*.tscn")] public string MatchScenePath { get; set; } = "res://game/Main.tscn";
    [Export(PropertyHint.File, "*.tscn")] public string PracticeScenePath { get; set; } = "res://game/Practice.tscn";
    [Export] public Button? PlayButton { get; set; }
    [Export] public Button? PracticeButton { get; set; }
    [Export] public Button? QuitButton { get; set; }

    public override void _Ready()
    {
        if (PlayButton is null || PracticeButton is null || QuitButton is null)
        {
            GD.PushError($"{nameof(MainMenu)} is missing button exports.");
            return;
        }

        PlayButton.Pressed += () => GetTree().ChangeSceneToFile(MatchScenePath);
        PracticeButton.Pressed += () => GetTree().ChangeSceneToFile(PracticeScenePath);
        QuitButton.Pressed += () => GetTree().Quit();
        PlayButton.GrabFocus();
    }
}
