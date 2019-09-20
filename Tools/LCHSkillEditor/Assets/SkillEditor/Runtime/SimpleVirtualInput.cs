using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVirtualInput : MonoBehaviour {

    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;


    public KeyCode button0 = KeyCode.U;
    public KeyCode button1 = KeyCode.I;
    public KeyCode button2 = KeyCode.J;
    public KeyCode button3 = KeyCode.K;
    public KeyCode button4 = KeyCode.L;

    public KeyCode button5 = KeyCode.O;
    public KeyCode button6 = KeyCode.P;
    public KeyCode button7 = KeyCode.Y;
    public KeyCode button8 = KeyCode.H;
    public KeyCode button9 = KeyCode.T;


    public KeyCode jump = KeyCode.Space;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        VirtualInput.SaveLastButton();

        //当摇杆输入过了，就别再处理键盘的方向输入了.
        if (LCHJoystick.golbalMoveCtrlState == LCHJoystick.STATE.NONE|| LCHJoystick.golbalMoveCtrlState == LCHJoystick.STATE.Up)
        {
            VirtualInput.isDirectKeyDown = false;
            VirtualInput.dir = Vector2.zero;
            if (Input.GetKey(up))
            {
                VirtualInput.isDirectKeyDown = true;
                VirtualInput.dir += new Vector2(0, 1);
            }
            if (Input.GetKey(down))
            {
                VirtualInput.isDirectKeyDown = true;
                VirtualInput.dir += new Vector2(0, -1);
            }
            if (Input.GetKey(left))
            {
                VirtualInput.dir += new Vector2(-1, 0);
                VirtualInput.isDirectKeyDown = true;
            }
            if (Input.GetKey(right))
            {
                VirtualInput.dir += new Vector2(1, 0);
                VirtualInput.isDirectKeyDown = true;
            }
            //Debug.LogError("from key "+ LCHJoystick.golbalMoveCtrlState.ToString());
        }
        else
        {
            //Debug.LogError("from js " + LCHJoystick.golbalMoveCtrlState.ToString());
        }
        
        VirtualInput.buttons[0] = Input.GetKey(button0) || VirtualInput.js_buttons[0]  ;
        VirtualInput.buttons[1] = Input.GetKey(button1) || VirtualInput.js_buttons[1];
        VirtualInput.buttons[2] = Input.GetKey(button2) || VirtualInput.js_buttons[2];
        VirtualInput.buttons[3] = Input.GetKey(button3) || VirtualInput.js_buttons[3];
        VirtualInput.buttons[4] = Input.GetKey(button4) || VirtualInput.js_buttons[4];
        VirtualInput.buttons[5] = Input.GetKey(button5) || VirtualInput.js_buttons[5];
        VirtualInput.buttons[6] = Input.GetKey(button6) || VirtualInput.js_buttons[6];
        VirtualInput.buttons[7] = Input.GetKey(button7) || VirtualInput.js_buttons[7];
        VirtualInput.buttons[8] = Input.GetKey(button8) || VirtualInput.js_buttons[8];
        VirtualInput.buttons[9] = Input.GetKey(button9) || VirtualInput.js_buttons[9];
        VirtualInput.buttons[19] = Input.GetKey(jump) || VirtualInput.js_buttons[19];







    }
}
