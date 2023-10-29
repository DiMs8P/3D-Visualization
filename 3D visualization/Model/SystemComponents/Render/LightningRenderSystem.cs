using System.Numerics;
using _3D_visualization.Model.Environment;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.Render;

public class LightningRenderSystem : IEcsRunSystem, IEcsInitSystem
{
    [EcsInject] private OpenGLControl _openGlControl;
    [EcsInject] private ShaderManager _shaderManager;
    
    float[] _vertices = new[] {
        -1f, -1f, -1f,  0.0f,  0.0f, -1.0f,
        1f, -1f, -1f,  0.0f,  0.0f, -1.0f,
        1f,  1f, -1f,  0.0f,  0.0f, -1.0f,
        1f,  1f, -1f,  0.0f,  0.0f, -1.0f,
        -1f,  1f, -1f,  0.0f,  0.0f, -1.0f,
        -1f, -1f, -1f,  0.0f,  0.0f, -1.0f,

        -1f, -1f,  1f,  0.0f,  0.0f,  1.0f,
        1f, -1f,  1f,  0.0f,  0.0f,  1.0f,
        1f,  1f,  1f,  0.0f,  0.0f,  1.0f,
        1f,  1f,  1f,  0.0f,  0.0f,  1.0f,
        -1f,  1f,  1f,  0.0f,  0.0f,  1.0f,
        -1f, -1f,  1f,  0.0f,  0.0f,  1.0f,

        -1f,  1f,  1f, -1.0f,  0.0f,  0.0f,
        -1f,  1f, -1f, -1.0f,  0.0f,  0.0f,
        -1f, -1f, -1f, -1.0f,  0.0f,  0.0f,
        -1f, -1f, -1f, -1.0f,  0.0f,  0.0f,
        -1f, -1f,  1f, -1.0f,  0.0f,  0.0f,
        -1f,  1f,  1f, -1.0f,  0.0f,  0.0f,

        1f,  1f,  1f,  1.0f,  0.0f,  0.0f,
        1f,  1f, -1f,  1.0f,  0.0f,  0.0f,
        1f, -1f, -1f,  1.0f,  0.0f,  0.0f,
        1f, -1f, -1f,  1.0f,  0.0f,  0.0f,
        1f, -1f,  1f,  1.0f,  0.0f,  0.0f,
        1f,  1f,  1f,  1.0f,  0.0f,  0.0f,

        -1f, -1f, -1f,  0.0f, -1.0f,  0.0f,
        1f, -1f, -1f,  0.0f, -1.0f,  0.0f,
        1f, -1f,  1f,  0.0f, -1.0f,  0.0f,
        1f, -1f,  1f,  0.0f, -1.0f,  0.0f,
        -1f, -1f,  1f,  0.0f, -1.0f,  0.0f,
        -1f, -1f, -1f,  0.0f, -1.0f,  0.0f,

        -1f,  1f, -1f,  0.0f,  1.0f,  0.0f,
        1f,  1f, -1f,  0.0f,  1.0f,  0.0f,
        1f,  1f,  1f,  0.0f,  1.0f,  0.0f,
        1f,  1f,  1f,  0.0f,  1.0f,  0.0f,
        -1f,  1f,  1f,  0.0f,  1.0f,  0.0f,
        -1f,  1f, -1f,  0.0f,  1.0f,  0.0f
    };

    private uint _lightVAO;

    private Vector3 _lightPos = new(1.2f, 1.0f, 1.0f);

    public void Init(IEcsSystems systems)
    {
        OpenGL gl = _openGlControl.OpenGL;
        
        uint[] lightVBO = new uint[1];
        uint[] lightVAO = new uint[1];
        
        gl.GenVertexArrays(1, lightVBO);
        gl.GenBuffers(1, lightVAO);
        
        gl.BindVertexArray(lightVAO[0]);
        
        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, lightVBO[0]);
        gl.BufferData(OpenGL.GL_ARRAY_BUFFER, _vertices, OpenGL.GL_STATIC_DRAW);
        
        gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 6 * sizeof(float), IntPtr.Zero);
        
        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
        gl.BindVertexArray(0);
        
        _lightVAO = lightVAO[0];
    }

    public void Run(IEcsSystems systems)
    {
        OpenGL gl = _openGlControl.OpenGL;
        gl.PushMatrix();
        
        gl.EnableVertexAttribArray(0);
        gl.Translate(_lightPos.X, _lightPos.Y, _lightPos.Z);
        gl.Scale(0.1, 0.1, 0.1);
        
        _shaderManager.UseLampShader();

        gl.BindVertexArray(_lightVAO);
        gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
        
        gl.DisableVertexAttribArray(0);
        
        _shaderManager.UseDefaultShader();
        
        gl.PopMatrix();
    }
}