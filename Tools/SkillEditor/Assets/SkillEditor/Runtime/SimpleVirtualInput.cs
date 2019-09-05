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

    public KeyCode jump = KeyCode.Space;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        VirtualInput.SaveLastButton();
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
            VirtualInput.dir += new Vector2(1, 0);
            VirtualInput.isDirectKeyDown = true;
        }
        if (Input.GetKey(right))
        {
            VirtualInput.dir += new Vector2(-1, 0);
            VirtualInput.isDirectKeyDown = true;
        }
        VirtualInput.buttons[0] = Input.GetKey(button0);
        VirtualInput.buttons[1] = Input.GetKey(button1);
        VirtualInput.buttons[2] = Input.GetKey(button2);
        VirtualInput.buttons[3] = Input.GetKey(button3);
        VirtualInput.buttons[9] = Input.GetKey(jump);
        
    }
}
