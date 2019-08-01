using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
//插入两个接口：
public class DragImageHyp : MonoBehaviour, IDragHandler, IEndDragHandler 
{
    Vector3 direction;
    public float maxDistanceHyp = 70;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        if (Vector3.Distance(transform.localPosition, Vector3.zero) > maxDistanceHyp)
        {
            direction = Vector3.Normalize( transform.localPosition - Vector3.zero);
            transform.localPosition = direction.normalized * maxDistanceHyp;
        }
    
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        
        transform.localPosition = Vector3.zero;
    }
}
