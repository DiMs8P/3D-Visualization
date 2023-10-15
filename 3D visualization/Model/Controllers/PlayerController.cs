using System.Windows;
using System.Windows.Input;
using _3D_visualization.Model.Player;

namespace _3D_visualization.Model.Controllers;

public class PlayerController : IController
{
    private Character _player;

    public PlayerController(Character player)
    {
        _player = player;
    }

    public PlayerController()
    {
        _player = new Character();
    }
}