using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleJoystick : ScrollRect
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

	SimpleJoystick.STATE _state = SimpleJoystick.STATE.NONE;


	public SimpleJoystick.STATE state
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

	public virtual void OnBeginDrag (PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		_state = STATE.Down;

		var contentPosition = content.anchoredPosition;

		if (contentPosition.magnitude > _mRadius)
		{   


			contentPosition = contentPosition.normalized * _mRadius;
			SetContentAnchoredPosition(contentPosition);
		}
		//pos = contentPosition/_mRadius;

        pos = contentPosition.normalized;

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
        //pos = contentPosition/_mRadius;
		if(_state == STATE.NONE)
			_state = STATE.Down;
		else
			_state = STATE.DRAG;
		//_state = STATE.DRAG;
	 
	}
	public override void OnEndDrag (PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		pos = Vector2.zero;
		_state = STATE.Up;
	}
}
 