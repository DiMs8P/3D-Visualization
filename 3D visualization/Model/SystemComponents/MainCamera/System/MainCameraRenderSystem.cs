using System.Numerics;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Player;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.MainCamera.System;

public class MainCameraRenderSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private OpenGLControl _openGlControl;
    private EcsPool<Location> _locationComponents;
    private EcsPool<Rotation> _rotationComponents;
    
    private EcsFilter _playerCameraFilter;
    
    private int _mainCameraEntityId;
    
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _playerCameraFilter = world.Filter<CameraMarker>().End();

        _locationComponents = world.GetPool<Location>();
        _rotationComponents = world.GetPool<Rotation>();
        
        _mainCameraEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_playerCameraFilter);
        
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
        ref Location cameraLocation = ref _locationComponents.Get(_mainCameraEntityId);
        ref Rotation cameraRotation = ref _rotationComponents.Get(_mainCameraEntityId);
        
        openGl.MatrixMode(OpenGL.GL_MODELVIEW);
        openGl.LoadIdentity();

        Vector3 cameraDir = Vector3.Add(cameraLocation.Position, cameraRotation.ForwardVector);

        openGl.LookAt(
            cameraLocation.Position.X, cameraLocation.Position.Y, cameraLocation.Position.Z,
            cameraDir.X, cameraDir.Y, cameraDir.Z,
            cameraRotation.UpVector.X, cameraRotation.UpVector.Y, cameraRotation.UpVector.Z);
    }
}