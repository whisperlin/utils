using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class LCHJoystickCameraCtrl : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    
    public LCharacterFellowCamera fellowCanera;
    public UnityEngine.UI.Image panel;


    bool slowly = false;
    public float yRotScale = 0.1f;
    public float xRotScale = 0.1f;
    public float disScale = 0.1f;
    void Start()
    {
        if (null != panel)
        {
            panel.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height );

            
        }

 
    }
 
    List<int> touchPoints = new List<int>();
    public void OnPointerDown(PointerEventData eventData)
    {
        int _id = eventData.pointerId;
 
        touchPoints.Add(_id);

    }
    void UpdateTouchCtrlPoint()
    {
        for (int i = 0; i < touchPoints.Count; )
        {
            bool enable = false;
            int id1 = touchPoints[i];
            for (int j = 0; j < Input.touchCount; j++)
            {
                var t = Input.GetTouch(j);
                int _id0 = t.fingerId;
                if (id1 == _id0)
                {
                    enable = true;
                    break;
                }
            }
            if (!enable)
                touchPoints.Remove(id1);
            else
                i++;
        }
    }
    public void Update()
    {
        UpdateTouchCtrlPoint();
#if UNITY_EDITOR
        float axis = Input.GetAxis("Mouse ScrollWheel");
        fellowCanera.distance -= disScale * axis*20;

#endif

    }
    public void SetSlow(UnityEngine.UI.Toggle t)
    {
        slowly = t.isOn;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
   
 
         
    }
    Touch _t0;
    Touch _t1;
    public void OnDrag(PointerEventData eventData)
    {

        if (touchPoints.Count == 2)
        {
            int _id0 = touchPoints[0];
            int _id1 = touchPoints[1];
            for (int j = 0; j < Input.touchCount; j++)
            {
                Touch t = Input.GetTouch(j);
                if (t.fingerId == _id0)
                    _t0 = t;
                else if (t.fingerId == _id1)
                    _t1 = t;
            }
            float d1 = Vector2.Distance(_t0.position, _t1.position);
            float d0 = Vector2.Distance(_t0.position-_t0.deltaPosition, _t1.position-_t1.deltaPosition);
            float delta = d1 - d0;
            fellowCanera.distance -= delta * disScale;
            
        }
       
        else
        {

 
            Vector2 dir = eventData.delta;
            fellowCanera.xRot += dir.x * xRotScale;
            fellowCanera.yRot -= dir.y * yRotScale;
            
             
        }
        if(!slowly)
            fellowCanera.SetToTarget();

    }

    public void OnEndDrag(PointerEventData eventData)
    {
         
    }

    
}
