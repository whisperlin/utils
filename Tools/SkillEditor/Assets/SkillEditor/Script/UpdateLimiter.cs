using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void Limiterfun(params object []args);
public class UpdateLimiter  {

    float  maxTime = 2f;
    float  lastUpdateTime = -1f;
    public UpdateLimiter(float count)
    {
        lastUpdateTime = float.MaxValue;
        maxTime = count;
    }
	// Update is called once per frame
	public void Update (Limiterfun fun,params object[] arg) {

        float delta = Time.realtimeSinceStartup - lastUpdateTime;
        if (delta < 0)
        {
            lastUpdateTime = Time.realtimeSinceStartup;
            fun(arg);
        }
        if (delta >= maxTime)
        {
            float d = maxTime - delta;
            lastUpdateTime = Time.realtimeSinceStartup - d;
            fun(arg);
             
        }
            	
	}
}
