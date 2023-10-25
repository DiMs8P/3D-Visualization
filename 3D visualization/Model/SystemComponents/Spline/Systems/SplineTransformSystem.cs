using System.Numerics;
using Leopotam.EcsLite;

namespace _3D_visualization.Model.SystemComponents.Spline.Systems;

// TODO refactor all that connected with spline (i won't)
public class SplineTransformSystem : IEcsInitSystem, IEcsRunSystem
{
    EcsPool<Components.Spline> _splineComponents;
    
    EcsFilter _splineFilter;
    public void Init(IEcsSystems systems)
    {

    }

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _splineFilter = world.Filter<Components.Spline>().End();
        _splineComponents = world.GetPool<Components.Spline>();

        foreach (var splineEntityId in _splineFilter)
        {
            ref Components.Spline spline = ref _splineComponents.Get(splineEntityId);
            if (spline.PointLocations.Count == 0)
            {
                UpdateSplinePointsLocation(ref spline);
            }
        }
    }

    private void UpdateSplinePointsLocation(ref Components.Spline spline)
    {
        for (int i = 0; i < spline.Path.Count; i++)
        {
            spline.PointLocations.Add(new List<Vector3>());

            foreach (var point in spline.Section)
            {
                spline.PointLocations[i].Add(new Vector3());
            }
        }

        InitializePoints(ref spline);
    }

    private void InitializePoints(ref Components.Spline spline)
    {
        Vector3 direction = spline.Path[0] - spline.Path[1];
        Vector3 nextDirection = spline.Path[1] - spline.Path[2];
        InitializeBottom(ref spline, direction, nextDirection);
        
        for (int i = 0; i < spline.Path.Count - 2; i++)
        {
            InitializeCenter(ref spline, i + 1, direction, nextDirection);
            
            direction = spline.Path[i] - spline.Path[i + 1];
            nextDirection = spline.Path[i + 1] - spline.Path[i + 2];
        }
        
        InitializeTop(ref spline, direction, nextDirection);
    }

    private void InitializeTop(ref Components.Spline spline, Vector3 direction, Vector3 nextDirection)
    {
        Vector3 crossVector = Vector3.Cross(direction, nextDirection);
        
        float zAngle = CalculateAngle(Vector3.UnitZ, direction with { Y = 0 });
        float xAngle = CalculateAngle(direction with { Y = 0 }, direction);
        float yAngle = CalculateAngle(crossVector with { Y = 0 }, crossVector);
        
        for (int i = 0; i < spline.Section.Count; i++)
        {
            Vector3 point = new Vector3(spline.Section[i].X, spline.Section[i].Y, 0);
            point = RotateY(point, zAngle);
            point = RotateX(point, xAngle);
            point = RotateZ(point, yAngle);
            point.X += spline.Path[^1].X;
            point.Y += spline.Path[^1].Y;
            point.Z += spline.Path[^1].Z;
            
            spline.PointLocations[^1][i] = point;
        }
    }

    private void InitializeCenter(ref Components.Spline spline, int currentLocation, Vector3 direction, Vector3 nextDirection)
    {
        Vector3 crossVector = Vector3.Cross(direction, nextDirection);
        
        float zAngle = CalculateAngle(Vector3.UnitZ, direction with { Y = 0 });
        float xAngle = CalculateAngle(direction with { Y = 0 }, direction);
        float yAngle = CalculateAngle(crossVector with { Y = 0 }, crossVector);
        float directionsAngle = CalculateAngle(direction, nextDirection);
        
        for (int i = 0; i < spline.Section.Count; i++)
        {
            Vector3 point = new Vector3(spline.Section[i].X, spline.Section[i].Y, 0);
            point = RotateY(point, zAngle);
            point = RotateX(point, xAngle);
            point = RotateZ(point, yAngle + directionsAngle / 2);
            point.X += spline.Path[currentLocation].X;
            point.Y += spline.Path[currentLocation].Y;
            point.Z += spline.Path[currentLocation].Z;
            
            spline.PointLocations[currentLocation][i] = point;
        }
    }

    private void InitializeBottom(ref Components.Spline spline, Vector3 direction, Vector3 nextDirection)
    {
        Vector3 crossVector = Vector3.Cross(direction, nextDirection);
        
        float zAngle = CalculateAngle(Vector3.UnitZ, direction with { Y = 0 });
        float xAngle = CalculateAngle(direction with { Y = 0 }, direction);
        float yAngle = CalculateAngle(crossVector with { Y = 0 }, crossVector);
        
        for (int i = 0; i < spline.Section.Count; i++)
        {
            Vector3 point = new Vector3(spline.Section[i].X, spline.Section[i].Y, 0);
            point = RotateY(point, zAngle);
            point = RotateX(point, xAngle);
            point = RotateZ(point, yAngle);
            point.X += spline.Path[0].X;
            point.Y += spline.Path[0].Y;
            point.Z += spline.Path[0].Z;
            
            spline.PointLocations[0][i] = point;
        }
    }
    
    float CalculateAngle(Vector3 vector1, Vector3 vector2)
    {
        float dotProduct = Vector3.Dot(vector1, vector2);
        float magnitude1 = vector1.Length();
        float magnitude2 = vector2.Length();

        float cosTheta = dotProduct / (magnitude1 * magnitude2);

        float thetaRad = (float)Math.Acos(cosTheta);

        float thetaDeg = thetaRad * 180 / (float)Math.PI;
        float sign = Math.Sign(Vector3.Cross(vector1, vector2).Y);

        if (sign == 0)
        {
            sign = 1;
        }

        return thetaDeg * sign;
    }
    
    Vector3 RotateY(Vector3 vector, float angle)
    {
        double radians = (Math.PI / 180) * angle;
        
        double[,] matrix = {
            {Math.Cos(radians), 0, Math.Sin(radians)},
            {0, 1, 0},
            {-Math.Sin(radians), 0, Math.Cos(radians)}
        };
        
        double x = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z;
        double y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z;
        double z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z;

        return new Vector3((float)x, (float)y, (float)z);
    }
    
    Vector3 RotateX(Vector3 vector, float angle)
    {
        double radians = (Math.PI / 180) * angle;
        
        double[,] matrix = {
            {1, 0, 0},
            {0, Math.Cos(radians), -Math.Sin(radians)},
            {0, Math.Sin(radians), Math.Cos(radians)}
        };
        
        double x = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z;
        double y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z;
        double z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z;

        return new Vector3((float)x, (float)y, (float)z);
    }
    
    Vector3 RotateZ(Vector3 vector, float angle)
    {
        double radians = (Math.PI / 180) * angle;
        
        double[,] matrix = {
            {Math.Cos(radians), -Math.Sin(radians), 0},
            {Math.Sin(radians), Math.Cos(radians), 0},
            {0, 0, 1}
        };
        
        double x = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z;
        double y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z;
        double z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z;

        return new Vector3((float)x, (float)y, (float)z);
    }
}