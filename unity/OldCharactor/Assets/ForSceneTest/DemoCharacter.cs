using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityExtend.Game.Scene;
using System.IO;
using System.Runtime.InteropServices;
using System;

public static class Util {
    public static float Epsilon = 0.0001f;
    public static bool IsZero(this Vector3 v1) {
        if (v1.x.IsZero()
            && v1.y.IsZero()
            && v1.z.IsZero()) {
            return true;
        }
        return false;
    }

    public static bool IsZero(this float v1) {
        return (Mathf.Abs(v1) <= Epsilon);
    }
}

public class NavExt {
    const string LUADLL = "UnityExtendNative";
    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool LoadNavFromLocal(string navPath);
}

public enum ActorMoveStatus {
    eStop,
    ePathing,
}

public class DemoCharacter : MonoBehaviour {
    public bool useNav = false;
    UnityEngine.AI.NavMeshAgent m_agent;
    Camera m_camera;
	Vector3 m_cam_offset;
	Animation m_animation;
	const float SPEED = 6;
	const float CAM_LERP_STEP = 0.15f;
	public float y_offset = 1;
	public float camera_distance = 15.5f;

    protected ActorMoveStatus _moveStatus = ActorMoveStatus.eStop;

    MateScene mateScene = new MateScene();
    UnityEngine.Transform TF = null;

    protected bool _isMoveTo = false;
    private float _acceleratorTime;
    private bool _ispathing;
    private bool _pauseByCorner;
    private Vector3 _velocity = Vector3.zero;
    private float _distance = 0;
    private float _distanceScalc = 0f;

    private Vector3 _moveStartPoint;

    private float _moveStartTime;
    protected float _serialMoveStartTime;

    private float _moveTimeElapased;

    protected float _remainingDistance;
    private float _currentMoveSpeed;

    private float _acceleratorEndTime;
    private float _acceleration;

    public static readonly float MoveAcceleratorTime = 0.2f;

    void Start()
    {
#if UNITY_EDITOR

        m_camera = Camera.main;
        m_animation = GetComponent<Animation>();

        if (this.useNav) {
            m_agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }
        else {
            this.TF = this.gameObject.transform;

            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (scene == null) {
                throw new System.Exception("scene not found!");
            }
            string blockFileName = Path.Combine("../data/configs/block", Path.GetFileNameWithoutExtension(scene.name) + ".block");
            if (!File.Exists(blockFileName)) {
                throw new System.Exception("block not found!");
            }

            string navFileName = Path.Combine("../data/configs/block", Path.GetFileNameWithoutExtension(scene.name) + ".pav");
            if (!File.Exists(navFileName)) {
                throw new System.Exception("nav not found!");
            }

            var blockData = File.ReadAllBytes(blockFileName);

            this.mateScene.SetMateInfo(blockData);

            NavExt.LoadNavFromLocal(navFileName);
        }
#endif
    }

	void OnStartMove() {
		m_animation.CrossFade("Run");
	}

	void OnMove(Vector2 dir) {
		var dir_world = m_camera.transform.rotation * new Vector3(dir.x, 0, dir.y);
		dir_world.y = 0;
		dir_world.Normalize();

		//var offset = dir_world * SPEED * Time.deltaTime;

		//transform.forward = dir_world;
        //m_agent.Move(offset);

        if(useNav) {
            var offset = dir_world * SPEED * Time.deltaTime;

            transform.forward = dir_world;
            m_agent.Move(offset);
        }
        else {
            doJoyMove(this.GetPositon(), dir_world);
        }
    }

    private void doJoyMove(Vector3 orign, Vector3 movement) {
        float fDistance = movement.magnitude;
        if (fDistance <= 0.3f) {
            return;
        }

        Vector3 dest = orign + movement * SPEED;

        //NavMeshHit navMeshHit;
        //bool navHit = TerrainUtil.GetNavMeshPosition(dest, out navMeshHit);
        //if (!navHit) { //长距离不可走，那么走一小步把
        //    dest = orign + movement;
        //    navHit = TerrainUtil.GetNavMeshPosition(dest, out navMeshHit);
        //    if (!navHit) {
        //        this.pathComponent.SetRotation(movement);
        //        return;
        //    }
        //}

        //流畅模式可以注释掉这个，不过也有其他的问题需要解决。可能需要对路径的线路条数进行限制
        //NavMesh.Raycast(owner.TF.position, navMeshHit.position,out navMeshHit, NavMesh.AllAreas);
        m_animation.CrossFade("Run");

        var result = this.StartMove(dest, MoveAcceleratorTime, true,  false, true);
        if (!result) {
            Vector3 minDist = Vector3.zero;

            for (int i = 0; i < 6; ++i) {
                var dir = Quaternion.AngleAxis(i * 10, Vector3.up) * movement;
                Vector3 tmpDst = orign + dir * 2f;
                Vector3 f;


                if (!RayCast(orign, tmpDst, out f)) {
                    continue;
                }

                float len = Vector3.Distance(orign, f);
                if (len > 0.01f) {
                    minDist = tmpDst;
                    break;
                }
                //Debug.DrawLine(orign, tmpDst, Color.red, 5);

                dir = Quaternion.AngleAxis(i * -10, Vector3.up) * movement;
                tmpDst = orign + dir * 2f;

                if (!RayCast(orign, tmpDst, out f)) {
                    continue;
                }

                len = Vector3.Distance(orign, f);
                if (len > 0.01f) {
                    minDist = tmpDst;
                    break;
                }
            }

            //Vector3 newDest = orign + minDir * 2f;
            //Debug.DrawLine(orign, newDest, Color.black, 5);
            //Debug.LogFormat("{0}", minAngle);

            if (!minDist.IsZero()) {
#if UNITY_EDITOR
                //Debug.DrawLine(orign, newDest, Color.red, 5);
#endif
                //Vector3 lastPosition;
                //RayCast(orign, newDest, out lastPosition);
                Vector3 lastPosition;

                if (!RayCast(orign, minDist, out lastPosition)) {
                    return;
                }

                result = this.StartMove(lastPosition, MoveAcceleratorTime, true, true);

                //minDist.y = this.logicMgr.MateScene.GetPositionHeight(minDist.x, minDist.z);
                //result = this.StartMove(minDist, MoveAcceleratorTime, true, true, true);
            }
        }
    }
    void OnStop() {
		m_animation.CrossFade("Idle");

        if(!this.useNav) {
            this.StopMove();
        }
	}

