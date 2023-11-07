using System.Numerics;

namespace _3D_visualization.Model.Utils;

public static class MathHelper
{
    public static float GetRadiansFrom(float angle)
    {
        return ((float)Math.PI / 180) * angle;
    }

    public static Vector3 GetPointInAnotherCoordinateSystem(Vector3 newX, Vector3 newY, Vector3 newZ, Vector3 point)
    {
        float[,] matrix = {
            {newX.X, newY.X, newZ.X},
            {newX.Y, newY.Y, newZ.Y},
            {newX.Z, newY.Z, newZ.Z}
        };

        return new Vector3(
            matrix[0, 0] * point.X + matrix[0, 1] * point.Y + matrix[0, 2] * point.Z,
            matrix[1, 0] * point.X + matrix[1, 1] * point.Y + matrix[1, 2] * point.Z,
            matrix[2, 0] * point.X + matrix[2, 1] * point.Y + matrix[2, 2] * point.Z
            );
    }
    
}