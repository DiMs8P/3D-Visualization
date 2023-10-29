using System.Numerics;

namespace _3D_visualization.Model.SystemComponents.Light;

public struct Attenuation
{
    public float Constant;
    public float Linear;
    public float Quadratic;
    
    public Attenuation(float constant, float linear, float quadratic)
    {
        Constant = constant;
        Linear = linear;
        Quadratic = quadratic;
    }
}