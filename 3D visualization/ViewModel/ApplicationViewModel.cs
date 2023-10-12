using System.Numerics;
using System.Timers;
using System.Windows.Input;
using _3D_visualization.Model;
using SharpGL;
using SharpGL.WPF;
using InputManager = _3D_visualization.Model.InputManager;
using Timer = System.Timers.Timer;

namespace _3D_visualization.ViewModel;

public class ApplicationViewModel
{
    /*private Timer _globalTimer;*/
    private readonly OpenGLControl _openGlControl;
    private readonly InputManager _inputManager;
    private readonly VisualManager _visualManager;

    public ApplicationViewModel(OpenGLControl openGlControl, int fps = 120)
    {
        /*InitializeGlobalTimer(fps);*/
        _inputManager = new InputManager();
        _visualManager = new VisualManager(openGlControl);
        _openGlControl = openGlControl;
    }

    /*private void InitializeGlobalTimer(int fps)
    {
        _globalTimer = new System.Timers.Timer((double)1000 / fps);
        _globalTimer.Elapsed += Update;
        _globalTimer.AutoReset = true;
        _globalTimer.Enabled = true;
    }*/

    public void Update(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        _visualManager.Update();
        _inputManager.Update();
    }
    
    public void OpenGLControl_KeyDown(object sender, KeyEventArgs e)
    {
       _inputManager.OnKeyPressed(e.Key);
    }
    
    public void OpenGLControl_KeyUp(object sender, KeyEventArgs e)
    {
        _inputManager.OnKeyReleased(e.Key);
    }
    
    public void OpenGLControl_MouseHover(object sender, MouseEventArgs e)
    {
        _inputManager.OnMouseHover(e.GetPosition(_openGlControl));
    }

    public void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        _visualManager.Initialize();
    } 
    
    public void OpenGLControl_OpenGLResized(double Width, double Height)
    {
        _visualManager.Resized(Width, Height);
    } 
}