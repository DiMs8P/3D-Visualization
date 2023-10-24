using System.Numerics;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.SystemComponents.Player.Input;

public class PlayerObservationSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<Location> _locationComponent;
    private EcsPool<Rotation> _rotationComponent;
    private EcsPool<Camera> _cameraComponent;
    private EcsPool<MousePosition> _mousePosition;
    
    private EcsFilter _playerFilter;
    private EcsFilter _playerCameraFilter;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _playerFilter = world.Filter<PlayerMarker>().Inc<Location>().Inc<MousePosition>().End();
        _playerCameraFilter = world.Filter<PlayerMarker>().Inc<Location>().Inc<Rotation>().Inc<Camera>().End();

        _locationComponent = world.GetPool<Location>();
        _rotationComponent = world.GetPool<Rotation>();
        _cameraComponent = world.GetPool<Camera>();
        _mousePosition = world.GetPool<MousePosition>();
    }

    // TODO refactor
    public void Run(IEcsSystems systems)
    {
        int playerId = 0;
        int playerCameraId = 0;
        foreach (int playerIds in _playerFilter)
        {
            playerId = playerIds;
        }
        
        foreach (int playersCameraIds in _playerCameraFilter)
        {
            playerCameraId = playersCameraIds;
        }

        ref Location playerLocation = ref _locationComponent.Get(playerId);
        ref Location cameraLocation = ref _locationComponent.Get(playerCameraId);
        cameraLocation.Position = playerLocation.Position;

        ref MousePosition mousePosition = ref _mousePosition.Get(playerId);
        ref Rotation cameraRotation = ref _rotationComponent.Get(playerCameraId);
        ref Camera camera = ref _cameraComponent.Get(playerCameraId);

        UpdateCameraView(ref camera, ref cameraRotation, ref mousePosition);
    }

    private void UpdateCameraView(ref Camera camera, ref Rotation cameraRotation, ref MousePosition mousePosition)
    {
        double xoffset = mousePosition.CurrentMousePosition.X - mousePosition.PreviousMousePosition.X;
        double yoffset = mousePosition.PreviousMousePosition.Y - mousePosition.CurrentMousePosition.Y;
        
        xoffset *= camera.Sensitivity;
        yoffset *= camera.Sensitivity;
        
        camera.Yaw   += (float)xoffset;
        camera.Pitch += (float)yoffset;
        
        if(camera.Pitch > 89.0f)
            camera.Pitch =  89.0f;
        if(camera.Pitch < -89.0f)
            camera.Pitch = -89.0f;
        
        Vector3 direction;
        direction.X = (float)(Math.Cos(ConvertDegreesToRadians(camera.Yaw)) * Math.Cos(ConvertDegreesToRadians(camera.Pitch)));
        direction.Y = (float)Math.Sin(ConvertDegreesToRadians(camera.Pitch));
        direction.Z = (float)(Math.Sin(ConvertDegreesToRadians(camera.Yaw)) * Math.Cos(ConvertDegreesToRadians(camera.Pitch)));
        cameraRotation.ForwardVector = Vector3.Normalize(direction);
    }
    
    public static float ConvertDegreesToRadians (float degrees)
    {
        float radians = ((float)Math.PI / 180) * degrees;
        return (radians);
    }
}