using System.Windows;
using _3D_visualization.Model.Events;
using _3D_visualization.Model.Input.Components;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.Input.Systems;

public class MouseInputsSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _mouseInputs;
    private EcsPool<MousePosition> _mouseInputsPool;
    private Point _previousMousePosition;
    private Point _currentMousePosition;
    
    [EcsInject] InputEventsListener _inputsEventsListener;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _mouseInputs = world.Filter<MousePosition>().End();
        _mouseInputsPool = world.GetPool<MousePosition>();
        _inputsEventsListener.OnMouseMoveEvent += point =>
        {
            _previousMousePosition = _currentMousePosition;
            _currentMousePosition = point;
        };
    }

    public void Run(IEcsSystems systems)
    {
        foreach (int entityWithMouseInput in _mouseInputs)
        {
            ref MousePosition mousePosition = ref _mouseInputsPool.Get(entityWithMouseInput);
            mousePosition.CurrentMousePosition = _currentMousePosition;
            mousePosition.PreviousMousePosition = _previousMousePosition;
        }
    }
}