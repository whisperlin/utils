using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager  {


    public static ScreenModifyHanle handles;
    public delegate void ScreenModifyHanle(int width, int height);

    static int baseWidth = -1;
    static int baseHiehgt = -1;
    public static void SetScreenSize(bool baseSize)
    {
        if (baseWidth == -1)
        {
            baseWidth = Screen.width;
            baseHiehgt = Screen.height;
        }
        
        if (baseSize)
        {
            Screen.SetResolution(baseWidth/2, baseHiehgt/2, true);
            handles(Screen.width, Screen.height);
            Debug.LogError(""+Screen.width+","+Screen.height);
            
        }
        else
        {
            Screen.SetResolution(baseWidth, baseHiehgt, true);
            handles(Screen.width, Screen.height);
            Debug.LogError("" + Screen.width + "," + Screen.height);
        }
        
    }
}
