using _3D_visualization.Model.SystemComponents.Render;
using Leopotam.EcsLite;
using SharpGL.SceneGraph;
using SharpGL.WPF;

namespace _3D_visualization.Model.Environment;

public class ShaderManager
{
    private EcsWorld _world;
    private OpenGLControl _openGlControl;

    private Shader _lampShader;
    private Shader _splineShader;
    
    public ShaderManager(EcsWorld world, OpenGLControl openGlControl)
    {
        _world = world;
        _openGlControl = openGlControl;

        CreateLampShader();
        CreateSplineShader();
    }
    
    private void CreateLampShader()
    {
        _lampShader = new Shader(
            _openGlControl.OpenGL,
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\lamp_vertex.txt",
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\lamp_fragment.txt"
        );
    }

    private void CreateSplineShader()
    {
        _splineShader = new Shader(
            _openGlControl.OpenGL,
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\basic_lightning_vertex.txt",
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\basic_lightning_fragment.txt"
        );
    }

    public void UseDefaultShader()
    {
        _openGlControl.OpenGL.UseProgram(0);
    }

    public void UseLampShader()
    {
        _lampShader.Use();
        _lampShader.SetMat4("projection", _openGlControl.OpenGL.GetProjectionMatrix().AsRowMajorArrayFloat);
        _lampShader.SetMat4("modelview", _openGlControl.OpenGL.GetModelViewMatrix().AsRowMajorArrayFloat);
    }

    public void UseSplineShader()
    {
        
    }
}