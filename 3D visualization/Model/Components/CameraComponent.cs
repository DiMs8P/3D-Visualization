using System.Numerics;
using System.Windows;
using System.Windows.Input;
using SharpGL;

namespace _3D_visualization.Model.Components;

public class CameraComponent : BaseComponent
{
    private Vector3 _cameraPos = new Vector3(0f, 0f, 3f);
    private Vector3 _cameraFront = new Vector3(0f, 0f, -1f);
    private Vector3 _cameraUp = new Vector3(0f, 1f, 0f);
    
    private float _yaw = -90.0f;
    private float _pitch = 0f;
    private float _speed = 0.05f;
    public void SetActive()
    {
        GlobalEnvironment.GetInstance.GetGlobalConfigurator().SceneController.SetActiveCamera(this);
    }

    public void SetSpeed(float newSpeed) => _speed = newSpeed;

    public void UpdateCameraComponent(OpenGL openGl)
    {
        openGl.LoadIdentity();

        Vector3 cameraDir = Vector3.Add(_cameraPos, _cameraFront);

        openGl.LookAt(
            _cameraPos.X, _cameraPos.Y, _cameraPos.Z,
            cameraDir.X, cameraDir.Y, cameraDir.Z,
            _cameraUp.X, _cameraUp.Y, _cameraUp.Z);

        openGl.PushMatrix();
    }
    
    public void MoveRight()
    {
        _cameraPos += Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * _speed;
    }

    public void MoveLeft()
    {
        _cameraPos -= Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * _speed;
    }

    public void MoveForward()
    {
        _cameraPos += _speed * _cameraFront;
    }

    public void MoveBackward()
    {
        _cameraPos -= _speed * _cameraFront;
    }
    
    public void MouseHover(Point currentMousePos, Point prevMousePos)
    {
        double xoffset = currentMousePos.X - prevMousePos.X;
        double yoffset = prevMousePos.Y - currentMousePos.Y;
        const float sensitivity = 0.15f;
        xoffset *= sensitivity;
        yoffset *= sensitivity;
        
        _yaw   += (float)xoffset;
        _pitch += (float)yoffset;
        
        if(_pitch > 89.0f)
            _pitch =  89.0f;
        if(_pitch < -89.0f)
            _pitch = -89.0f;
        
        Vector3 direction;
        direction.X = (float)(Math.Cos(ConvertDegreesToRadians(_yaw)) * Math.Cos(ConvertDegreesToRadians(_pitch)));
        direction.Y = (float)Math.Sin(ConvertDegreesToRadians(_pitch));
        direction.Z = (float)(Math.Sin(ConvertDegreesToRadians(_yaw)) * Math.Cos(ConvertDegreesToRadians(_pitch)));
        _cameraFront = Vector3.Normalize(direction);
    }
    
    public static float ConvertDegreesToRadians (float degrees)
    {
        float radians = ((float)Math.PI / 180) * degrees;
        return (radians);
    }
}