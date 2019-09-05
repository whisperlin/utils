using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayHelper : MonoBehaviour {

    public static int[] emptyIntList = new int[0];
    public static string[] emptyStringList = new string[0];
    public static int BinarySearch(int[] nums, int low, int high, int target)
    {
        while (low <= high)
        {
            int middle = (low + high) / 2;
            if (target == nums[middle])
            {
                return middle;
            }
            else if (target > nums[middle])
            {
                low = middle + 1;
            }
            else if (target < nums[middle])
            {
                high = middle - 1;
            }
        }
        return -1;
    }

   
    public static void ResizeArray<T>(ref List<T> list, int count) where T : class, new()
    {
        while (list.Count < count)
        {
            list.Add(new T());
        }

        while (list.Count > count)
        {
            list.RemoveAt(list.Count-1);
        }
    }
    public static void ResizeArray<T>(ref T[] list, int count)
    {
        if (list == null || list.Length != count)
        {
            list = new T[count];
        }
    }
    public static T[] AddItem<T>(T[] list, T id)
    {
        int c = list.Length;
        T[] res = new T[c + 1];
        for (int i = 0; i < c; i++)
        {
            res[i] = list[i];
        }
        res[c] = id;
        return res;
    }
    public static T[] DeleteItemAt<T>(T[] list, int index)  
    {
        int c = list.Length;
        T[] list2 = new T[c - 1];
        for (int j = 0; j < index; j++)
        {
            list2[j] = list[j];
        }
        for (int j = index + 1; j < c; j++)
        {
            list2[j - 1] = list[j];
        }
        return list2;
 
    }
    public static T[] DeleteItem<T>(T[] list, T v)  where T: struct
    {
        int c = list.Length;
        for (int i = 0; i < c; i++)
        {
             if(list[i].Equals( v) )
            {
                T[] list2 = new T[c-1];
                for (int j = 0; j < i; j++)
                {
                    list2[j] = list[j];
                }
                for (int j = i + 1; j < c; j++)
                {
                    list2[j - 1] = list[j];
                }
                return  list2;
            }
        }
        return list;
    }

    public static string [] DeleteItem (string[] list, string v)  
    {
        int c = list.Length;
        for (int i = 0; i < c; i++)
        {
            if (list[i].Equals(v))
            {
                string[] list2 = new string[c - 1];
                for (int j = 0; j < i; j++)
                {
                    list2[j] = list[j];
                }
                for (int j = i + 1; j < c; j++)
                {
                    list2[j - 1] = list[j];
                }
                return list2;
            }
        }
        return list;
    }
}
