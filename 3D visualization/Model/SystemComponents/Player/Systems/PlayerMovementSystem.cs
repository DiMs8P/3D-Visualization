using System.Numerics;
using System.Windows.Input;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.SystemComponents.Player.Input;

public class PlayerMovementSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<Location> _locationComponents;
    private EcsPool<Rotation> _rotationComponents;
    private EcsPool<Movement> _movementComponents;
    private EcsPool<KeyboardKeys> _keyboardInputComponent;
    
    private EcsFilter _playerFilter;
    private EcsFilter _inputFilter;
    
    private int _playerEntityId;
    private int _inputEntityId;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _playerFilter = world.Filter<PlayerMarker>().End();
        _inputFilter = world.Filter<MouseRotation>().Inc<MousePosition>().End();

        _locationComponents = world.GetPool<Location>();
        _rotationComponents = world.GetPool<Rotation>();
        _movementComponents = world.GetPool<Movement>();
        _keyboardInputComponent = world.GetPool<KeyboardKeys>();

        _playerEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_playerFilter);
        _inputEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_inputFilter);
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