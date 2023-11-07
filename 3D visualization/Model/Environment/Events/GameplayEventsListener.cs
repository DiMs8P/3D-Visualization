using System.Windows.Input;
using _3D_visualization.DataTypes;

namespace _3D_visualization.Model.Events;

public class GameplayEventsListener
{
    public delegate void OnDirectionalLightEnable(bool directionalLightEnable);
    public event OnDirectionalLightEnable OnDirectionalLightEnableEvent;
    
    public delegate void OnPointLightEnable(bool pointLightEnable);
    public event OnPointLightEnable OnPointLightEnableEvent;
    
    public delegate void OnSpotLightEnable(bool spotLightEnable);
    public event OnSpotLightEnable OnSpotLightEnableEvent;
    
    public delegate void OnTextureEnable(bool textureEnable);
    public event OnTextureEnable OnTextureEnableEvent;
    
    public delegate void OnDrawNormals(bool drawNormals);
    public event OnDrawNormals OnDrawNormalsEvent;
    
    public delegate void OnShowWireFrame(bool showWireFrame);
    public event OnShowWireFrame OnShowWireFrameEvent;
    
    public delegate void OnSmoothNormalsEnable(bool smoothNormalsEnable);
    public event OnSmoothNormalsEnable OnSmoothNormalsEnableEvent;
    
    public void InvokeOnDirectionalLightEnable(bool directionalLightEnable)
    {
        var handler = OnDirectionalLightEnableEvent;
        if (handler != null)
        {
            handler.Invoke(directionalLightEnable);
        }
    }
    
    public void InvokeOnPointLightEnable(bool pointLightEnable)
    {
        var handler = OnPointLightEnableEvent;
        if (handler != null)
        {
            handler.Invoke(pointLightEnable);
        }
    }
    
    public void InvokeOnSpotLightEnable(bool spotLightEnable)
    {
        var handler = OnSpotLightEnableEvent;
        if (handler != null)
        {
            handler.Invoke(spotLightEnable);
        }
    }
    
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