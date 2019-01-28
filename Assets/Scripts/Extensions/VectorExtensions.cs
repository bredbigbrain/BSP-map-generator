using UnityEngine;
using System.Collections;

public static class VectorExtensions
{
    public static Vector3 X(this Vector3 vec) { return new Vector3(vec.x, 0f, 0f); }
    public static Vector3 Y(this Vector3 vec) { return new Vector3(0f, vec.y, 0f); }
    public static Vector3 Z(this Vector3 vec) { return new Vector3(0f, 0f, vec.z); }

    public static Vector3 XY(this Vector3 vec) { return new Vector3(vec.x, vec.y, 0f); }
    public static Vector3 XZ(this Vector3 vec) { return new Vector3(vec.x, 0f, vec.z); }
    public static Vector3 YZ(this Vector3 vec) { return new Vector3(0f, vec.y, vec.z); }

    public static Vector3 X(this Vector3 vec, Vector3 replacement) { return new Vector3(vec.x, replacement.y, replacement.z); }
    public static Vector3 Y(this Vector3 vec, Vector3 replacement) { return new Vector3(replacement.x, vec.y, replacement.z); }
    public static Vector3 Z(this Vector3 vec, Vector3 replacement) { return new Vector3(replacement.x, replacement.y, vec.z); }

    public static Vector3 X(this Vector3 vec, float y, float z) { return new Vector3(vec.x, y, z); }
    public static Vector3 Y(this Vector3 vec, float x, float z) { return new Vector3(x, vec.y, z); }
    public static Vector3 Z(this Vector3 vec, float x, float y) { return new Vector3(x, y, vec.z); }

    public static Vector3 XY(this Vector3 vec, Vector3 replacement) { return new Vector3(vec.x, vec.y, replacement.z); }
    public static Vector3 XZ(this Vector3 vec, Vector3 replacement) { return new Vector3(vec.x, replacement.y, vec.z); }
    public static Vector3 YZ(this Vector3 vec, Vector3 replacement) { return new Vector3(replacement.x, vec.y, vec.z); }

    public static Vector3 XY(this Vector3 vec, float replacement) { return new Vector3(vec.x, vec.y, replacement); }
    public static Vector3 XZ(this Vector3 vec, float replacement) { return new Vector3(vec.x, replacement, vec.z); }
    public static Vector3 YZ(this Vector3 vec, float replacement) { return new Vector3(replacement, vec.y, vec.z); }

    public static Vector3 XYZ(this Vector3 vec) { return new Vector3(vec.x, vec.y, vec.z); }

    public static Vector3 RandomVector()
    {
        Vector3 random = new Vector3(Random.value, Random.value, Random.value);
        return random;
    }

    public static Vector3 ToAngles(this Vector3 vec)
    {
        if(vec.x > 360f)
        {
            vec.x = vec.x - 360f;
        }
        if (vec.y > 360f)
        {
            vec.y = vec.y - 360f;
        }
        if (vec.z > 360f)
        {
            vec.z = vec.z - 360f;
        }
        return vec;
    }

    public static Vector3 Randomize(this Vector3 original, float range)
    {
        Vector3 randomized = original;
        randomized.x += Random.Range(-range, range);
        randomized.y += Random.Range(-range, range);
        randomized.z += Random.Range(-range, range);

        return randomized;
    }
    public static Vector3 RandomizeXZ(this Vector3 original, float range)
    {
        Vector3 randomized = original;
        randomized.x += Random.Range(-range, range);
        //randomized.y += Random.Range(-range, range);
        randomized.z += Random.Range(-range, range);

        return randomized;
    }
    public static Vector3 Vector3FromString(string rString)
    {
        if(rString.Length < 2)
        {
            return Vector3.zero;
        }
        string[] temp = rString.Substring(1, rString.Length - 2).Split(',');
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);
        Vector3 rValue = new Vector3(x, y, z);
        return rValue;
    }

    public static Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Quaternion Angle)
    {
        return Angle * (Point - Pivot) + Pivot;
    }
    public static Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Vector3 Euler)
    {
        return Quaternion.Euler(Euler) * (Point - Pivot) + Pivot;
    }
}
