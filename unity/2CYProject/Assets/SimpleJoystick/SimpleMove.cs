using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(SimpleFellowCamera))]
public class SimpleMove : MonoBehaviour {

	public SimpleJoystick joystick = null;

    
    public float speed = 5f;

    //public Transform target;
	[Header("角色对象")]
    public Animation animationObj = null;
	[Header("飞行模式")]
	public bool fly = false;
    public string idle = "Idle";
    public string run = "Run";
    SimpleFellowCamera fc;

	Transform target ;
    
    GameObject cmp;
    //可以登上的斜坡度数。
    [Header("可以登上的斜坡的最大角度")]
    public float arge = 30f;
	// Use this for initialization
	void Start () {
		if (null == joystick) {
            joystick = GameObject.FindObjectOfType<SimpleJoystick>();
		}

        fc = GetComponent<SimpleFellowCamera>();
        

        NavMeshTriangulation tmpNavMeshTriangulation = NavMesh.CalculateTriangulation();

        hasMavMesh = tmpNavMeshTriangulation.vertices.Length > 0;

       

    }
    [Header("强制物理碰撞")]
    public bool forePhy = true;
    bool hasMavMesh = true;

    bool TryMove(Vector3 dir)
    {
		if (fly) {
			target.position = target.position + dir;
			return true;
		}

        if (hasMavMesh  )
        {
            return TryNavMesMove(dir);
        }
        else
        {
            return TryPhysiceMove(dir);
        }

     
    }
    bool TryNavMesMove(Vector3 dir)
    {
 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target.position+dir , out hit, 1.0f, NavMesh.AllAreas))
        {
            target.position = hit.position;
            return true;
        }

        return false;
    }

    bool TryPhysiceMove(Vector3 dir)
    {
		
        //角度转幅度。
        float tan = Mathf.Tan(arge * Mathf.PI / 180f);
        RaycastHit hit;
        Vector3 newPos = target.position + dir + Vector3.up * tan;
        
        if (Physics.Raycast(newPos, Vector3.down,out hit, 10f))
        {
            target.position = hit.point;
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
		if (null == animationObj)
            return;
		target = animationObj.transform;
        fc.target = target;
        if(null != target.parent)
        {

            target.parent = null;
        }
        
        if (null == cmp)
        {
            cmp = new GameObject("test");
            //cmp.hideFlags = HideFlags.HideAndDontSave;
        }
		//Transform target;
        

        Collider c =  target.GetComponent<Collider>();
        if (c)
            c.enabled = false;
        if (null == joystick)
            return;


        


		cmp.transform.forward  = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
		cmp.transform.position = Camera.main.transform.position;
        //cmp.transform.Rotate(Vector3.up, fc.xRot); 


        if (joystick.state == SimpleJoystick.STATE.Up)
		{


		}

		else if (joystick.state == SimpleJoystick.STATE.Down)
		{
            Vector3 dir = cmp.transform.right * joystick.pos.x + cmp.transform.forward * joystick.pos.y;
            TryMove(dir * speed * Time.deltaTime);

		}
		else if (joystick.state == SimpleJoystick.STATE.DRAG)
		{

            Vector3 dir =  cmp.transform.right* joystick.pos.x + cmp.transform.forward * joystick.pos.y;
            target.transform.forward = dir;
            TryMove(dir * speed * Time.deltaTime);

            //Vector3 dir2 = new Vector3(joystick.pos.x, 0, joystick.pos.y);
			if (!animationObj.IsPlaying(run))
            {
				animationObj.Play(run);
            }
            
		}
		else
		{
			if (!animationObj.IsPlaying(idle))
            {
				animationObj.Play(idle);
            }

        }
       
    }
}
