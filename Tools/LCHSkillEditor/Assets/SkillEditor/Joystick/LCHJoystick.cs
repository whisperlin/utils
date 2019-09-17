using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LCHJoystick : ScrollRect
{
	public enum STATE
	{
		NONE,
		Up,
		Down,
		DRAG,
	}
	private float _mRadius = 0f;
	private const float Dis = 0.5f;

    LCHJoystick.STATE _state = LCHJoystick.STATE.NONE;

    public static LCHJoystick.STATE golbalMoveCtrlState = STATE.NONE;

    //如果这个被钩上，则会设置 golbalMoveCtrlState的值。
    public bool isGlobalMoveCtrl = true;

    public LCHJoystick.STATE state
	{
		get 
		{
			if(_state == STATE.Up)
			{
				_state = STATE.NONE;
				return STATE.Up;
			}
			return _state;
		}
	}

	public Vector2 pos = Vector2.zero;

	protected override void Start()
	{
		base.Start();


		_mRadius = content.sizeDelta.x * Dis;
	}

	public override void OnBeginDrag (PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		_state = STATE.Down;

		var contentPosition = content.anchoredPosition;

		if (contentPosition.magnitude > _mRadius)
		{   


			contentPosition = contentPosition.normalized * _mRadius;
			SetContentAnchoredPosition(contentPosition);
		}

        
        pos = contentPosition.normalized;
        VirtualInput.dir = pos;
        VirtualInput.isDirectKeyDown = true;
        if (isGlobalMoveCtrl)
            golbalMoveCtrlState = _state;
        Debug.LogError("OnBeginDrag " + VirtualInput.dir);
    }
	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);

		var contentPosition = content.anchoredPosition;

		if (contentPosition.magnitude > _mRadius)
		{   
			contentPosition = contentPosition.normalized * _mRadius;
			SetContentAnchoredPosition(contentPosition);
		}
        pos = contentPosition.normalized ;
        VirtualInput.isDirectKeyDown = true;
        VirtualInput.dir = pos;
        //pos = contentPosition/_mRadius;
        if (_state == STATE.NONE)
			_state = STATE.Down;
		else
			_state = STATE.DRAG;
        if (isGlobalMoveCtrl)
            golbalMoveCtrlState = _state;
        Debug.LogError("OnDrag " + VirtualInput.dir);
    }
	public override void OnEndDrag (PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		pos = Vector2.zero;
        VirtualInput.dir = pos;
        VirtualInput.isDirectKeyDown = false;
        _state = STATE.Up;
        if (isGlobalMoveCtrl)
            golbalMoveCtrlState = _state;
        Debug.LogError("OnEndDrag " + VirtualInput.dir);
    }
}
 