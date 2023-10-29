using System.Numerics;
using _3D_visualization.Model.Factory;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.Input.Components;
using _3D_visualization.Model.SystemComponents.Light;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Markers;
using _3D_visualization.Model.SystemComponents.Player;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.SystemComponents;

public class InitEnvironmentSystem : IEcsInitSystem
{
    [EcsInject] ObjectsFactory _objectsFactory;
    public void Init(IEcsSystems systems)
    {
        CreateTimePresence(_objectsFactory);
        CreateInputPresence(_objectsFactory);
        CreatePlayer(_objectsFactory);
        CreateLampLights(_objectsFactory);
    }
    
    private void CreateTimePresence(ObjectsFactory objectsFactory)
    {
        objectsFactory.Create()
            .Add<TimeOffset>(new TimeOffset())
            .End();
    }

    private void CreateInputPresence(ObjectsFactory objectsFactory)
    {
        objectsFactory.Create()
            .Add<KeyboardKeys>(new KeyboardKeys())
            .Add<MousePosition>(new MousePosition())
            .Add<MouseRotation>(new MouseRotation())
            .End();
    }
    
    private void CreatePlayer(ObjectsFactory objectsFactory)
    {
        objectsFactory.Create()
            .Add<PlayerMarker>(new PlayerMarker())
            .Add<Location>(new Location())
            .Add<Rotation>(new Rotation())
            .Add<Movement>(new Movement())
            .End();

        CreatePlayerCamera(objectsFactory);
    }
    
    private void CreatePlayerCamera(ObjectsFactory objectsFactory)
    {
        objectsFactory.Create()
            .Add<CameraMarker>(new CameraMarker())
            .Add<Location>(new Location())
            .Add<Rotation>(new Rotation())
            .End();
    }

    private void CreateLampLights(ObjectsFactory objectsFactory)
    {
        CreateDirectionLight(objectsFactory);
        CreatePointLights(objectsFactory);
        CreateSpotLight(objectsFactory);
    }

    private void CreateDirectionLight(ObjectsFactory objectsFactory)
    {
        objectsFactory.Create()
            .Add<DirectionalLight>(new DirectionalLight())
            .Add<Direction>(new Direction(new Vector3(-0.2f, -1.0f, -0.3f)))
            .Add<LightProperties>(new LightProperties(
                new Vector3(0.05f, 0.05f, 0.05f),
                new Vector3(0.4f, 0.4f, 0.4f),
                new Vector3(0.5f, 0.5f, 0.5f)))
            .End();
    }

    private void CreatePointLights(ObjectsFactory objectsFactory)
    {
        Vector3[] positions = new Vector3[]
        {
            new Vector3(1.2f, 1.0f, 1.0f),
            new Vector3(-1.2f, 1.0f, 1.0f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            objectsFactory.Create()
                .Add<PointLight>(new PointLight())
                .Add<Location>(new Location(positions[i]))
                .Add<LightProperties>(new LightProperties(
                    new Vector3(0.05f, 0.05f, 0.05f),
                    new Vector3(0.8f, 0.8f, 0.8f),
                    new Vector3(1.0f, 1.0f, 1.0f)))
                .Add<Attenuation>(new Attenuation(1.0f, 0.09f, 0.032f))
                .End();
        }
    }

    private void CreateSpotLight(ObjectsFactory objectsFactory)
    {
        objectsFactory.Create()
            .Add<SpotLight>(new SpotLight((float)Math.Cos(MathHelper.GetRadiansFrom(12.5f)), (float)Math.Cos(MathHelper.GetRadiansFrom(15.0f))))
            .Add<Location>(new Location())
            .Add<Direction>(new Direction())
            .Add<LightProperties>(new LightProperties(
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 1.0f),
                new Vector3(1.0f, 1.0f, 1.0f)))
            .Add<Attenuation>(new Attenuation(1.0f, 0.09f, 0.032f))
            .End();
    }
}