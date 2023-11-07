using System.Numerics;
using _3D_visualization.Model.Environment;
using _3D_visualization.Model.Events;
using _3D_visualization.Model.SystemComponents.Light;
using _3D_visualization.Model.SystemComponents.Transform.Components;
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
    [EcsInject] private GameplayEventsListener _eventsListener;
    
    [EcsPool] private EcsPool<Location> _locationComponents;
    [EcsPool] private EcsPool<LightProperties> _lightPropertiesComponents;

    private EcsFilter _pointLightFilter;
    
    float[] _vertices = {
        -0.5f, -0.5f, -0.5f,
        0.5f, -0.5f, -0.5f,
        0.5f,  0.5f, -0.5f,
        0.5f,  0.5f, -0.5f,
        -0.5f,  0.5f, -0.5f,
        -0.5f, -0.5f, -0.5f,
        
        -0.5f, -0.5f,  0.5f,
        0.5f, -0.5f,  0.5f,
        0.5f,  0.5f,  0.5f,
        0.5f,  0.5f,  0.5f,
        -0.5f,  0.5f,  0.5f,
        -0.5f, -0.5f,  0.5f,
        
        -0.5f,  0.5f,  0.5f,
        -0.5f,  0.5f, -0.5f,
        -0.5f, -0.5f, -0.5f,
        -0.5f, -0.5f, -0.5f,
        -0.5f, -0.5f,  0.5f,
        -0.5f,  0.5f,  0.5f,
        
        0.5f,  0.5f,  0.5f,
        0.5f,  0.5f, -0.5f,
        0.5f, -0.5f, -0.5f,
        0.5f, -0.5f, -0.5f,
        0.5f, -0.5f,  0.5f,
        0.5f,  0.5f,  0.5f,
        
        -0.5f, -0.5f, -0.5f,
        0.5f, -0.5f, -0.5f,
        0.5f, -0.5f,  0.5f,
        0.5f, -0.5f,  0.5f,
        -0.5f, -0.5f,  0.5f,
        -0.5f, -0.5f, -0.5f,
        
        -0.5f,  0.5f, -0.5f,
        0.5f,  0.5f, -0.5f,
        0.5f,  0.5f,  0.5f,
        0.5f,  0.5f,  0.5f,
        -0.5f,  0.5f,  0.5f,
        -0.5f,  0.5f, -0.5f
    };

    private uint _lightVAO;
    private bool _pointLightEnable = false; 

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _pointLightFilter = world.Filter<PointLight>().End();
        _eventsListener.OnPointLightEnableEvent += enable => _pointLightEnable = enable;

        InitializeSpotLightVao();
    }

    private void InitializeSpotLightVao()
    {
        OpenGL gl = _openGlControl.OpenGL;

        uint[] lightVBO = new uint[1];
        uint[] lightVAO = new uint[1];
        
        gl.GenVertexArrays(1, lightVBO);
        gl.GenBuffers(1, lightVAO);
        
        gl.BindVertexArray(lightVAO[0]);
        
        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, lightVBO[0]);
        gl.BufferData(OpenGL.GL_ARRAY_BUFFER, _vertices, OpenGL.GL_STATIC_DRAW);
        
        gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 3 * sizeof(float), IntPtr.Zero);
        
        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
        gl.BindVertexArray(0);
        
        _lightVAO = lightVAO[0];
    }

    public void Run(IEcsSystems systems)
    {
        if (_pointLightEnable)
        {
            OpenGL gl = _openGlControl.OpenGL;
        
            Shader lampShader = _shaderManager.UseLampShader();

            gl.BindVertexArray(_lightVAO);
        
            gl.EnableVertexAttribArray(0);
        
            foreach (var pointLightEntityId in _pointLightFilter)
            {
                ref Location pointLightLocation = ref _locationComponents.Get(pointLightEntityId);
                ref LightProperties pointLightProperties = ref _lightPropertiesComponents.Get(pointLightEntityId);
            
                gl.PushMatrix();
            
                gl.Translate(pointLightLocation.Position.X, pointLightLocation.Position.Y, pointLightLocation.Position.Z);
                gl.Scale(0.2, 0.2, 0.2);
            
                lampShader.SetMat4("projection", _openGlControl.OpenGL.GetProjectionMatrix().AsRowMajorArrayFloat);
                lampShader.SetMat4("modelview", _openGlControl.OpenGL.GetModelViewMatrix().AsRowMajorArrayFloat);
                lampShader.SetVec3("lightColor", pointLightProperties.Diffuse.X, pointLightProperties.Diffuse.Y, pointLightProperties.Diffuse.Z);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);

                gl.PopMatrix();
            }
        
            gl.DisableVertexAttribArray(0);
            gl.BindVertexArray(0);
        }
    }
}