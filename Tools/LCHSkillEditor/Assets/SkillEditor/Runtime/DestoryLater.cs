using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryLater : MonoBehaviour {
    public float time;
    
    
	
	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;
        if (time < 0)
        {
            GameObject.DestroyImmediate(this.gameObject);
        }

    }
}
