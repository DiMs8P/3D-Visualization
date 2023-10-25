using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Player;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.SystemComponents.MainCamera.System;

public class CameraSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsPool] EcsPool<Rotation> _rotationComponents;
    [EcsPool] EcsPool<Location> _locationComponents;

    EcsFilter _playerFilter;
    EcsFilter _mainCameraFilter;
    
    private int _playerEntityId;
    private int _mainCameraEntityId;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().End();
        _mainCameraFilter = world.Filter<CameraMarker>().End();
        
        _rotationComponents = world.GetPool<Rotation>();
        _locationComponents = world.GetPool<Location>();

        _playerEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_playerFilter);
        _mainCameraEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_mainCameraFilter);
    }

    public void Run(IEcsSystems systems)
    {
        ref Rotation playerRotation = ref _rotationComponents.Get(_playerEntityId);
        ref Rotation cameraRotation = ref _rotationComponents.Get(_mainCameraEntityId);
        cameraRotation.ForwardVector = playerRotation.ForwardVector;
        cameraRotation.UpVector = playerRotation.UpVector;
        cameraRotation.RightVector = playerRotation.RightVector;
        
        ref Location playerLocation = ref _locationComponents.Get(_playerEntityId);
        ref Location cameraLocation = ref _locationComponents.Get(_mainCameraEntityId);
        cameraLocation.Position = playerLocation.Position;
    }
}