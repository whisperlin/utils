using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UGUIAreaControl : MonoBehaviour
{
    public enum Align{LEFT ,CENTER };
 
    public Align align = Align.LEFT;
    public float left;
	public float top;

	public float width;
	public float height;
	public bool needUpdate = true;

	RectTransform tfrect;
	void SetArea()
	{
        
        tfrect = gameObject.GetComponent <RectTransform>();
        RectTransform p = tfrect.parent as RectTransform;
        var r = p.rect;
        
        if (align == Align.CENTER)
        {
            float y0 = r.height * (top );
            float x0 = r.width * (left+0.5f);
            tfrect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y0, r.height * height);
            tfrect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x0- r.width * width*0.5f, r.width * width);
        }
        else
        {
            float y0 = r.height * top;
            float x0 = r.width * left;
            tfrect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y0, r.height * height);
            tfrect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x0, r.width * width);
        }
         

        

    }

    // Start is called before the first frame update
    void Start()
    {
		SetArea();
    }

    // Update is called once per frame
    void Update()
    {
		if(needUpdate)
		{
			SetArea();
			
		}
    }
}
