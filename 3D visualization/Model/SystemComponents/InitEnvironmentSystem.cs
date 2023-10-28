using _3D_visualization.Model.Factory;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.Input.Components;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Markers;
using _3D_visualization.Model.SystemComponents.Player;
using _3D_visualization.Model.SystemComponents.Transform.Components;
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
        CreateLampLight(_objectsFactory);
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

    private void CreateLampLight(ObjectsFactory objectsFactory)
    {
        /*objectsFactory.Create()
            .Add<SpotLight>
            .Add<Location>(new Location())
            .End();*/
    }
}