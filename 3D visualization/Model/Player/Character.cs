using System.Windows.Input;
using _3D_visualization.Model.Components;
using Object = _3D_visualization.Model.Core.Object;

namespace _3D_visualization.Model.Player;

public class Character : Object
{
    private InputComponent _inputComponent;
    private CameraComponent _cameraComponent;
    
    public Character()
    {
        _cameraComponent = new CameraComponent();
        _cameraComponent.SetActive();
        
        _inputComponent = new InputComponent();
        SetupInput(_inputComponent);
    }

    private void SetupInput(InputComponent inputComponent)
    {
        inputComponent.BindAxis(Key.W, _cameraComponent.MoveForward);
        inputComponent.BindAxis(Key.S, _cameraComponent.MoveBackward);
        inputComponent.BindAxis(Key.A, _cameraComponent.MoveLeft);
        inputComponent.BindAxis(Key.D, _cameraComponent.MoveRight);
        inputComponent.BindMouseHover(_cameraComponent.MouseHover);
    }
}