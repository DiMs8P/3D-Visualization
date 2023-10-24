namespace _3D_visualization.Model.SystemComponents.MainCamera.Components;

public struct Camera
{
    public float Sensitivity;
    public float Yaw;
    public float Pitch;

    public Camera()
    {
        Sensitivity = 0.15f;
        Yaw = -90.0f;
        Pitch = 0.0f;
    }
}