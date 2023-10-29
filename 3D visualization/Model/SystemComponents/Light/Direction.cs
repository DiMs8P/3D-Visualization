using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Light;

public struct Direction
{
    public Vector3 To;

    public Direction(Vector3 direction)
    {
        To = direction;
    }
}