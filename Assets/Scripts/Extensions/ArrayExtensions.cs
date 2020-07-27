﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Video;

public static class ArrayExtensions
{
    public static T RandomItem<T>(this T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static T[] RandomArray<T>(this T[] array, int count)
    {
        T[] newArray = new T[count];
        for (int i = 0; i < count; i++)
        {
            if (count < array.Length)
            {
                T temp = array.RandomItem();
                while (newArray.Contains(temp))
                {
                    temp = array.RandomItem();
                }
                newArray[i] = temp;
            }
            else
            {
                newArray[i] = array.RandomItem();
            }
        }
        return newArray;
    }
    public static T[] RandomArray<T>(this T[] array, int count, bool canRepeat)
    {
        T[] newArray = new T[count];
        for (int i = 0; i < count; i++)
        {
            if (count < array.Length)
            {
                T temp = array.RandomItem();
                if (!canRepeat)
                {
                    while (newArray.Contains(temp))
                    {
                        temp = array.RandomItem();
                    }
                }
                newArray[i] = temp;
            }
            else
            {
                newArray[i] = array.RandomItem();
            }
        }
        return newArray;
    }

    public static bool Contains<T>(this T[] array, T obj)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                if (array[i].Equals(obj))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public delegate void Callback<T>(int index, ref T value);
    public static T[] ForEach<T>(this T[] array, Callback<T> action)
    {
        for (int i = 0; i < array.Length; i++)
            action(i, ref array[i]);
        return array;
    }
}
