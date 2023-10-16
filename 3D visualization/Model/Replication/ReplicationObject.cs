using System.Numerics;

namespace _3D_visualization.Model.Replication;

public class ReplicationObject
{
    public List<Vector3> Path;
    public List<Vector2> Section;

    public ReplicationObject()
    {
        Path = new List<Vector3>();
        Section = new List<Vector2>();
    }
}