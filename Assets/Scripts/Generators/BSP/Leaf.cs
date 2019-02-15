using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf
{
    public readonly int x, y, width, height, minSize;
    public RectInt room = new RectInt(-1, -1, 0, 0);
    public List<RectInt> halls = new List<RectInt>();

    public Leaf leftChild; //Left or lower.
    public Leaf rightChild; //Right or higher.
    public Leaf parent; //Connect child's rooms, if any.

    private bool isChildrenConnected = false, splitH = false;

    public Leaf(int x, int y, int width, int height, int minSize)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.minSize = minSize;
        parent = null;
    }

    public Leaf(int x, int y, int width, int height, Leaf parent)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.parent = parent;
        minSize = parent.minSize;
    }

    public bool Split()
    {
        if (leftChild != null || rightChild != null)
        {
            return false;
        }
        
        if(width > height)// && width / height >= 1.25f)
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
        if(max <= minSize)
        {
            return false;
        }

        int split = Random.Range(minSize, max + 1);

        if(splitH)
        {
            leftChild = new Leaf(x, y, width, split, this);
            rightChild = new Leaf(x, y + split, width, height - split, this);
        }
        else
        {
            leftChild = new Leaf(x, y, split, height, this);
            rightChild = new Leaf(x + split, y, width - split, height, this);
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

    public void ConnectChildren()
    {
        if (leftChild != null && rightChild != null && !isChildrenConnected)
        {
            RectInt lRoom = leftChild.room;
            RectInt rRoom = rightChild.room;

            if(lRoom.x == -1)
            {
                lRoom = leftChild.GetClosestChild(this).room;
            }
            else if(rRoom.x == -1)
            {
                rRoom = rightChild.GetClosestChild(this).room;
            }

            Connect(lRoom, rRoom);

            parent = null;
            isChildrenConnected = true;
        }

        void Connect(RectInt lRoom, RectInt rRoom)
        {
            if (splitH)
            {
                if (lRoom.xMin < rRoom.xMax && rRoom.xMin < lRoom.xMax)//Strait vertical hall.
                {
                    CreateVerticalHall();
                }
                else
                {
                    CreateCornerHallH();
                }
            }
            else
            {
                if (lRoom.yMin < rRoom.yMax && rRoom.yMin < lRoom.yMax) //Strait horisontal hall.
                {
                    CreateHorisontalHall();
                }
                else
                {
                    CreateCornerHallW();
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

                if (smallRoom.xMax - 1 == bigRoom.xMin)
                {
                    hallX = bigRoom.xMin;
                }
                else if (smallRoom.xMin == bigRoom.xMax - 1)
                {
                    hallX = smallRoom.xMin;
                }
                else if (smallRoom.xMin >= bigRoom.xMin && smallRoom.xMax <= bigRoom.xMax)
                {
                    hallX = Random.Range(smallRoom.xMin, smallRoom.xMax);
                }
                else if (smallRoom.xMin <= bigRoom.xMin)
                {
                    hallX = Random.Range(bigRoom.xMin + 1, smallRoom.xMax);
                }
                else
                {
                    hallX = Random.Range(smallRoom.xMin + 1, bigRoom.xMax);
                }

                AddNewHall(hallX, lRoom.yMax, 1, hallHeight);
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

                int hallWidth = rRoom.xMin - lRoom.xMax;
                int hallY;

                if (smallRoom.yMin == bigRoom.yMax - 1)
                {
                    hallY = smallRoom.yMin;
                }
                else if (smallRoom.yMax - 1 == bigRoom.yMin)
                {
                    hallY = bigRoom.yMin;
                }
                else if (smallRoom.yMin >= bigRoom.yMin && smallRoom.yMax <= bigRoom.yMax)
                {
                    hallY = Random.Range(smallRoom.yMin + 1, smallRoom.yMax);
                }
                else if (smallRoom.yMin <= bigRoom.yMin)
                {
                    hallY = Random.Range(bigRoom.yMin + 1, smallRoom.yMax);
                }
                else
                {
                    hallY = Random.Range(smallRoom.yMin + 1, bigRoom.yMax);
                }

                AddNewHall(lRoom.xMax, hallY, hallWidth, 1);
            }

            void CreateCornerHallH()
            {
                bool leftBelow = true;
                if(lRoom.xMin >= rRoom.xMax)
                {
                    var temp = lRoom;
                    lRoom = rRoom;
                    rRoom = temp;
                    leftBelow = false;
                }

                int x1, x2, y1, y2;

                if(Random.value > 0.5f)
                {
                    x1 = Random.Range(lRoom.xMin + 1, lRoom.xMax);
                    y1 = leftBelow ? lRoom.yMax : lRoom.yMin;

                    x2 = rRoom.xMin;
                    y2 = Random.Range(rRoom.yMin + 1, rRoom.yMax);

                    AddNewHall(x1, leftBelow ? y1 : y2, 1, Mathf.Abs(y2 - y1));
                    AddNewHall(x1, y2, Mathf.Abs(x2 - x1), 1);
                }
                else
                {
                    x1 = lRoom.xMax;
                    y1 = Random.Range(lRoom.yMin + 1, lRoom.yMax);

                    x2 = Random.Range(rRoom.xMin + 1, rRoom.xMax);
                    y2 = leftBelow ? rRoom.yMin : rRoom.yMax;

                    AddNewHall(x1, y1, Mathf.Abs(x2 - x1), 1);
                    AddNewHall(x2, leftBelow ? y1 : y2, 1, Mathf.Abs(y2 - y1));

                    Debug.Log($"{x1} {y1} {Mathf.Abs(x2 - x1)} {1}");
                    Debug.Log($"{x2} {(leftBelow ? y1 : y2)} {1} {Mathf.Abs(y2 - y1)}");
                }
            }

            void CreateCornerHallW()
            { }
        }

        void AddNewHall(int x, int y, int width, int height)
        {
            //Debug.Log($"{x} {y} {leftChild.room} {rightChild.room}");

            RectInt newHall = new RectInt(x, y, width, height);
            if (!halls.Contains(newHall))
            {
                halls.Add(newHall);
            }
        }
    }

    public bool IsPointInside(int x, int y)
    {
        return this.x <= x && this.x + width > x && this.y <= y && this.y + height > y;
    }

    public bool IsPointInsideRoom(int x, int y)
    {
        return room.Contains(x, y);
    }

    public override string ToString()
    {
        return string.Concat("(", x, " ", y, " ", width, " ", height, ")");
    }
}
