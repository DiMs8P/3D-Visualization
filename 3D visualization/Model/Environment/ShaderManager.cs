using System.Drawing;
using System.Drawing.Imaging;
using _3D_visualization.Model.SystemComponents.Light;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Render;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.WPF;

namespace _3D_visualization.Model.Environment;

// TODO maybe create it as system
public class ShaderManager
{
    private EcsWorld _world;
    
    [EcsPool] EcsPool<Location> _locationComponents;
    [EcsPool] EcsPool<Rotation> _rotationComponents;
    
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
    private Shader _splineDebugShader;

    private uint diffuseMap;
    private uint specularMap;

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
        _rotationComponents = world.GetPool<Rotation>();
        _directionalLightComponents = world.GetPool<DirectionalLight>();
        
        _pointLightComponents = world.GetPool<PointLight>();
        _spotLightComponents = world.GetPool<SpotLight>();

        _lightPropertiesComponents = world.GetPool<LightProperties>();
        _attenuationComponents = world.GetPool<Attenuation>();
        _directionComponents = world.GetPool<Direction>();

        _mainCameraEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_mainCameraFilter);

        CreateLampShader();
        CreateSplineShader();
        CreateSplineDebugShader();
        
        diffuseMap = LoadTexture("D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\Textures\\diffuse.png");
        specularMap = LoadTexture("D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\Textures\\specular.png");
    }

    private void CreateLampShader()
    {
        _lampShader = new Shader(
            _openGlControl.OpenGL,
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\Shaders\\lamp_vertex.txt",
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\Shaders\\lamp_fragment.txt"
        );
    }

    private void CreateSplineShader()
    {
        _splineShader = new Shader(
            _openGlControl.OpenGL,
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\Shaders\\basic_lightning_vertex.txt",
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\Shaders\\basic_lightning_fragment.txt"
        );
    }
    
    private void CreateSplineDebugShader()
    {
        _splineDebugShader = new Shader(
            _openGlControl.OpenGL,
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\Shaders\\debug_vertex.txt",
            "D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\Shaders\\debug_fragment.txt"
        );
    }

    public void UseDefaultShader()
    {
        _openGlControl.OpenGL.UseProgram(0);
    }

    public Shader UseLampShader()
    {
        _lampShader.Use();

        return _lampShader;
    }

    public Shader UseSuperRealisticShader(bool showTextures)
    {
        OpenGL gl = _openGlControl.OpenGL;
        _splineShader.Use();

        _splineShader.SetBool("material.active", showTextures);
        if (showTextures)
        {
            
            _splineShader.SetInt("material.diffuse", 0);
            _splineShader.SetInt("material.specular", 1);
        
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, diffuseMap);

            gl.ActiveTexture(OpenGL.GL_TEXTURE1);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, specularMap);
        }
        
        _splineShader.SetMat4("projection", _openGlControl.OpenGL.GetProjectionMatrix().AsRowMajorArrayFloat);
        _splineShader.SetMat4("modelview", _openGlControl.OpenGL.GetModelViewMatrix().AsRowMajorArrayFloat);
        
        ref Location cameraLocation = ref _locationComponents.Get(_mainCameraEntityId);
        _splineShader.SetVec3("viewPos", cameraLocation.Position.X, cameraLocation.Position.Y, cameraLocation.Position.Z);
        _splineShader.SetFloat("material.shininess", 32.0f);

        SetUniformDirectionalsLightVariables();
        SetUniformPointLightsVariables();
        SetUniformFlashlightVariables();

        return _splineShader;
    }
    
    public Shader UseSplineDebugShader()
    {
        _splineDebugShader.Use();
        
        _splineDebugShader.SetVec3("objectColor", 1.0f, 0.5f, 0.31f);
        _splineDebugShader.SetVec3("lightColor", 1.0f, 1.0f, 1.0f);
        _splineDebugShader.SetVec3("lightPos", 1.2f, 1.0f, 1.0f);
        _splineDebugShader.SetVec3("viewPos", 0.0f, 0.0f, 0.3f);

        _splineDebugShader.SetMat4("projection", _openGlControl.OpenGL.GetProjectionMatrix().AsRowMajorArrayFloat);
        _splineDebugShader.SetMat4("modelview", _openGlControl.OpenGL.GetModelViewMatrix().AsRowMajorArrayFloat);

        return _splineDebugShader;
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

    private void SetUniformFlashlightVariables()
    {
        int currentIndex = 0;
        foreach (int spotLightEntityId in _spotLightFilter)
        {
            ref Location spotLightLocation = ref _locationComponents.Get(spotLightEntityId);
            ref Rotation spotLightRotation = ref _rotationComponents.Get(spotLightEntityId);
            ref LightProperties lightProperties = ref _lightPropertiesComponents.Get(spotLightEntityId);
            ref Attenuation lightAttenuation = ref _attenuationComponents.Get(spotLightEntityId);
            ref SpotLight spotLight = ref _spotLightComponents.Get(spotLightEntityId);

            _splineShader.SetVec3($"spotLight[{currentIndex}].position", spotLightLocation.Position.X, spotLightLocation.Position.Y, spotLightLocation.Position.Z);
            _splineShader.SetVec3($"spotLight[{currentIndex}].direction", spotLightRotation.ForwardVector.X, spotLightRotation.ForwardVector.Y, spotLightRotation.ForwardVector.Z);
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
    
    private uint LoadTexture(string path)
    {
        OpenGL gl = _openGlControl.OpenGL;
        
        uint[] textureID = new uint[1];
        gl.GenTextures(1, textureID);
        
        Bitmap bitmap = new Bitmap(path);

        IntPtr pixels = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb).Scan0;
        
        if (pixels != IntPtr.Zero)
        {
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureID[0]);
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, 3, bitmap.Width, bitmap.Height, 0, OpenGL.GL_BGR,
                OpenGL.GL_UNSIGNED_BYTE, pixels);
            gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
        }
        else
        {
            throw new System.Exception("");
        }
        
        return textureID[0];
    }
}