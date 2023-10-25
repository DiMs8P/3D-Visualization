using System.Numerics;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.Input.Components;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Markers;
using _3D_visualization.Model.SystemComponents.Player;
using _3D_visualization.Model.SystemComponents.Spline.Components;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.Factory;

public class ObjectsFactory
{
    protected EcsWorld _world;

    protected EcsPool<CameraMarker> _cameraComponents;
    protected EcsPool<TimeOffset> _timeComponent;
    protected EcsPool<PlayerMarker> _playerMarker;
    protected EcsPool<MousePosition> _mousePositions;
    protected EcsPool<MouseRotation> _mouseRotations;
    protected EcsPool<KeyboardKeys> _keyboardInputs;
    protected EcsPool<Location> _locationComponents;
    protected EcsPool<Movement> _movementComponents;
    protected EcsPool<Rotation> _rotationComponent;
    protected EcsPool<Spline> _splineComponents;
    
    public ObjectsFactory(EcsWorld world)
    {
        _world = world;

        _cameraComponents = world.GetPool<CameraMarker>();
        _timeComponent = world.GetPool<TimeOffset>();
        _playerMarker = world.GetPool<PlayerMarker>();
        _mousePositions = world.GetPool<MousePosition>();
        _mouseRotations = world.GetPool<MouseRotation>();
        _keyboardInputs = world.GetPool<KeyboardKeys>();
        _locationComponents = world.GetPool<Location>();
        _movementComponents = world.GetPool<Movement>();
        _rotationComponent = world.GetPool<Rotation>();
        _splineComponents = world.GetPool<Spline>();
    }
    
    public void CreateTimePresence()
    {
        int entity = _world.NewEntity();
        
        _timeComponent.Add(entity) = new TimeOffset();
    }
    
    public void CreateInputPresence()
    {
        int entity = _world.NewEntity();
        
        _keyboardInputs.Add(entity) = new KeyboardKeys();
        _mousePositions.Add(entity) = new MousePosition();
        _mouseRotations.Add(entity) = new MouseRotation();
    }
    
    public void CreatePlayer()
    {
        int entity = _world.NewEntity();

        _playerMarker.Add(entity) = new PlayerMarker();
        _locationComponents.Add(entity) = new Location();
        _rotationComponent.Add(entity) = new Rotation();
        _movementComponents.Add(entity) = new Movement();

        CreatePlayerCamera();
    }

    private void CreatePlayerCamera()
    {
        int entity = _world.NewEntity();
        
        _cameraComponents.Add(entity) = new CameraMarker();
        _locationComponents.Add(entity) = new Location();
        _rotationComponent.Add(entity) = new Rotation();
    }

    public void CreateSpline(List<Vector2> section, List<Vector3> path)
    {
        int entity = _world.NewEntity();

        _splineComponents.Add(entity) = new Spline(section, path);
    }
}