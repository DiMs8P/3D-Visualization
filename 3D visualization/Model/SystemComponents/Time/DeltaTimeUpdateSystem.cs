using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.SystemComponents.Markers;

public class DeltaTimeUpdateSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _deltaTimeFilter;
    private EcsPool<TimeOffset> _deltaTimeComponent;
    
    private int _deltaTimeId;

    private float _deltaTime = 0.0f;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _deltaTimeFilter = world.Filter<TimeOffset>().End();
        _deltaTimeComponent = world.GetPool<TimeOffset>();

        _deltaTimeId = EntityUtils.GetUniqueEntityIdFromFilter(_deltaTimeFilter);
    }

    public void Run(IEcsSystems systems)
    {
        ref TimeOffset deltaTime = ref _deltaTimeComponent.Get(_deltaTimeId);
        deltaTime.DeltaTime = _deltaTime;
    }

    public void SetDeltaTime(float deltaTime)
    {
        _deltaTime = deltaTime;
    }
}