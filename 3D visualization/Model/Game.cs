﻿using _3D_visualization.Model.Environment;
using _3D_visualization.Model.Events;
using _3D_visualization.Model.Factory;
using _3D_visualization.Model.SystemComponents;
using _3D_visualization.Model.SystemComponents.Input.Systems;
using _3D_visualization.Model.SystemComponents.MainCamera.System;
using _3D_visualization.Model.SystemComponents.Markers;
using _3D_visualization.Model.SystemComponents.Ownership;
using _3D_visualization.Model.SystemComponents.Player.Systems;
using _3D_visualization.Model.SystemComponents.Render;
using _3D_visualization.Model.SystemComponents.Spline.Systems;
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
    private EcsSystems _timeSystems;
    private EcsSystems _inputSystems;
    private EcsSystems _gameplaySystems;
    private EcsSystems _renderSystems;

    private DeltaTimeUpdateSystem _deltaTimeUpdateSystem;
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
        
        _deltaTimeUpdateSystem = new DeltaTimeUpdateSystem();

        _setupSystems = new EcsSystems(_world);
        _setupSystems
            .Add(new InitEnvironmentSystem())
            .Inject(Factory)
            .Init();

        _timeSystems = new EcsSystems(_world);
        _timeSystems
            .Add(_deltaTimeUpdateSystem)
            .Init();

        _inputSystems = new EcsSystems(_world);
        _inputSystems
            .Add(new KeyboardInputsSystem())
            .Add(new MouseInputsSystem())
            .Inject(InputEventsListener)
            .Init();

        // TODO remove auto mapper
        _gameplaySystems = new EcsSystems(_world);
        _gameplaySystems
            .Add(new PlayerObservationSystem())
            .Add(new PlayerMovementSystem())
            .Add(new OwnershipSystem())
            .Add(new SplineTransformSystem())
            .Inject(new AutoMapper2D(), GameplayEventsListener)
            .Init();

        _renderSystems = new EcsSystems(_world);
        _renderSystems
            .Add(new OpenGlPreRunSystem())
            .Add(new MainCameraRenderSystem())
            .Add(new LightningRenderSystem())
            .Add(new SplineRenderSystem())
            .Add(new OpenGlPostRunSystem())
            .Inject(GameplayEventsListener, new ShaderManager(_world, openGlControl, GameplayEventsListener), openGlControl)
            .Init();
    }

    public void Update(float deltaTime)
    {
        _deltaTimeUpdateSystem.SetDeltaTime(deltaTime);
        
        _timeSystems.Run();
        _inputSystems.Run();
        _gameplaySystems.Run();
        _renderSystems.Run();
    }
}