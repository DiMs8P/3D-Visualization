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

}