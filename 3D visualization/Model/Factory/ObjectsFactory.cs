using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Markers;
using _3D_visualization.Model.SystemComponents.Player;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.Factory;

public class ObjectsFactory
{
    protected EcsWorld _world;

    protected EcsPool<Camera> _cameraComponents;
    protected EcsPool<TimeOffset> _timeComponent;
    protected EcsPool<PlayerMarker> _playerMarker;
    protected EcsPool<MousePosition> _mouseInputs;
    protected EcsPool<KeyboardKeys> _keyboardInputs;
    protected EcsPool<Location> _locationComponents;
    protected EcsPool<Movement> _movementComponents;
    protected EcsPool<Rotation> _rotationComponent;
    
    public ObjectsFactory(EcsWorld world)
    {
        _world = world;

        _cameraComponents = world.GetPool<Camera>();
        _timeComponent = world.GetPool<TimeOffset>();
        _playerMarker = world.GetPool<PlayerMarker>();
        _mouseInputs = world.GetPool<MousePosition>();
        _keyboardInputs = world.GetPool<KeyboardKeys>();
        _locationComponents = world.GetPool<Location>();
        _movementComponents = world.GetPool<Movement>();
        _rotationComponent = world.GetPool<Rotation>();
    }
    
    public void CreateTimeComponent()
    {
        int entity = _world.NewEntity();
        
        _timeComponent.Add(entity);
    }
    
    public void CreatePlayer()
    {
        int entity = _world.NewEntity();

        _mouseInputs.Add(entity);
        _playerMarker.Add(entity);
        _keyboardInputs.Add(entity);
        _locationComponents.Add(entity);
        _movementComponents.Add(entity);

        CreatePlayerCamera();
    }

    private void CreatePlayerCamera()
    {
        int entity = _world.NewEntity();
        
        _cameraComponents.Add(entity);
        _locationComponents.Add(entity);
        _rotationComponent.Add(entity);
        _playerMarker.Add(entity);
    }
}