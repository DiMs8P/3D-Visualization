using System.Numerics;
using _3D_visualization.Model.Input.Components;
using _3D_visualization.Model.SystemComponents.Input.Components;
using _3D_visualization.Model.SystemComponents.MainCamera.Components;
using _3D_visualization.Model.SystemComponents.Markers;
using _3D_visualization.Model.SystemComponents.Player;
using _3D_visualization.Model.SystemComponents.Spline.Components;
using _3D_visualization.Model.SystemComponents.Transform.Components;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.Factory;

public class ObjectsFactory
{
    protected EcsWorld _world;

    private int _currentEntity = -1;
    
    public ObjectsFactory(EcsWorld world)
    {
        _world = world;
    }
    
    public ObjectsFactory Create() {
        _currentEntity = _world.NewEntity();
        return this;
    }
    
    public ObjectsFactory Add<T>(T component) where T : struct {
        if (_currentEntity == -1)
        {
            throw new System.Exception("No current entity. You must call Create() before adding components.");
        }

        var poolWithTemplateComponent = _world.GetPool<T>();
        poolWithTemplateComponent.Add(_currentEntity) = component;
        
        return this;
    }
    
    public int End() {
        if (_currentEntity == -1)
            throw new System.Exception("No current entity. You must call Create() before calling End().");

        var result = _currentEntity;
        _currentEntity = -1;
        return result;
    }
}