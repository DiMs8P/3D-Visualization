using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.Render;

public class OpenGlPostRunSystem : IEcsRunSystem
{
    [EcsInject] OpenGLControl _openGlControl;
    public void Run(IEcsSystems systems)
    {
        OpenGL gl = _openGlControl.OpenGL;
        
        gl.Flush();
    }
}