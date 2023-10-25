using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.Spline.Systems;

public class SplineRenderSystem: IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] OpenGLControl _openGlControl;
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
            RenderSpline(ref spline, _openGlControl.OpenGL);
        }
    }

    private void RenderSpline(ref Components.Spline spline, OpenGL openGl)
    {
        DebugPath(ref spline, openGl);
        DrawBottom(ref spline, openGl);
        
        for (int i = 0; i < spline.Path.Count - 1; i++)
        {
            DrawCenter(ref spline, openGl, i);
        }
        
        DrawTop(ref spline, openGl);
    }

    private void DrawTop(ref Components.Spline spline, OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_POLYGON);
        
        for (int i = 0; i < spline.PointLocations[^1].Count; i++)
        {
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(spline.PointLocations[^1][i].X, spline.PointLocations[^1][i].Y, spline.PointLocations[^1][i].Z);
        }
        
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawCenter(ref Components.Spline spline, OpenGL openGl, int currentLocation)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_QUAD_STRIP);

        for (int i = 0; i < spline.Section.Count; i++)
        {
            openGl.Color(0.5f, 0.5f, 0f);
            openGl.Vertex(spline.PointLocations[currentLocation][i].X, spline.PointLocations[currentLocation][i].Y, spline.PointLocations[currentLocation][i].Z);
            
            openGl.Color(0.0f, 0.5f, 0.5f);
            openGl.Vertex(spline.PointLocations[currentLocation + 1][i].X, spline.PointLocations[currentLocation + 1][i].Y, spline.PointLocations[currentLocation + 1][i].Z);
        }
        
        openGl.Color(0.5f, 0.5f, 0f);
        openGl.Vertex(spline.PointLocations[currentLocation][0].X, spline.PointLocations[currentLocation][0].Y, spline.PointLocations[currentLocation][0].Z);
            
        openGl.Color(0.0f, 0.5f, 0.5f);
        openGl.Vertex(spline.PointLocations[currentLocation + 1][0].X, spline.PointLocations[currentLocation + 1][0].Y, spline.PointLocations[currentLocation + 1][0].Z);
        
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawBottom(ref Components.Spline spline, OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_POLYGON);
        
        for (int i = 0; i < spline.PointLocations[0].Count; i++)
        {
            openGl.Color(1.0f, 0.0f, 0.0f);
            openGl.Vertex(spline.PointLocations[0][i].X, spline.PointLocations[0][i].Y, spline.PointLocations[0][i].Z);
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