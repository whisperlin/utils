﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRole : MonoBehaviour {

  
	// Use this for initialization
	void Start () {
		
	}
    Vector3 mousePos = Vector3.zero;
 
 
 
    float rot = 0;
    float t = 0;
    // Update is called once per frame
    void Update () {


        /*if (Input.touchCount > 1) 
        {

            if (Input.touches[0].phase == TouchPhase.Moved)
            {
                if (Time.realtimeSinceStartup - t > 0.2f)
                {
                   
                    Vector2 delta = Input.touches[0].deltaPosition;
                   
                    float x = delta.x *  3 * Time.deltaTime;
                    Debug.LogError("1)"+x);
                    x = Mathf.Max(x, -10f);
                    x = Mathf.Min(x, 10f);
                    rot += x;
                    transform.rotation = Quaternion.Euler(0f, rot, 0f);

                }
            }
 
        }
        else
        {
            t = Time.realtimeSinceStartup;
        }*/

        if (Application.platform == RuntimePlatform.Android ||
        Application.platform == RuntimePlatform.IPhonePlayer)

        {
            if (Input.touchCount > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    float x = Input.touches[0].deltaPosition.x;
                    if (x != 0)
                    {
                        x = x * 30 * Time.deltaTime;
                        rot -= x;
                        transform.rotation = Quaternion.Euler(0f, rot, 0f);
 
                    }
                        
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Debug.LogError("(Input.touchCount" + Input.touchCount);

                Vector3 delta = Input.mousePosition - mousePos;

                float x = delta.x * 3 * Time.deltaTime;
 
                rot -= x;



                transform.rotation = Quaternion.Euler(0f, rot, 0f);

            }
            mousePos = Input.mousePosition;
        }
            

       

    }
}