	void Update() {
        float distance = Input.GetAxis("Mouse ScrollWheel");
        if (distance != 0) {
            camera_distance += distance * 8;
        } 

		var target_pos = transform.position + new Vector3(0, y_offset, 0) - m_camera.transform.forward * camera_distance;

		var lerp = Vector3.Lerp(m_camera.transform.position, target_pos, CAM_LERP_STEP);
		m_camera.transform.position = lerp;

        if (!this.useNav) {
            this.UpdateMoving();
        }
	}

    public Vector3 GetPositon() {
        return this.gameObject.transform.position;
    }

    public bool RayCast(Vector3 orign, Vector3 dest, out Vector3 lastPosition) {
        var result = UnityExtend.DLL.NavMesh.Raycast(-this.GetPositon().x, this.GetPositon().y, this.GetPositon().z, -dest.x, dest.y, dest.z,
            out lastPosition.x, out lastPosition.y, out lastPosition.z);
        if (result && !(orign - lastPosition).IsZero()) {
            lastPosition.x = -lastPosition.x;

            this.mateScene.BresenhamLine(this.GetPositon(), lastPosition, out lastPosition);
            return true;
        }

        return false;
    }

    public bool StartMove(Vector3 dist, float acceleratorTime, bool pauseByCorner = false, bool forceRun = false, bool rayCast = false) {
        bool success = false;

        Vector3 oldDist = dist;

        var myPosition = this.GetPositon();
        Vector3 targetPoint = Vector3.zero;

        if (!forceRun) {
            Vector3 lastPosition;
            if (RayCast(this.GetPositon(), dist, out lastPosition)) {
                targetPoint = lastPosition;

                float len = Vector3.Distance(this.GetPositon(), targetPoint);
                if (len > 0.01f) {
                    //可移动区域太小了, 给一个偏转
                    success = true;
                }
            }
        }
        else {
            targetPoint = dist;
            success = true;
        }

        if (!success) {
            SetDirection(oldDist - this.GetPositon(), false);
            return success;
        }

        this._isMoveTo = false;
        return _startMove(targetPoint, SPEED, acceleratorTime, pauseByCorner, true);
    }

    private bool _startMove(Vector3 targetPoint, float speed, float acceleratorTime,  bool pauseByCorner, bool changeDir) {
        //Debug.LogFormat("_startMove from {0} to {1}, dir {2}", this.GetPositon(), targetPoint, (targetPoint - this.GetPositon()).normalized);

        this._acceleratorTime = acceleratorTime;
        this._pauseByCorner = pauseByCorner;

        this._targetPoint = targetPoint;
        _velocity = _targetPoint - this.TF.position;
        Vector3 tempDistance = _velocity;
        tempDistance.y = 0;
        _distance = tempDistance.magnitude; //注意这边要用平面距离

        if (_distance.IsZero()) {
            //说明寻路失败.
            _distance = 0f;
            return false;
        }


        if (!_ispathing) {
            _ispathing = true;
        }


        _distanceScalc = _velocity.magnitude / _distance;
        _velocity.Normalize();

        if (changeDir) {
            SetDirection(_targetPoint - this.GetPositon(), false);
        }

        _moveStartPoint = this.TF.position;

        this._moveStartTime = Time.time;
        this._moveTimeElapased = 0f;

        this._remainingDistance = _distance;
        this._currentMoveSpeed = speed;

        if (acceleratorTime != 0f && _moveStatus != ActorMoveStatus.ePathing) {
            this._acceleratorEndTime = Time.time + acceleratorTime;
            this._acceleration = SPEED / acceleratorTime;
        }
        else {
            this._acceleratorEndTime = 0f;
            this._acceleration = 0f;
        }

        _moveStatus = ActorMoveStatus.ePathing;

        return true;
    }

