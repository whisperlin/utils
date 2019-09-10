using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowlyControl : MonoBehaviour
{

    static SlowlyControl _ist;
    public static void Slowly(float s, float t)
    {
        if (null == _ist)
        {
            GameObject g = new GameObject();
            g.name = "slowly";
            g.hideFlags = HideFlags.HideAndDontSave;
            _ist = g.AddComponent<SlowlyControl>();
        }
        _ist.slowTime = _ist.slowTime > t ? _ist.slowTime : t;
        _ist.slowScale = s;

    }
    public float slowScale = 0.0001f;
    public float timeScale = 1f;
    private float slowTime = 0f;

    float lastTime = 0;


    // Update is called once per frame
    void Update()
    {
        float delta = Time.realtimeSinceStartup - lastTime;
        lastTime = Time.realtimeSinceStartup;
        if (delta > 1f)
        {
            delta = 0.01f;
        }
        if (slowTime > 0f)
        {
            Time.timeScale = slowScale;
            slowTime -= delta;
        }
        else
        {
            Time.timeScale = 1f;
        }
        
    }
}
