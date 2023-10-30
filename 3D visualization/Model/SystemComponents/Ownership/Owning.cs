namespace _3D_visualization.Model.SystemComponents.Ownership;

public struct Owning
{
    public int Owner;

    public Owning(int ownerEntityId)
    {
        Owner = ownerEntityId;
    }
}