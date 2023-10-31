using System.Numerics;
using System.Runtime.InteropServices;
using _3D_visualization.Model.Environment;
using _3D_visualization.Model.Events;
using _3D_visualization.Model.SystemComponents.Render;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.WPF;

namespace _3D_visualization.Model.SystemComponents.Spline.Systems;

public class SplineRenderSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] OpenGLControl _openGlControl;
    [EcsInject] ShaderManager _shaderManager;
    [EcsInject] GameplayEventsListener _eventsListener;

    EcsPool<Components.Spline> _splineComponents;

    EcsFilter _splineFilter;

    private bool _drawNormals = false;
    private bool _showTextures = false;
    private bool _shoothNormals = false;

    public void Init(IEcsSystems systems)
    {
        _eventsListener.OnDrawNormalsEvent += drawNormals => _drawNormals = drawNormals;
        _eventsListener.OnTextureEnableEvent += showTextures => _showTextures = showTextures;

        _eventsListener.OnShowWireFrameEvent += useWireframeMode =>
        {
            if (useWireframeMode)
            {
                OpenGL gl = _openGlControl.OpenGL;
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
            }
            else
            {
                OpenGL gl = _openGlControl.OpenGL;
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
            }
        };

        _eventsListener.OnSmoothNormalsEnableEvent += smoothNormals =>
        {
            _shoothNormals = smoothNormals;

            foreach (var splineEntityId in _splineFilter)
            {
                ref Components.Spline spline = ref _splineComponents.Get(splineEntityId);

                OpenGL gl = _openGlControl.OpenGL;
                
                gl.BindVertexArray(spline.SplineVAO);
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, spline.SplineVBO);
                
                IntPtr ptr1 = GCHandle.Alloc(spline.VBOdata, GCHandleType.Pinned).AddrOfPinnedObject();
                gl.BufferSubData(OpenGL.GL_ARRAY_BUFFER, 0, spline.VBOdata.Length * sizeof(float), ptr1);
                
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
                gl.BindVertexArray(0);
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

            if (spline.SplineVAO == 0)
            {
                InitializeSplineVao(ref spline);
            }

            _shaderManager.UseSuperRealisticShader(_showTextures);
            DrawSpline(ref spline, _openGlControl.OpenGL);

            _shaderManager.UseDefaultShader();

            if (_drawNormals)
            {
                DrawNormals(ref spline, _openGlControl.OpenGL);
            }

            DebugPath(ref spline, _openGlControl.OpenGL);
        }
    }

    private void InitializeSplineVao(ref Components.Spline spline)
    {
        OpenGL gl = _openGlControl.OpenGL;

        uint[] splineVBO = new uint[1];
        uint[] splineVAO = new uint[1];
        uint[] splineEBO = new uint[1];

        gl.GenVertexArrays(1, splineVBO);
        gl.GenBuffers(1, splineVAO);
        gl.GenBuffers(1, splineEBO);

        gl.BindVertexArray(splineVAO[0]);

        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, splineVBO[0]);
        IntPtr ptr1 = GCHandle.Alloc(spline.VBOdata, GCHandleType.Pinned).AddrOfPinnedObject();

        gl.BufferData(OpenGL.GL_ARRAY_BUFFER, spline.VBOdata.Length * sizeof(float), ptr1, OpenGL.GL_STATIC_DRAW);

        gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, splineEBO[0]);
        IntPtr ptr2 = GCHandle.Alloc(spline.Indexes, GCHandleType.Pinned).AddrOfPinnedObject();

        gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, spline.Indexes.Length * sizeof(int), ptr2, OpenGL.GL_STATIC_DRAW);

        gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 11 * sizeof(float), IntPtr.Zero);
        gl.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, 11 * sizeof(float), new IntPtr(sizeof(float) * 3));
        gl.VertexAttribPointer(2, 3, OpenGL.GL_FLOAT, false, 11 * sizeof(float), new IntPtr(sizeof(float) * 6));
        gl.VertexAttribPointer(3, 2, OpenGL.GL_FLOAT, false, 11 * sizeof(float), new IntPtr(sizeof(float) * 9));

        gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
        gl.BindVertexArray(0);
        gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, 0);

        spline.SplineVBO = splineVBO[0];
        spline.SplineVAO = splineVAO[0];
        spline.SplineEBO = splineEBO[0];
    }

    private void DrawSpline(ref Components.Spline spline, OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.BindVertexArray(spline.SplineVAO);
        openGl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, spline.SplineEBO);

        openGl.EnableVertexAttribArray(0);
        openGl.EnableVertexAttribArray(1);
        openGl.EnableVertexAttribArray(2);
        openGl.EnableVertexAttribArray(3);

        openGl.DrawElements(OpenGL.GL_TRIANGLES, spline.Indexes.Length - 2 * spline.Section.Count(),
            OpenGL.GL_UNSIGNED_INT, new IntPtr(sizeof(float) * spline.Section.Count()));
        openGl.DrawElements(OpenGL.GL_POLYGON, spline.Section.Count(), OpenGL.GL_UNSIGNED_INT, IntPtr.Zero);
        openGl.DrawElements(OpenGL.GL_POLYGON, spline.Section.Count(), OpenGL.GL_UNSIGNED_INT,
            new IntPtr(sizeof(float) * (spline.Indexes.Length - spline.Section.Count())));

        openGl.DisableVertexAttribArray(0);
        openGl.DisableVertexAttribArray(1);
        openGl.DisableVertexAttribArray(2);
        openGl.DisableVertexAttribArray(3);

        openGl.BindVertexArray(0);
        openGl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, 0);

        openGl.PopMatrix();
    }

    private void DrawNormals(ref Components.Spline spline, OpenGL openGl)
    {
        DrawBottomNormal(ref spline, openGl);

        if (_shoothNormals)
        {
            for (int i = 1; i < spline.Path.Count - 1; i++)
            {
                DrawCenterSmoothNormals(ref spline, openGl, i);
            }
        }
        else
        {
            for (int i = 0; i < spline.Path.Count - 1; i++)
            {
                DrawCenterNormals(ref spline, openGl, i);
            }
        }

        DrawTopNormal(ref spline, openGl);
    }

    private void DrawTopNormal(ref Components.Spline spline, OpenGL openGl)
    {
        for (int i = 0; i < spline.Section.Count(); i++)
        {
            openGl.PushMatrix();

            openGl.Translate(spline.PointsLocation[^1][i].X, spline.PointsLocation[^1][i].Y,
                spline.PointsLocation[^1][i].Z);

            openGl.Begin(OpenGL.GL_LINES);

            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(0.0f, 0.0f, 0.0f);

            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(
                _shoothNormals ? spline.SmoothNormals[^1][i].X : spline.Normals[^1][0].X,
                _shoothNormals ? spline.SmoothNormals[^1][i].Y : spline.Normals[^1][0].Y,
                _shoothNormals ? spline.SmoothNormals[^1][i].Z : spline.Normals[^1][0].Z);

            openGl.End();

            openGl.PopMatrix();
        }
    }

    private void DrawCenterSmoothNormals(ref Components.Spline spline, OpenGL openGl, int currentLocation)
    {
        openGl.Begin(OpenGL.GL_LINES);
        openGl.PushMatrix();

        for (int i = 0; i < spline.Section.Count() - 1; i++)
        {
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(spline.PointsLocation[currentLocation][i].X, spline.PointsLocation[currentLocation][i].Y,
                spline.PointsLocation[currentLocation][i].Z);

            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(spline.PointsLocation[currentLocation][i].X + spline.SmoothNormals[currentLocation][i].X,
                spline.PointsLocation[currentLocation][i].Y + spline.SmoothNormals[currentLocation][i].Y,
                spline.PointsLocation[currentLocation][i].Z + spline.SmoothNormals[currentLocation][i].Z);
        }

        openGl.Color(0.0f, 0.0f, 1.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation][^1].X, spline.PointsLocation[currentLocation][^1].Y,
            spline.PointsLocation[currentLocation][^1].Z);

        openGl.Color(0.0f, 1.0f, 0.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation][^1].X + spline.SmoothNormals[currentLocation][^1].X,
            spline.PointsLocation[currentLocation][^1].Y + spline.SmoothNormals[currentLocation][^1].Y,
            spline.PointsLocation[currentLocation][^1].Z + spline.SmoothNormals[currentLocation][^1].Z);

        openGl.PopMatrix();

        openGl.End();
    }

    private void DrawCenterNormals(ref Components.Spline spline, OpenGL openGl, int currentLocation)
    {
        openGl.Begin(OpenGL.GL_LINES);
        openGl.PushMatrix();

        for (int i = 0; i < spline.Section.Count() - 1; i++)
        {
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(spline.PointsLocation[currentLocation][i].X, spline.PointsLocation[currentLocation][i].Y,
                spline.PointsLocation[currentLocation][i].Z);

            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(spline.PointsLocation[currentLocation][i].X + spline.Normals[currentLocation + 1][i].X,
                spline.PointsLocation[currentLocation][i].Y + spline.Normals[currentLocation + 1][i].Y,
                spline.PointsLocation[currentLocation][i].Z + spline.Normals[currentLocation + 1][i].Z);

            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(spline.PointsLocation[currentLocation + 1][i].X,
                spline.PointsLocation[currentLocation + 1][i].Y, spline.PointsLocation[currentLocation + 1][i].Z);

            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(spline.PointsLocation[currentLocation + 1][i].X + spline.Normals[currentLocation + 1][i].X,
                spline.PointsLocation[currentLocation + 1][i].Y + spline.Normals[currentLocation + 1][i].Y,
                spline.PointsLocation[currentLocation + 1][i].Z + spline.Normals[currentLocation + 1][i].Z);

            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(spline.PointsLocation[currentLocation][i + 1].X,
                spline.PointsLocation[currentLocation][i + 1].Y, spline.PointsLocation[currentLocation][i + 1].Z);

            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(spline.PointsLocation[currentLocation][i + 1].X + spline.Normals[currentLocation + 1][i].X,
                spline.PointsLocation[currentLocation][i + 1].Y + spline.Normals[currentLocation + 1][i].Y,
                spline.PointsLocation[currentLocation][i + 1].Z + spline.Normals[currentLocation + 1][i].Z);

            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(spline.PointsLocation[currentLocation + 1][i + 1].X,
                spline.PointsLocation[currentLocation + 1][i + 1].Y,
                spline.PointsLocation[currentLocation + 1][i + 1].Z);

            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(
                spline.PointsLocation[currentLocation + 1][i + 1].X + spline.Normals[currentLocation + 1][i].X,
                spline.PointsLocation[currentLocation + 1][i + 1].Y + spline.Normals[currentLocation + 1][i].Y,
                spline.PointsLocation[currentLocation + 1][i + 1].Z + spline.Normals[currentLocation + 1][i].Z);
        }

        openGl.Color(0.0f, 0.0f, 1.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation][0].X, spline.PointsLocation[currentLocation][0].Y,
            spline.PointsLocation[currentLocation][0].Z);

        openGl.Color(0.0f, 1.0f, 0.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation][0].X + spline.Normals[currentLocation + 1][^1].X,
            spline.PointsLocation[currentLocation][0].Y + spline.Normals[currentLocation + 1][^1].Y,
            spline.PointsLocation[currentLocation][0].Z + spline.Normals[currentLocation + 1][^1].Z);

        openGl.Color(0.0f, 0.0f, 1.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation + 1][0].X, spline.PointsLocation[currentLocation + 1][0].Y,
            spline.PointsLocation[currentLocation + 1][0].Z);

        openGl.Color(0.0f, 1.0f, 0.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation + 1][0].X + spline.Normals[currentLocation + 1][^1].X,
            spline.PointsLocation[currentLocation + 1][0].Y + spline.Normals[currentLocation + 1][^1].Y,
            spline.PointsLocation[currentLocation + 1][0].Z + spline.Normals[currentLocation + 1][^1].Z);

        openGl.Color(0.0f, 0.0f, 1.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation][^1].X, spline.PointsLocation[currentLocation][^1].Y,
            spline.PointsLocation[currentLocation][^1].Z);

        openGl.Color(0.0f, 1.0f, 0.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation][^1].X + spline.Normals[currentLocation + 1][^1].X,
            spline.PointsLocation[currentLocation][^1].Y + spline.Normals[currentLocation + 1][^1].Y,
            spline.PointsLocation[currentLocation][^1].Z + spline.Normals[currentLocation + 1][^1].Z);

        openGl.Color(0.0f, 0.0f, 1.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation + 1][^1].X,
            spline.PointsLocation[currentLocation + 1][^1].Y, spline.PointsLocation[currentLocation + 1][^1].Z);

        openGl.Color(0.0f, 1.0f, 0.0f);
        openGl.Vertex(spline.PointsLocation[currentLocation + 1][^1].X + spline.Normals[currentLocation + 1][^1].X,
            spline.PointsLocation[currentLocation + 1][^1].Y + spline.Normals[currentLocation + 1][^1].Y,
            spline.PointsLocation[currentLocation + 1][^1].Z + spline.Normals[currentLocation + 1][^1].Z);

        openGl.PopMatrix();

        openGl.End();
    }

    private void DrawBottomNormal(ref Components.Spline spline, OpenGL openGl)
    {
        for (int i = 0; i < spline.Section.Count(); i++)
        {
            openGl.PushMatrix();

            openGl.Translate(spline.PointsLocation[0][i].X, spline.PointsLocation[0][i].Y,
                spline.PointsLocation[0][i].Z);

            openGl.Begin(OpenGL.GL_LINES);

            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(0.0f, 0.0f, 0.0f);

            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(
                _shoothNormals ? spline.SmoothNormals[0][i].X : spline.Normals[0][0].X,
                _shoothNormals ? spline.SmoothNormals[0][i].Y : spline.Normals[0][0].Y,
                _shoothNormals ? spline.SmoothNormals[0][i].Z : spline.Normals[0][0].Z);

            openGl.End();

            openGl.PopMatrix();
        }
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