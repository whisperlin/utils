using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScaleUI : MonoBehaviour {

    public float standard_width = 800f;        //初始宽度    
    public float standard_height = 480f;       //初始高度    
    float device_width = 0f;                //当前设备宽度    
    float device_height = 0f;               //当前设备高度    
    float adjustor = 0f;         //屏幕矫正比例    
    void Start()
    {  
               
        //获取设备宽高    
        device_width = Screen.width;  
        device_height = Screen.height;  
        //计算宽高比例    
        float standard_aspect = standard_width / standard_height;  
        float device_aspect = device_width / device_height;  
        //计算矫正比例    
         if (device_aspect<standard_aspect)  
         {  
            adjustor = standard_aspect / device_aspect;  
        }  
        CanvasScaler canvasScalerTemp = transform.GetComponent<CanvasScaler>();  
        if (adjustor == 0)  
        {  
        canvasScalerTemp.matchWidthOrHeight = 1;  
        }  
        else  
        {  
         canvasScalerTemp.matchWidthOrHeight = 0;  
        }  
     }  
}
