using Godot;

namespace CyberHoops.Presentation.Characters;

/// <summary>
/// Humanoid rig with jointed limbs and procedural locomotion. Limb pivots
/// (shoulders, hips) swing from the parent body's horizontal speed so the
/// figure walks/runs like a human; idle gets a subtle breathing sway. Also
/// recolors all emissive accent parts from one exported AccentColor so the
/// same rig serves both players. Phase advances with distance travelled, so
/// the animation is deterministic.
/// </summary>
[GlobalClass]
public partial class CharacterRig : Node3D
{
    [Export] public Color AccentColor { get; set; } = new(0.1f, 0.9f, 1f);

    /// <summary>Leg swing amplitude at full run, in radians.</summary>
    [Export] public float LegSwing { get; set; } = 0.7f;

    /// <summary>Arm swing amplitude at full run, in radians.</summary>
    [Export] public float ArmSwing { get; set; } = 0.55f;

    /// <summary>Stride cycles per metre travelled.</summary>
    [Export] public float StrideFrequency { get; set; } = 0.55f;

    /// <summary>Speed treated as a full run for animation blending, in m/s.</summary>
    [Export] public float RunSpeed { get; set; } = 8.0f;

    /// <summary>Vertical body bob at full run, in metres.</summary>
    [Export] public float BobHeight { get; set; } = 0.05f;

    [Export] public Node3D? Pelvis { get; set; }
    [Export] public Node3D? ShoulderPivotL { get; set; }
    [Export] public Node3D? ShoulderPivotR { get; set; }
    [Export] public Node3D? HipPivotL { get; set; }
    [Export] public Node3D? HipPivotR { get; set; }

    /// <summary>Shoulder pitch when arms are fully raised overhead, in radians.</summary>
    [Export] public float ArmsRaisedAngle { get; set; } = -2.7f;

    /// <summary>How fast the arms blend to/from the raised pose, per second.</summary>
    [Export] public float ArmsRaiseSpeed { get; set; } = 10.0f;

    private CharacterBody3D? _body;
    private float _phase;
    private float _idleTime;
    private float _pelvisRestY;
    private float _armsRaisedTarget;
    private float _armsRaised;

    /// <summary>0 = arms follow gait, 1 = both arms overhead (dunk wind-up).</summary>
    public void SetArmsRaised(float amount) => _armsRaisedTarget = Mathf.Clamp(amount, 0f, 1f);

    public override void _Ready()
    {
        Recolor(this);
        _body = GetParent() as CharacterBody3D;
        if (Pelvis is not null)
        {
            _pelvisRestY = Pelvis.Position.Y;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Pelvis is null || ShoulderPivotL is null || ShoulderPivotR is null || HipPivotL is null || HipPivotR is null)
        {
            return;
        }

        var velocity = _body?.Velocity ?? Vector3.Zero;
        var speed = (velocity with { Y = 0f }).Length();
        var stride = Mathf.Clamp(speed / RunSpeed, 0f, 1f);

        _phase += speed * (float)delta * StrideFrequency * Mathf.Tau;
        _phase %= Mathf.Tau;
        _idleTime += (float)delta;

        var swing = Mathf.Sin(_phase);

        // Legs alternate; arms counter-swing their same-side leg.
        HipPivotL.Rotation = new Vector3(swing * LegSwing * stride, 0f, 0f);
        HipPivotR.Rotation = new Vector3(-swing * LegSwing * stride, 0f, 0f);

        // Blend gait swing against the raised-overhead pose (dunks).
        _armsRaised = Mathf.MoveToward(_armsRaised, _armsRaisedTarget, ArmsRaiseSpeed * (float)delta);
        var armL = Mathf.Lerp(-swing * ArmSwing * stride, ArmsRaisedAngle, _armsRaised);
        var armR = Mathf.Lerp(swing * ArmSwing * stride, ArmsRaisedAngle, _armsRaised);
        ShoulderPivotL.Rotation = new Vector3(armL, 0f, 0f);
        ShoulderPivotR.Rotation = new Vector3(armR, 0f, 0f);

        // Two bobs per stride while moving; slow breathing sway at rest.
        var bob = Mathf.Abs(Mathf.Sin(_phase * 2f)) * BobHeight * stride;
        var breathe = (1f - stride) * 0.012f * Mathf.Sin(_idleTime * 2.2f);
        Pelvis.Position = Pelvis.Position with { Y = _pelvisRestY + bob + breathe };
    }

    private void Recolor(Node node)
    {
        if (node is MeshInstance3D mesh
            && mesh.GetSurfaceOverrideMaterial(0) is StandardMaterial3D { EmissionEnabled: true } material)
        {
            var tinted = (StandardMaterial3D)material.Duplicate();
            tinted.AlbedoColor = AccentColor;
            tinted.Emission = AccentColor;
            mesh.SetSurfaceOverrideMaterial(0, tinted);
        }

        foreach (var child in node.GetChildren())
        {
            Recolor(child);
        }
    }
}
