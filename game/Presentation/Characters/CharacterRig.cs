using Godot;

namespace CyberHoops.Presentation.Characters;

/// <summary>
/// Recolors the rig's emissive accent parts (visor, core, ring, trims) so one
/// shared humanoid scene serves both players — cyan for the human, magenta
/// for the CPU. Any child mesh whose override material has emission enabled
/// is treated as an accent part.
/// </summary>
[GlobalClass]
public partial class CharacterRig : Node3D
{
    [Export] public Color AccentColor { get; set; } = new(0.1f, 0.9f, 1f);

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
