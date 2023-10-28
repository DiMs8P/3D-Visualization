using System.Numerics;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.Render;

public class LightningRenderSystem : IEcsRunSystem, IEcsInitSystem
{
    [EcsInject] private OpenGLControl _openGlControl;
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

    private Shader _lightingShader;
    private Shader _lampShader;
    private uint _lightVAO;

    private Vector3 _lightPos = new(1.2f, 1.0f, 0.0f);

    public void Init(IEcsSystems systems)
    {
        OpenGL gl = _openGlControl.OpenGL;

        _lightingShader = new Shader(
            gl,
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\basic_lightning_vertex.txt",
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\basic_lightning_fragment.txt"
        );
        
        _lampShader = new Shader(
            gl,
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\lamp_vertex.txt",
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\lamp_fragment.txt"
        );
        
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
        
        _lampShader.Use();
        _lampShader.SetMat4("projection", gl.GetProjectionMatrix().AsRowMajorArrayFloat);
        _lampShader.SetMat4("modelview", gl.GetModelViewMatrix().AsRowMajorArrayFloat);

        gl.BindVertexArray(_lightVAO);
        gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
        
        gl.DisableVertexAttribArray(0);
        
        gl.PopMatrix();
        
        _lightingShader.Use();
        _lightingShader.SetVec3("objectColor", 1.0f, 0.5f, 0.31f);
        _lightingShader.SetVec3("lightColor", 1.0f, 1.0f, 1.0f);
        _lightingShader.SetVec3("lightPos", _lightPos.X, _lightPos.Y, _lightPos.Z);
        _lightingShader.SetVec3("viewPos", 0.0f, 0.0f, 0.3f);
        
        _lightingShader.SetMat4("projection", gl.GetProjectionMatrix().AsRowMajorArrayFloat);
        _lightingShader.SetMat4("modelview", gl.GetModelViewMatrix().AsRowMajorArrayFloat);
        
    }
}