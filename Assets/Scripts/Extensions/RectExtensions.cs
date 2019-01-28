using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectExtensions
{
    public static bool Contains(this Rect rect, int x, int y)
    {
        return (x >= (int)rect.xMin) && (x < (int)rect.xMax) && (y >= (int)rect.yMin) && (y < (int)rect.yMax);
    }
}
