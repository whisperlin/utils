using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEffectPool : MonoBehaviour {

    public SimpleGameObjectPool pools = new SimpleGameObjectPool();
    static GlobalEffectPool global;
    public static GlobalEffectPool Instacne()
    {
        if (null == global)
        {
            GameObject g = new GameObject("GlobalEffectPool");
            global = g.AddComponent<GlobalEffectPool>();
        }
        return global;
    }
     
}
