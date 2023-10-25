using System.Numerics;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.SystemComponents.Player.Input;

public class PlayerObservationSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<Rotation> _rotationComponents;
    private EcsPool<MouseRotation> _mouseRotationComponent;
    
    private EcsFilter _playerFilter;
    private EcsFilter _inputFilter;
    
    private int _playerEntityId;
    private int _inputEntityId;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _playerFilter = world.Filter<PlayerMarker>().End();
        _inputFilter = world.Filter<MouseRotation>().Inc<MousePosition>().End();
        
        _rotationComponents = world.GetPool<Rotation>();
        _mouseRotationComponent = world.GetPool<MouseRotation>();

        _playerEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_playerFilter);
        _inputEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_inputFilter);
    }

    public void Run(IEcsSystems systems)
    {
        ref Rotation playerRotation = ref _rotationComponents.Get(_playerEntityId);
        ref MouseRotation mouseRotation = ref _mouseRotationComponent.Get(_inputEntityId);
        
        UpdatePlayerView(ref mouseRotation, ref playerRotation);
    }

    private void UpdatePlayerView(ref MouseRotation camera, ref Rotation playerRotation)
    {
        Vector3 direction;
        direction.X = (float)(Math.Cos(ConvertDegreesToRadians(camera.Yaw)) * Math.Cos(ConvertDegreesToRadians(camera.Pitch)));
        direction.Y = (float)Math.Sin(ConvertDegreesToRadians(camera.Pitch));
        direction.Z = (float)(Math.Sin(ConvertDegreesToRadians(camera.Yaw)) * Math.Cos(ConvertDegreesToRadians(camera.Pitch)));
        playerRotation.ForwardVector = Vector3.Normalize(direction);
    }
    
    public static float ConvertDegreesToRadians (float degrees)
    {
        float radians = ((float)Math.PI / 180) * degrees;
        return (radians);
    }
}