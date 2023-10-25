using System.Numerics;
using System.Windows.Input;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.Markers;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.SystemComponents.Player.Systems;

public class PlayerMovementSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<Location> _locationComponents;
    private EcsPool<Rotation> _rotationComponents;
    private EcsPool<Movement> _movementComponents;
    private EcsPool<TimeOffset> _deltaTimeComponent;
    private EcsPool<KeyboardKeys> _keyboardInputComponent;
    
    private EcsFilter _playerFilter;
    private EcsFilter _inputFilter;
    private EcsFilter _timeFilter;
    
    private int _playerEntityId;
    private int _inputEntityId;
    private int _deltaTimeEntityId;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _playerFilter = world.Filter<PlayerMarker>().End();
        _inputFilter = world.Filter<MouseRotation>().Inc<MousePosition>().End();
        _timeFilter = world.Filter<TimeOffset>().End();

        _locationComponents = world.GetPool<Location>();
        _rotationComponents = world.GetPool<Rotation>();
        _movementComponents = world.GetPool<Movement>();
        _deltaTimeComponent = world.GetPool<TimeOffset>();
        _keyboardInputComponent = world.GetPool<KeyboardKeys>();

        _playerEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_playerFilter);
        _inputEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_inputFilter);
        _deltaTimeEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_timeFilter);
    }

    public void Run(IEcsSystems systems)
    {
        ref KeyboardKeys pressedKeys = ref _keyboardInputComponent.Get(_inputEntityId);

        if (pressedKeys.PressedKeys.Count == 0)
        {
            return;
        }
            
        ref Location playerLocation = ref _locationComponents.Get(_playerEntityId);
        ref Rotation playerRotation = ref _rotationComponents.Get(_playerEntityId);
        ref Movement playerMovement = ref _movementComponents.Get(_playerEntityId);
        ref TimeOffset deltaTime = ref _deltaTimeComponent.Get(_deltaTimeEntityId);

        if (pressedKeys.PressedKeys.Contains(Key.W))
        {
            playerLocation.Position += playerMovement.Speed * playerRotation.ForwardVector * deltaTime.DeltaTime;
        }
            
        if (pressedKeys.PressedKeys.Contains(Key.S))
        {
            playerLocation.Position -= playerMovement.Speed * playerRotation.ForwardVector * deltaTime.DeltaTime;
        }
            
        if (pressedKeys.PressedKeys.Contains(Key.A))
        {
            playerLocation.Position -= playerMovement.Speed * Vector3.Normalize(Vector3.Cross(playerRotation.ForwardVector, playerRotation.UpVector)) * deltaTime.DeltaTime;
        }
            
        if (pressedKeys.PressedKeys.Contains(Key.D))
        {
            playerLocation.Position += playerMovement.Speed * Vector3.Normalize(Vector3.Cross(playerRotation.ForwardVector, playerRotation.UpVector)) * deltaTime.DeltaTime;
        }
    }
}