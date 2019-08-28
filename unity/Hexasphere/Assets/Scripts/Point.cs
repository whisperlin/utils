using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// that looks funny but I need it to keep same references when projection hex to spheres, cause Unity Vector3 is not reftype
public class Point
{
    private Vector3 mPointCoord;
    public List<Face> Faces = new List<Face>();

    public void RegisterFace(Face face)
    {
        Faces.Add(face);
    }
 
    public Point(float x, float y, float z)
    {
        mPointCoord.Set(x, y, z);
    }

    public Point(Vector3 vec)
    {
        mPointCoord = vec;
    }

    public float x
    {
        get
        {
            return mPointCoord.x;
        }
    }

    public float y
    {
        get
        {
            return mPointCoord.y;
        }
    }

    public float z
    {
        get
        {
            return mPointCoord.z;
        }
    }

    public void Set(Vector3 vec)
    {
        mPointCoord = vec;
    }

    public Vector3 ToVector3()
    {
        return mPointCoord;
    }

    public override string ToString()
    {
        return mPointCoord.ToString();
    }

    public Point Segment(Point point, float percent)
    {
        percent = Mathf.Max(0.01f, Mathf.Min(1, percent));

        var x = point.x * (1 - percent) + this.x * percent;
        var y = point.y * (1 - percent) + this.y * percent;
        var z = point.z * (1 - percent) + this.z * percent;

        var newPoint = new Point(x, y, z);
        return newPoint;
    }

    public List<Point> Subdivide(Point point,int count, Icosahedron.Checker checkPoint)
    {
        var segments = new List<Point>();
        segments.Add(this);

        for (var j = 1; j < count; j++)
        {
            float i = j;
            var np = new Point(this.x * (1 - (i / count)) + point.x * (i / count),
                this.y * (1 - (i / count)) + point.y * (i / count),
                this.z * (1 - (i / count)) + point.z * (i / count));
            np = checkPoint(np);
            segments.Add(np);
        }

        segments.Add(point);
        return segments;
    }

    public List<Face> GetOrderedFaces()
    {
        List<Face> workingArray = new List<Face>();
        workingArray.AddRange(Faces);
        var ret = new List<Face>();

        var i = 0;
        while (i < Faces.Count)
        {
            if (i == 0)
            {
                ret.Add(workingArray[i]);
                workingArray.RemoveAt(i);
            }
            else
            {
                var hit = false;
                var j = 0;
                while (j < workingArray.Count && !hit)
                {
                    if (workingArray[j].IsAdjacentTo(ret[i - 1]))
                    {
                        hit = true;
                        ret.Add(workingArray[j]);
                        workingArray.RemoveAt(j);
                    }
                    j++;
                }
            }
            i++;
        }

        return ret;
    }
}

