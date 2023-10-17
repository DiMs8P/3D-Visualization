using System.Numerics;
using SharpGL;
using SharpGL.SceneGraph;

namespace _3D_visualization.Model.Components.Render;

public class SplineRenderComponent : RenderComponent
{
    private List<Vector2> _section;
    private List<Vector3> _path;
    private List<Vector3> Locations;
    private List<Vector3> Rotations;
    private Vector3 _baseDirection = new Vector3(0, 0, 1);
    public SplineRenderComponent(List<Vector2> section, List<Vector3> path) : base()
    {
        _section = section;
        _path = path;
    }
    public override void Render(OpenGL openGl)
    {
        DebugPath(openGl);
        Vector3 direction = _path[0] - _path[1];
        Vector3 nextDirection = new Vector3(0, 0, 1);

        DrawBottom(openGl, direction);
        
        for (int i = 0; i < _path.Count - 1; i++)
        {
            direction = _path[i] - _path[i + 1];

            if (i == _path.Count - 2)
            {
                nextDirection = direction;
            }
            else
            {
                nextDirection = _path[i + 1] - _path[i + 2];
            }
            
            DrawCenter(openGl, i, direction, nextDirection);
        }
        
        DrawTop(openGl, direction);
    }

    private void DebugPath(OpenGL openGl)
    {
        openGl.PushMatrix();
        openGl.Begin(OpenGL.GL_LINE_STRIP);

        for (int i = 0; i < _path.Count; i++)
        {
            openGl.Color(1.0f, 1.0f, 1.0f);
            openGl.Vertex(_path[i].X, _path[i].Y, _path[i].Z);
        }

        openGl.End();
        openGl.PopMatrix();
    }

    private void DrawCenter(OpenGL openGl, int currentLocation, Vector3 direction, Vector3 nextDirection)
    {
        openGl.PushMatrix();

        float angle = GetAngle(direction, _baseDirection);
        float angleBetweenDirections = GetAngle(direction, nextDirection) / 2;
        Vector3 axis = Vector3.Normalize(Vector3.Cross(direction, _baseDirection));
        openGl.Translate(_path[currentLocation].X, _path[currentLocation].Y, _path[currentLocation].Z);
        openGl.Rotate(-angle, axis.X, axis.Y, axis.Z);
        
        openGl.Begin(OpenGL.GL_TRIANGLE_STRIP);

        foreach (var point in _section)
        {
            openGl.Color(0.0f, 1.0f, 1.0f);
            openGl.Vertex(point.X, point.Y, 0.0f);
            
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(point.X, point.Y, -direction.Length() + point.X * float.Sin(angleBetweenDirections));
        }
        
        openGl.Color(0.0f, 1.0f, 1.0f);
        openGl.Vertex(_section[0].X, _section[0].Y, 0.0f);
            
        openGl.Color(0.0f, 0.0f, 1.0f);
        openGl.Vertex(_section[0].X, _section[0].Y, -direction.Length() + _section[0].X * float.Sin(angleBetweenDirections));
        
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawTop(OpenGL openGl, Vector3 direction)
    {
        openGl.PushMatrix();
        
        float angle = GetAngle(direction, _baseDirection);
        Vector3 axis = Vector3.Normalize(Vector3.Cross(direction, _baseDirection));
        openGl.Translate(_path[^1].X, _path[^1].Y, _path[^1].Z);
        openGl.Rotate(-angle, axis.X, axis.Y, axis.Z);

        openGl.Begin(OpenGL.GL_POLYGON);

        foreach (var point in _section)
        {
            openGl.Color(0.0f, 1.0f, 0.0f);
            openGl.Vertex(point.X, point.Y, 0.0f);
        }
        
        openGl.End();
        openGl.PopMatrix();
    }

    private void DrawBottom(OpenGL openGl, Vector3 direction)
    {
        openGl.PushMatrix();

        float angle = GetAngle(direction, _baseDirection);
        Vector3 axis = Vector3.Normalize(Vector3.Cross(direction, _baseDirection));
        openGl.Translate(_path[0].X, _path[0].Y, _path[0].Z);
        openGl.Rotate(-angle, axis.X, axis.Y, axis.Z);

        openGl.Begin(OpenGL.GL_POLYGON);

        foreach (var point in _section)
        {
            openGl.Color(1.0f, 0.0f, 0.0f);
            openGl.Vertex(point.X, point.Y, 0.0f);
        }
        
        openGl.End();
        
        openGl.PopMatrix();
    }

    private float GetAngle(Vector3 vector1, Vector3 vector2)
    {
        float dotProduct = Vector3.Dot(vector1, vector2);

        float magnitude1 = vector1.Length();
        float magnitude2 = vector2.Length();

        float angleRadians = (float)Math.Acos(dotProduct / (magnitude1 * magnitude2));

        return angleRadians * 180 / (float)Math.PI;
    }
}