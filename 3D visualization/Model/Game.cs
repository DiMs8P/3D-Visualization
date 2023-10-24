using _3D_visualization.Model.Events;
using _3D_visualization.Model.Factory;
using _3D_visualization.Model.Input;
using _3D_visualization.Model.Input.Systems;
using _3D_visualization.Model.SystemComponents.Initialization;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL.WPF;

namespace _3D_visualization.Model;

public class Game
{
    public InputEventsListener InputEventsListener { get; private set; }
    public GameplayEventsListener GameplayEventsListener { get; private set; }
    public ObjectsFactory Factory { get; private set; }

    private EcsWorld _world;
    private EcsSystems _setupSystems;
    private EcsSystems _inputSystems;
    private EcsSystems _gameplaySystems;
    private EcsSystems _renderSystems;
    public Game(OpenGLControl openGlControl)
    {
        InitializeSystems(openGlControl);
    }

    private void InitializeSystems(OpenGLControl openGlControl)
    {
        _world = new EcsWorld();
        
        Factory = new ObjectsFactory(_world);
        GameplayEventsListener = new GameplayEventsListener();
        InputEventsListener = new InputEventsListener();

        _setupSystems = new EcsSystems(_world);
        _setupSystems
            .Add(new InitEnvironmentSystem())
            .Inject(Factory)
            .Init();
        
        _inputSystems = new EcsSystems(_world);
        _inputSystems
            .Add(new KeyboardInputsSystem())
            .Add(new MouseInputsSystem())
            .Inject(InputEventsListener)
            .Init();
    }

    public void Update(float deltaTime)
    {
        _inputSystems.Run();
    }
}