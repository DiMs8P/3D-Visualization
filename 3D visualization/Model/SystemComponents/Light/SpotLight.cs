using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Light;

public struct SpotLight
{
    public Vector3 LightColor;

    public SpotLight(Vector3 color)
    {
        LightColor = color;
    }
}