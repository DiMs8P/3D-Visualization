using System.Windows;
using System.Windows.Input;

namespace _3D_visualization.Model;

public class InputManager
{
    private HashSet<Key> _pressedKeys;
    private Point _currentMousePos;
    private Point _prevMousePos;
    public InputManager()
    {
    }
    
    public void OnKeyPressed(Key pressedKey)
    {
        _pressedKeys.Add(pressedKey);
    }

    public void OnKeyReleased(Key releasedKey)
    {
        _pressedKeys.Remove(releasedKey);
    }

    public void OnMouseHover(Point currentMousePos)
    {
        _currentMousePos = currentMousePos;
    }

    public void Update()
    {
        _prevMousePos = _currentMousePos;
    }
}