using _3D_visualization.Model.Components;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.WPF;

namespace _3D_visualization.Model.Controllers;

public class SceneController : IController
{
    private CameraComponent _currentCameraComponent;
    private List<RenderComponent> _renderComponents;
    private OpenGLControl _openGlControl;
    /*private List<BaseRenderObject> _renderObjects;*/
    
    float rotatePyramid = 0;
    float rquad = 0;

    public SceneController(OpenGLControl openGlControl)
    {
        _openGlControl = openGlControl;
        _currentCameraComponent = new CameraComponent();
        _renderComponents = new List<RenderComponent>();
    }

    public void RegisterRenderComponent(RenderComponent renderComponent)
    {
        _renderComponents.Add(renderComponent);
    }

    public void SetActiveCamera(CameraComponent cameraComponent)
    {
        _currentCameraComponent = cameraComponent;
    }
    
    /*public void AddFigureToRender(BaseRenderObject renderObject)
    {
        _renderObjects.Add(renderObject);
    }*/

    public void Update()
    {
        OpenGL gl = _openGlControl.OpenGL;
        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        gl.LoadIdentity();
        
        UpdateCameraView();

        foreach (var renderComponent in _renderComponents)
        {
            renderComponent.Render(gl);
        }

        gl.Flush();
        
        rotatePyramid += 3.0f;
        rquad -= 3.0f;
    }

    private void UpdateCameraView()
    {
        _currentCameraComponent.UpdateCameraComponent(_openGlControl.OpenGL);
    }

    public void Resized(double width, double height)
    {
        /*openGLControl.Focus();*/

        OpenGL gl = _openGlControl.OpenGL;
        gl.GetModelViewMatrix();

        gl.MatrixMode(OpenGL.GL_PROJECTION);

        //  Load the identity.
        gl.LoadIdentity();
        
        //  Create a perspective transformation.
        
        // Set Ortho projection
        //gl.Ortho(-8.0, 8.0, -8.0, 8.0, 0.01, 100.0);
        gl.Perspective(60.0f, width / height, 0.01, 100.0);

        //  Set the modelview matrix.
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
    }
    
    public void Initialize()
    {
        _openGlControl.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
    }
}