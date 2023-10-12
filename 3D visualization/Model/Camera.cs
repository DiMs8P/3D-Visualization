using System.Numerics;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model;

public class Camera
{
    Vector3 cameraPos = new Vector3(0f, 0f, 3f);
    Vector3 cameraFront = new Vector3(0f, 0f, -1f);
    Vector3 cameraUp = new Vector3(0f, 1f, 0f);
    private readonly OpenGLControl _openGLControl;

    public Camera(OpenGLControl openGlControl)
    {
        _openGLControl = openGlControl;
    }

    public void Update()
    {
        OpenGL gl = _openGLControl.OpenGL;
        //  Reset the modelview matrix.
        gl.LoadIdentity();

        Vector3 cameraDir = Vector3.Add(cameraPos, cameraFront);

        gl.LookAt(
            cameraPos.X, cameraPos.Y, cameraPos.Z,
            cameraDir.X, cameraDir.Y, cameraDir.Z,
            cameraUp.X, cameraUp.Y, cameraUp.Z);

        gl.PushMatrix();
    }
}