using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControl : MonoBehaviour {

    enum SWITCH_TYPE
    {
        WAITING,
        SWITING 
    }

    public Material mat;

    public int[] Indexs = { 0,1,2,3,4};
  
    public float waitTime = 4;
    public float switchTime = 2;
    public bool  isRandom = true;
    public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f) });

    float  curTime = 0;
    int curIndex0 = 0;
    int curIndex1 = 1;

    
    SWITCH_TYPE switchType = SWITCH_TYPE.WAITING;

    // Use this for initialization
    void Start () {
        MeshRenderer r = GetComponent<MeshRenderer>();
        if (null == mat)
        {
            mat = new Material(r.sharedMaterial);
            //mat = r.sharedMaterial;
        }
            
        r.material = mat;
        curIndex0 = Random.Range(0, Indexs.Length);
        mat.SetFloat("_Index0", Indexs[curIndex0]);
        mat.SetFloat("_Progress", 0);
  

    }
    public int switch_type = 0;
    void OnDestory()
    {
        GameObject.DestroyImmediate(mat,true);
    }
	// Update is called once per frame
	void Update () {

        curTime += Time.deltaTime;
        if (switchType == SWITCH_TYPE.WAITING)
        {
            if (curTime >= waitTime)
            {
                curTime = 0;
                switchType = SWITCH_TYPE.SWITING;

                if(isRandom)
                    curIndex1 = (curIndex0 + Random.Range(1, Indexs.Length- 1))% Indexs.Length;
                else
                    curIndex1 = (curIndex0 + 1) % Indexs.Length;
                mat.SetFloat("_Index0", Indexs[curIndex0]);
                mat.SetFloat("_Index1", Indexs[curIndex1]);
                //_SWITCH_BYC _SWITCH_NORMAL _SWITCH_BROKEN _SWITCH_ANIM _SWITCH_FY _SWITCH_FY2
                switch_type = (switch_type+Random.Range(1,7))%8;
                mat.DisableKeyword("_SWITCH_BYC");
                mat.DisableKeyword("_SWITCH_NORMAL");
                mat.DisableKeyword("_SWITCH_BROKEN");
                mat.DisableKeyword("_SWITCH_ANIM");
                mat.DisableKeyword("_SWITCH_FY");
                mat.DisableKeyword("_SWITCH_FY2");

                switch (switch_type)
                {
                    case 0:
                        mat.EnableKeyword("_SWITCH_BYC");
                        mat.EnableKeyword("_DIR_TYPE1");
                        mat.DisableKeyword("_DIR_TYPE2");
                        break;
                    case 1:
                        mat.EnableKeyword("_SWITCH_NORMAL");
                        mat.EnableKeyword("_DIR_TYPE1");
                        mat.DisableKeyword("_DIR_TYPE2");
                        break;
                    case 2:
                        mat.EnableKeyword("_SWITCH_BROKEN");
                        break;
                    case 3:
                        mat.EnableKeyword("_SWITCH_ANIM");
                        break;
                    case 4:
                        mat.EnableKeyword("_SWITCH_FY");
                        break;
                    case 5:
                        mat.EnableKeyword("_SWITCH_FY2");
                        break;
                    case 6:
                        mat.EnableKeyword("_SWITCH_BYC");
                        mat.EnableKeyword("_DIR_TYPE2");
                        mat.DisableKeyword("_DIR_TYPE1");

                        break;
                    case 7:
                        mat.EnableKeyword("_SWITCH_NORMAL");
                        mat.EnableKeyword("_DIR_TYPE2");
                        mat.DisableKeyword("_DIR_TYPE1");

                        break;
                     
                }


               
                return;
            }
        }
        else
        {
            if (curTime >= switchTime)
            {
                curTime = 0;
                switchType = SWITCH_TYPE.WAITING;
                curIndex0 = curIndex1;
                mat.SetFloat("_Index0", Indexs[curIndex0]);
                mat.SetFloat("_Progress", 0);
                return;

            }
            float t = curTime / switchTime;
            float v = curve.Evaluate(t  );
            mat.SetFloat("_Progress", v);
        }
        

    }
}
