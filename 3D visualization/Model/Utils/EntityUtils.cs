using Leopotam.EcsLite;

namespace _3D_visualization.Model.Utils;

public class EntityUtils
{
    // TODO add custom exception
    public static int GetUniqueEntityIdFromFilter(EcsFilter filter)
    {
        if (filter.GetEntitiesCount() != 1)
        {
            throw new System.Exception();
        }
        
        int uniqueEntityId = 0;
        foreach (int entityId in filter)
        {
            uniqueEntityId = entityId;
        }

        return uniqueEntityId;
    }
}