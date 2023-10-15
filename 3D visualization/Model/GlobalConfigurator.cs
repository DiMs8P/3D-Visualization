using _3D_visualization.Model.Controllers;

using SharpGL.WPF;

namespace _3D_visualization.Model;

public class GlobalConfigurator
{
    public InputController InputController { get; }
    public SceneController SceneController { get; }

    public GlobalConfigurator(OpenGLControl openGlControl)
    {
        InputController = new InputController();
        SceneController = new SceneController(openGlControl);
    }

    public void Update()
    {
        InputController.Update();
        SceneController.Update();
    }
}