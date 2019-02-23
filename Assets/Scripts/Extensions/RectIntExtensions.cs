using UnityEngine;

public static class RectIntExtensions
{
    public static bool Contains(this RectInt self, int x, int y)
    {
        return x >= self.xMin && x < self.xMax && y >= self.yMin && y < self.yMax;
    }

    public static bool Overlaps(this RectInt self, RectInt other)
    {
        return (other.xMax > self.xMin &&
            other.xMin < self.xMax &&
            other.yMax > self.yMin &&
            other.yMin < self.yMax);
    }

    public static bool Overlaps(this RectInt self, RectInt other, bool allowInverse)
    {
        if (allowInverse)
        {
            self = OrderMinMax(self);
            other = OrderMinMax(other);
        }
        return self.Overlaps(other);

        RectInt OrderMinMax(RectInt rect)
        {
            if (rect.xMin > rect.xMax)
            {
                int temp = rect.xMin;
                rect.xMin = rect.xMax;
                rect.xMax = temp;
            }
            if (rect.yMin > rect.yMax)
            {
                int temp = rect.yMin;
                rect.yMin = rect.yMax;
                rect.yMax = temp;
            }
            return rect;
        }
    }
}
