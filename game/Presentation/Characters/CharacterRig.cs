using Godot;

namespace CyberHoops.Presentation.Characters;

/// <summary>
/// Dumb pose surface: exposes the rig's joint pivots as channels for
/// <see cref="AnimationController"/> (the only writer) and recolors emissive
/// accent parts from one exported AccentColor so the same scene serves both
/// players. No behavior of its own.
/// </summary>
[GlobalClass]
public partial class CharacterRig : Node3D
{
    [Export] public Color AccentColor { get; set; } = new(0.1f, 0.9f, 1f);

    [Export] public Node3D? Pelvis { get; set; }
    [Export] public Node3D? Head { get; set; }
    [Export] public Node3D? ShoulderPivotL { get; set; }
    [Export] public Node3D? ShoulderPivotR { get; set; }
    [Export] public Node3D? HipPivotL { get; set; }
    [Export] public Node3D? HipPivotR { get; set; }

    public override void _Ready()
    {
        Recolor(this);
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
