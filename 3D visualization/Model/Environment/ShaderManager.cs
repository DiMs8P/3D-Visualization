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

// TODO maybe create it as system
public class ShaderManager
{
    private EcsWorld _world;
    
    [EcsPool] EcsPool<Location> _locationComponents;
    
    [EcsPool] EcsPool<DirectionalLight> _directionalLightComponents;
    [EcsPool] EcsPool<PointLight> _pointLightComponents;
    [EcsPool] EcsPool<SpotLight> _spotLightComponents;
    
    [EcsPool] EcsPool<LightProperties> _lightPropertiesComponents;
    [EcsPool] EcsPool<Attenuation> _attenuationComponents;
    [EcsPool] EcsPool<Direction> _directionComponents;
    
    EcsFilter _mainCameraFilter;
    EcsFilter _directionalLightFilter;
    EcsFilter _pointLightFilter;
    EcsFilter _spotLightFilter;
    
    private OpenGLControl _openGlControl;

    private Shader _lampShader;
    private Shader _splineShader;

    private int _mainCameraEntityId;
    
    public ShaderManager(EcsWorld world, OpenGLControl openGlControl)
    {
        _world = world;
        _openGlControl = openGlControl;
        
        _mainCameraFilter = world.Filter<CameraMarker>().End();
        _directionalLightFilter = world.Filter<DirectionalLight>().End();
        _pointLightFilter = world.Filter<PointLight>().End();
        _spotLightFilter = world.Filter<SpotLight>().End();
        
        _locationComponents = world.GetPool<Location>();
        _directionalLightComponents = world.GetPool<DirectionalLight>();
        
        _pointLightComponents = world.GetPool<PointLight>();
        _spotLightComponents = world.GetPool<SpotLight>();

        _lightPropertiesComponents = world.GetPool<LightProperties>();
        _attenuationComponents = world.GetPool<Attenuation>();
        _directionComponents = world.GetPool<Direction>();

        _mainCameraEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_mainCameraFilter);

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

    public void UseSuperRealisticShader()
    {
        _splineShader.Use();

        SetUniformDirectionalsLightVariables();
        SetUniformPointLightsVariables();
        SetUniformSpotLightsVariables();
    }

    private void SetUniformDirectionalsLightVariables()
    {
        int currentIndex = 0;
        foreach (int directionalLightEntityId in _directionalLightFilter)
        {
            ref DirectionalLight directionalLight = ref _directionalLightComponents.Get(directionalLightEntityId);
            ref Direction lightDirection = ref _directionComponents.Get(directionalLightEntityId);
            ref LightProperties lightProperties = ref _lightPropertiesComponents.Get(directionalLightEntityId);

            _splineShader.SetVec3($"dirLight[{currentIndex}].direction", lightDirection.To.X, lightDirection.To.Y, lightDirection.To.Z);
            _splineShader.SetVec3($"dirLight[{currentIndex}].ambient", lightProperties.Ambient.X, lightProperties.Ambient.Y, lightProperties.Ambient.Z);
            _splineShader.SetVec3($"dirLight[{currentIndex}].diffuse", lightProperties.Diffuse.X, lightProperties.Diffuse.Y, lightProperties.Diffuse.Z);
            _splineShader.SetVec3($"dirLight[{currentIndex}].specular", lightProperties.Specular.X, lightProperties.Specular.Y, lightProperties.Specular.Z);

            ++currentIndex;
        }
    }

    private void SetUniformPointLightsVariables()
    {
        int currentIndex = 0;
        foreach (int pointLightEntityId in _pointLightFilter)
        {
            ref Location lightLocation = ref _locationComponents.Get(pointLightEntityId);
            ref LightProperties lightProperties = ref _lightPropertiesComponents.Get(pointLightEntityId);
            ref Attenuation lightAttenuation = ref _attenuationComponents.Get(pointLightEntityId);
            ref PointLight pointLight = ref _pointLightComponents.Get(pointLightEntityId);

            _splineShader.SetVec3($"pointLights[{currentIndex}].position", lightLocation.Position.X, lightLocation.Position.Y, lightLocation.Position.Z);
            _splineShader.SetVec3($"pointLights[{currentIndex}].ambient", lightProperties.Ambient.X, lightProperties.Ambient.Y, lightProperties.Ambient.Z);
            _splineShader.SetVec3($"pointLights[{currentIndex}].diffuse", lightProperties.Diffuse.X, lightProperties.Diffuse.Y, lightProperties.Diffuse.Z);
            _splineShader.SetVec3($"pointLights[{currentIndex}].specular", lightProperties.Specular.X, lightProperties.Specular.Y, lightProperties.Specular.Z);
            _splineShader.SetFloat($"pointLights[{currentIndex}].constant", lightAttenuation.Constant);
            _splineShader.SetFloat($"pointLights[{currentIndex}].linear", lightAttenuation.Linear);
            _splineShader.SetFloat($"pointLights[{currentIndex}].quadratic", lightAttenuation.Quadratic);

            ++currentIndex;
        }
    }

    private void SetUniformSpotLightsVariables()
    {
        int currentIndex = 0;
        foreach (int spotLightEntityId in _spotLightFilter)
        {
            ref Location lightLocation = ref _locationComponents.Get(spotLightEntityId);
            ref Direction lightDirection = ref _directionComponents.Get(spotLightEntityId);
            ref LightProperties lightProperties = ref _lightPropertiesComponents.Get(spotLightEntityId);
            ref Attenuation lightAttenuation = ref _attenuationComponents.Get(spotLightEntityId);
            ref SpotLight spotLight = ref _spotLightComponents.Get(spotLightEntityId);

            _splineShader.SetVec3($"spotLight[{currentIndex}].position", lightLocation.Position.X, lightLocation.Position.Y, lightLocation.Position.Z);
            _splineShader.SetVec3($"spotLight[{currentIndex}].direction", lightDirection.To.X, lightDirection.To.Y, lightDirection.To.Z);
            _splineShader.SetVec3($"spotLight[{currentIndex}].ambient", lightProperties.Ambient.X, lightProperties.Ambient.Y, lightProperties.Ambient.Z);
            _splineShader.SetVec3($"spotLight[{currentIndex}].diffuse", lightProperties.Diffuse.X, lightProperties.Diffuse.Y, lightProperties.Diffuse.Z);
            _splineShader.SetVec3($"spotLight[{currentIndex}].specular", lightProperties.Specular.X, lightProperties.Specular.Y, lightProperties.Specular.Z);
            _splineShader.SetFloat($"spotLight[{currentIndex}].constant", lightAttenuation.Constant);
            _splineShader.SetFloat($"spotLight[{currentIndex}].linear", lightAttenuation.Linear);
            _splineShader.SetFloat($"spotLight[{currentIndex}].quadratic", lightAttenuation.Quadratic);
            _splineShader.SetFloat($"spotLight[{currentIndex}].cutOff", spotLight.CutOff);
            _splineShader.SetFloat($"spotLight[{currentIndex}].outerCutOff", spotLight.OuterCutOff);

            ++currentIndex;
        }
    }
}