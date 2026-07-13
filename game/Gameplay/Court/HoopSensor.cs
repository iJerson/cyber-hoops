using Godot;
using CyberHoopsBall = CyberHoops.Gameplay.Ball.Ball;

namespace CyberHoops.Gameplay.Court;

/// <summary>
/// Area just below the rim. A free ball crossing it while moving downward
/// counts as a made shot (Observer pattern — score/rules systems subscribe in M5).
/// </summary>
[GlobalClass]
public partial class HoopSensor : Area3D
{
    /// <summary>Emitted once per made shot with the scoring ball.</summary>
    [Signal] public delegate void MadeShotEventHandler(CyberHoopsBall ball);

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is CyberHoopsBall ball
            && ball.CurrentStateName == CyberHoopsBall.FreeState
            && ball.LinearVelocity.Y < 0f)
        {
            EmitSignal(SignalName.MadeShot, ball);
            GD.Print("Made shot!");
        }
    }
}
