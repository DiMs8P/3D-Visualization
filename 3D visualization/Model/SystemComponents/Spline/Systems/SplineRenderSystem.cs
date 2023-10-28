using System.Numerics;
using _3D_visualization.Model.Environment;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.Spline.Systems;

public class SplineRenderSystem: IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] OpenGLControl _openGlControl;
    [EcsInject] private ShaderManager _shaderManager;
    
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
            _shaderManager.UseSplineShader();
            DrawSpline(ref spline, _openGlControl.OpenGL);
            
            _shaderManager.UseDefaultShader();
            DebugPath(ref spline, _openGlControl.OpenGL);
            DrawNormals(ref spline, _openGlControl.OpenGL);
        }
    }

    private void DrawSpline(ref Components.Spline spline, OpenGL openGl)
    {
        DrawBottomSpline(ref spline, openGl);
        
        for (int i = 0; i < spline.Path.Count - 1; i++)
        {
            DrawCenterSpline(ref spline, openGl, i);
        }
        
        DrawTopSpline(ref spline, openGl);
    }

    private void DrawNormals(ref Components.Spline spline, OpenGL openGl)
    {
        DrawBottomNormal(ref spline, openGl);
        
        for (int i = 0; i < spline.Path.Count - 1; i++)
        {
            DrawCenterNormals(ref spline, openGl, i);
        }
        
        DrawTopNormal(ref spline, openGl);
    }

    private void DrawTopNormal(ref Components.Spline spline, OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.Translate(spline.Path[^1].X, spline.Path[^1].Y, spline.Path[^1].Z);
        
        openGl.Begin(OpenGL.GL_LINES);
        
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(0.0f, 0.0f, 0.0f);
                
            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(spline.Normals[^1].X, spline.Normals[^1].Y, spline.Normals[^1].Z);
            
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawCenterNormals(ref Components.Spline spline, OpenGL openGl, int currentLocation)
    {
        openGl.PushMatrix();

        Vector3 pointToDrawFrom = (spline.Path[currentLocation] + spline.Path[currentLocation + 1]) / 2;
        openGl.Translate(pointToDrawFrom.X, pointToDrawFrom.Y, pointToDrawFrom.Z);

        int currentFirstNormalIndex = currentLocation * spline.Section.Count() + 1;
        
        openGl.Begin(OpenGL.GL_LINES);

        for (int i = 0; i < spline.Section.Count(); i++)
        {
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(0.0f, 0.0f, 0.0f);
            
            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(spline.Normals[currentFirstNormalIndex + i].X, spline.Normals[currentFirstNormalIndex + i].Y, spline.Normals[currentFirstNormalIndex + i].Z);
        }
        
        openGl.End();

        openGl.PopMatrix();
    }

    private void DrawBottomNormal(ref Components.Spline spline, OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.Translate(spline.Path[0].X, spline.Path[0].Y, spline.Path[0].Z);
        
        openGl.Begin(OpenGL.GL_LINES);
        
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(0.0f, 0.0f, 0.0f);
            
            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(spline.Normals[0].X, spline.Normals[0].Y, spline.Normals[0].Z);
            
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawTopSpline(ref Components.Spline spline, OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_POLYGON);
        
        for (int i = 0; i < spline.PointsLocation[^1].Count; i++)
        {
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(spline.PointsLocation[^1][i].X, spline.PointsLocation[^1][i].Y, spline.PointsLocation[^1][i].Z);
        }
        
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawCenterSpline(ref Components.Spline spline, OpenGL openGl, int currentLocation)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_QUAD_STRIP);

        for (int i = 0; i < spline.Section.Count; i++)
        {
            openGl.Color(0.5f, 0.5f, 0f);
            openGl.Vertex(spline.PointsLocation[currentLocation][i].X, spline.PointsLocation[currentLocation][i].Y, spline.PointsLocation[currentLocation][i].Z);
            
            openGl.Color(0.0f, 0.5f, 0.5f);
            openGl.Vertex(spline.PointsLocation[currentLocation + 1][i].X, spline.PointsLocation[currentLocation + 1][i].Y, spline.PointsLocation[currentLocation + 1][i].Z);
        }
        
        openGl.Color(0.5f, 0.5f, 0f);
        openGl.Vertex(spline.PointsLocation[currentLocation][0].X, spline.PointsLocation[currentLocation][0].Y, spline.PointsLocation[currentLocation][0].Z);
            
        openGl.Color(0.0f, 0.5f, 0.5f);
        openGl.Vertex(spline.PointsLocation[currentLocation + 1][0].X, spline.PointsLocation[currentLocation + 1][0].Y, spline.PointsLocation[currentLocation + 1][0].Z);
        
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawBottomSpline(ref Components.Spline spline, OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_POLYGON);
        
        for (int i = 0; i < spline.PointsLocation[0].Count; i++)
        {
            openGl.Color(1.0f, 0.0f, 0.0f);
            openGl.Vertex(spline.PointsLocation[0][i].X, spline.PointsLocation[0][i].Y, spline.PointsLocation[0][i].Z);
        }
        
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DebugPath(ref Components.Spline spline, OpenGL openGl)
    {
        openGl.PushMatrix();
        openGl.Begin(OpenGL.GL_LINE_STRIP);

        for (int i = 0; i < spline.Path.Count; i++)
        {
            openGl.Color(1.0f, 1.0f, 1.0f);
            openGl.Vertex(spline.Path[i].X, spline.Path[i].Y, spline.Path[i].Z);
        }

        openGl.End();
        openGl.PopMatrix();
    }
}