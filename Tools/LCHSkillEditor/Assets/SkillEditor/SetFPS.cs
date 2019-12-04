using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFPS : MonoBehaviour {

    public UnityEngine.UI.Text text;

    int fps = 60;
    float curTime = 0f;
    int count = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        curTime += Time.unscaledDeltaTime;
        count++;
        if (curTime > 1f)
        {
            fps = count;
            curTime -= 1f;
            count = 0;
            if (null != text)
                text.text = "FPS:" + fps;
        }


    }
}
