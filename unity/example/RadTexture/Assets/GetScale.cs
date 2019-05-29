using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetScale : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //-0.83442    0.00000 - 0.00096    463.00000
//0.00000 2.92996 0.00000 44.54000
//0.00085 0.00000 - 0.94937    227.61000
//0.00000 0.00000 0.00000 1.00000
        Matrix4x4 m = new Matrix4x4();
        m.SetRow(0, new Vector4(-0.83442f,    0.00000f, - 0.00096f,    463.0000f));
        m.SetRow(1, new Vector4(0.00000f, 2.92996f, 0.00000f, 44.54000f));
        m.SetRow(2, new Vector4(0.00085f, 0.00000f, - 0.94937f,    227.61000f));
        m.SetRow(3, new Vector4(0.00000f, 0.00000f, 0.00000f, 1.00000f));

        //Debug.LogWarning(m);
       // Debug.LogWarning("s"+m.lossyScale+" t:"+m.transpose);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
