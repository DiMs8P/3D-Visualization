using System.Windows.Input;

namespace _3D_visualization.Model.Events;

public class GameplayEventsListener
{
    public delegate void OnTextureEnable(bool textureEnable);
    public event OnTextureEnable OnTextureEnableEvent;
    
    public delegate void OnDrawNormals(bool drawNormals);
    public event OnDrawNormals OnDrawNormalsEvent;
    
    public delegate void OnShowWireFrame(bool showWireFrame);
    public event OnShowWireFrame OnShowWireFrameEvent;
    
    public delegate void OnSmoothNormalsEnable(bool smoothNormalsEnable);
    public event OnSmoothNormalsEnable OnSmoothNormalsEnableEvent;

    public void InvokeOnTextureEnable(bool textureEnable)
    {
        var handler = OnTextureEnableEvent;
        if (handler != null)
        {
            handler.Invoke(textureEnable);
        }
    }
    
    public void InvokeOnDrawNormals(bool drawNormals)
    {
        var handler = OnDrawNormalsEvent;
        if (handler != null)
        {
            handler.Invoke(drawNormals);
        }
    }
    
    public void InvokeOnShowWireFrame(bool showWireFrame)
    {
        var handler = OnShowWireFrameEvent;
        if (handler != null)
        {
            handler.Invoke(showWireFrame);
        }
    }
    
    public void InvokeOnSmoothNormalsEnable(bool smoothNormalsEnable)
    {
        var handler = OnSmoothNormalsEnableEvent;
        if (handler != null)
        {
            handler.Invoke(smoothNormalsEnable);
        }
    }
}