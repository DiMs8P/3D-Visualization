using System.Windows.Input;

namespace _3D_visualization.Model.Events;

public class GameplayEventsListener
{
    public delegate void EventOccurredHandler();
    public event EventOccurredHandler EventOccurred;

    public delegate void OnKeyPressed(Key pressedKey);
    public event OnKeyPressed OnKeyPressedEvent;

}