using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ugly.MapGenerators.BinarySpacePartitioning
{ 
    public class Hall
    {
        public RectInt[] rects;
        public List<Leaf> leaves = new List<Leaf>();

        public Hall(params RectInt[] rects)
        {
            this.rects = rects;
        }

        public bool Connects(Leaf leaf)
        {
            if(rects == null)
            {
                return false;
            }
            RectInt expandedRect;
            for (int i = 0; i < rects.Length; i++)
            {
                expandedRect = new RectInt(rects[i].xMin - 1, rects[i].yMin - 1, rects[i].width + 2, rects[i].height + 2);
                if(expandedRect.Overlaps(leaf.room))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(int x, int y)
        {
            if (rects == null)
            {
                return false;
            }
            for (int i = 0; i < rects.Length; i++)
            {
                if(rects[i].Contains(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Hall))
            {
                return false;
            }

            Hall hall = obj as Hall;

            if (hall.rects == null && rects == null)
            {
                return true;
            }
            if (hall.rects.Length == rects.Length)
            {
                for (int i = 0; i < rects.Length; i++)
                {
                    if (!rects[i].Equals(hall.rects[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return rects.GetHashCode();
        }
    }
}
