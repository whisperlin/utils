using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontMaterialManager : MonoBehaviour {

    static FontMaterialManager _instance;

    public static FontMaterialManager Instance
    {
        get
        {
            if (null == _instance)
            {
                GameObject g = new GameObject("MaterialManager");
                g.hideFlags = HideFlags.HideAndDontSave;
                _instance = g.AddComponent<FontMaterialManager>();
            }
            return _instance;
        }
    }
    Dictionary<Material, Dictionary<int, Material>> mats = new Dictionary<Material, Dictionary<int, Material>>();

    //alpha = 0~100
    public Material GetMaterial(Material m, float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        int _alpha = (int)(alpha * 10); 
        Dictionary<int, Material> dis;
        Material m1;
        if (mats.TryGetValue(m, out dis))
        {
            
            if (dis.TryGetValue(_alpha, out m1))
            {
                return m1;
            }
            else
            {
                m1 = new Material(m);
                dis[_alpha] = m1;
                m1.SetFloat("_Alpha",   alpha);
                return m1; 
            }
        }
        else
        {
            dis = new Dictionary<int, Material>();
            m1 = new Material(m); 
            dis[_alpha] = m1;
            m1.SetFloat("_Alpha",  alpha );
            mats[m] = dis;
            return m1;
        }
    }
}
