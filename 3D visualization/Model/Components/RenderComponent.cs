using SharpGL;

namespace _3D_visualization.Model.Components;

public class RenderComponent : BaseComponent
{
    /*private List<BasePrimitive> _primitives;
    private RenderMode _renderMode = 0;
    private bool drawNormals = false;*/
    
    public RenderComponent()
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().SceneController.RegisterRenderComponent(this);
    }
    
    /*public RenderComponent(List<BasePrimitive> primitives)
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().SceneController.RegisterRenderComponent(this);
        /*_primitives = primitives;#1#
    }*/

    public virtual void Render(OpenGL openGl)
    {
        /*openGl.Begin((uint)_renderMode);

        foreach (var primitive in _primitives)
        {
            openGl.PopMatrix();
            
            openGl.Translate(primitive.Location.X, primitive.Location.Y, primitive.Location.Z);
            openGl.Rotate(primitive.Rotation.X, primitive.Rotation.Y, primitive.Rotation.Z);
            openGl.Scale(primitive.Scale.X, primitive.Scale.Y, primitive.Scale.Z);

            foreach (var point in primitive.Points)
            {
                openGl.Vertex(point.X, point.Y, point.Z);
            }
        }
        openGl.End();*/
    }

    /*public void SetRenderMode(RenderMode renderMode)
    {
        _renderMode = renderMode;
    }

    public void AddPrimitive(BasePrimitive basePrimitive)
    {
        _primitives.Add(basePrimitive);
    }
    
    public void EnableNormals(bool enableNormalsDraw)
    {
        drawNormals = enableNormalsDraw;
    }*/
}

public enum RenderMode
{
    Points = 0,
    Lines = 1,
    LineLoop = 2,
    LineStrip = 3,
    Triangles = 4,
    TrianglesStrip = 5,
    TriangleFan = 6,
    Quads = 7,
    QuadStrip = 8,
}
