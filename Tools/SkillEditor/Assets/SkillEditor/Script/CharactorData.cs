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
    Vector3 GetPointFall(Vector3 pos, RaycastHit hit ,float maxDelta)
    {
        //跌落
        if (pos.y > hit.point.y)
        {
            //角度转弧度。
             
            if (hit.distance > maxDelta)
            {
                
                Vector3 p = hit.point + Vector3.down * maxDelta;
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
    }

    Vector3 GetPointFallNav(Vector3 pos, Vector3 newPoint, float maxDelta)
    {
        //跌落
        if (pos.y > newPoint.y)
        {
            //角度转弧度。
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
                return GetPointNotFall(pos, hit.position);
        }
        if (NavMesh.SamplePosition(newPos, out hit, 10f, NavMesh.AllAreas))
        {
            if (fixToGround)
                return GetPointFallNav(pos, hit.position, delta);
            else
                return GetPointNotFall(pos, hit.position);
        }
        if (NavMesh.SamplePosition(newPos, out hit, 100f, NavMesh.AllAreas))
        {
            if (fixToGround)
                return GetPointFallNav(pos, hit.position, delta);
            else
                return GetPointNotFall(pos, hit.position);
        }
        if (NavMesh.SamplePosition(newPos, out hit, 1000f, NavMesh.AllAreas))
        {
            if (fixToGround)
                return GetPointFallNav(pos, hit.position, delta);
            else
                return GetPointNotFall(pos, hit.position);
        }
        return pos;
    }

    Vector3 TryPhysiceMove(Vector3 pos ,Vector3 dir, bool fixToGround)
    {
        RaycastHit hit;
        Vector3 newPos = pos + dir;
        float l = Vector3.Distance(newPos, pos);
        float delta = l * ageSin;
        newPos += Vector3.up * delta;
        delta = delta * 2f;
        if (Physics.Raycast(new Ray(newPos, Vector3.down), out hit, 100f))
        //if (mc.Raycast(pos0, Vector3.down, out hit, 100f))
        {
            if (fixToGround)
                return GetPointFall(pos, hit, delta);
            else
                return GetPointNotFall(newPos, hit.point);

        }
        if (!fixToGround)
        {
            dir.x = 0f;
            dir.y = 0f;
            if (Physics.Raycast(new Ray(pos , Vector3.down), out hit, 100f))
            {
                return GetPointNotFall(pos+ dir, hit.point);
            }
        }
        return pos;

         
    }
    Vector3 TryNavPhysiceMove(Vector3 pos, Vector3 dir, bool fixToGround)
    {
        RaycastHit hit;
        Vector3 newPos = pos  + dir;
        float l = Vector3.Distance(newPos, pos);
        float delta = l * ageSin;
        newPos += Vector3.up * delta;
        delta = delta * 2f;
        if (mc.Raycast(new Ray(newPos, Vector3.down), out hit, 100f))
        //if (mc.Raycast(pos0, Vector3.down, out hit, 100f))
        {
            if (fixToGround)
                return GetPointFall(pos, hit, delta);
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
            //return TryNavMesMove(pos, dir,fixToGround);
            return TryNavPhysiceMove(pos, dir, fixToGround);
        }
        else
        {
            return TryPhysiceMove(pos, dir,fixToGround);
        }
    }
     
    public Vector3  getGroundHight(Vector3 pos)
    {
        CheckSceneInformation();
        if (hasMavMesh)
        {
            RaycastHit hit;
            if (mc.Raycast(new Ray(pos + Vector3.up, Vector3.down), out hit, 100f))
            {
                return hit.point;

            }
            return pos;
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(pos + Vector3.up, Vector3.down), out hit, 100f))
            {
                return hit.point;

            }
            return pos;
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
