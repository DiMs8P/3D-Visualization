using _3D_visualization.Model.Controllers;
using _3D_visualization.Model.Player;
using SharpGL.WPF;

namespace _3D_visualization.Model;

public sealed class GlobalEnvironment
{
    public static GlobalEnvironment GetInstance { get; } = new GlobalEnvironment();

    private GlobalConfigurator _globalConfigurator;
    private PlayerController _payerController;
    
    static GlobalEnvironment(){}
    private GlobalEnvironment(){}

    public void Register(OpenGLControl openGlControl)
    {
        _globalConfigurator = new GlobalConfigurator(openGlControl);
        _payerController = new PlayerController(new Character());
    }

    public void Update()
    {
        _globalConfigurator.Update();
    }

    public GlobalConfigurator GetGlobalConfigurator()
    {
        return _globalConfigurator;
    }
    
    public PlayerController GetPlayerController()
    {
        return _payerController;
    }
}