using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Spline.Components;

public struct Spline
{
    public List<Vector2> Section;
    public List<Vector3> Path;
    public List<List<Vector3>> PointsLocation;
    public List<Vector3> Normals;
    public List<Vector3> PointsColor;
    public List<Vector2> TexCoords;

    public Spline(List<Vector2> section, List<Vector3> path)
    {
        Section = section;
        Path = path;
        PointsLocation = new List<List<Vector3>>();
        Normals = new List<Vector3>();
        PointsColor = new List<Vector3>();
        TexCoords = new List<Vector2>();
    }
}