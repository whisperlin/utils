using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorSkillResourceLoader : SkillResourceLoader
{
    //AssetDatabaseHelper assetDatabasseHeldper;
   // SimpleGameObjectPool gameObjectPool = new SimpleGameObjectPool();
    //SimplePool<BoxCollider> simplePool = new SimplePool<BoxCollider>();
    public EditorSkillResourceLoader( )
    {
     
    }
    public SimpleGameObjectPool gos = new SimpleGameObjectPool();

    public GameObject  LoadModul(int type,string name, string path)
    {
        if (type == -1)
        {
            var a =  AssetDatabase.LoadAssetAtPath<Animation>(path);  
            if(a ==null)
            {
                return null;
            }
            GameObject g = GameObject.Instantiate(a.gameObject);
            g.hideFlags = HideFlags.DontSave;
            return g;
            //return gameObjectPool.GetObject(a.gameObject);
        }
        else if (type == 2)
        {
            GameObject g = new GameObject();
            g.AddComponent<BoxCollider>();
            g.hideFlags = HideFlags.DontSave;
            return g;
            //return simplePool.Get().gameObject;
        }
        else //if (type == 1)
        {
            var a = AssetDatabase.LoadAssetAtPath<GameObject>(path);  
            if (a == null)
            {
                return null;
            }
            GameObject g = GameObject.Instantiate(a.gameObject);

            g.hideFlags = HideFlags.DontSave;
            return g;
            
            // return gameObjectPool.GetObject(assetDatabasseHeldper.GetGameObject(path));
        }
    }
    public void  ReleaseModel(int type,string name, string path, GameObject g)
    {
        if (null == g)
            return;
        if (type == 3)
        {
            return;
        }
        if (type == -1)
        {
            GameObject.DestroyImmediate(g);
            //gameObjectPool.Release(g);
        }
        else if (type == 1)
        {
            GameObject.DestroyImmediate(g);
            //gameObjectPool.Release(g);
        }
        else //if (type == 2)
        {
            GameObject.DestroyImmediate(g);
            //BoxCollider b = g.GetComponent<BoxCollider>();
            //simplePool.Release(b);
        }
    }
}
