using Godot;
using CyberHoops.Gameplay.Match;

namespace CyberHoops.Presentation.UI;

/// <summary>
/// Minimal match HUD: live score line and a centre message on game over.
/// Pure observer of <see cref="MatchManager"/> signals.
/// </summary>
[GlobalClass]
public partial class HudComponent : CanvasLayer
{
    [Export] public MatchManager? Match { get; set; }
    [Export] public Label? ScoreLabel { get; set; }
    [Export] public Label? MessageLabel { get; set; }

    public override void _Ready()
    {
        if (Match is null || ScoreLabel is null || MessageLabel is null)
        {
            GD.PushError($"{nameof(HudComponent)} is missing required exports.");
            return;
        }

        MessageLabel.Visible = false;
        UpdateScore(0, 0);
        Match.ScoreChanged += UpdateScore;
        Match.MatchWon += OnMatchWon;
    }

    private void UpdateScore(int humanPoints, int aiPoints)
    {
        ScoreLabel!.Text = $"YOU {humanPoints}  —  {aiPoints} CPU";
    }

    private void OnMatchWon(int winningTeam)
    {
        MessageLabel!.Text = winningTeam == 0
            ? "YOU WIN!\nPress Enter to play again"
            : "CPU WINS\nPress Enter to play again";
        MessageLabel.Visible = true;
    }
}
