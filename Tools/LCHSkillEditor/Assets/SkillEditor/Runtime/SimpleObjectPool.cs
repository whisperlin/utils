using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGameObjectPool
{

    public Dictionary<int, List<GameObject>> objects = new Dictionary<int, List<GameObject>>();
    public Dictionary<int,int> instances = new Dictionary<int, int>();
    public GameObject GetObject(GameObject gameObject)
    {
        int id = gameObject.GetInstanceID();
        List<GameObject> ls;
        if (!objects.TryGetValue(id,out ls))
        {
            ls = new List<GameObject>();
            objects[id] = ls;
        }
        GameObject g;
        if (ls.Count == 0)
        {
            g = GameObject.Instantiate(gameObject);
        }
        else
        {
            g = ls[ls.Count - 1];
            ls.RemoveAt(ls.Count-1);
        }
        int _id = g.GetInstanceID();
        instances[_id] = id;
        g.SetActive(true);
        return g;
    }
    public bool Release(GameObject gameObject)
    {
        int id = gameObject.GetInstanceID();
        int baseId;
        if (instances.TryGetValue(id, out baseId))
        {
            instances.Remove(id);
            gameObject.transform.parent = null;
            gameObject.SetActive(false);
            objects[baseId].Add(gameObject);
            return true;
        }
        return false;
    }
}

public class SimplePool<T> where T:Component
{
    public delegate T CreateHandle();
    public CreateHandle createHandle = null;
    List<T> objs = new List<T>();
    public T Get()
    {
        T o;
        if (objs.Count > 0)
        {
            o = objs[objs.Count - 1];
            objs.RemoveAt(objs.Count - 1);
        }
        else
        {
            if (null == createHandle)
            {
                GameObject g = new GameObject();
                o = g.AddComponent<T>();
            }
            else
            {
                o = createHandle();
                
            }
            
        }
        
        return o;
    }
    public void Release(T o)
    {
        objs.Add(o);
        o.gameObject.SetActive(false);
    }
}
