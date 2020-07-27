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
            return Connects(leaf.room);
        }

        public bool Connects(Hall hall)
        {
            foreach(var rect in hall.rects)
            {
                if (Connects(rect))
                    return true;
            }
            return false;
        }

        public bool Connects(RectInt rect)
        {
            if (rects == null)
            {
                return false;
            }

            RectInt expandedRect = new RectInt();
            for (int i = 0; i < rects.Length; i++)
            {
                expandedRect.xMin = rects[i].xMin - 1;
                expandedRect.yMin = rects[i].yMin - 1;
                expandedRect.width = rects[i].width + 2;
                expandedRect.height = rects[i].height + 2;
                if (expandedRect.Overlaps(rect))
                {
                    bool cornerCase = (rect.xMax == rects[i].xMin || rect.xMin == rects[i].xMax) && (rect.yMax == rects[i].yMin || rect.yMin == rects[i].yMax);
                    if(!cornerCase)
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
