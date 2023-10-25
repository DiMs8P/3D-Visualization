using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Spline.Components;

public struct Spline
{
    public List<Vector2> Section;
    public List<Vector3> Path;
    public List<List<Vector3>> PointLocations;

    public Spline(List<Vector2> section, List<Vector3> path)
    {
        Section = section;
        Path = path;
        PointLocations = new List<List<Vector3>>();
    }
}