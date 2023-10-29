using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Spline.Components;

public struct Spline
{
    public List<Vector2> Section;
    public List<Vector3> Path;
    public List<List<Vector3>> PointsLocation;
    public List<List<Vector3>> Normals;
    public List<List<Vector3>> PointsColor;
    public List<Vector2> TexCoords;

    public float[] VBOdata;
    public int[] Indexes;
    public uint SplineVAO = 0;
    public uint SplineEBO = 0;


    public Spline(List<Vector2> section, List<Vector3> path)
    {
        Section = section;
        Path = path;
        PointsLocation = new List<List<Vector3>>();
        Normals = new List<List<Vector3>>();
        PointsColor = new List<List<Vector3>>();
        TexCoords = new List<Vector2>();

        VBOdata = new float[11 * ((2 * section.Count()) +
                                  (4 * (path.Count() - 1) * section.Count()))];

        Indexes = new int[2 * section.Count() + 6 * section.Count() * (path.Count() - 1)];
    }
}