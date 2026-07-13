using CyberHoops.Core.Match;
using Godot;
using CyberHoopsBall = CyberHoops.Gameplay.Ball.Ball;
using CyberHoops.Gameplay.Court;

namespace CyberHoops.Gameplay.Match;

/// <summary>
/// Runs the 1v1 half-court match to the target score. Observes the hoop's
/// <see cref="HoopSensor.MadeShot"/>, awards points via the engine-free
/// <see cref="MatchScore"/>, hands the ball to the conceding player after each
/// basket, and pauses the tree on a win until restart is pressed.
/// </summary>
[GlobalClass]
public partial class MatchManager : Node
{
    [Signal] public delegate void ScoreChangedEventHandler(int humanPoints, int aiPoints);
    [Signal] public delegate void MatchWonEventHandler(int winningTeam);

    [Export] public MatchRules? Rules { get; set; }
    [Export] public CharacterBody3D? HumanPlayer { get; set; }
    [Export] public CharacterBody3D? AIPlayer { get; set; }
    [Export] public CyberHoopsBall? Ball { get; set; }
    [Export] public HoopSensor? Sensor { get; set; }
    [Export] public StringName RestartAction { get; set; } = "restart";

    /// <summary>Played when the match is won. Optional.</summary>
    [Export] public AudioStreamPlayer? WinSound { get; set; }

    private MatchScore? _score;
    private Transform3D _humanSpawn;
    private Transform3D _aiSpawn;
    private bool _gameOver;

    public override void _Ready()
    {
        if (Rules is null || HumanPlayer is null || AIPlayer is null || Ball is null || Sensor is null)
        {
            GD.PushError($"{nameof(MatchManager)} is missing required exports.");
            return;
        }

        // Must keep processing while the tree is paused to catch the restart input.
        ProcessMode = ProcessModeEnum.Always;
        _score = new MatchScore(Rules.TargetScore);
        _humanSpawn = HumanPlayer.GlobalTransform;
        _aiSpawn = AIPlayer.GlobalTransform;
        Sensor.MadeShot += OnMadeShot;
    }

    public override void _Process(double delta)
    {
        if (_gameOver && Input.IsActionJustPressed(RestartAction))
        {
            GetTree().Paused = false;
            GetTree().ReloadCurrentScene();
        }
    }

    private void OnMadeShot(CyberHoopsBall ball)
    {
        var team = ball.LastShooterTeam;
        if (_score is null || _gameOver || team is not (0 or 1))
        {
            return;
        }

        var rules = Rules!;
        var points = ball.LastShotGroundDistance > rules.ThreePointDistance
            ? rules.OutsideShotPoints
            : rules.InsideShotPoints;

        var won = _score.AddPoints(team, points);
        EmitSignal(SignalName.ScoreChanged, _score.PointsFor(0), _score.PointsFor(1));

        if (won)
        {
            _gameOver = true;
            EmitSignal(SignalName.MatchWon, team);
            WinSound?.Play();
            GetTree().Paused = true;
            return;
        }

        ResetAfterBasket(concedingTeam: team == 0 ? 1 : 0);
    }

    /// <summary>Both players back to spawn; ball checked to the team that conceded.</summary>
    private void ResetAfterBasket(int concedingTeam)
    {
        ResetBody(HumanPlayer!, _humanSpawn);
        ResetBody(AIPlayer!, _aiSpawn);

        var receiverSpawn = concedingTeam == 0 ? _humanSpawn : _aiSpawn;
        var towardCentre = (Vector3.Zero - receiverSpawn.Origin) with { Y = 0f };
        var offset = towardCentre.LengthSquared() < 0.0001f
            ? Vector3.Zero
            : towardCentre.Normalized() * Rules!.CheckBallOffset;
        Ball!.ResetAt(receiverSpawn.Origin + offset + Vector3.Up * Ball.Stats!.Radius);
    }

    private static void ResetBody(CharacterBody3D body, Transform3D spawn)
    {
        body.GlobalTransform = spawn;
        body.Velocity = Vector3.Zero;
    }
}
