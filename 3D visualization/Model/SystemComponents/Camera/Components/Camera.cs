using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Camera.Components;

public struct Camera
{
    public Vector3 Location;
    public Vector3 UpVector;
    public Vector3 ForwardVector;

    public Camera()
    {
        Location = Vector3.Zero;
        UpVector = Vector3.UnitY;
        ForwardVector = Vector3.UnitZ;
    }
}