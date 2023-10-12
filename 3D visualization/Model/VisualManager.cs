using System.Numerics;
using _3D_visualization.Model.Figures;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model;

public class VisualManager
{
    private readonly OpenGLControl _openGLControl;
    private Camera _camera;
    private List<BaseFigure> _figuresToDraw;
    float rotatePyramid = 0;
    float rquad = 0;

    public VisualManager(OpenGLControl openGlControl)
    {
        _openGLControl = openGlControl;
        _camera = new Camera(openGlControl);

    }

    public void SetFiguresToDraw(List<BaseFigure> figuresToDraw)
    {
        _figuresToDraw = figuresToDraw;
    }

    public void Update()
    {
        _camera.Update();
        
        //  Get the OpenGL instance that's been passed to us.
        OpenGL gl = _openGLControl.OpenGL;

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

        //  Rotate the geometry a bit.
        rotatePyramid += 3.0f;
        rquad -= 3.0f;
    }

    public void Resized(double width, double height)
    {
        /*openGLControl.Focus();*/

        OpenGL gl = _openGLControl.OpenGL;

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
        //  Enable the OpenGL depth testing functionality.
        _openGLControl.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
    }
}