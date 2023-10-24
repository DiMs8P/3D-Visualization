using System.Numerics;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Player;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.MainCamera.System;

public class MainCameraRenderSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private OpenGLControl _openGlControl;
    private EcsPool<Location> _locationComponent;
    private EcsPool<Rotation> _rotationComponent;
    private EcsPool<Camera> _cameraComponent;
    
    private EcsFilter _playerCameraFilter;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _playerCameraFilter = world.Filter<PlayerMarker>().Inc<Location>().Inc<Rotation>().Inc<Camera>().End();

        _locationComponent = world.GetPool<Location>();
        _rotationComponent = world.GetPool<Rotation>();
        _cameraComponent = world.GetPool<Camera>();
        
        _openGlControl.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
    }

    public void Run(IEcsSystems systems)
    {
        OpenGL gl = _openGlControl.OpenGL;
        
        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        gl.LoadIdentity();

        UpdateCameraView(gl);
        
        gl.Begin(OpenGL.GL_TRIANGLES);
            gl.Vertex(-1, -1, 0);
            gl.Color(1, 0, 0);
            gl.Vertex(1, -1, 0);
            gl.Color(0, 1, 0);
            gl.Vertex(0, 1, 0);
            gl.Color(0, 0, 1);
        gl.End();
        gl.Flush();

    }

    private void UpdateCameraView(OpenGL openGl)
    {
        int playerCameraId = 0;
        foreach (int playersCameraIds in _playerCameraFilter)
        {
            playerCameraId = playersCameraIds;
        }
        
        ref Location cameraLocation = ref _locationComponent.Get(playerCameraId);
        ref Rotation cameraRotation = ref _rotationComponent.Get(playerCameraId);
        
        openGl.MatrixMode(OpenGL.GL_MODELVIEW);
        openGl.LoadIdentity();

        Vector3 cameraDir = Vector3.Add(cameraLocation.Position, cameraRotation.ForwardVector);

        openGl.LookAt(
            cameraLocation.Position.X, cameraLocation.Position.Y, cameraLocation.Position.Z,
            cameraDir.X, cameraDir.Y, cameraDir.Z,
            cameraRotation.UpVector.X, cameraRotation.UpVector.Y, cameraRotation.UpVector.Z);
    }
}