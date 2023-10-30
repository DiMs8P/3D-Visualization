using _3D_visualization.Model.SystemComponents.Transform.Components;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.SystemComponents.Ownership;

public class OwnershipSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsPool] EcsPool<Owning> _owningComponents;
    
    [EcsPool] EcsPool<Rotation> _rotationComponents;
    [EcsPool] EcsPool<Location> _locationComponents;
    
    [EcsPool] EcsPool<RotationRequest> _rotationRequestComponents;
    [EcsPool] EcsPool<LocationRequest> _locationRequestComponents;
    
    [EcsPool] EcsFilter _owningFilter;
    
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _owningFilter = world.Filter<Owning>().End();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var owningEntityId in _owningFilter)
        {
            if (_locationRequestComponents.Has(owningEntityId))
            {
                UpdateLocation(owningEntityId);
            }
            
            if (_rotationRequestComponents.Has(owningEntityId))
            {
                UpdateRotation(owningEntityId);
            }
        }
    }
    
    // TODO add custom exception
    private void UpdateLocation(int owningEntityId)
    {
        ref Owning owningComponent = ref _owningComponents.Get(owningEntityId);

        int ownerEntityId = owningComponent.Owner;
        if (!_locationComponents.Has(ownerEntityId))
        {
            throw new System.Exception("");
        }

        ref Location ownerLocation = ref _locationComponents.Get(ownerEntityId);
        ref Location owningLocation = ref _locationComponents.Get(owningEntityId);

        owningLocation.Position = ownerLocation.Position;
    }
    
    private void UpdateRotation(int owningEntityId)
    {
        ref Owning owningComponent = ref _owningComponents.Get(owningEntityId);

        int ownerEntityId = owningComponent.Owner;
        if (!_rotationComponents.Has(ownerEntityId))
        {
            throw new System.Exception("");
        }

        ref Rotation ownerRotation = ref _rotationComponents.Get(ownerEntityId);
        ref Rotation owningRotation = ref _rotationComponents.Get(owningEntityId);

        owningRotation.ForwardVector = ownerRotation.ForwardVector;
        owningRotation.RightVector = ownerRotation.RightVector;
        owningRotation.UpVector = ownerRotation.UpVector;
    }
}