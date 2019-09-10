using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReleaseEffect : MonoBehaviour {

    public float time = 3f;
	
	
	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;
       
        if (time < 0)
        {
            this.gameObject.SetActive(false);
            GlobalEffectPool.Instacne().pools.Release(this.gameObject);
            GameObject.DestroyObject(this);
        }
        
	}
}
