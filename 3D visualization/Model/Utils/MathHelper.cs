using System.Numerics;

namespace _3D_visualization.Model.Utils;

public static class MathHelper
{
    public static float CalculateAngle(Vector3 vector1, Vector3 vector2)
    {
        float dotProduct = Vector3.Dot(vector1, vector2);
        float magnitude1 = vector1.Length();
        float magnitude2 = vector2.Length();

        float cosTheta = dotProduct / (magnitude1 * magnitude2);

        float thetaRad = (float)System.Math.Acos(cosTheta);

        float thetaDeg = thetaRad * 180 / (float)System.Math.PI;
        float sign = System.Math.Sign(Vector3.Cross(vector1, vector2).Y);

        if (sign == 0)
        {
            sign = 1;
        }

        return thetaDeg * sign;
    }

    public static float GetRadiansFrom(float angle)
    {
        return ((float)Math.PI / 180) * angle;
    }
    
    public static Vector3 RotateY(Vector3 vector, float angle)
    {
        double radians = (System.Math.PI / 180) * angle;
        
        double[,] matrix = {
            {System.Math.Cos(radians), 0, System.Math.Sin(radians)},
            {0, 1, 0},
            {-System.Math.Sin(radians), 0, System.Math.Cos(radians)}
        };
        
        double x = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z;
        double y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z;
        double z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z;

        return new Vector3((float)x, (float)y, (float)z);
    }
    
    public static Vector3 RotateX(Vector3 vector, float angle)
    {
        double radians = (System.Math.PI / 180) * angle;
        
        double[,] matrix = {
            {1, 0, 0},
            {0, System.Math.Cos(radians), -System.Math.Sin(radians)},
            {0, System.Math.Sin(radians), System.Math.Cos(radians)}
        };
        
        double x = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z;
        double y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z;
        double z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z;

        return new Vector3((float)x, (float)y, (float)z);
    }
    
    public static Vector3 RotateZ(Vector3 vector, float angle)
    {
        double radians = (System.Math.PI / 180) * angle;
        
        double[,] matrix = {
            {System.Math.Cos(radians), -System.Math.Sin(radians), 0},
            {System.Math.Sin(radians), System.Math.Cos(radians), 0},
            {0, 0, 1}
        };
        
        double x = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z;
        double y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z;
        double z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z;

        return new Vector3((float)x, (float)y, (float)z);
    }
}