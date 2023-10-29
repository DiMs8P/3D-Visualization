using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Light;

public struct PointLight
{
    public Vector3 LightColor;

    public PointLight(Vector3 color)
    {
        LightColor = color;
    }
}