using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static List<Vector3> ToUnityVectorsList(this IEnumerable<Point> points)
    {
        var vecList = new List<Vector3>();
        foreach (var p in points)
        {
            vecList.Add(p.ToVector3());
        }
        return vecList;
    }

    public static Vector3[] ToUnityVectorsArray(this IEnumerable<Point> points)
    {
        return ToUnityVectorsList(points).ToArray();
    }
}
