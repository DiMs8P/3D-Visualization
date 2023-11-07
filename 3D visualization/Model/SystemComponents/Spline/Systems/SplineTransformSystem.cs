using System.Numerics;
using _3D_visualization.Model.Environment;
using _3D_visualization.Model.Events;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.SystemComponents.Spline.Systems;

// TODO refactor all that connected with spline (i won't)
public class SplineTransformSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] AutoMapper2D _autoMapper2D;
    [EcsInject] GameplayEventsListener _gameplayEventsListener;
    
    EcsPool<Components.Spline> _splineComponents;
    
    EcsFilter _splineFilter;

    bool _shoothNormals = false;
    public void Init(IEcsSystems systems)
    {
        _gameplayEventsListener.OnSmoothNormalsEnableEvent += smoothNormals =>
        {
            _shoothNormals = smoothNormals;
            
            foreach (var splineEntityId in _splineFilter)
            {
                ref Components.Spline spline = ref _splineComponents.Get(splineEntityId);
                if (spline.PointsLocation.Count != 0)
                {
                    RecalculateNormals(ref spline);
                }
            }
        };
    }

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _splineFilter = world.Filter<Components.Spline>().End();
        _splineComponents = world.GetPool<Components.Spline>();

        foreach (var splineEntityId in _splineFilter)
        {
            ref Components.Spline spline = ref _splineComponents.Get(splineEntityId);
            if (spline.PointsLocation.Count == 0)
            {
                AllocateMemory(ref spline);
                
                UpdateSplinePointsLocation(ref spline);
                UpdateSplinePointsNormals(ref spline);
                UpdateSplinePointsSmoothNormals(ref spline);
                UpdateSplinePointsColors(ref spline);
                UpdateSplinePointsTexCoords(ref spline);

                FillVboData(ref spline);
                RecalculateNormals(ref spline);
            }
        }
    }

    private void AllocateMemory(ref Components.Spline spline)
    {
        for (int i = 0; i < spline.Path.Count; i++)
        {
            spline.PointsLocation.Add(new List<Vector3>());
            spline.Normals.Add(new List<Vector3>());
            spline.SmoothNormals.Add(new List<Vector3>());
            spline.PointsColor.Add(new List<Vector3>());

            foreach (var point in spline.Section)
            {
                spline.PointsLocation[i].Add(new Vector3());
            }
        }
        
        spline.Normals.Add(new List<Vector3>());
    }

    private void UpdateSplinePointsLocation(ref Components.Spline spline)
    {
        Vector3 direction = spline.Path[0] - spline.Path[1];
        Vector3 nextDirection = spline.Path[1] - spline.Path[2];
        InitializeBottomLocations(ref spline, direction, nextDirection);
        
        for (int i = 0; i < spline.Path.Count - 2; i++)
        {
            direction = spline.Path[i] - spline.Path[i + 1];
            nextDirection = spline.Path[i + 1] - spline.Path[i + 2];
            
            InitializeCenterLocations(ref spline, i + 1, Vector3.Normalize(direction), Vector3.Normalize(nextDirection));
        }
        
        InitializeTopLocations(ref spline, nextDirection, direction);
    }
    
    private void InitializeBottomLocations(ref Components.Spline spline, Vector3 direction, Vector3 nextDirection)
    {
        int sign = nextDirection.Y < direction.Y ? 1 : -1;
        Vector3 crossVector = Vector3.Cross(direction, nextDirection) * sign;
        
        Vector3 newX = Vector3.Normalize(crossVector);
        Vector3 newZ = Vector3.Normalize(direction);
        Vector3 newY = Vector3.Normalize(Vector3.Cross(direction, crossVector));
        
        for (int i = 0; i < spline.Section.Count; i++)
        {
            Vector3 point = new Vector3(spline.Section[i].X * spline.Scale[0], spline.Section[i].Y * spline.Scale[0], 0);
            Vector3 newPoint = MathHelper.GetPointInAnotherCoordinateSystem(newX, newY, newZ, point);
            
            point.X = spline.Path[0].X + newPoint.X;
            point.Y = spline.Path[0].Y + newPoint.Y;
            point.Z = spline.Path[0].Z + newPoint.Z;
            
            spline.PointsLocation[0][i] = point;
        }
    }

    private void InitializeCenterLocations(ref Components.Spline spline, int currentLocation, Vector3 direction, Vector3 nextDirection)
    {
        int sign = nextDirection.Y < direction.Y ? 1 : -1;
        Vector3 crossVector = Vector3.Cross(direction, nextDirection) * sign;
        
        Vector3 newZ = Vector3.Normalize((direction + nextDirection) / 2);
        Vector3 newX = Vector3.Normalize(crossVector);
        Vector3 newY = Vector3.Normalize(Vector3.Cross(newZ, crossVector));

        for (int i = 0; i < spline.Section.Count; i++)
        {
            Vector3 point = new Vector3(spline.Section[i].X * spline.Scale[currentLocation], spline.Section[i].Y  * spline.Scale[currentLocation], 0);
            Vector3 newPoint = MathHelper.GetPointInAnotherCoordinateSystem(newX, newY, newZ, point);

            point.X = spline.Path[currentLocation].X + newPoint.X;
            point.Y = spline.Path[currentLocation].Y + newPoint.Y;
            point.Z = spline.Path[currentLocation].Z + newPoint.Z;
            
            spline.PointsLocation[currentLocation][i] = point;
        }
    }
    
    private void InitializeTopLocations(ref Components.Spline spline, Vector3 direction, Vector3 nextDirection)
    {
        int sign = nextDirection.Y < direction.Y ? 1 : -1;
        Vector3 crossVector = Vector3.Cross(direction, nextDirection) * sign;
        
        Vector3 newX = Vector3.Normalize(crossVector);
        Vector3 newZ = Vector3.Normalize(direction);
        Vector3 newY = Vector3.Normalize(Vector3.Cross(direction, crossVector));

        
        for (int i = 0; i < spline.Section.Count; i++)
        {
            Vector3 point = new Vector3(spline.Section[i].X * spline.Scale[^1] , spline.Section[i].Y * spline.Scale[^1], 0);
            Vector3 newPoint = MathHelper.GetPointInAnotherCoordinateSystem(newX, newY, newZ, point);
            
            point.X = spline.Path[^1].X + newPoint.X;
            point.Y = spline.Path[^1].Y + newPoint.Y;
            point.Z = spline.Path[^1].Z + newPoint.Z;
            
            spline.PointsLocation[^1][i] = point;
        }
    }
    
    private void UpdateSplinePointsNormals(ref Components.Spline spline)
    {
        InitializeBottomNormals(ref spline);
        
        for (int i = 0; i < spline.Path.Count - 1; i++)
        {
            InitializeCenterNormals(ref spline, i);
        }
        
        InitializeTopNormals(ref spline);
    }
    
    private void UpdateSplinePointsSmoothNormals(ref Components.Spline spline)
    {
        InitializeBottomSmoothNormals(ref spline);
        
        for (int i = 1; i < spline.Path.Count - 1; i++)
        {
            InitializeCenterSmoothNormals(ref spline, i);
        }
        
        InitializeTopSmoothNormals(ref spline);
    }

    private void InitializeTopSmoothNormals(ref Components.Spline spline)
    {
        spline.SmoothNormals[^1].Add(Vector3.Normalize(spline.Normals[^1][0] + spline.Normals[^2][^1] + spline.Normals[^2][0]));

        for (int i = 1; i < spline.Section.Count(); i++)
        {
            spline.SmoothNormals[^1].Add(Vector3.Normalize(spline.Normals[^1][0] + spline.Normals[^2][i - 1] + spline.Normals[^2][i]));
        }
    }
    
    private void InitializeCenterSmoothNormals(ref Components.Spline spline, int currentLocation)
    {
        spline.SmoothNormals[currentLocation].Add(Vector3.Normalize(spline.Normals[currentLocation][0] + spline.Normals[currentLocation][^1] + spline.Normals[currentLocation + 1][0] + spline.Normals[currentLocation + 1][^1]));
        
        for (int i = 1; i < spline.Section.Count(); i++)
        {
            spline.SmoothNormals[currentLocation].Add(Vector3.Normalize(spline.Normals[currentLocation][i - 1] + spline.Normals[currentLocation][i] + spline.Normals[currentLocation + 1][i - 1] + spline.Normals[currentLocation + 1][i]));
        }
    }

    private void InitializeBottomSmoothNormals(ref Components.Spline spline)
    {
        spline.SmoothNormals[0].Add(Vector3.Normalize(spline.Normals[0][0] + spline.Normals[1][^1] + spline.Normals[1][0]));

        for (int i = 1; i < spline.Section.Count(); i++)
        {
            spline.SmoothNormals[0].Add(Vector3.Normalize(spline.Normals[0][0] + spline.Normals[1][i - 1] + spline.Normals[1][i]));
        }
    }

    private void InitializeBottomNormals(ref Components.Spline spline)
    {
        spline.Normals[0].Add(Vector3.Normalize(spline.Path[0] - spline.Path[1]));
    }
    
    private void InitializeCenterNormals(ref Components.Spline spline, int currentLocation)
    {
        Vector3 a, b;
        for (int i = 1; i < spline.Section.Count; i++)
        {
            a = spline.PointsLocation[currentLocation + 1][i - 1] - spline.PointsLocation[currentLocation][i - 1];
            b = spline.PointsLocation[currentLocation][i] - spline.PointsLocation[currentLocation][i - 1];
            
            spline.Normals[currentLocation + 1].Add(Vector3.Normalize(Vector3.Cross(a, b)));
        }
        
        a = spline.PointsLocation[currentLocation + 1][^1] - spline.PointsLocation[currentLocation][^1];
        b = spline.PointsLocation[currentLocation][0] - spline.PointsLocation[currentLocation][^1];
            
        spline.Normals[currentLocation + 1].Add(Vector3.Normalize(Vector3.Cross(a, b)));
    }
    
    private void InitializeTopNormals(ref Components.Spline spline)
    {
        spline.Normals[^1].Add(Vector3.Normalize(spline.Path[^1] - spline.Path[^2]));
    }
    
    private void UpdateSplinePointsColors(ref Components.Spline spline)
    {
        InitializeBottomColors(ref spline);
        
        for (int i = 1; i < spline.Path.Count - 1; i++)
        {
            InitializeCenterColors(ref spline, i);
        }
        
        InitializeTopColors(ref spline);
    }

    private void InitializeTopColors(ref Components.Spline spline)
    {
        for (int i = 0; i < spline.Section.Count; i++)
        {
            spline.PointsColor[^1].Add(new Vector3(1.0f, 0.0f, 1.0f));
        }
    }
    
    private void InitializeCenterColors(ref Components.Spline spline, int currentLocation)
    {
        Random random = new Random();

        for (int i = 0; i < spline.Section.Count; i++)
        {
            spline.PointsColor[currentLocation].Add(new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble()));
        }
    }

    private void InitializeBottomColors(ref Components.Spline spline)
    {
        for (int i = 0; i < spline.Section.Count; i++)
        {
            spline.PointsColor[0].Add(new Vector3(0.0f, 0.0f, 1.0f));
        }
    }

    private void UpdateSplinePointsTexCoords(ref Components.Spline spline)
    {
        InitializeBottomTexCoords(ref spline);
    }

    
    private void InitializeBottomTexCoords(ref Components.Spline spline)
    {
        _autoMapper2D.Start(spline.Section);

        for (int i = 0; i < spline.Section.Count(); i++)
        {
            spline.TexCoords.Add(_autoMapper2D.Lerp(spline.Section[i]));
        }
        
        _autoMapper2D.Finish();
    }
    
    private void FillVboData(ref Components.Spline spline)
    {
        int currentLine = 0;
        FillBottomVbo(ref spline, ref currentLine);
        
        for (int i = 0; i < spline.Path.Count - 1; i++)
        {
            FillCenterVbo(ref spline, i, ref currentLine);
        }
        
        FillTopVbo(ref spline, ref currentLine);
    }

    private void FillTopVbo(ref Components.Spline spline, ref int currentLine)
    {
        for (int i = 0; i < spline.Section.Count; i++)
        {
            spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[^1][i].X;
            spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[^1][i].Y;
            spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[^1][i].Z;
            
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[^1][0].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[^1][0].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[^1][0].Z;
            
            spline.VBOdata[currentLine * 11 + 9] = spline.TexCoords[i].X;
            spline.VBOdata[currentLine * 11 + 10] = spline.TexCoords[i].Y;
            
            spline.Indexes[^(spline.Section.Count - i)] = currentLine;

            ++currentLine;
        }
    }
    
    private void FillCenterVbo(ref Components.Spline spline, int currentLocation, ref int currentLine)
    {
        for (int i = 0; i < spline.Section.Count - 1; i++)
        {
            spline.Indexes[currentLine     + (2 * i + 2 * currentLocation * spline.Section.Count)] = currentLine;
            spline.Indexes[currentLine + 1 + (2 * i + 2 * currentLocation * spline.Section.Count)] = currentLine + 1;
            spline.Indexes[currentLine + 2 + (2 * i + 2 * currentLocation * spline.Section.Count)] = currentLine + 2;
            spline.Indexes[currentLine + 3 + (2 * i + 2 * currentLocation * spline.Section.Count)] = currentLine + 1;
            spline.Indexes[currentLine + 4 + (2 * i + 2 * currentLocation * spline.Section.Count)] = currentLine + 2;
            spline.Indexes[currentLine + 5 + (2 * i + 2 * currentLocation * spline.Section.Count)] = currentLine + 3;

            for (int k = 0; k < 2; k++)
            {
                for (int j = 0; j < 2; j++)
                {
                    spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation + j][i + k].X;
                    spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation + j][i + k].Y;
                    spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation + j][i + k].Z;
            
                    spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation + j][i + k].X;
                    spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation + j][i + k].Y;
                    spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation + j][i + k].Z;
            
                    spline.VBOdata[currentLine * 11 + 9] = k;
                    spline.VBOdata[currentLine * 11 + 10] = j;

                    ++currentLine;
                }
            }
        }
        
        spline.Indexes[currentLine - 1 + (2 * spline.Section.Count - 1 + 2 * currentLocation * spline.Section.Count)] = currentLine;
        spline.Indexes[currentLine     + (2 * spline.Section.Count - 1 + 2 * currentLocation * spline.Section.Count)] = currentLine + 1;
        spline.Indexes[currentLine + 1 + (2 * spline.Section.Count - 1 + 2 * currentLocation * spline.Section.Count)] = currentLine + 2;
        spline.Indexes[currentLine + 2 + (2 * spline.Section.Count - 1 + 2 * currentLocation * spline.Section.Count)] = currentLine + 1;
        spline.Indexes[currentLine + 3 + (2 * spline.Section.Count - 1 + 2 * currentLocation * spline.Section.Count)] = currentLine + 2;
        spline.Indexes[currentLine + 4 + (2 * spline.Section.Count - 1 + 2 * currentLocation * spline.Section.Count)] = currentLine + 3;

        for (int j = 0; j < 2; j++)
        {
            spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation + j][^1].X;
            spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation + j][^1].Y;
            spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation + j][^1].Z;
        
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation + j][^1].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation + j][^1].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation + j][^1].Z;
        
            spline.VBOdata[currentLine * 11 + 9] = 0;
            spline.VBOdata[currentLine * 11 + 10] = j;
        
            ++currentLine;
        }
        
        for (int j = 0; j < 2; j++)
        {
            spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation + j][0].X;
            spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation + j][0].Y;
            spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation + j][0].Z;
        
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation + j][0].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation + j][0].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation + j][0].Z;
        
            spline.VBOdata[currentLine * 11 + 9] = 1;
            spline.VBOdata[currentLine * 11 + 10] = j;
        
            ++currentLine;
        }
    }

    private void FillBottomVbo(ref Components.Spline spline, ref int currentLine)
    {
        for (int i = 0; i < spline.Section.Count; i++)
        {
            spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[0][i].X;
            spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[0][i].Y;
            spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[0][i].Z;
            
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[0][0].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[0][0].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[0][0].Z;
            
            spline.VBOdata[currentLine * 11 + 9] = spline.TexCoords[i].X;
            spline.VBOdata[currentLine * 11 + 10] = spline.TexCoords[i].Y;

            spline.Indexes[i] = currentLine;

            ++currentLine;
        }
    }
    
    private void RecalculateNormals(ref Components.Spline spline)
    {
        int currentLine = 0;
        FillBottomVboNormals(ref spline, ref currentLine);

        if (_shoothNormals)
        {
            for (int i = 0; i < spline.Path.Count - 1; i++)
            {
                FillCenterVboSmoothNormals(ref spline, i, ref currentLine);
            }
        }
        else
        {
            for (int i = 0; i < spline.Path.Count - 1; i++)
            {
                FillCenterVboNormals(ref spline, i, ref currentLine);
            }
        }

        FillTopVboNormals(ref spline, ref currentLine);
    }

    private void FillTopVboNormals(ref Components.Spline spline, ref int currentLine)
    {
        for (int i = 0; i < spline.Section.Count; i++)
        {
            spline.VBOdata[currentLine * 11 + 3] = _shoothNormals ? spline.SmoothNormals[^1][i].X : spline.Normals[^1][0].X;
            spline.VBOdata[currentLine * 11 + 4] = _shoothNormals ? spline.SmoothNormals[^1][i].Y : spline.Normals[^1][0].Y;
            spline.VBOdata[currentLine * 11 + 5] = _shoothNormals ? spline.SmoothNormals[^1][i].Z : spline.Normals[^1][0].Z;

            ++currentLine;
        }
    }
    
    private void FillCenterVboNormals(ref Components.Spline spline, int currentLocation, ref int currentLine)
    {
        for (int i = 0; i < spline.Section.Count - 1; i++)
        {
            for (int k = 0; k < 4; k++)
            {
                spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][i].X;
                spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][i].Y;
                spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][i].Z;
    
                ++currentLine;
            }
        }
        
        for (int k = 0; k < 4; k++)
        {
            spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][^1].X;
            spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][^1].Y;
            spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][^1].Z;
        
            ++currentLine;
        }
    }

    private void FillCenterVboSmoothNormals(ref Components.Spline spline, int currentLocation, ref int currentLine)
    {
        for (int i = 0; i < spline.Section.Count - 1; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                for (int j = 0; j < 2; j++)
                {
                    spline.VBOdata[currentLine * 11 + 3] = spline.SmoothNormals[currentLocation + j][i + k].X;
                    spline.VBOdata[currentLine * 11 + 4] = spline.SmoothNormals[currentLocation + j][i + k].Y;
                    spline.VBOdata[currentLine * 11 + 5] = spline.SmoothNormals[currentLocation + j][i + k].Z;
    
                    ++currentLine;
                }
            }
        }
        
        for (int j = 0; j < 2; j++)
        {
            spline.VBOdata[currentLine * 11 + 3] = spline.SmoothNormals[currentLocation + j][^1].X;
            spline.VBOdata[currentLine * 11 + 4] = spline.SmoothNormals[currentLocation + j][^1].Y;
            spline.VBOdata[currentLine * 11 + 5] = spline.SmoothNormals[currentLocation + j][^1].Z;
    
            ++currentLine;
        }
        
        for (int j = 0; j < 2; j++)
        {
            spline.VBOdata[currentLine * 11 + 3] = spline.SmoothNormals[currentLocation + j][0].X;
            spline.VBOdata[currentLine * 11 + 4] = spline.SmoothNormals[currentLocation + j][0].Y;
            spline.VBOdata[currentLine * 11 + 5] = spline.SmoothNormals[currentLocation + j][0].Z;
    
            ++currentLine;
        }
    }
    
    private void FillBottomVboNormals(ref Components.Spline spline, ref int currentLine)
    {
        for (int i = 0; i < spline.Section.Count; i++)
        {
            spline.VBOdata[currentLine * 11 + 3] = _shoothNormals ? spline.SmoothNormals[0][i].X : spline.Normals[0][0].X;
            spline.VBOdata[currentLine * 11 + 4] = _shoothNormals ? spline.SmoothNormals[0][i].Y : spline.Normals[0][0].Y;
            spline.VBOdata[currentLine * 11 + 5] = _shoothNormals ? spline.SmoothNormals[0][i].Z : spline.Normals[0][0].Z;

            ++currentLine;
        }
    }
}