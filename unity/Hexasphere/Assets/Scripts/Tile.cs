using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Point CenterPoint;
    public List<Face> Faces;
    public List<Point> NeighborIds;
    public List<Point> Boundary;
    public List<Tile> Neighbors;

    public Tile(Point centerPoint, float? hexSize)
    {
        if (!hexSize.HasValue)
            hexSize = 1;

        hexSize = Mathf.Clamp(hexSize.Value, 0.01f, 1);
        CenterPoint = centerPoint;
        Faces = centerPoint.GetOrderedFaces();
        Boundary = new List<Point>();
        NeighborIds = new List<Point>();    // this holds the centerpoints, will resolve to references after
        Neighbors = new List<Tile>();       // this is filled in after all the tiles have been created

        var neighborHash = new Dictionary<Point, int>();

        for (var f = 0; f < Faces.Count; f++)
        {
            // build boundary
            Boundary.Add(Faces[f].GetCentroid().Segment(centerPoint, hexSize.Value));

            // get neighboring tiles
            var otherPoints = Faces[f].GetOtherPoints(CenterPoint);
            for (var o = 0; o < 2; o++)
            {
                neighborHash[otherPoints[o]] = 1;
            }
        }

        foreach (var key in neighborHash.Keys)
        {
            NeighborIds.Add(key);
        }

        var normal = СalculateSurfaceNormal(Boundary[0].ToVector3(), Boundary[1].ToVector3(), Boundary[2].ToVector3());

        if (!IsPointingAwayFromOrigin(CenterPoint.ToVector3(), normal))
        {
            Boundary.Reverse();
        }
    }

    public Vector3 PointsToVector(Vector3 p1, Vector3 p2)
    {
        return new Vector3(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
    }

    public Vector3 СalculateSurfaceNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var U = PointsToVector(p1, p2);
        var V = PointsToVector(p1, p3);

        var N = new Vector3(
            U.y * V.z - U.z * V.y,
            U.z * V.x - U.x * V.z,
            U.x * V.y - U.y * V.x
            );

        return N;
    }

    bool IsPointingAwayFromOrigin(Vector3 p, Vector3 v)
    {
        return ((p.x * v.x) >= 0) && ((p.y * v.y) >= 0) && ((p.z * v.z) >= 0);
    }
}
