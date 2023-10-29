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

            if (spline.SplineVAO == 0)
            {
                InitializeSplineVao(ref spline);
            }
            
            _shaderManager.UseSplineShader();
            DrawSpline(ref spline, _openGlControl.OpenGL);
            
            _shaderManager.UseDefaultShader();
            DebugPath(ref spline, _openGlControl.OpenGL);
            DrawNormals(ref spline, _openGlControl.OpenGL);
        }
    }

    private void InitializeSplineVao(ref Components.Spline spline)
    {
        OpenGL gl = _openGlControl.OpenGL;
        
        uint[] splineVBO = new uint[1];
        uint[] splineVAO = new uint[1];
        
        gl.GenVertexArrays(1, splineVBO);
        gl.GenBuffers(1, splineVAO);
        
        gl.BindVertexArray(splineVAO[0]);
        
        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, splineVBO[0]);
        gl.BufferData(OpenGL.GL_ARRAY_BUFFER, spline.VBOdata, OpenGL.GL_STATIC_DRAW);
        
        gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 11 * sizeof(float), IntPtr.Zero);
        gl.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, 11 * sizeof(float), new IntPtr(sizeof(float) * 3));
        gl.VertexAttribPointer(2, 3, OpenGL.GL_FLOAT, false, 11 * sizeof(float), new IntPtr(sizeof(float) * 6));
        gl.VertexAttribPointer(3, 2, OpenGL.GL_FLOAT, false, 11 * sizeof(float), new IntPtr(sizeof(float) * 9));
        
        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
        gl.BindVertexArray(0);

        spline.SplineVAO = splineVAO[0];
    }

    private void DrawSpline(ref Components.Spline spline, OpenGL openGl)
    {
        //DrawBottomSpline(ref spline, openGl);
        
        DrawCenterSpline(ref spline, openGl);
        
        //DrawTopSpline(ref spline, openGl);
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
            openGl.Vertex(spline.Normals[^1][0].X, spline.Normals[^1][0].Y, spline.Normals[^1][0].Z);
            
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawCenterNormals(ref Components.Spline spline, OpenGL openGl, int currentLocation)
    {
        openGl.PushMatrix();

        Vector3 pointToDrawFrom = (spline.Path[currentLocation] + spline.Path[currentLocation + 1]) / 2;
        openGl.Translate(pointToDrawFrom.X, pointToDrawFrom.Y, pointToDrawFrom.Z);

        openGl.Begin(OpenGL.GL_LINES);

        for (int i = 0; i < spline.Section.Count(); i++)
        {
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(0.0f, 0.0f, 0.0f);
            
            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(spline.Normals[currentLocation + 1][i].X, spline.Normals[currentLocation + 1][i].Y, spline.Normals[currentLocation + 1][i].Z);
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
            openGl.Vertex(spline.Normals[0][0].X, spline.Normals[0][0].Y, spline.Normals[0][0].Z);
            
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

    private void DrawCenterSpline(ref Components.Spline spline, OpenGL openGl)
    {
        OpenGL gl = _openGlControl.OpenGL;
        gl.PushMatrix();
        
        gl.EnableVertexAttribArray(0);
        gl.EnableVertexAttribArray(1);
        gl.EnableVertexAttribArray(2);
        gl.EnableVertexAttribArray(3);
        
        _shaderManager.UseSplineShader();
        gl.BindVertexArray(spline.SplineVAO);
        gl.DrawArrays(OpenGL.GL_QUADS, 3, 27);
        
        gl.DisableVertexAttribArray(0);
        gl.DisableVertexAttribArray(1);
        gl.DisableVertexAttribArray(2);
        gl.DisableVertexAttribArray(3);
        
        gl.PopMatrix();
    }

    private void DrawBottomSpline(ref Components.Spline spline, OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_POLYGON);
        
        for (int i = 0; i < spline.PointsLocation[0].Count; i++)
        {
            openGl.Color(0.0f, 0.0f, 1.0f);
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