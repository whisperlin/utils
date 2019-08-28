using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    public readonly List<Point> Points;
    private Point mCentroid;

    public Face(Point p1, Point p2, Point p3, bool registerFace = false)
    {
        Points = new List<Point>(3);
        Points.Add(p1);
        Points.Add(p2);
        Points.Add(p3);
        if (registerFace)
        {
            p1.RegisterFace(this);
            p2.RegisterFace(this);
            p3.RegisterFace(this);
        }
    }

    public Point GetCentroid()
    {
        if (mCentroid != null)
            return mCentroid;

        var cent = (Points[0].ToVector3() + Points[1].ToVector3() + Points[2].ToVector3()) / 3;

        mCentroid = new Point(cent);
        return mCentroid;
    }

    public Point[] GetOtherPoints(Point p)
    {
        var other = new Point[2];
        int c = 0;
        foreach (var point in Points)
        {
            if (point.ToVector3() != p.ToVector3())
            {
                other[c++] = point;
            }
        }
        return other;
    }

    // Adjacent if 2 of the points are the same
    public bool IsAdjacentTo(Face face2)
    {
        var count = 0;
        for (var i = 0; i < Points.Count; i++)
        {
            for (var j = 0; j < face2.Points.Count; j++)
            {
                if (Points[i].ToVector3() == face2.Points[j].ToVector3())
                {
                    count++;

                }
            }
        }

        return (count == 2);
    }
}