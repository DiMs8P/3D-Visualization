﻿using System.Numerics;
using _3D_visualization.Model.Environment;
using _3D_visualization.Model.Utils;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace _3D_visualization.Model.SystemComponents.Spline.Systems;

// TODO refactor all that connected with spline (i won't)
public class SplineTransformSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] AutoMapper2D _autoMapper2D;
    
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
            if (spline.PointsLocation.Count == 0)
            {
                AllocateMemory(ref spline);
                
                UpdateSplinePointsLocation(ref spline);
                UpdateSplinePointsNormals(ref spline);
                UpdateSplinePointsColors(ref spline);
                UpdateSplinePointsTexCoords(ref spline);

                FillVboData(ref spline);
            }
        }
    }

    private void AllocateMemory(ref Components.Spline spline)
    {
        for (int i = 0; i < spline.Path.Count; i++)
        {
            spline.PointsLocation.Add(new List<Vector3>());
            spline.Normals.Add(new List<Vector3>());
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
            InitializeCenterLocations(ref spline, i + 1, direction, nextDirection);
            
            direction = spline.Path[i] - spline.Path[i + 1];
            nextDirection = spline.Path[i + 1] - spline.Path[i + 2];
        }
        
        InitializeTopLocations(ref spline, direction, nextDirection);
    }
    
    private void InitializeBottomLocations(ref Components.Spline spline, Vector3 direction, Vector3 nextDirection)
    {
        Vector3 crossVector = Vector3.Cross(direction, nextDirection);
        
        float zAngle = MathHelper.CalculateAngle(Vector3.UnitZ, direction with { Y = 0 });
        float xAngle = MathHelper.CalculateAngle(direction with { Y = 0 }, direction);
        float yAngle = MathHelper.CalculateAngle(crossVector with { Y = 0 }, crossVector);
        
        for (int i = 0; i < spline.Section.Count; i++)
        {
            Vector3 point = new Vector3(spline.Section[i].X, spline.Section[i].Y, 0);
            point = MathHelper.RotateY(point, zAngle);
            point = MathHelper.RotateX(point, xAngle);
            point = MathHelper.RotateZ(point, yAngle);
            point.X += spline.Path[0].X;
            point.Y += spline.Path[0].Y;
            point.Z += spline.Path[0].Z;
            
            spline.PointsLocation[0][i] = point;
            
            /*
            spline.VBOdata[i * 11] = point.X;
            spline.VBOdata[i * 11 + 1] = point.Y;
            spline.VBOdata[i * 11 + 2] = point.Z;*/
        }
    }

    private void InitializeCenterLocations(ref Components.Spline spline, int currentLocation, Vector3 direction, Vector3 nextDirection)
    {
        Vector3 crossVector = Vector3.Cross(direction, nextDirection);
        
        float zAngle = MathHelper.CalculateAngle(Vector3.UnitZ, direction with { Y = 0 });
        float xAngle = MathHelper.CalculateAngle(direction with { Y = 0 }, direction);
        float yAngle = MathHelper.CalculateAngle(crossVector with { Y = 0 }, crossVector);
        float directionsAngle = MathHelper.CalculateAngle(direction, nextDirection);
        
        for (int i = 0; i < spline.Section.Count; i++)
        {
            Vector3 point = new Vector3(spline.Section[i].X, spline.Section[i].Y, 0);
            point = MathHelper.RotateY(point, zAngle);
            point = MathHelper.RotateX(point, xAngle);
            point = MathHelper.RotateZ(point, yAngle + directionsAngle / 2);
            point.X += spline.Path[currentLocation].X;
            point.Y += spline.Path[currentLocation].Y;
            point.Z += spline.Path[currentLocation].Z;
            
            spline.PointsLocation[currentLocation][i] = point;
        }
    }
    
    private void InitializeTopLocations(ref Components.Spline spline, Vector3 direction, Vector3 nextDirection)
    {
        Vector3 crossVector = Vector3.Cross(direction, nextDirection);
        
        float zAngle = MathHelper.CalculateAngle(Vector3.UnitZ, direction with { Y = 0 });
        float xAngle = MathHelper.CalculateAngle(direction with { Y = 0 }, direction);
        float yAngle = MathHelper.CalculateAngle(crossVector with { Y = 0 }, crossVector);
        
        for (int i = 0; i < spline.Section.Count; i++)
        {
            Vector3 point = new Vector3(spline.Section[i].X, spline.Section[i].Y, 0);
            point = MathHelper.RotateY(point, zAngle);
            point = MathHelper.RotateX(point, xAngle);
            point = MathHelper.RotateZ(point, yAngle);
            point.X += spline.Path[^1].X;
            point.Y += spline.Path[^1].Y;
            point.Z += spline.Path[^1].Z;
            
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
    private void InitializeBottomNormals(ref Components.Spline spline)
    {
        spline.Normals[0].Add(spline.Path[0] - spline.Path[1]);
    }
    
    private void InitializeCenterNormals(ref Components.Spline spline, int currentLocation)
    {
        Vector3 a, b;
        for (int i = 1; i < spline.Section.Count; i++)
        {
            a = spline.PointsLocation[currentLocation][i - 1] - spline.PointsLocation[currentLocation][i];
            b = spline.PointsLocation[currentLocation + 1][i] - spline.PointsLocation[currentLocation][i];
            
            spline.Normals[currentLocation + 1].Add(Vector3.Normalize(Vector3.Cross(a, b)));
        }
        
        a = spline.PointsLocation[currentLocation][^1] - spline.PointsLocation[currentLocation][0];
        b = spline.PointsLocation[currentLocation + 1][0] - spline.PointsLocation[currentLocation][0];
            
        spline.Normals[currentLocation + 1].Add(Vector3.Normalize(Vector3.Cross(a, b)));
    }
    
    private void InitializeTopNormals(ref Components.Spline spline)
    {
        spline.Normals[^1].Add(spline.Path[^1] - spline.Path[^2]);
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
        
        for (int i = 1; i < spline.Path.Count - 1; i++)
        {
            InitializeCenterTexCoords(ref spline, i);
        }
        
        InitializeTopTexCoords(ref spline);
    }

    private void InitializeTopTexCoords(ref Components.Spline spline)
    {
    }
    
    private void InitializeCenterTexCoords(ref Components.Spline spline, int currentLocation)
    {
        
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
            
            spline.VBOdata[currentLine * 11 + 3] = spline.Normals[^1][0].X;
            spline.VBOdata[currentLine * 11 + 4] = spline.Normals[^1][0].Y;
            spline.VBOdata[currentLine * 11 + 5] = spline.Normals[^1][0].Z;
            
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[^1][0].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[^1][0].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[^1][0].Z;
            
            spline.VBOdata[currentLine * 11 + 9] = spline.TexCoords[i].X;
            spline.VBOdata[currentLine * 11 + 10] = spline.TexCoords[i].Y;

            ++currentLine;
        }
    }
    
    private void FillCenterVbo(ref Components.Spline spline, int currentLocation, ref int currentLine)
    {
        for (int i = 0; i < spline.Section.Count - 1; i++)
        {
            spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation][i].X;
            spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation][i].Y;
            spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation][i].Z;
            
            spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][i].X;
            spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][i].Y;
            spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][i].Z;
            
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation][i].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation][i].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation][i].Z;
            
            spline.VBOdata[currentLine * 11 + 9] = 0;
            spline.VBOdata[currentLine * 11 + 10] = 0;

            ++currentLine;
            
            spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation + 1][i].X;
            spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation + 1][i].Y;
            spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation + 1][i].Z;
            
            spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][i].X;
            spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][i].Y;
            spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][i].Z;
            
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation + 1][i].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation + 1][i].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation + 1][i].Z;
            
            spline.VBOdata[currentLine * 11 + 9] = 0;
            spline.VBOdata[currentLine * 11 + 10] = 1;

            ++currentLine;
            
            spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation][i + 1].X;
            spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation][i + 1].Y;
            spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation][i + 1].Z;
            
            spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][i].X;
            spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][i].Y;
            spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][i].Z;
            
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation][i + 1].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation][i + 1].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation][i + 1].Z;
            
            spline.VBOdata[currentLine * 11 + 9] = 1;
            spline.VBOdata[currentLine * 11 + 10] = 0;

            ++currentLine;
            
            spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation + 1][i + 1].X;
            spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation + 1][i + 1].Y;
            spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation + 1][i + 1].Z;
            
            spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][i].X;
            spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][i].Y;
            spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][i].Z;
            
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation + 1][i + 1].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation + 1][i + 1].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation + 1][i + 1].Z;
            
            spline.VBOdata[currentLine * 11 + 9] = 1;
            spline.VBOdata[currentLine * 11 + 10] = 1;

            ++currentLine;
        }
        
        spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation][^1].X;
        spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation][^1].Y;
        spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation][^1].Z;
        
        spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][^1].X;
        spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][^1].Y;
        spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][^1].Z;
        
        spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation][^1].X;
        spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation][^1].Y;
        spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation][^1].Z;
        
        spline.VBOdata[currentLine * 11 + 9] = 0;
        spline.VBOdata[currentLine * 11 + 10] = 0;
        
        ++currentLine;
        
        spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation + 1][^1].X;
        spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation + 1][^1].Y;
        spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation + 1][^1].Z;
        
        spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][^1].X;
        spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][^1].Y;
        spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][^1].Z;
        
        spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation + 1][^1].X;
        spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation + 1][^1].Y;
        spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation + 1][^1].Z;
        
        spline.VBOdata[currentLine * 11 + 9] = 0;
        spline.VBOdata[currentLine * 11 + 10] = 1;
        
        ++currentLine;
        
        spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation][0].X;
        spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation][0].Y;
        spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation][0].Z;
        
        spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][^1].X;
        spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][^1].Y;
        spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][^1].Z;
        
        spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation][0].X;
        spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation][0].Y;
        spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation][0].Z;
        
        spline.VBOdata[currentLine * 11 + 9] = 1;
        spline.VBOdata[currentLine * 11 + 10] = 0;
        
        ++currentLine;
        
        spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[currentLocation + 1][0].X;
        spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[currentLocation + 1][0].Y;
        spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[currentLocation + 1][0].Z;
        
        spline.VBOdata[currentLine * 11 + 3] = spline.Normals[currentLocation + 1][^1].X;
        spline.VBOdata[currentLine * 11 + 4] = spline.Normals[currentLocation + 1][^1].Y;
        spline.VBOdata[currentLine * 11 + 5] = spline.Normals[currentLocation + 1][^1].Z;
        
        spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[currentLocation + 1][0].X;
        spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[currentLocation + 1][0].Y;
        spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[currentLocation + 1][0].Z;
        
        spline.VBOdata[currentLine * 11 + 9] = 1;
        spline.VBOdata[currentLine * 11 + 10] = 1;
        
        ++currentLine;
    }

    private void FillBottomVbo(ref Components.Spline spline, ref int currentLine)
    {
        for (int i = 0; i < spline.Section.Count; i++)
        {
            spline.VBOdata[currentLine * 11    ] = spline.PointsLocation[0][i].X;
            spline.VBOdata[currentLine * 11 + 1] = spline.PointsLocation[0][i].Y;
            spline.VBOdata[currentLine * 11 + 2] = spline.PointsLocation[0][i].Z;
            
            spline.VBOdata[currentLine * 11 + 3] = spline.Normals[0][0].X;
            spline.VBOdata[currentLine * 11 + 4] = spline.Normals[0][0].Y;
            spline.VBOdata[currentLine * 11 + 5] = spline.Normals[0][0].Z;
            
            spline.VBOdata[currentLine * 11 + 6] = spline.PointsColor[0][0].X;
            spline.VBOdata[currentLine * 11 + 7] = spline.PointsColor[0][0].Y;
            spline.VBOdata[currentLine * 11 + 8] = spline.PointsColor[0][0].Z;
            
            spline.VBOdata[currentLine * 11 + 9] = spline.TexCoords[i].X;
            spline.VBOdata[currentLine * 11 + 10] = spline.TexCoords[i].Y;

            ++currentLine;
        }
    }
}