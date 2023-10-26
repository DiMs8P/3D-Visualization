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
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

        0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
        0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
        0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
        0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
        0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
        0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
        0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
        0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
    };

    private Shader _lightingShader;
    private Shader _lampShader;
    private uint _lightVAO;
    private uint _cubeVAO;

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
        
        uint[] VBO = new uint[1];
        uint[] VAO = new uint[1];
        gl.GenVertexArrays(1, VAO);
        gl.GenBuffers(1, VBO);
        
        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, VBO[0]);
        gl.BufferData(OpenGL.GL_ARRAY_BUFFER, _vertices, OpenGL.GL_STATIC_DRAW);
        
        gl.BindVertexArray(VAO[0]);
        
        gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 0 * sizeof(float), IntPtr.Zero);
        gl.EnableVertexAttribArray(0);

        gl.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, 3 * sizeof(float), IntPtr.Zero);
        gl.EnableVertexAttribArray(1);
        
        uint[] lightVAO = new uint[1];
        gl.GenVertexArrays(1, lightVAO);
        gl.BindVertexArray(lightVAO[0]);

        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, VBO[0]);

        gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 6 * sizeof(float), IntPtr.Zero);
        gl.EnableVertexAttribArray(0);
        
        _cubeVAO = VAO[0];
        _lightVAO = lightVAO[0];
    }

    public void Run(IEcsSystems systems)
    {
        OpenGL gl = _openGlControl.OpenGL;
        
        gl.PushMatrix();
        
        _lampShader.Use();
        _lampShader.SetMat4("projection", gl.GetProjectionMatrix().AsRowMajorArrayFloat);
        _lampShader.SetMat4("modelview", gl.GetModelViewMatrix().AsRowMajorArrayFloat);

        gl.BindVertexArray(_lightVAO);
        gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
        
        _lightingShader.Use();
        _lightingShader.SetVec3("objectColor", 1.0f, 0.5f, 0.31f);
        _lightingShader.SetVec3("lightColor", 1.0f, 1.0f, 1.0f);
        _lightingShader.SetVec3("lightPos", 1.2f, 1.0f, 2.0f);
        _lightingShader.SetVec3("viewPos", 0.0f, 0.0f, 0.3f);
        
        _lightingShader.SetMat4("projection", gl.GetProjectionMatrix().AsRowMajorArrayFloat);
        _lightingShader.SetMat4("modelview", gl.GetModelViewMatrix().AsRowMajorArrayFloat);
        
        gl.BindVertexArray(_cubeVAO);
        gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
        
        gl.PopMatrix();
    }
}