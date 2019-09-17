using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCharacterFellowCamera : MonoBehaviour {

 
    public Camera cam;
    public Transform target;
    
    

    public float distance = 10f;
    public float xRot = 45f;
    public float yRot = 30f;


    public float distanceDelta = 10f;
    public float RotDelta = 45;

    float cur_distance = 10f;
    float cur_xRot = 45f;
    float cur_yRot = 30f;

    //[Header("事件所在层")]
    //public int eventLayer = 31;


    //target;
    //SphereCollider sphereCollider = null;
  
    float RotTo(float des,float src,float moveDelta)
    {
        while (des < src)
        {
            src -= 360f;
        }
        while (des > src + 360f)
        {
            src += 360f;
        }
        float delta0 = des - src;
        //选择旋转方向.
        if (delta0 > 180f)
        {
            src += 360f;
            delta0 = des - src;
      
        }
        return  MoveToLerp(des, src, moveDelta) ;
    }

    float MoveToLerp(float des, float src, float moveDelta)
    {
        float delta = des - src;
        if (delta == 0)
        {
            return des;
        }
        
        float absDelta = Mathf.Abs(delta);
        float sign = delta > 0 ? 1f : -1;
        if (absDelta < moveDelta)
        {
            return des;
        }
        else
        {
            return src + moveDelta * sign;
        }
        
    }
    public void GoToTarget()
    {
        //确保大于0f.
        if (distanceDelta < 0.001f)
            distanceDelta = 0.001f;
        if (RotDelta < 0.001f)
            RotDelta = 0.001f;

        if (yRot > 90f)
            yRot = 90f;

        if (yRot < -90f)
            yRot = -90f;



        cur_distance = MoveToLerp(distance, cur_distance, Time.deltaTime * distanceDelta);
        cur_xRot = RotTo(xRot, cur_xRot,Time.deltaTime * RotDelta);
        cur_yRot = RotTo(yRot, cur_yRot, Time.deltaTime * RotDelta);
 
    }
    public void SetToTarget()
    {
        cur_distance = distance;
        cur_xRot = xRot;
        cur_yRot = yRot;
    }

    private void Start()
    {
  
        SetToTarget();
    }
    

    void LateUpdate () {
        
        if (null == cam)
        {
            cam = Camera.main;
        }
        if (null == cam)
            return;
        if (null == target)
            return;

        
        /*if (null == sphereCollider)
        {
            GameObject g = new GameObject("ForEvent");
            sphereCollider = g.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            g.transform.parent = target;
            Rigidbody r = g.AddComponent<Rigidbody>();
            r.isKinematic = true;
            r.useGravity = false;
            g.layer = eventLayer;
            SimpleTarget st = g.AddComponent<SimpleTarget>();
            st.sfc = this;
            g.transform.localPosition = Vector3.zero;
        }*/
        GoToTarget();

        if (null != target.parent)
        {
            target.parent = null;
        }
        cam.transform.position = target.position;
        cam.transform.rotation = Quaternion.identity;

        cam.transform.Rotate(Vector3.up, cur_xRot );
        cam.transform.Rotate(Vector3.right, cur_yRot);
        
        cam.transform.position = cam.transform.position - cam.transform.forward * cur_distance;


    }
}
