using _3D_visualization.Model.Factory;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.SystemComponents.World.System;

public class InitEnvironmentSystem : IEcsInitSystem
{
    [EcsInject] ObjectsFactory _objectsFactory;
    public void Init(IEcsSystems systems)
    {
        CreateSingletonComponents(_objectsFactory);
        CreatePlayer(_objectsFactory);
    }

    private void CreateSingletonComponents(ObjectsFactory objectsFactory)
    {
        objectsFactory.CreateTimePresence();
        objectsFactory.CreateInputPresence();
    }
    
    private void CreatePlayer(ObjectsFactory objectsFactory)
    {
        objectsFactory.CreatePlayer();
    }
}