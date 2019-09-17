using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharactorData : LChatacterInformationInterface
{
    private static CharactorData _instance;
    private bool hasMavMesh = false;

    [Header("可以登上的斜坡的最大角度")]
    //public float arge = 30f;
    public float ageSin = 0.5f;
    //用来判断是否是在新场景中
    GameObject newScene = null;
    MeshCollider mc;
    public static CharactorData Instance()
    {
        if (null == _instance)
        {
            _instance = new CharactorData();
        }
        return _instance;
    }
    public void slowly(float v, float slow_motion)
    {
        SlowlyControl.Slowly(v, slow_motion);
    }
    void CheckSceneInformation()
    {
        if (null == newScene)
        {
            newScene = new GameObject();
            newScene.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            //这个接口比较慢，用这种笨方法检查每个场景是否有导航网格。每个场景只检查一次。
            NavMeshTriangulation tmpNavMeshTriangulation = NavMesh.CalculateTriangulation();
            hasMavMesh = tmpNavMeshTriangulation.vertices.Length > 0;
            mc = newScene.AddComponent<MeshCollider>();
            Mesh mesh = new Mesh();
            mesh.vertices = tmpNavMeshTriangulation.vertices;
            mesh.triangles = tmpNavMeshTriangulation.indices;
            mc.sharedMesh = mesh;
            //MeshFilter mf = newScene.AddComponent<MeshFilter>();
           // mf.sharedMesh = mesh;
            //newScene.AddComponent<MeshRenderer>();

        }
    }


    Vector3 GetPointNotFall(Vector3 pos, Vector3 goundPos)
    {
        if (pos.y > goundPos.y)
        {
            goundPos.y = pos.y;
        }
        
        return goundPos;
    }
   /* Vector3 GetPointFall(Vector3 pos, Ray r, RaycastHit hit ,float maxDelta)
    {
        //跌落
        if (pos.y > hit.point.y)
        {
            if (hit.distance > maxDelta)
            {
                
                Vector3 p = r.origin + Vector3.down * maxDelta;
                return p;
            }
            else
            {
                return hit.point;
            }
        }
        else
        {
            return hit.point;
        }
    }*/

    Vector3 GetPointFallNav(Vector3 pos, Vector3 newPoint, float maxDelta)
    {
        //跌落
        if (pos.y > newPoint.y)
        {
            float d = pos.y - newPoint.y;
            if (d > maxDelta)
            {
                return newPoint + Vector3.up  * (d- maxDelta);
            }
            else
            {
                return newPoint;
            }
        }
        else
        {
            return newPoint;
        }
    }
    
    Vector3 TryNavMesMove(Vector3 pos ,Vector3 dir, bool fixToGround)
    {
        var newPos = pos + dir;
        float l = Vector3.Distance(newPos, pos);
        float delta = l * ageSin;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPos, out hit, 1f, NavMesh.AllAreas))
        {
            if (fixToGround)
                return GetPointFallNav(pos, hit.position,delta);
            else
                return GetPointNotFall(newPos, hit.position);
        }
        if (NavMesh.SamplePosition(newPos, out hit, 10f, NavMesh.AllAreas))
        {
            if (fixToGround)
                return GetPointFallNav(pos, hit.position, delta);
            else
                return GetPointNotFall(newPos, hit.position);
        }
        if (NavMesh.SamplePosition(newPos, out hit, 100f, NavMesh.AllAreas))
        {
            if (fixToGround)
                return GetPointFallNav(pos, hit.position, delta);
            else
                return GetPointNotFall(newPos, hit.position);
        }
        if (NavMesh.SamplePosition(newPos, out hit, 1000f, NavMesh.AllAreas))
        {
            if (fixToGround)
                return GetPointFallNav(pos, hit.position, delta);
            else
                return GetPointNotFall(newPos, hit.position);
        }
        return pos;
    }
    static Vector3 debugV;
    Vector3 TryPhysiceMove(Vector3 pos ,Vector3 dir, bool fixToGround)
    {
        RaycastHit hit;
        if (fixToGround)
        {
            Vector3 newPos = pos + dir;
            float l = Vector3.Distance(newPos, pos);
            float delta = l * ageSin;
            newPos += Vector3.up * delta;
            delta = delta * 2f;
            Ray r = new Ray(newPos, Vector3.down);
            if (Physics.Raycast(r, out hit, 100f, PhysicesData.ground))
            {

                return GetPointFallNav(pos, hit.point, delta);

            }
             
            return pos;
        }
        else
        {
            float distance = Vector3.Distance(dir, Vector3.zero);
            Vector3 pos0;
            if (Physics.Raycast(new Ray(pos, dir), out hit, distance, PhysicesData.ground))
            {

                pos0 =  hit.point;
            }
            else
            {
                pos0 =  pos + dir;
            }
            //最终点是否可行走。
            if (Physics.Raycast(new Ray(pos0, Vector3.down), out hit, 100f, PhysicesData.ground))
            {
               // Debug.LogError("dit ground end "+pos0);
                debugV = pos0;
                return pos0;
            }
            else
            {
                distance = pos.y - pos0.y  ;

                Vector3 test;
                if (!getGroundHight(pos, out test))
                {
                    Debug.LogError("pos not hit");
                    Debug.LogError(pos0.x == debugV.x);
                    Debug.LogError(pos0.z == debugV.z);
                }
                 
                //下落,并且能碰到地面，取地面高度.
                if (distance > 0 &&Physics.Raycast(new Ray(pos+Vector3.up, Vector3.down), out hit, distance+1,PhysicesData.ground))
                {
                    //Debug.LogError("hit ground");
                    return hit.point;
                }
                else
                {
                   // Debug.LogError("not hit ground "+ pos);
                    return new Vector3(pos.x, pos0.y, pos.z);
                }
            }
        }
        
        

         
    }
    Vector3 TryNavPhysiceMove(Vector3 pos, Vector3 dir, bool fixToGround)
    {
        RaycastHit hit;
        Vector3 newPos = pos  + dir;
        float l = Vector3.Distance(newPos, pos);
        float delta = l * ageSin;
        newPos += Vector3.up * delta;
        delta = delta * 2f;
        Ray r = new Ray(newPos, Vector3.down);
        if (mc.Raycast(r, out hit, 100f))
        //if (mc.Raycast(pos0, Vector3.down, out hit, 100f))
        {
            if (fixToGround)
                return GetPointFallNav(pos, hit.point, delta);
                //return GetPointFall(pos,r, hit, delta);
            else
                return GetPointNotFall(newPos, hit.point);

        }
        if (!fixToGround)
        {
            dir.x = 0f;
            dir.z = 0f;
            if (dir.y > 0)
            {
                newPos = pos+dir;
            }
            else
            {
                newPos = pos;
            }
            if (mc.Raycast(new Ray(newPos, Vector3.down), out hit, 100f))
            {
                return GetPointNotFall(pos+ dir, hit.point);
            }
        }
        return pos;
    }
    public Vector3 tryMove(  Vector3 pos ,Vector3 dir, bool fixToGround)
    {
        CheckSceneInformation();
        if (hasMavMesh)
        {
            //return TryNavPhysiceMove(pos, dir, fixToGround);
            return TryNavPhysiceMove(pos, dir, fixToGround); 
            if (fixToGround)
            {
                return TryNavMesMove(pos, dir, fixToGround);
            }
            else
            {
                return TryNavPhysiceMove(pos, dir, fixToGround);
            }
            //return TryNavMesMove2(pos, dir, fixToGround);
            //TryNavMesMove2
            //return TryNavMesMove(pos, dir,fixToGround);
            //return TryNavPhysiceMove(pos, dir, fixToGround);
        }
        else
        {
            return TryPhysiceMove(pos, dir,fixToGround);
        }
    }
    public Vector3 GetNewPointCanWalk(Vector3 pos)
    {
        CheckSceneInformation();
        if (hasMavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pos, out hit, 100f, NavMesh.AllAreas))
            {
                return new Vector3(hit.position.x,  pos.y, hit.position.z);
            }
        }
        
        return pos;
    }
    public bool  getGroundHight(Vector3 pos ,out Vector3 ground)
    {
        CheckSceneInformation();
        if (hasMavMesh)
        {
            RaycastHit hit;
            if (mc.Raycast(new Ray(pos + Vector3.up, Vector3.down), out hit, 100f ))
            {
                ground = hit.point;
                return true;

            }
            ground = Vector3.zero;
            return false;
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(pos + Vector3.up, Vector3.down), out hit, 100f, PhysicesData.ground))
            {
                ground =  hit.point;
                return true;
            }
            ground = Vector3.zero;
            return false;
        }
    }
    List<LChatacterInterface> characters = new List<LChatacterInterface>();
    Dictionary<int, LChatacterInterface> charactersMap = new Dictionary<int, LChatacterInterface>();
    public LChatacterInterface GetCharacter(int targetId)
    {
        return charactersMap[targetId];
    }

    public void AddCharacter(LChatacterInterface character)
    {
        characters.Add(character);
        charactersMap[character.GetId()] = character;
    }

    public void RemoveCharacter(LChatacterInterface character)
    {
        charactersMap.Remove(character.GetId());
        characters.Remove(character);
    }
}
