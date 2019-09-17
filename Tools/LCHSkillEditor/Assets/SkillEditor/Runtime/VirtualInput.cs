using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualInput  {
    public enum KeyCode
    {
        Button0,
        Button1,
        Button2,
        Button3,
        Button4,
        Button5,
        Button6,
        Button7,
        Button8,
        Button9,
        Button10,
        Button11,
        Button12,
        Button13,
        Button14,
        Button15,
        Button16,
        Button17,
        Button18,
        Button19,
 



    }
    public static Vector2 dir = Vector2.zero;
    public static bool isDirectKeyDown = false;
    public static bool[] lastButtons = new bool[20];
    public static bool[] buttons = new bool[20];


    public static bool IsButtonDown(KeyCode kc)
    {
        int id = (int)kc;
        return !lastButtons[id] && buttons[id];
    }
    public static void SaveLastButton()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            lastButtons[i] = buttons[i];
        }
    }

    
}
