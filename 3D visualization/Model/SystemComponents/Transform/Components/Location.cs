using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Transform.Components;

public struct Location
{
    public Vector3 Position;

    public Location()
    {
        Position = new Vector3(0f, 0f, 3f);
    }
    
    public Location(Vector3 position)
    {
        Position = position;
    }
}