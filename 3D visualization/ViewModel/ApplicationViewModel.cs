using System.Numerics;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using _3D_visualization.Model;
using _3D_visualization.Model.Replication;
using SharpGL;
using SharpGL.WPF;
using Timer = System.Timers.Timer;

namespace _3D_visualization.ViewModel;

public class ApplicationViewModel
{
    public ApplicationViewModel(OpenGLControl openGlControl, int fps = 120)
    {
        GlobalEnvironment.GetInstance.Register(openGlControl);
    }

    public void Update(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        GlobalEnvironment.GetInstance.Update();
    }

    public void Initialize(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().SceneController.Initialize();
    }

    public void Resized(double width, double height)
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().SceneController.Resized(width, height);
    }

    public void KeyDown(object sender, KeyEventArgs keyEventArgs)
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().InputController.OnKeyPressed(sender, keyEventArgs);
    }

    public void KeyUp(object sender, KeyEventArgs keyEventArgs)
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().InputController.OnKeyReleased(sender, keyEventArgs);
    }

    public void MouseHover(Point currentMousePos)
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().InputController.OnMouseHover(currentMousePos);
    }

    public void SetReplicationObjects(string fileName)
    {
        var replicationData = ReplicationParser.TryParse(fileName);
        ReplicationObject figuresGenerator = new ReplicationObject(replicationData);
        /*GlobalEnvironment.GetInstance.GetGlobalConfigurator().SceneController.Set*/
    }
}