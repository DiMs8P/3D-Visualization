using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Light;

public struct LightProperties
{
    public Vector3 Ambient;
    public Vector3 Diffuse;
    public Vector3 Specular;
    
    public LightProperties(Vector3 ambient, Vector3 diffuse, Vector3 specular)
    {
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
    }
}