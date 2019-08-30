using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetDatabasePools<T>
    where T : UnityEngine.Object
{
    Dictionary<string, T> pools = new Dictionary<string, T>();

    public T LoadAsset(string path)
    {
        T v;
        if (!pools.TryGetValue(path, out v))
        {
            v = AssetDatabase.LoadAssetAtPath<T>(path);
            pools[path] = v;
        }
        return v;
    }
    public void Clear()
    {
        pools.Clear();
    }

}
/*public class AssetDatabaseHelper    {

    AssetDatabasePools<Animation> animations = new AssetDatabasePools<Animation>();
    AssetDatabasePools<GameObject> gameObjects = new AssetDatabasePools<GameObject>();

    public Animation GetAnim(string path)
    {
        return animations.LoadAsset(path);
    }
    public GameObject GetGameObject(string path)
    {
        return gameObjects.LoadAsset(path);
    }

    public void Clear()
    {
        animations.Clear();
        gameObjects.Clear();
    }


}*/
