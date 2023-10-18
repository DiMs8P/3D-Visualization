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
    private List<List<Vector3>> _pointLocations;
    private Vector3 _baseDirection = new Vector3(0, 0, 1);
    public SplineRenderComponent(List<Vector2> section, List<Vector3> path) : base()
    {
        _section = section;
        _path = path;
        _pointLocations = new List<List<Vector3>>();

        for (int i = 0; i < _path.Count; i++)
        {
            _pointLocations.Add(new List<Vector3>());

            foreach (var point in section)
            {
                _pointLocations[i].Add(new Vector3());
            }
        }

        InitializePoints();
    }

    private void InitializePoints()
    {
        Vector3 direction = _path[0] - _path[1];
        Vector3 nextDirection = _path[1] - _path[2];
        InitializeBottom(direction, nextDirection);
        
        for (int i = 0; i < _path.Count - 2; i++)
        {
            InitializeCenter(i + 1, direction, nextDirection);
            
            direction = _path[i] - _path[i + 1];
            nextDirection = _path[i + 1] - _path[i + 2];
        }
        
        InitializeTop(direction, nextDirection);
    }

    private void InitializeTop(Vector3 direction, Vector3 nextDirection)
    {
        Vector3 crossVector = Vector3.Cross(direction, nextDirection);
        
        float zAngle = CalculateAngle(Vector3.UnitZ, direction with { Y = 0 });
        float xAngle = CalculateAngle(direction with { Y = 0 }, direction);
        float yAngle = CalculateAngle(crossVector with { Y = 0 }, crossVector);
        
        for (int i = 0; i < _section.Count; i++)
        {
            Vector3 point = new Vector3(_section[i].X, _section[i].Y, 0);
            point = RotateY(point, zAngle);
            point = RotateX(point, xAngle);
            point = RotateZ(point, yAngle);
            point.X += _path[^1].X;
            point.Y += _path[^1].Y;
            point.Z += _path[^1].Z;
            
            _pointLocations[^1][i] = point;
        }
    }

    private void InitializeCenter(int currentLocation, Vector3 direction, Vector3 nextDirection)
    {
        Vector3 crossVector = Vector3.Cross(direction, nextDirection);
        
        float zAngle = CalculateAngle(Vector3.UnitZ, direction with { Y = 0 });
        float xAngle = CalculateAngle(direction with { Y = 0 }, direction);
        float yAngle = CalculateAngle(crossVector with { Y = 0 }, crossVector);
        float directionsAngle = CalculateAngle(direction, nextDirection);
        
        for (int i = 0; i < _section.Count; i++)
        {
            Vector3 point = new Vector3(_section[i].X, _section[i].Y, 0);
            point = RotateY(point, zAngle);
            point = RotateX(point, xAngle);
            point = RotateZ(point, yAngle + directionsAngle / 2);
            point.X += _path[currentLocation].X;
            point.Y += _path[currentLocation].Y;
            point.Z += _path[currentLocation].Z;
            
            _pointLocations[currentLocation][i] = point;
        }
    }

    private void InitializeBottom(Vector3 direction, Vector3 nextDirection)
    {
        Vector3 crossVector = Vector3.Cross(direction, nextDirection);
        
        float zAngle = CalculateAngle(Vector3.UnitZ, direction with { Y = 0 });
        float xAngle = CalculateAngle(direction with { Y = 0 }, direction);
        float yAngle = CalculateAngle(crossVector with { Y = 0 }, crossVector);
        
        for (int i = 0; i < _section.Count; i++)
        {
            Vector3 point = new Vector3(_section[i].X, _section[i].Y, 0);
            point = RotateY(point, zAngle);
            point = RotateX(point, xAngle);
            point = RotateZ(point, yAngle);
            point.X += _path[0].X;
            point.Y += _path[0].Y;
            point.Z += _path[0].Z;
            
            _pointLocations[0][i] = point;
        }
    }

    public override void Render(OpenGL openGl)
    {
        DebugPath(openGl);
        DrawBottom(openGl);
        
        for (int i = 0; i < _path.Count - 1; i++)
        {
            DrawCenter(openGl, i);
        }
        
        DrawTop(openGl);
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

    private void DrawCenter(OpenGL openGl, int currentLocation)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_QUAD_STRIP);

        for (int i = 0; i < _section.Count; i++)
        {
            openGl.Color(0.5f, 0.5f, 0f);
            openGl.Vertex(_pointLocations[currentLocation][i].X, _pointLocations[currentLocation][i].Y, _pointLocations[currentLocation][i].Z);
            
            openGl.Color(0.0f, 0.5f, 0.5f);
            openGl.Vertex(_pointLocations[currentLocation + 1][i].X, _pointLocations[currentLocation + 1][i].Y, _pointLocations[currentLocation + 1][i].Z);
        }
        
        openGl.Color(0.5f, 0.5f, 0f);
        openGl.Vertex(_pointLocations[currentLocation][0].X, _pointLocations[currentLocation][0].Y, _pointLocations[currentLocation][0].Z);
            
        openGl.Color(0.0f, 0.5f, 0.5f);
        openGl.Vertex(_pointLocations[currentLocation + 1][0].X, _pointLocations[currentLocation + 1][0].Y, _pointLocations[currentLocation + 1][0].Z);
        
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawTop(OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_POLYGON);
        
        for (int i = 0; i < _pointLocations[^1].Count; i++)
        {
            openGl.Color(0.0f, 0.0f, 1.0f);
            openGl.Vertex(_pointLocations[^1][i].X, _pointLocations[^1][i].Y, _pointLocations[^1][i].Z);
        }
        
        openGl.End();
        
        openGl.PopMatrix();
    }

    private void DrawBottom(OpenGL openGl)
    {
        openGl.PushMatrix();

        openGl.Begin(OpenGL.GL_POLYGON);
        
        for (int i = 0; i < _pointLocations[0].Count; i++)
        {
            openGl.Color(1.0f, 0.0f, 0.0f);
            openGl.Vertex(_pointLocations[0][i].X, _pointLocations[0][i].Y, _pointLocations[0][i].Z);
        }
        
        openGl.End();
        
        openGl.PopMatrix();
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
    
    private void DrawAxis(OpenGL openGl)
    {
        Matrix matrix = openGl.GetModelViewMatrix();

        openGl.Begin(OpenGL.GL_LINES);

        openGl.Color(1.0f, 0.0f, 0.0f);
        openGl.Vertex(0, 0, 0);
        
        openGl.Color(1.0f, 0.0f, 0.0f);
        openGl.Vertex(1, 0, 0);
        
        openGl.Color(0.0f, 1.0f, 0.0f);
        openGl.Vertex(0, 0, 0);
        
        openGl.Color(0.0f, 1.0f, 0.0f);
        openGl.Vertex(0, 1, 0);
        
        openGl.Color(0.0f, 0.0f, 1.0f);
        openGl.Vertex(0, 0, 0);
        
        openGl.Color(0.0f, 0.0f, 1.0f);
        openGl.Vertex(0, 0, 1);

        openGl.End();
    }
    
    private void DrawVector(OpenGL openGl, Vector3 vectorToDraw)
    {
        openGl.Begin(OpenGL.GL_LINES);

        openGl.Color(1.0f, 0.0f, 1.0f);
        openGl.Vertex(0, 0, 0);
        
        openGl.Color(1.0f, 0.0f, 1.0f);
        openGl.Vertex(vectorToDraw.X, vectorToDraw.Y, vectorToDraw.Z);

        openGl.End();
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