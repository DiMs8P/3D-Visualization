using System.Windows;
using _3D_visualization.Model.Events;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.Input.Systems;

public class MouseInputsSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<MousePosition> _mousePositionComponent;
    private EcsPool<MouseRotation> _mouseRotationComponent;
    
    private EcsFilter _mouseInputs;
    
    private Point _previousMousePosition;
    private Point _currentMousePosition;
    
    private int _mouseInputEntityId;
    
    [EcsInject] InputEventsListener _inputsEventsListener;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _mouseInputs = world.Filter<MousePosition>().End();
        _mousePositionComponent = world.GetPool<MousePosition>();
        _mouseRotationComponent = world.GetPool<MouseRotation>();
        _inputsEventsListener.OnMouseMoveEvent += point =>
        {
            _previousMousePosition = _currentMousePosition;
            _currentMousePosition = point;
        };

        _mouseInputEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_mouseInputs);
    }

    public void Run(IEcsSystems systems)
    {
        ref MousePosition mousePosition = ref _mousePositionComponent.Get(_mouseInputEntityId);
        ref MouseRotation mouseRotation = ref _mouseRotationComponent.Get(_mouseInputEntityId);
        mousePosition.CurrentMousePosition = _currentMousePosition;
        mousePosition.PreviousMousePosition = _previousMousePosition;
        
        double xOffset = _currentMousePosition.X - _previousMousePosition.X;
        double yOffset = _previousMousePosition.Y - _currentMousePosition.Y;
        
        xOffset *= mouseRotation.Sensitivity;
        yOffset *= mouseRotation.Sensitivity;
        
        mouseRotation.Yaw   += (float)xOffset;
        mouseRotation.Pitch += (float)yOffset;
        
        if(mouseRotation.Pitch > 89.0f)
            mouseRotation.Pitch =  89.0f;
        if(mouseRotation.Pitch < -89.0f)
            mouseRotation.Pitch = -89.0f;
    }
}