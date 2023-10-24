using System.Windows;
using System.Windows.Input;

namespace _3D_visualization.Model.Events;

public class InputEventsListener
{
    public delegate void OnKeyPressed(Key pressedKey);
    public event OnKeyPressed OnKeyPressedEvent;
    
    public delegate void OnKeyReleased(Key releasedKey);
    public event OnKeyReleased OnKeyReleasedEvent;
    
    public delegate void OnMouseMove(Point newMousePosition);
    public event OnMouseMove OnMouseMoveEvent;

    public void InvokeOnKeyPressed(Key pressedKey)
    {
        var handler = OnKeyPressedEvent;
        if (handler != null)
        {
            handler.Invoke(pressedKey);
        }
    }
    
    public void InvokeOnKeyReleased(Key releasedKey)
    {
        var handler = OnKeyReleasedEvent;
        if (handler != null)
        {
            handler.Invoke(releasedKey);
        }
    }
    
    public void InvokeOnMouseMove(Point newMousePosition)
    {
        var handler = OnMouseMoveEvent;
        if (handler != null)
        {
            handler.Invoke(newMousePosition);
        }
    }
}