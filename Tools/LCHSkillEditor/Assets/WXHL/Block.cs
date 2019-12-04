using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public struct WayPoint
{
    [Header("位置")]
 
    public Vector3 pos;


    public Vector3 hitPoint;

   
     
}
public class Block : MonoBehaviour {

    public enum TYPE
    {
        WAY,
        PITFALL,
        TRAMPOLINE,
    }
    public TYPE type = TYPE.WAY;
    public List<WayPoint> points = new List<WayPoint>();

    [System.NonSerialized]
    public MeshFilter[] meshFilters;

    [System.NonSerialized]
    public MeshRenderer[] meshRenders;

    public Color connectColor = Color.red; 
    public static int SaveInt(float f)
    {
        if (f > 0)
        {
            return (int)(f + 0.00001f);
        }
        else
        {
            return (int)(f - 0.00001f);
        }
    }
    public void UpdateWayPoint(int minRange = -500, int  maxRange = 500)
    {
 
        points.Clear();
        for (int x = minRange; x < maxRange; x++)
        {
            for (int z = minRange; z < maxRange; z++)
            {
        
                WayPoint p = new WayPoint();
                p.hitPoint = new Vector3(x, -1001, z);
                bool isHit = false;
                for (int j = 0; j < meshFilters.Length; j++)
                {
                    var mr = meshRenders[j];
                    var mf = meshFilters[j];
                    var max = mr.bounds.max;
                    var min = mr.bounds.min;
                    if (min.x < x && x < max.x && min.z < z && z < max.z)
                    {
                        RaycastHit hitInfor;
                        if (RaycastHelper.Raycast(mf, new Ray(new Vector3(x, 500, z), Vector3.down), out hitInfor, 1000f))
                        {
                            if (isHit)
                            {
                                if (hitInfor.point.y > p.hitPoint.y)
                                {
                                    p.hitPoint = hitInfor.point;
                                }
                            }
                            else
                            {
                                p.hitPoint = hitInfor.point;
                                
                            }
                            isHit = true;
                        }
                    }
                }
                if (isHit)
                {
                    
                    p.pos = new Vector3(SaveInt(p.hitPoint.x), SaveInt(p.hitPoint.y), SaveInt(p.hitPoint.z));
                    points.Add(p);
                }
     
            }
        }
    }

    static Color wayPointColor = new Color(0.5f, 0.5f, 1.0f);
    
    static Vector3 waySize = new Vector3(1f, 0.001f, 1f);
    void OnDrawGizmos()
    {
        for (int i = 0, c = points.Count; i < c; i++)
        {
            var wp = points[i];
           

            
           
           
            Gizmos.color = connectColor;
            Gizmos.DrawCube(wp.hitPoint, waySize);

            Gizmos.color = wayPointColor;
            Gizmos.DrawWireCube(wp.pos, Vector3.one);
            Gizmos.DrawSphere(wp.hitPoint, 0.2f);

        }
        
        
        //Debug.DrawLine(transform.position, transform.position+Vector3.up*2,Color.red);
    }
}
