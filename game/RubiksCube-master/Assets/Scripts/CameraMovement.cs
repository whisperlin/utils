using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public int speed = 5;
    float x = 45.0f;
    float y = 30.0f;
    bool rotating = false;

    void LateUpdate()
    {
        if (RubiksCube.CCount==4)
            transform.localPosition = new Vector3(20,0,0);
        else
            transform.localPosition = new Vector3(15, 0, 0);
        if (RubiksCube.operating)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)))
                rotating = true;
        }
        // MouseMoveDirection = new Vector2( Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        else if (Input.GetMouseButtonUp(0))
        {
            rotating = false;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 MouseMoveDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))*100;

            x += MouseMoveDirection.x * speed * Time.deltaTime;
            y -= MouseMoveDirection.y * speed * Time.deltaTime;
            y = Mathf.Clamp(y, -85, 85);
        }
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        if (!Physics.Raycast(Camera.main.ScreenPointToRay(touch.position)))
                            rotating = true;
                        break;
                    }
                case TouchPhase.Moved:
                    {
                        if (rotating)
                        {
                            x += touch.deltaPosition.x * speed * Time.deltaTime;
                            y -= touch.deltaPosition.y * speed * Time.deltaTime;
                            y = Mathf.Clamp(y, -85, 85);
                        }
                        break;
                    }
                case TouchPhase.Ended:
                    {
                        rotating = false;
                        break;
                    }
                default: break;
            }
        }
        if (x % 45 == 0)
            x += 1;
        Quaternion rotation = Quaternion.Euler(0, x, y);
        transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, rotation, Time.deltaTime * speed);
    }
}
