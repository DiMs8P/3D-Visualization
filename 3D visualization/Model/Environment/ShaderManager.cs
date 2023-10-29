using _3D_visualization.Model.SystemComponents.Light;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Render;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL.SceneGraph;
using SharpGL.WPF;

namespace _3D_visualization.Model.Environment;

public class ShaderManager
{
    private EcsWorld _world;
    
    [EcsPool] EcsPool<Location> _locationComponents;
    [EcsPool] EcsPool<PointLight> _spotLightComponents;
    
    EcsFilter _mainCameraFilter;
    EcsFilter _spotLightFilter;
    
    private OpenGLControl _openGlControl;

    private Shader _lampShader;
    private Shader _splineShader;

    private int _mainCameraEntityId;
    private int _spotLightEntityId;
    
    public ShaderManager(EcsWorld world, OpenGLControl openGlControl)
    {
        _world = world;
        _openGlControl = openGlControl;
        
        _mainCameraFilter = world.Filter<CameraMarker>().End();
        _spotLightFilter = world.Filter<PointLight>().End();
        
        _locationComponents = world.GetPool<Location>();
        _spotLightComponents = world.GetPool<PointLight>();

        _mainCameraEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_mainCameraFilter);
        _spotLightEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_spotLightFilter);

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
        ref Location cameraPosition = ref _locationComponents.Get(_mainCameraEntityId);
        ref Location spotLightPosition = ref _locationComponents.Get(_spotLightEntityId);

        ref PointLight pointLight = ref _spotLightComponents.Get(_spotLightEntityId);
        
        _splineShader.Use();
        _splineShader.SetVec3("objectColor", 1.0f, 0.5f, 0.31f);
        _splineShader.SetVec3("lightColor", pointLight.LightColor.X, pointLight.LightColor.Y, pointLight.LightColor.Z);
        _splineShader.SetVec3("lightPos", spotLightPosition.Position.X, spotLightPosition.Position.Y, spotLightPosition.Position.Z);
        _splineShader.SetVec3("viewPos", cameraPosition.Position.X, cameraPosition.Position.Y, cameraPosition.Position.Z);
        
        _splineShader.SetMat4("projection", _openGlControl.OpenGL.GetProjectionMatrix().AsRowMajorArrayFloat);
        _splineShader.SetMat4("modelview", _openGlControl.OpenGL.GetModelViewMatrix().AsRowMajorArrayFloat);
    }
}