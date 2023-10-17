using System.Globalization;
using System.IO;
using System.Net;
using System.Numerics;
using _3D_visualization.Exception;

namespace _3D_visualization.Model.Replication;

public class ReplicationParser
{
    public static (List<Vector2>, List<Vector3>) TryParse(string fileName)
    {
        string[] lines = File.ReadAllLines(fileName);

        if (IsFileValid(lines))
        {
            return ParseLines(lines);
        }

        return (null, null);
    }

    private static (List<Vector2>, List<Vector3>) ParseLines(string[] lines)
    {
        List<Vector2> section = new List<Vector2>();
        List<Vector3> path = new List<Vector3>();
        for (int i = 0; i < lines.Length; i++)
        {
            string[] tokens = lines[i].Split(' ');

            if (tokens.Length == 2)
            {
                section.Add(
                    new Vector2(
                        float.Parse(tokens[0], CultureInfo.InvariantCulture),
                        float.Parse(tokens[1], CultureInfo.InvariantCulture)
                        )
                    );
            }
            
            if (tokens.Length == 3)
            {
                path.Add(
                    new Vector3(
                        float.Parse(tokens[0], CultureInfo.InvariantCulture),
                        float.Parse(tokens[1], CultureInfo.InvariantCulture),
                        float.Parse(tokens[2], CultureInfo.InvariantCulture)
                    )
                );
            }
        }
        
        if (path.Count < 2)
        {
            throw new ReplicationArgumentException(-1, "path can't contain 1 path point");
        }

        return (section, path);
    }

    public static bool IsFileValid(string[] lines)
    {
        float result;
        
        for (int i = 0; i < lines.Length; i++)
        {
            string[] tokens = lines[i].Split(' ');

            if (tokens.Length == 2)
            {
                if (!float.TryParse(tokens[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result)
                    || !float.TryParse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                {
                    throw new ReplicationArgumentException(i, "failed parse section");
                }
            }
            else if (tokens.Length == 3)
            {
                if (!float.TryParse(tokens[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result)
                    || !float.TryParse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result)
                    || !float.TryParse(tokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                {
                    throw new ReplicationArgumentException(i, "failed parse path");
                }
            }
            else
            {
                throw new ReplicationArgumentException(i, "in line you must have 2 or 3 numbers");
            }
        }

        return true;
    }
}