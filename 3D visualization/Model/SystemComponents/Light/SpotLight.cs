namespace _3D_visualization.Model.SystemComponents.Light;

public struct SpotLight
{
    public float CutOff;
    public float OuterCutOff;
    
    public SpotLight(float cutOff, float outerCutOff)
    {
        CutOff = cutOff;
        OuterCutOff = outerCutOff;
    }
}