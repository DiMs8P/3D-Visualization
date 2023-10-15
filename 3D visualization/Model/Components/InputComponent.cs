using System.Windows;
using System.Windows.Input;

namespace _3D_visualization.Model.Components;

public class InputComponent : BaseComponent
{
    public void BindAxis(Key key, Action action)
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().InputController.BindAxis(this, key, action);
    }

    public void BindMouseHover(Action<Point, Point> mouseHover)
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().InputController.BindOnMouseHover(this, mouseHover);
    }
}