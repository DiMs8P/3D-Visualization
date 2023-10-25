using System.Windows.Input;
using _3D_visualization.Model.Events;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.Input.Systems;

public class KeyboardInputsSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _keyboardInputs;
    private EcsPool<KeyboardKeys> _keyboardInputsPool;
    private HashSet<Key> _pressedKeys;

    private int _keyboardInputEntityId;
    
    [EcsInject] InputEventsListener _inputsEventsListener;

    public KeyboardInputsSystem()
    {
        _pressedKeys = new HashSet<Key>();
    }
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _keyboardInputs = world.Filter<KeyboardKeys>().End();
        _keyboardInputsPool = world.GetPool<KeyboardKeys>();
        _inputsEventsListener.OnKeyPressedEvent += key => _pressedKeys.Add(key);
        _inputsEventsListener.OnKeyReleasedEvent += key => _pressedKeys.Remove(key);

        _keyboardInputEntityId = EntityUtils.GetUniqueEntityIdFromFilter(_keyboardInputs);
    }

    public void Run(IEcsSystems systems)
    {
        ref var keyboardInput = ref _keyboardInputsPool.Get(_keyboardInputEntityId);
        keyboardInput.PressedKeys = _pressedKeys;   
    }
}