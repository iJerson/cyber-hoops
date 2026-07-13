using Godot;

namespace CyberHoops.Presentation.Court;

/// <summary>
/// Impact feedback on the hoop: a decaying rim shake plus a one-shot particle
/// burst. Gameplay triggers <see cref="Slam"/> through the "hoop_effects"
/// group so it stays decoupled from the court scene layout.
/// </summary>
[GlobalClass]
public partial class HoopEffects : Node3D
{
    [Export] public Node3D? Rim { get; set; }
    [Export] public GpuParticles3D? Burst { get; set; }

    /// <summary>How long the rim rattles after a slam, in seconds.</summary>
    [Export] public float ShakeDuration { get; set; } = 0.45f;

    /// <summary>Peak vertical rattle of the rim, in metres.</summary>
    [Export] public float ShakeAmplitude { get; set; } = 0.06f;

    /// <summary>Rattle oscillations per second.</summary>
    [Export] public float ShakeFrequency { get; set; } = 18.0f;

    private Vector3 _rimRestPosition;
    private double _shakeRemaining;

    public override void _Ready()
    {
        AddToGroup("hoop_effects");
        if (Rim is not null)
        {
            _rimRestPosition = Rim.Position;
        }
    }

    /// <summary>Kicks off the shake and burst. Safe to call repeatedly.</summary>
    public void Slam()
    {
        _shakeRemaining = ShakeDuration;
        if (Burst is not null)
        {
            Burst.Restart();
            Burst.Emitting = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Rim is null || _shakeRemaining <= 0.0)
        {
            return;
        }

        _shakeRemaining -= delta;
        if (_shakeRemaining <= 0.0)
        {
            Rim.Position = _rimRestPosition;
            return;
        }

        var elapsed = ShakeDuration - (float)_shakeRemaining;
        var decay = (float)(_shakeRemaining / ShakeDuration);
        var offset = Mathf.Sin(elapsed * ShakeFrequency * Mathf.Tau) * ShakeAmplitude * decay;
        Rim.Position = _rimRestPosition + Vector3.Down * Mathf.Abs(offset) + Vector3.Up * offset * 0.3f;
    }
}
