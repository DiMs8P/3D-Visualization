using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.Render;

public class LightningRenderSystem : IEcsRunSystem, IEcsInitSystem
{
    [EcsInject] private OpenGLControl _openGlControl;

    public void Init(IEcsSystems systems)
    {
        OpenGL gl = _openGlControl.OpenGL;

        uint[] VBO = new uint[1];
        uint[] VAO = new uint[1];
        gl.GenVertexArrays(1, VAO);
        gl.GenBuffers(1, VBO);
        
        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, VBO[0]);
        //gl.BufferData();
    }

    public void Run(IEcsSystems systems)
    {
        OpenGL gl = _openGlControl.OpenGL;
        
        gl.PushMatrix();

        gl.Begin(OpenGL.GL_POLYGON);
        
        gl.Color(0.0f, 0.0f, 1.0f);
        gl.Vertex(-1.0f, -1.0f, 0.0f);
        
        gl.Color(0.0f, 0.0f, 1.0f);
        gl.Vertex(1.0f, -1.0f, 0.0f);
        
        gl.Color(0.0f, 0.0f, 1.0f);
        gl.Vertex(0.0f, 1.0f, 0.0f);
        
        gl.End();
        
        gl.PopMatrix();
    }
}