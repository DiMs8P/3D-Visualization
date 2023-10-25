using System.Windows;
using _3D_visualization.Model.Events;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.Input.Systems;

public class MouseInputsSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _mouseInputs;
    private EcsPool<MousePosition> _mouseInputsPool;
    private Point _previousMousePosition;
    private Point _currentMousePosition;
    
    private int _mouseInputEntityId;
    
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

        _mouseInputEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_mouseInputs);
    }

    public void Run(IEcsSystems systems)
    {
        ref MousePosition mousePosition = ref _mouseInputsPool.Get(_mouseInputEntityId);
        mousePosition.CurrentMousePosition = _currentMousePosition;
        mousePosition.PreviousMousePosition = _previousMousePosition;

    }
}