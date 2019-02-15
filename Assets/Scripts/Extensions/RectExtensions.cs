using UnityEngine;

public static class RectIntExtensions
{
    public static bool Contains(this RectInt rect, int x, int y)
    {
        return (x >= rect.xMin) && (x < rect.xMax) && (y >= rect.yMin) && (y < rect.yMax);
    }
}
