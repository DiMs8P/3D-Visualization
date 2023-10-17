using System.Numerics;
using _3D_visualization.Model.Components;
using _3D_visualization.Model.Components.Render;

namespace _3D_visualization.Model.Replication;

public class ReplicationObject
{
    private SplineRenderComponent _renderComponent;
    public ReplicationObject((List<Vector2>, List<Vector3>) replicationData)
    {
        _renderComponent = new SplineRenderComponent(replicationData.Item1, replicationData.Item2);
        /*InitializeSplineComponent(replicationData.Item1, replicationData.Item2);
        _renderComponent = new RenderComponent();
        _renderComponent.SetRenderMode(RenderMode.TrianglesStrip);*/
    }

    private void InitializeSplineComponent(List<Vector2> section, List<Vector3> path)
    {
        /*Vector3 direction = path[1] - path[0];
        Vector3 baseDirection = new Vector3(0, 0, -1);
        BasePrimitive bottom = new BasePrimitive();
        bottom.Location = path[0];
        
        float angleX = GetAngle(direction, baseDirection, Axis.X);
        float angleY = GetAngle(direction, baseDirection, Axis.Y);
        float angleZ = GetAngle(direction, baseDirection, Axis.Z);
        bottom.Rotation = new Vector3(angleX, angleY, angleZ);

        for (int i = 0; i < path.Count - 1; i++)
        {
            direction = path[i] - path[i + 1];
            
            angleX = GetAngle(direction, baseDirection, Axis.X);
            angleY = GetAngle(direction, baseDirection, Axis.Y);
            angleZ = GetAngle(direction, baseDirection, Axis.Z);

            bottom.Location = path[i];
            bottom.Rotation = new Vector3(angleX, angleY, angleZ);
        }*/
    }

    /*static float GetAngle(Vector3 vector1, Vector3 vector2, Axis axis)
    {
        /*float dotProduct = Vector3.Dot(vector1, vector2);
        float magnitude1 = vector1.Length();
        float magnitude2 = vector2.Length();

        float cosTheta = dotProduct / (magnitude1 * magnitude2);

        float angle = (float)Math.Acos(cosTheta);

        switch (axis)
        {
            case Axis.X:
                return angle * Math.Sign(vector2.Y - vector1.Y);
            case Axis.Y:
                return angle * Math.Sign(vector1.X - vector2.X);
            case Axis.Z:
                return angle * Math.Sign(vector2.X - vector1.X);
            default:
                throw new ArgumentException("Unnown axis");
        }#1#
        /*}#1#
    }*/
}
