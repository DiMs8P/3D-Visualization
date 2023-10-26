using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.Render;

public class OpenGlPreRunSystem : IEcsRunSystem
{
    [EcsInject] OpenGLControl _openGlControl;
    public void Run(IEcsSystems systems)
    {
        OpenGL gl = _openGlControl.OpenGL;
        
        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        gl.LoadIdentity();
    }
}