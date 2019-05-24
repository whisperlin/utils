using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LineTriangleIntersection : MonoBehaviour
{

    bool pointInTriangle(Vector3 query_point,
                     Vector3 triangle_vertex_0,
                     Vector3 triangle_vertex_1,
                     Vector3 triangle_vertex_2)
    {
        // u=P2−P1
        Vector3 u = triangle_vertex_1 - triangle_vertex_0;
        // v=P3−P1
        Vector3 v = triangle_vertex_2 - triangle_vertex_0;
        // n=u×v
        Vector3 n = Vector3.Cross (u,v) ;
        // w=P−P1
        Vector3 w = query_point - triangle_vertex_0;
        // Barycentric coordinates of the projection P′of P onto T:
        // γ=[(u×w)⋅n]/n²
        //u.cross(w).dot(n)
        float gamma = Vector3.Dot(Vector3.Cross (u,w),n)  / Vector3.Dot(n,n);
        // β=[(w×v)⋅n]/n²
        //float beta = w.cross(v).dot(n) / n.dot(n);
        float beta = Vector3.Dot(Vector3.Cross(w, v), n) / Vector3.Dot(n, n);
        float alpha = 1 - gamma - beta;
        // The point P′ lies inside T if:
        return ((0 <= alpha) && (alpha <= 1) &&
                (0 <= beta)  && (beta  <= 1) &&
                (0 <= gamma) && (gamma <= 1));
    }
    bool lineWithTrangle(Vector3 trip0 , Vector3 trip1 , Vector3 trip2 ,  Vector3 linep0 , Vector3 linep1 ,out Vector3 result)
    {
        result = Vector3.zero;
        return false;
    }

public Transform tri0 = null;
    public Transform tri1 = null;
    public Transform tri2 = null;

    public Transform line0 = null;
    public Transform line1 = null;

    // UE4:kDOP.h appLineCheckTriangleSOA
    private void OnDrawGizmos()
    {
        Vector3 trip0 = tri0.position;
        Vector3 trip1 = tri1.position;
        Vector3 trip2 = tri2.position;

        Vector3 linep0 = line0.position;
        Vector3 linep1 = line1.position;

        Gizmos.matrix = Matrix4x4.identity;

        // Draw triangle
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(trip0, trip1);
        Gizmos.DrawLine(trip1, trip2);
        Gizmos.DrawLine(trip2, trip0);

        // Draw line
        Gizmos.color = Color.red;
        Gizmos.DrawLine(linep0, linep1);

        // Line is on the same side of triangle-plane
        Vector3 triNormal = Vector3.Cross(trip1 - trip0, trip2 - trip0);
        triNormal.Normalize();
        float triPlaneD = Vector3.Dot(trip0, triNormal);
        Vector4 triPlane = new Vector4(triNormal.x, triNormal.y, triNormal.z, triPlaneD);
        float line0D = Vector3.Dot(linep0, triNormal) - triPlaneD;
        float line1D = Vector3.Dot(linep1, triNormal) - triPlaneD;
        if (line0D * line1D > 0)
        {
            return;
        }

        // Figure out the hit point(intersection)
        float hitTime = line0D / (line0D - line1D);
        Vector3 lineDir = linep1 - linep0;
        Vector3 hitP = linep0 + lineDir * hitTime;

        // Check if the point point is inside the triangle
        Vector3[] trips = new Vector3[] { trip0, trip1, trip2 };
        for (int sideIndex = 0; sideIndex < 3; ++sideIndex)
        {
            Vector3 edge = trips[(sideIndex + 1) % 3] - trips[sideIndex];
            Vector3 sideDir = Vector3.Cross(triNormal, edge);
            Vector3 hitDir = hitP - trips[sideIndex];
            float side = Vector3.Dot(hitDir, sideDir);
            if (side < 0)
            {
                return;
            }
        }

        // Draw intersection
        Gizmos.color = Color.green;
        Gizmos.DrawCube(hitP, Vector3.one * 0.5f);
    }
}