using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LCHJoystickEX : MonoBehaviour ,/*IPointerDownHandler,IPointerUpHandler,IPointerClickHandler,*/ IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [Header("触碰区域占屏幕的高度比例")]
    [Range(0,1f)]
    public float width = 0.5f;
    [Header("触碰区域占屏幕的高度比例")]
    [Range(0, 1f)]
    public float height = 0.5f;

    public float backgroundHeight = 0.2f;
    public float buttonHeight = 0.1f;
    public UnityEngine.UI.Image panel;
    public UnityEngine.UI.Image background;
    public UnityEngine.UI.Image button;
    Vector2 beginPos;
    public float radius = 50f;



    public void UpdateScreenSize(int w,int h)
    {
        if (null != panel)
        {
            if (width < 0.001f)
            {
                panel.rectTransform.sizeDelta = new Vector2(h * height, h * height);
            }
            else
            {
                panel.rectTransform.sizeDelta = new Vector2(w * width, h * height);
            }
            background.rectTransform.sizeDelta = new Vector2(h * backgroundHeight, h * backgroundHeight);
            button.rectTransform.sizeDelta = new Vector2(h * buttonHeight, h * buttonHeight);
        }
    }
    void Start()
    {
 
        //UpdateScreenSize(Screen.width, Screen.height);
        background.gameObject.SetActive(false);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (null != background)
        {
            beginPos = eventData.position;
            background.rectTransform.position = new Vector3(beginPos.x, beginPos.y,0f);
            button.rectTransform.position = background.rectTransform.position;
            VirtualInput.isDirectKeyDown = false;
            VirtualInput.dir = Vector2.zero;
            LCHJoystick.golbalMoveCtrlState = LCHJoystick.STATE.NONE;
        }
        background.gameObject.SetActive(true);
        //Debug.LogError("OnBeginDrag" + eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (null != button)
        {
            var p = eventData.position;
            float len = Vector2.Distance(p,beginPos);
            Vector2 dir = (p - beginPos);
            if (len < radius)
            {
                Vector2 finalPos = beginPos + dir;
                button.rectTransform.position = new Vector3(finalPos.x, finalPos.y, 0f);

            }
            else
            {
                Vector2 finalPos = beginPos + dir.normalized * radius;
                button.rectTransform.position = new Vector3(finalPos.x, finalPos.y, 0f);
            }
            VirtualInput.isDirectKeyDown = true;

            VirtualInput.dir = dir.normalized;
            //Debug.LogError("VirtualInput.dir = "+ VirtualInput.dir);
            LCHJoystick.golbalMoveCtrlState = LCHJoystick.STATE.DRAG;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        VirtualInput.isDirectKeyDown = false;
        VirtualInput.dir = Vector2.zero;
        LCHJoystick.golbalMoveCtrlState = LCHJoystick.STATE.NONE;
        // Debug.LogError("OnEndDrag" + eventData.position);
        //button.rectTransform.position = new Vector3(0f, 0f, 0f);
    }

 

    
}
