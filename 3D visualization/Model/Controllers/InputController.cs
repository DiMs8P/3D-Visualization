using System.Windows;
using System.Windows.Input;
using Object = _3D_visualization.Model.Core.Object;

namespace _3D_visualization.Model.Controllers;

public class InputController : IController
{
    private HashSet<Key> _pressedKeys;
    private Point _prevMousePos;
    private Point _currentMousePos;

    private Dictionary<Key, Dictionary<Object, Action>> _keyboardAxisBindings;
    private Dictionary<Object, Action<Point, Point>> _mouseHoverBindings;

    public InputController()
    {
        _pressedKeys = new HashSet<Key>();
        _keyboardAxisBindings = new Dictionary<Key, Dictionary<Object, Action>>();
        _mouseHoverBindings = new Dictionary<Object, Action<Point, Point>>();
    }
    
    public void OnKeyPressed(object sender, KeyEventArgs e)
    {
        _pressedKeys.Add(e.Key);
    }

    public void OnKeyReleased(object sender, KeyEventArgs e)
    {
        _pressedKeys.Remove(e.Key);
    }

    public void OnMouseHover(Point currentMousePos)
    {
        _currentMousePos = currentMousePos;
    }

    public void Update()
    {
        foreach (var keyboardAxisBinding in _keyboardAxisBindings)
        {
            if (_pressedKeys.Contains(keyboardAxisBinding.Key))
            {
                foreach (var inputComponent in keyboardAxisBinding.Value)
                {
                    inputComponent.Value.Invoke();
                }
            }
        }
        
        foreach (var mouseHoverBinding in _mouseHoverBindings)
        {
            mouseHoverBinding.Value.Invoke(_currentMousePos, _prevMousePos);
        }
        
        _prevMousePos = _currentMousePos;
    }

    public void BindAxis(Object contextObject, Key key, Action action)
    {
        if (!_keyboardAxisBindings.ContainsKey(key))
        {
            _keyboardAxisBindings[key] = new Dictionary<Object, Action>();
        }

        _keyboardAxisBindings[key][contextObject] = action;
    }

    public void BindOnMouseHover(Object contextObject, Action<Point, Point> action)
    {
        _mouseHoverBindings.Add(contextObject, action);
    }
}