namespace _3D_visualization.Model.SystemComponents.Input.Components;

public struct MouseRotation
{
    public readonly float Sensitivity;
    public float Yaw;
    public float Pitch;

    public MouseRotation()
    {
        Sensitivity = 0.15f;
        Yaw = -90.0f;
        Pitch = 0.0f;
    }
}