    protected Vector3 _targetPoint;
    bool _rotating = false;
    Vector3 _targetDir;


    public virtual void SetDirection(Vector3 dir, bool immediately = true) {
        //Debug.LogFormat("SetDirection from {0} to {1}", this.GetDir(), dir.normalized);
        if (immediately) {
            this._rotating = false;
            if (!dir.IsZero()) {
                dir.y = 0f;
                this.TF.rotation = Quaternion.LookRotation(dir);
            }
        }
        else {
            _targetDir = dir;
            _targetDir.y = 0f;
            _rotating = true;
        }
    }

    public void UpdateRotation() {
        if (!_rotating) {//|| _moveType != ActorMoveType.eTouch) {
            return;
        }

        if (_targetDir.IsZero()) {
            _rotating = false;
            return;
        }

        Quaternion rot = Quaternion.LookRotation(_targetDir);

        float angle = Quaternion.Angle(this.TF.rotation, rot);
        if (angle <= 0.01f) {
            this.TF.rotation = rot;
            _rotating = false;
            return;
        }

        float rt = 14.0f * Time.deltaTime;
        this.TF.rotation = Quaternion.Slerp(this.TF.rotation, rot, rt);
    }

    protected virtual void _SetPosition(float posX, float posY) {
        Vector3 pos = Vector3.zero;
        pos.x = posX;
        pos.y = 0;
        pos.z = posY;
        _SetPosition(pos);
    }
    protected virtual void _SetPosition(Vector3 pos) {
        if (pos.y == 0) {
            pos.y = this.mateScene.GetPositionHeight(pos.x, pos.z, 0);
        }

        Debug.Assert(pos.y != 0);
        this.TF.position = pos;
    }

    public virtual void SetPosition(float posX, float posY) {
        _SetPosition(posX, posY);
    }

    public virtual void SetPosition(Vector3 pos) {
        _SetPosition(pos);
    }


    public void UpdateMoving() {
        if (_rotating) {
            this.UpdateRotation();
        }

        if (_moveStatus == ActorMoveStatus.eStop) {
            return;
        }

        if (_remainingDistance.IsZero()) {
            return;
        }

        this._moveTimeElapased += Time.deltaTime;

        float timeElapased = this._moveTimeElapased;

        float acceMoveLen = 0f;

        if (this._acceleratorEndTime != 0f) {

            float accTime = (timeElapased < _acceleratorTime) ? timeElapased : _acceleratorTime;
            timeElapased -= accTime;
            float t2 = accTime * accTime;
            acceMoveLen = this._acceleration * t2 * 0.5f;

            if (acceMoveLen > _distance) {
                acceMoveLen = _distance;
            }

            if (accTime >= _acceleratorTime) {
                Vector3 _realOffset = _velocity * acceMoveLen * _distanceScalc;
                _moveStartTime += _acceleratorTime;
                this._moveTimeElapased -= _acceleratorTime;
                this._acceleratorEndTime = 0f;
                this._moveStartPoint = this._moveStartPoint + _realOffset;
                _distance -= acceMoveLen;
                acceMoveLen = 0;
            }
        }

        float len = this._currentMoveSpeed * timeElapased + acceMoveLen;
        float moveLen = 0f;

        if (len > _distance) {
            moveLen = _distance;
        }
        else {
            moveLen = len;
        }

        //Debug.LogFormat("{0} move len {1} {2} {3} {4} {5}", this.actorName, len, this.Speed, this._currentMoveSpeed, timeElapased, acceMoveLen);

        //if(false) {
        //    processOldMove();
        //}
        //else {
        Vector3 newPoint = this._moveStartPoint + _velocity * moveLen * _distanceScalc;
        //var ll = Time.deltaTime * this.Speed;
        //if (ll > _remainingDistance) {
        //    ll = _remainingDistance;
        //}
        // Debug.LogFormat("move len {0:0.00000}.", (_remainingDistance - (_distance - moveLen)) - ll);
        newPoint.y = this.mateScene.GetPositionHeight(newPoint.x, newPoint.z, this.GetPositon().y);
        this._SetPosition(newPoint);

        _remainingDistance = _distance - moveLen;
        //_remainingDistance -= ll;

        if (_remainingDistance.IsZero()) {
            _remainingDistance = 0;
            _distance = 0;

            // Debug.LogFormat("wrap {0} to {1}", this.TF.position, _targetPoint);
            _targetPoint.y = this.mateScene.GetPositionHeight(_targetPoint.x, _targetPoint.z, this.GetPositon().y);
            this._SetPosition(_targetPoint);
            if (!this._pauseByCorner) {
                this.StopMove();
            }
        }
        // }
    }

    public virtual void StopMove(bool localStop = true) {
        _distance = 0;
        _ispathing = false;
        _moveStatus = ActorMoveStatus.eStop;
    }

}

