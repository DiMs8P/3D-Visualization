using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Transform.Components;

public struct Transform
{
    public Vector3 Position;

    public Transform()
    {
        Position = Vector3.Zero;
    }
}