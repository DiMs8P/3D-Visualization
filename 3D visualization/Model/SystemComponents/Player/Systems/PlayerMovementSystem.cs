using System.Numerics;
using System.Windows.Input;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.SystemComponents.Player.Input;

public class PlayerMovementSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<Location> _locationComponent;
    private EcsPool<Rotation> _rotationComponent;
    private EcsPool<Movement> _movementComponent;
    private EcsPool<KeyboardKeys> _pressedKeys;
    
    private EcsFilter _playerFilter;
    
    private int _playerEntityId;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _playerFilter = world.Filter<PlayerMarker>().Inc<Location>().Inc<Movement>().Inc<KeyboardKeys>().End();

        _locationComponent = world.GetPool<Location>();
        _rotationComponent = world.GetPool<Rotation>();
        _movementComponent = world.GetPool<Movement>();
        _pressedKeys = world.GetPool<KeyboardKeys>();

        _playerEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_playerFilter);
    }

    public void Run(IEcsSystems systems)
    {
        ref KeyboardKeys pressedKeys = ref _pressedKeys.Get(_playerEntityId);

        if (pressedKeys.PressedKeys.Count == 0)
        {
            return;
        }
            
        ref Location playerLocation = ref _locationComponent.Get(_playerEntityId);
        ref Rotation playerRotation = ref _rotationComponent.Get(_playerEntityId);
        ref Movement playerMovement = ref _movementComponent.Get(_playerEntityId);

        if (pressedKeys.PressedKeys.Contains(Key.W))
        {
            MoveForward(ref playerLocation, ref playerRotation, ref playerMovement);
        }
            
        if (pressedKeys.PressedKeys.Contains(Key.S))
        {
            MoveBackward(ref playerLocation, ref playerRotation, ref playerMovement);
        }
            
        if (pressedKeys.PressedKeys.Contains(Key.A))
        {
            MoveLeft(ref playerLocation, ref playerRotation, ref playerMovement);
        }
            
        if (pressedKeys.PressedKeys.Contains(Key.D))
        {
            MoveRight(ref playerLocation, ref playerRotation, ref playerMovement);
        }
    }

    private void MoveRight(ref Location playerLocation, ref Rotation playerRotation, ref Movement playerMovement)
    {
        playerLocation.Position += playerMovement.Speed * Vector3.Normalize(Vector3.Cross(playerRotation.ForwardVector, playerRotation.UpVector));
    }

    private void MoveLeft(ref Location playerLocation, ref Rotation playerRotation, ref Movement playerMovement)
    {
        playerLocation.Position -= playerMovement.Speed * Vector3.Normalize(Vector3.Cross(playerRotation.ForwardVector, playerRotation.UpVector));
    }

    private void MoveBackward(ref Location playerLocation, ref Rotation playerRotation, ref Movement playerMovement)
    {
        playerLocation.Position -= playerMovement.Speed * playerRotation.ForwardVector;
    }

    private void MoveForward(ref Location playerLocation, ref Rotation playerRotation, ref Movement playerMovement)
    {
        playerLocation.Position += playerMovement.Speed * playerRotation.ForwardVector;
    }
}