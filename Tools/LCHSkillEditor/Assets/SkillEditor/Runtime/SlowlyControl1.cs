using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowlyControl1 : MonoBehaviour
{
    [Range(0,1)]
    public float timeScale = 1;


    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;


    }
}
