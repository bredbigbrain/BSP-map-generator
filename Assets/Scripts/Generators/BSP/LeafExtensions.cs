using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class LeafExtensions
{
    public static float Distance(this Leaf self, Leaf other)
    {
        int x = self.x - other.x;
        int y = self.y - other.y;

        return Mathf.Sqrt(x * x + y * y);
    }

    public static float SqrDistance(this Leaf self, Leaf other)
    {
        int x = self.x - other.x;
        int y = self.y - other.y;

        return x * x + y * y;
    }

    public static Leaf GetClosestChild(this Leaf self, Leaf other)
    {
        float lDist = self.leftChild.SqrDistance(other);
        float rDist = self.rightChild.SqrDistance(other);

        return lDist < rDist ? self.leftChild : self.rightChild;
    }

}
