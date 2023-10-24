using System.Numerics;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using _3D_visualization.Model;
using SharpGL;
using SharpGL.WPF;
using Timer = System.Timers.Timer;

namespace _3D_visualization.ViewModel;

public class ApplicationViewModel
{
    private Game _game;
    private GameLoop _gameLoop;
    public ApplicationViewModel(OpenGLControl openGlControl, int fps = 120)
    {
        _game = new Game(openGlControl);

        _gameLoop = new GameLoop(fps);
        _gameLoop.LoadGame(_game);
        _gameLoop.Start();
    }

    public void Update(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
    }

    public void Initialize(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
    }

    public void Resized(double width, double height)
    {
    }

    public void KeyDown(object sender, KeyEventArgs keyEventArgs)
    {
        _game.InputEventsListener.InvokeOnKeyPressed(keyEventArgs.Key);
    }

    public void KeyUp(object sender, KeyEventArgs keyEventArgs)
    {
        _game.InputEventsListener.InvokeOnKeyReleased(keyEventArgs.Key);
    }

    public void MouseHover(Point currentMousePos)
    {
        _game.InputEventsListener.InvokeOnMouseMove(currentMousePos);
    }

    public void SetReplicationObjects(string fileName)
    {
    }
}