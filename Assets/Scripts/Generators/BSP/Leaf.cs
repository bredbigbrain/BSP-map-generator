using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf
{
    public readonly int x, y, width, height, minSize;
    public Rect room;
    public List<Rect> halls = new List<Rect>();

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
        float xSize = Random.Range(minSize > width - 2 ? width - 2 : minSize, maxSize > width - 1 ? width - 1 : maxSize);
        float ySize = Random.Range(minSize > height - 2 ? height - 2 : minSize, maxSize > height - 1 ? height - 1 : maxSize);
        Vector2 roomSize = new Vector2(xSize, ySize);
        Vector2 roomPosition = new Vector2(x + Random.Range(1, width - (int)roomSize.x), y + Random.Range(1, height - (int)roomSize.y));
        room = new Rect(roomPosition, roomSize);
    }

    public void ConnectChildren()
    {
        if(leftChild != null && leftChild != null && !isChildrenConnected)
        {
            Connect();

            parent = null;
            isChildrenConnected = true;
        }

        void Connect()
        {
            var lRoom = leftChild.room;
            var rRoom = rightChild.room;

            if (splitH)
            {
                if (lRoom.xMin < rRoom.xMax && rRoom.xMin < lRoom.xMax)//Strait vertical hall.
                {
                    float hallHeight = rRoom.yMin - lRoom.yMax;
                    float hallX = lRoom.xMin;
                    int delta = (int)(lRoom.xMin - rRoom.xMax);
                    if (delta != 0)
                    {

                        hallX = lRoom.xMax > rRoom.xMax ? Random.Range((int)(lRoom.xMin + 1), (int)(rRoom.xMax)) : Random.Range((int)(rRoom.xMin + 1), (int)(lRoom.xMax));
                        Debug.Log($"{delta} {(int)hallX} {(int)lRoom.yMax} {lRoom.ToString()} {rRoom.ToString()}");
                    }
                    else
                    {
                        Debug.Log($"{(int)hallX} {(int)lRoom.yMax}");
                    }
                  
                    AddNewHall((int)hallX, (int)lRoom.yMax, 1, (int)hallHeight);
                }
                else
                {/*
                    bool rand = Random.value > 0.5f;
                    if(lRoom.xMax <= rRoom.xMin)
                    {
                        if(rand)
                        {
                            float hallHeight = 
                            int hallX = 
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }*/
                }
            }
            else
            {
                if (lRoom.yMin < rRoom.yMax && rRoom.yMin < lRoom.yMax) //Strait horisontal hall.
                {
                    float hallWidth = rRoom.xMin - lRoom.xMax;
                    float hallY = lRoom.yMin;
                    int delta = (int)(lRoom.yMax - rRoom.yMax);
                    if(delta != 0)
                    {
                        hallY = lRoom.yMax > rRoom.yMax ? Random.Range((int)(lRoom.yMin + 1), (int)(rRoom.yMax)) : Random.Range((int)(rRoom.yMin + 1), (int)(lRoom.yMax));
                    }
                    AddNewHall((int)lRoom.xMax, (int)hallY, (int)hallWidth, 1);
                }
            }
        }

        void AddNewHall(int x, int y, int width, int height)
        {
            Rect newHall = new Rect(x, y, width, height);
            if (!halls.Contains(newHall))
            {
                halls.Add(newHall);
            }
        }
    }

    public bool IsPointInsize(int x, int y)
    {
        return this.x <= x && this.x + width > x && this.y <= y && this.y + height > y;
    }

    public bool IsPointInsideRoom(int x, int y)
    {
        return room.Contains(x, y);
    }
}
