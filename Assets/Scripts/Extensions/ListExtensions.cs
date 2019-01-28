using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    public static T RandomItem<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            return default(T);
        }
        return list[Random.Range(0, list.Count)];
    }

    public static int CheckEmptyItems<T>(this List<T> list)
    {
        int removedCount = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                list.RemoveAt(i);
                i--;
                removedCount++;
            }
        }
        return removedCount;
    }

    public static T LastItem<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }

    public static List<T> Shuffle<T>(this List<T> list)
    {
        return list.OrderBy(x => Random.value).ToList();
    }

    public static List<T> RandomList<T>(this List<T> list, int count)
    {
        List<T> newList = new List<T>(count);
        for (int i = 0; i < count; i++)
        {
            if (count < list.Count)
            {
                T temp = list.RandomItem();
                while (newList.Contains(temp))
                {
                    temp = list.RandomItem();
                }
                newList.Add(temp);
            }
            else
            {
                newList.Add(list.RandomItem());
            }
        }
        return newList;
    }

    public static List<T> RandomList<T>(this List<T> list, int count, bool canRepeat)
    {
        List<T> newList = new List<T>(count);
        for (int i = 0; i < count; i++)
        {
            if (count < list.Count)
            {
                T temp = list.RandomItem();
                if (!canRepeat)
                {
                    while (newList.Contains(temp))
                    {
                        temp = list.RandomItem();
                    }
                }
                newList.Add(temp);
            }
            else
            {
                newList.Add(list.RandomItem());
            }
        }
        return newList;
    }
}
