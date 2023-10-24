using System.Numerics;
using System.Windows;

namespace _3D_visualization.Model.Input.Components;

public struct MousePosition
{
    public Point CurrentMousePosition;
    public Point PreviousMousePosition;

    public MousePosition()
    {
        CurrentMousePosition = new Point();
        PreviousMousePosition = CurrentMousePosition;
    }
}