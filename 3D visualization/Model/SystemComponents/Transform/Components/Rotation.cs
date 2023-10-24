using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Transform.Components;

public struct Rotation
{
    public Vector3 ForwardVector;
    public Vector3 UpVector;
    public Vector3 RightVector;

    public Rotation()
    {
        ForwardVector = -Vector3.Zero;
        UpVector = Vector3.UnitY;
        RightVector = Vector3.UnitX;
    }
}