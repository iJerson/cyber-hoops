using Godot;

namespace CyberHoops.Presentation.Court;

/// <summary>
/// Draws court markings (boundary, key, free-throw circle, three-point arc)
/// as flat geometry just above the floor. Purely presentational; dimensions
/// are exported so they can match the match rules (e.g. three-point radius).
/// </summary>
[GlobalClass]
public partial class CourtLines : MeshInstance3D
{
    [Export] public float CourtWidth { get; set; } = 15.0f;
    [Export] public float CourtLength { get; set; } = 14.0f;

    /// <summary>Z offset of the hoop from court centre; arcs are centred here.</summary>
    [Export] public float HoopZ { get; set; } = -6.0f;

    /// <summary>Must match MatchRules.ThreePointDistance for honest visuals.</summary>
    [Export] public float ThreePointRadius { get; set; } = 6.75f;

    [Export] public float KeyWidth { get; set; } = 4.9f;
    [Export] public float KeyLength { get; set; } = 5.8f;
    [Export] public float FreeThrowCircleRadius { get; set; } = 1.8f;
    [Export] public float LineWidth { get; set; } = 0.05f;
    [Export] public Color LineColor { get; set; } = new(0.95f, 0.95f, 0.95f);

    private const float LineY = 0.01f;
    private const int ArcSegments = 48;

    public override void _Ready()
    {
        var surface = new SurfaceTool();
        surface.Begin(Mesh.PrimitiveType.Triangles);

        var halfW = CourtWidth / 2f;
        var halfL = CourtLength / 2f;
        var backZ = HoopZ - 1.2f; // baseline behind the hoop

        // Boundary rectangle.
        AddSegment(surface, new Vector2(-halfW, backZ), new Vector2(halfW, backZ));
        AddSegment(surface, new Vector2(-halfW, halfL), new Vector2(halfW, halfL));
        AddSegment(surface, new Vector2(-halfW, backZ), new Vector2(-halfW, halfL));
        AddSegment(surface, new Vector2(halfW, backZ), new Vector2(halfW, halfL));

        // Key (paint) from baseline toward centre.
        var keyHalf = KeyWidth / 2f;
        var keyEnd = backZ + KeyLength;
        AddSegment(surface, new Vector2(-keyHalf, backZ), new Vector2(-keyHalf, keyEnd));
        AddSegment(surface, new Vector2(keyHalf, backZ), new Vector2(keyHalf, keyEnd));
        AddSegment(surface, new Vector2(-keyHalf, keyEnd), new Vector2(keyHalf, keyEnd));

        // Free-throw circle at the top of the key.
        AddArc(surface, new Vector2(0f, keyEnd), FreeThrowCircleRadius, 0f, Mathf.Tau);

        // Three-point arc centred on the hoop, clipped to the court half-circle facing centre.
        AddArc(surface, new Vector2(0f, HoopZ), ThreePointRadius, 0f, Mathf.Pi);

        surface.GenerateNormals();
        Mesh = surface.Commit();

        MaterialOverride = new StandardMaterial3D
        {
            AlbedoColor = LineColor,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        };
    }

    private void AddArc(SurfaceTool surface, Vector2 centre, float radius, float fromAngle, float toAngle)
    {
        var step = (toAngle - fromAngle) / ArcSegments;
        for (var i = 0; i < ArcSegments; i++)
        {
            var a0 = fromAngle + step * i;
            var a1 = a0 + step;
            AddSegment(
                surface,
                centre + new Vector2(Mathf.Cos(a0), Mathf.Sin(a0)) * radius,
                centre + new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * radius);
        }
    }

    /// <summary>Adds one flat quad strip between two ground-plane points.</summary>
    private void AddSegment(SurfaceTool surface, Vector2 from, Vector2 to)
    {
        var direction = (to - from).Normalized();
        var side = new Vector2(-direction.Y, direction.X) * (LineWidth / 2f);

        var a = ToFloor(from + side);
        var b = ToFloor(from - side);
        var c = ToFloor(to - side);
        var d = ToFloor(to + side);

        surface.AddVertex(a);
        surface.AddVertex(b);
        surface.AddVertex(c);
        surface.AddVertex(a);
        surface.AddVertex(c);
        surface.AddVertex(d);
    }

    private static Vector3 ToFloor(Vector2 point) => new(point.X, LineY, point.Y);
}
