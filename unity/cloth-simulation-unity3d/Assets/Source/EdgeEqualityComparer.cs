using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeEqualityComparer : IEqualityComparer<Edge> {

    /// <summary>
    /// Compares two edges by their main vertices indexes
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool Equals(Edge a, Edge b)
    {
        if (a.vertexA == b.vertexA && a.vertexB == b.vertexB)
        {
            return true;
        }

        return false;
    }

    public int GetHashCode(Edge edge)
    {
        int hCode = 4756 * edge.vertexA + 47357 * edge.vertexB;
        return hCode.GetHashCode();
    }
}
