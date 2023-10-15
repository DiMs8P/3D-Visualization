using _3D_visualization.Model.Components;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model.Controllers;

public class SceneController : IController
{
    private CameraComponent _currentCameraComponent;
    private OpenGLControl _openGlControl;
    
    float rotatePyramid = 0;
    float rquad = 0;

    public SceneController(OpenGLControl openGlControl)
    {
        _openGlControl = openGlControl;
        _currentCameraComponent = new CameraComponent();
    }

    public void SetActiveCamera(CameraComponent cameraComponent)
    {
        _currentCameraComponent = cameraComponent;
    }

    public void Update()
    {
        OpenGL gl = _openGlControl.OpenGL;

        UpdateCameraView();

        //  Clear the color and depth buffers.
        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

        //  Move the geometry into a fairly central position.
        gl.Translate(-1.5f, 0.0f, -6.0f);

        //  Draw a pyramid. First, rotate the modelview matrix.
        gl.Rotate(rotatePyramid, 0.0f, 1.0f, 0.0f);

        //  Start drawing triangles.
        gl.Begin(OpenGL.GL_TRIANGLES);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);

        gl.End();

        //  Reset the modelview.
        gl.PopMatrix();

        //  Move into a more central position.
        gl.Translate(1.5f, 0.0f, -7.0f);

        //  Rotate the cube.
        gl.Rotate(rquad, 1.0f, 1.0f, 1.0f);

        //  Provide the cube colors and geometry.
        gl.Begin(OpenGL.GL_QUADS);

            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);

            gl.Color(1.0f, 0.5f, 0.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);

            gl.Color(1.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);

            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);

            gl.Color(1.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);

        gl.End();

        //  Flush OpenGL.
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

        gl.MatrixMode(OpenGL.GL_PROJECTION);

        //  Load the identity.
        gl.LoadIdentity();
        
        //  Create a perspective transformation.
        gl.Perspective(60.0f, width / height, 0.01, 100.0);

        //  Set the modelview matrix.
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
    }
    
    public void Initialize()
    {
        _openGlControl.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
    }
}