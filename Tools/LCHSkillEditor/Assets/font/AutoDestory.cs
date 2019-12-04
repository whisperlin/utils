using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestory : MonoBehaviour {

    public float destoryTime = 2f;
    void Start()
    {
        StartCoroutine(load());  
    }
    
    IEnumerator load()
    {
        yield return new WaitForSeconds(destoryTime);    //注意等待时间的写法

    }

}
