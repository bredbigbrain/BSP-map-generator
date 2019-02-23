using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ugly.MapGenerators.BinarySpacePartitioning
{
    public class Leaf
    {
        public readonly int x, y, width, height, minSize;
        public RectInt room = new RectInt(-1, -1, 0, 0);
        public List<Hall> halls = new List<Hall>();

        public BSP bsp;

        public Leaf leftChild; //Left or lower.
        public Leaf rightChild; //Right or higher.
        public Leaf parent; //Connects child's rooms, if any.

        public List<Leaf> connections = new List<Leaf>();

        private bool splitH = false;

        public Leaf(int x, int y, int width, int height, int minSize, BSP bsp)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.minSize = minSize;
            this.bsp = bsp;
            parent = null;
        }

        public Leaf(int x, int y, int width, int height, Leaf parent, BSP bsp)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.parent = parent;
            this.bsp = bsp;
            minSize = parent.minSize;
        }

        public bool Split()
        {
            if (leftChild != null || rightChild != null)
            {
                return false;
            }

            if (width > height)// && width / height >= 1.25f)
            {
                splitH = false;
            }
            else //if(height > width && height / width >= 1.25f)
            {
                splitH = true;
            }
            /*else
            {
                splitH = Random.value > 0.5f;
            }*/

            int max = (splitH ? height : width) - minSize;
            if (max <= minSize)
            {
                return false;
            }

            int split = Random.Range(minSize, max + 1);

            if (splitH)
            {
                leftChild = new Leaf(x, y, width, split, this, bsp);
                rightChild = new Leaf(x, y + split, width, height - split, this, bsp);
            }
            else
            {
                leftChild = new Leaf(x, y, split, height, this, bsp);
                rightChild = new Leaf(x + split, y, width - split, height, this, bsp);
            }

            return true;
        }

        public void CreateRoom(int minSize, int maxSize)
        {
            int xSize = Random.Range(minSize > width - 2 ? width - 2 : minSize, maxSize > width - 1 ? width - 1 : maxSize);
            int ySize = Random.Range(minSize > height - 2 ? height - 2 : minSize, maxSize > height - 1 ? height - 1 : maxSize);
            Vector2Int roomSize = new Vector2Int(xSize, ySize);
            Vector2Int roomPosition = new Vector2Int(x + Random.Range(1, width - roomSize.x), y + Random.Range(1, height - roomSize.y));
            room = new RectInt(roomPosition, roomSize);
        }

        public void ConnectChildren(int hallWidth)
        {
            if (leftChild != null && rightChild != null)
            {
                if (leftChild.room.x == -1 && rightChild.room.x == -1)
                {
                    ConnectDeepChildren(hallWidth);
                    return;
                }

                Leaf lLeaf = leftChild, rLeaf = rightChild;

                if (leftChild.room.x == -1)
                {
                    lLeaf = leftChild.GetClosestChild(rightChild, true);
                }
                if (rightChild.room.x == -1)
                {
                    rLeaf = rightChild.GetClosestChild(leftChild, true);
                }

                Connect(lLeaf, rLeaf, hallWidth);
            }
        }

        private void ConnectDeepChildren(int hallWidth)
        {
            int splitCenterX, splitCenterY;

            if (splitH)
            {
                splitCenterX = x + width / 2;
                splitCenterY = y + leftChild.height;
            }
            else
            {
                splitCenterX = x + leftChild.width;
                splitCenterY = y + height / 2;
            }

            Leaf lLeaf, rLeaf;

            List<Leaf> leaves = new List<Leaf>();
            leftChild.GetInnerLeaves(ref leaves, true);

            int minDist = int.MaxValue, dist, index = -1;
            for (int i = 0; i < leaves.Count; i++)
            {
                dist = leaves[i].SqrDistance(splitCenterX, splitCenterY);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }
            lLeaf = leaves[index];

            leaves.Clear();
            rightChild.GetInnerLeaves(ref leaves, true);

            minDist = int.MaxValue;
            for (int i = 0; i < leaves.Count; i++)
            {
                dist = leaves[i].SqrDistance(splitCenterX, splitCenterY);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }
            rLeaf = leaves[index];

            Connect(lLeaf, rLeaf, hallWidth);
        }

        private void Connect(Leaf lLeaf, Leaf rLeaf, int hallWidth)
        {
            RectInt lRoom, rRoom;
            if (!lLeaf.connections.Contains(rLeaf))
            {
                lRoom = lLeaf.room;
                rRoom = rLeaf.room;

                if (splitH)
                {
                    if (lRoom.xMin < rRoom.xMax && rRoom.xMin < lRoom.xMax)
                    {
                        CreateVerticalHall();
                    }
                    else
                    {
                        CreateCornerHall(splitH);
                    }
                }
                else
                {
                    if (lRoom.yMin < rRoom.yMax && rRoom.yMin < lRoom.yMax)
                    {
                        CreateHorisontalHall();
                    }
                    else
                    {
                        CreateCornerHall(splitH);
                    }
                }
            }

            void CreateVerticalHall()
            {
                RectInt smallRoom, bigRoom;
                if (lRoom.width > rRoom.width)
                {
                    smallRoom = rRoom;
                    bigRoom = lRoom;
                }
                else
                {
                    smallRoom = lRoom;
                    bigRoom = rRoom;
                }

                int hallHeight = rRoom.yMin - lRoom.yMax;
                int hallX;

                if (smallRoom.xMin >= bigRoom.xMin && smallRoom.xMax <= bigRoom.xMax)
                {
                    if (smallRoom.width <= hallWidth)
                    {
                        hallWidth = smallRoom.width;
                        hallX = smallRoom.xMin;
                    }
                    else
                    {
                        hallX = Random.Range(smallRoom.xMin, smallRoom.xMax - hallWidth);
                    }
                }
                else
                {
                    int minIntersection, maxIntersection;

                    if (lRoom.xMin < rRoom.xMin)
                    {
                        minIntersection = rRoom.xMin;
                        maxIntersection = lRoom.xMax;
                    }
                    else
                    {
                        minIntersection = lRoom.xMin;
                        maxIntersection = rRoom.xMax;
                    }

                    int delta = Mathf.Abs(maxIntersection - minIntersection);
                    if (delta <= hallWidth)
                    {
                        hallWidth = delta;
                        hallX = minIntersection;
                    }
                    else
                    {
                        hallX = Random.Range(minIntersection, maxIntersection - hallWidth);
                    }
                }

                AddNewHall(new RectInt(hallX, lRoom.yMax, hallWidth, hallHeight));
            }

            void CreateHorisontalHall()
            {
                RectInt smallRoom, bigRoom;
                if (lRoom.height > rRoom.height)
                {
                    smallRoom = rRoom;
                    bigRoom = lRoom;
                }
                else
                {
                    smallRoom = lRoom;
                    bigRoom = rRoom;
                }

                int hallWidth_ = rRoom.xMin - lRoom.xMax;
                int hallY;

                if (smallRoom.yMin >= bigRoom.yMin && smallRoom.yMax <= bigRoom.yMax)
                {
                    if (smallRoom.height <= hallWidth)
                    {
                        hallWidth = smallRoom.height;
                        hallY = smallRoom.yMin;
                    }
                    else
                    {
                        hallY = Random.Range(smallRoom.yMin, smallRoom.yMax - hallWidth);
                    }
                }
                else
                {
                    int minIntersection, maxIntersection;

                    if (lRoom.yMin < rRoom.yMin)
                    {
                        minIntersection = rRoom.yMin;
                        maxIntersection = lRoom.yMax;
                    }
                    else
                    {
                        minIntersection = lRoom.yMin;
                        maxIntersection = rRoom.yMax;
                    }

                    int delta = Mathf.Abs(maxIntersection - minIntersection);
                    if (delta <= hallWidth)
                    {
                        hallWidth = delta;
                        hallY = minIntersection;
                    }
                    else
                    {
                        hallY = Random.Range(minIntersection, maxIntersection - hallWidth);
                    }
                }

                AddNewHall(new RectInt(lRoom.xMax, hallY, hallWidth_, hallWidth));
            }

            void CreateCornerHall(bool splitH)
            {
                bool leftBelow = true;

                if (splitH)
                {
                    if (lRoom.xMin >= rRoom.xMax)
                    {
                        leftBelow = false;

                        var temp = lRoom;
                        lRoom = rRoom;
                        rRoom = temp;
                    }
                }
                else if (lRoom.yMin >= rRoom.yMax)
                {
                    leftBelow = false;
                }

                int x1 = -1, x2 = -1, y1 = -1, y2 = -1;

                if (Random.value > 0.5f)
                {
                    int min = Mathf.Min(lRoom.width, rRoom.height);
                    if (min < hallWidth)
                    {
                        hallWidth = min;
                    }
                    if (lRoom.width <= hallWidth)
                    {
                        x1 = lRoom.xMin;
                    }
                    if (rRoom.height <= hallWidth)
                    {
                        y2 = rRoom.yMin;
                    }

                    if (x1 == -1)
                    {
                        x1 = Random.Range(lRoom.xMin + 1, lRoom.xMax - hallWidth);
                    }
                    y1 = leftBelow ? lRoom.yMax : lRoom.yMin;

                    x2 = rRoom.xMin;
                    if (y2 == -1)
                    {
                        y2 = Random.Range(rRoom.yMin + 1, rRoom.yMax - hallWidth);
                    }

                    AddNewHall(
                        new RectInt(x1, leftBelow ? y1 : y2, hallWidth, Mathf.Abs(y2 - y1)),
                        new RectInt(x1, y2, Mathf.Abs(x2 - x1), hallWidth)
                        );
                }
                else
                {
                    int min = Mathf.Min(lRoom.height, rRoom.width);
                    if (min < hallWidth)
                    {
                        hallWidth = min;
                    }
                    if (lRoom.height <= hallWidth)
                    {
                        y1 = lRoom.yMin;
                    }
                    if (rRoom.width <= hallWidth)
                    {
                        x2 = rRoom.xMin;
                    }

                    x1 = lRoom.xMax;
                    if (y1 == -1)
                    {
                        y1 = Random.Range(lRoom.yMin + 1, lRoom.yMax - hallWidth);
                    }

                    if (x2 == -1)
                    {
                        x2 = Random.Range(rRoom.xMin + 1, rRoom.xMax - hallWidth);
                    }
                    y2 = leftBelow ? rRoom.yMin : rRoom.yMax;

                    AddNewHall(
                        new RectInt(x1, y1, Mathf.Abs(x2 - x1) + hallWidth, hallWidth),
                        new RectInt(x2, leftBelow ? y1 : y2, hallWidth, Mathf.Abs(y2 - y1))
                        );
                }
            }

            void AddNewHall(params RectInt[] rects)
            {
                //Debug.Log($"{x} {y} {width} {leftChild.room} {rightChild.room}");

                bsp.AddHall(new Hall(rects));
            }
        }

        private void GetInnerLeaves(ref List<Leaf> list, bool withRooms = false)
        {
            leftChild?.GetInnerLeaves(ref list);
            rightChild?.GetInnerLeaves(ref list);

            if (room.x == -1 && withRooms)
            {
                return;
            }
            list.Add(this);
        }

        public bool IsPointInside(int x, int y)
        {
            return this.x <= x && this.x + width > x && this.y <= y && this.y + height > y;
        }

        public bool IsPointInsideRoom(int x, int y)
        {
            return room.Contains(x, y);
        }

        public Leaf GetClosestChild(Leaf other, bool checkRoom = false)
        {
            if (checkRoom)
            {
                if (leftChild.room.x == -1 && rightChild.room.x == -1)
                {
                    return null;
                }
                else if(leftChild.room.x == -1)
                {
                    return rightChild;
                }
                else if(rightChild.room.x == -1)
                {
                    return leftChild;
                }
            }

            int lDist = leftChild.SqrDistance(other);
            int rDist = rightChild.SqrDistance(other);

            return lDist < rDist ? leftChild : rightChild;
        }

        public float Distance(Leaf other)
        {
            int x = this.x - other.x;
            int y = this.y - other.y;

            return Mathf.Sqrt(x * x + y * y);
        }

        public float Distance(int x, int y)
        {
            x = this.x - x;
            y = this.y - y;

            return Mathf.Sqrt(x * x + y * y);
        }

        public int SqrDistance(Leaf other)
        {
            int x = this.x - other.x;
            int y = this.y - other.y;

            return x * x + y * y;
        }

        public int SqrDistance(int x, int y)
        {
            x = this.x - x;
            y = this.y - y;

            return x * x + y * y;
        }

        public override string ToString()
        {
            return string.Concat("(", x, " ", y, " ", width, " ", height, ")");
        }
    }
}