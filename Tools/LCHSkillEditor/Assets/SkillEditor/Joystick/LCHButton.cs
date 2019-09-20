using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LCHButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerClickHandler,IPointerDownHandler
{
    public LCHJsButtonObject button;
    public LCharacterInterface character;
    public Material dirMat;
    public Material pointMat;
    int count = -1;
    private Vector2 beginPos;

    static LCHFellowCharacter fellowCharacter;
   
    static LCHButton holdingDrag = null;
    Vector3 skillDir;
    public float pointOperaSize;
    public Material randMat;

    bool dragging = false;
    static void CircularRing(Mesh m, float radius0, float radius1)
    {
        List<Vector3> vectors = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indexs = new List<int>();

        float deltaA = Mathf.PI / 180f;
        float _x0 = 0f;
        float _z0 = 0f;
        float _x1 = 0f;
        float _z1 = 0f;

        Vector3 _p0;
        Vector3 _p1;
        Vector3 p0;
        Vector3 p1;

        for (int i = 0; i < 361; i++)
        {
            float a = deltaA * i;
            float x = Mathf.Sin(a);
            float z = Mathf.Cos(a);

            float x0 = x * radius0;
            float z0 = z * radius0;

            float x1 = x * radius1;
            float z1 = z * radius1;

            if (i > 0)
            {
                _p0 = new Vector3(_x0, 0f, _z0);
                _p1 = new Vector3(_x1, 0f, _z1);

                p0 = new Vector3(x0, 0f, z0);
                p1 = new Vector3(x1, 0f, z1);

                vectors.Add(_p0);
                vectors.Add(_p1);
                vectors.Add(p0);
                vectors.Add(p1);
                uvs.Add(new Vector2(0.5f, 1f));
                uvs.Add(new Vector2(0.5f, 0f));
                uvs.Add(new Vector2(0.5f, 1f));
                uvs.Add(new Vector2(0.5f, 0f));

                int index = i - 1;
                indexs.Add(1 + index * 4);
                indexs.Add(0 + index * 4);
                indexs.Add(2 + index * 4);
                indexs.Add(1 + index * 4);
                indexs.Add(2 + index * 4);
                indexs.Add(3 + index * 4);

            }

            _x0 = x0;
            _z0 = z0;
            _x1 = x1;
            _z1 = z1;
        }
 
        m.vertices = vectors.ToArray();
        m.uv = uvs.ToArray();
        m.uv2 = uvs.ToArray();
        m.triangles = indexs.ToArray();
        m.RecalculateNormals();
    }
    static void BuildDirCtrl(Mesh m, float width, float length)
    {
        if (width >= length)
        {
            float dw = width * 0.5f;
            m.vertices = new Vector3[] {
                    new Vector3(-dw, 0, 0), new Vector3(0, 0, 0), new Vector3(-dw, 0, length), new Vector3(0, 0, length) ,
                    new Vector3(0, 0, 0), new Vector3(dw, 0, 0), new Vector3(0, 0, length), new Vector3(dw, 0, length) };
            m.uv = new Vector2[] {
                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
                    new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1)};
            m.uv2 = m.uv;
            m.triangles = new int[] {
                    0, 2, 1, 1, 2, 3,
                    4, 6, 5, 5, 6, 7
                };
            m.RecalculateNormals();
        }
        else
        {
            float undraw = 0.1f;
            float dw = width * 0.5f;
            float l1 = length - width * 1 - (undraw);
            float l2 = 0f;

            m.vertices = new Vector3[] {
                    new Vector3(-dw, 0, l1), new Vector3(0, 0, l1), new Vector3(-dw, 0, length), new Vector3(0, 0, length) ,
                    new Vector3(0, 0, l1), new Vector3(dw, 0, l1), new Vector3(0, 0, length), new Vector3(dw, 0, length),

                     new Vector3(-dw, 0, l2), new Vector3(0, 0, l2), new Vector3(-dw, 0, l1), new Vector3(0, 0, l1) ,
                    new Vector3(0, 0, l2), new Vector3(dw, 0, l2), new Vector3(0, 0, l1), new Vector3(dw, 0, l1)
            };
            m.uv = new Vector2[] {
                    new Vector2(0, undraw), new Vector2(1, undraw), new Vector2(0, 1), new Vector2(1, 1),
                    new Vector2(1, undraw), new Vector2(0, undraw), new Vector2(1, 1), new Vector2(0, 1),

                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, undraw), new Vector2(1, undraw),
                    new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, undraw), new Vector2(0, undraw)

            };
            m.uv2 = m.uv;
            m.triangles = new int[] {
                    0, 2, 1, 1, 2, 3,
                    4, 6, 5, 5, 6, 7,
                    8, 10, 9, 9, 10, 11,
                    12, 14, 13, 13, 14, 15
                };
            m.RecalculateNormals();
        }
    }


    static void BuildPointCtrl(Mesh m, float width )
    {
        float dw = width  ;
        m.vertices = new Vector3[] { new Vector3(-dw, 0, -dw), new Vector3(dw, 0, -dw), new Vector3(-dw, 0, dw), new Vector3(dw, 0, dw) };
        m.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        m.uv2 = m.uv;
        m.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
        m.RecalculateNormals();

    }
    static void initMesh(Material dirMat,Material randMat)
    {
 
        if (null != fellowCharacter)
            return;
        
        GameObject dirMeshObject = new GameObject("directer");
        fellowCharacter = dirMeshObject.AddComponent<LCHFellowCharacter>();
        fellowCharacter.dirMesh = new Mesh();
        fellowCharacter.pointMesh = new Mesh();

        //dirMesh.hideFlags = HideFlags.HideAndDontSave;
        
        
        fellowCharacter.filter = dirMeshObject.AddComponent<MeshFilter>();
        fellowCharacter.filter.sharedMesh = fellowCharacter.dirMesh;
        fellowCharacter.meshRender =  dirMeshObject.AddComponent<MeshRenderer>();
        fellowCharacter.meshRender.sharedMaterial = dirMat;

        fellowCharacter.rangeGamObject = new GameObject("rand");

        Mesh randMesh = new Mesh();
        fellowCharacter.randMesh = randMesh;
        fellowCharacter.rangeGamObject.AddComponent<MeshFilter>().sharedMesh = randMesh;
        fellowCharacter.rangeGamObject.AddComponent<MeshRenderer>().sharedMaterial = randMat;
        CircularRing(randMesh, 1.0f, 0.8f);

        GameObject.DontDestroyOnLoad(fellowCharacter.dirMesh);
        GameObject.DontDestroyOnLoad(randMesh);
        GameObject.DontDestroyOnLoad(fellowCharacter.rangeGamObject);
        GameObject.DontDestroyOnLoad(dirMeshObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dragging)
            return;
        var _param = character.GetSkillCDSkillParams(button.information.cdName);
        if (null == _param)
            return;
        VirtualInput.KeyCode bt = _param.button;
        VirtualInput.skillDir = Vector3.zero;
        VirtualInput.buttons[(int)bt] = true;
        VirtualInput.js_buttons[(int)bt] = true;
        if (holdingDrag == this)
        {
            fellowCharacter.gameObject.SetActive(false);
            fellowCharacter.rangeGamObject.SetActive(false);
            holdingDrag = null;
        } 
        count = 2;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (holdingDrag != null)
        {
            return;
        }
        dragging = false;

        var _param = character.GetSkillCDSkillParams(button.information.cdName);
        if (null == _param)
            return;
        
        //Debug.LogError("_param.isUsing = " + _param.isUsing);
        if (_param.isUsing == false 
            &&
            (
            (_param.State == 0 && _param.cd <0.0001f ) ||
            (_param.State > 0 && _param.cd >0.0001f )
            )
            
            )
        {
           
            if (_param.type == SkillParams.TYPE.DRAG_DIR)
            {
                initMesh(dirMat, randMat);

                BuildDirCtrl(fellowCharacter.dirMesh, _param.skillWidth, _param.skillRange);
                
                fellowCharacter.transform.position = character.GetCurPosition() + Vector3.up * 0.1f;
                fellowCharacter.forward = character.GetCurForward();
                fellowCharacter.meshRender.sharedMaterial = dirMat;
                fellowCharacter.filter.sharedMesh = fellowCharacter.dirMesh;
                fellowCharacter.type = _param.type;
                fellowCharacter.meshRender.enabled = true;

            }
            else if (_param.type == SkillParams.TYPE.DRAG_POINT)
            {
                initMesh(dirMat, randMat);
 

                BuildPointCtrl(fellowCharacter.pointMesh, _param.skillWidth);
 
                fellowCharacter.transform.position = character.GetCurPosition() + Vector3.up * 0.1f;
                fellowCharacter.forward = character.GetCurForward();
                fellowCharacter.meshRender.sharedMaterial = pointMat;
                fellowCharacter.filter.sharedMesh = fellowCharacter.pointMesh;
                fellowCharacter.type = _param.type;
                fellowCharacter.skillRange = _param.skillRange;
                fellowCharacter.meshRender.enabled = true;

            }
            else
            {
                initMesh(dirMat, randMat);
                BuildPointCtrl(fellowCharacter.pointMesh, _param.skillWidth);
                fellowCharacter.meshRender.enabled = false;


            }
            
            fellowCharacter.character = character;
            holdingDrag = this;
            fellowCharacter.gameObject.SetActive(true);
            fellowCharacter.rangeGamObject.SetActive(true);
            fellowCharacter.rangeGamObject.transform.localScale = new Vector3(_param.skillRange, 1, _param.skillRange);

        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (holdingDrag == this)
        {
            dragging = true;
            beginPos = eventData.position;
            var _param = character.GetSkillCDSkillParams(button.information.cdName);
            if (null == _param)
                return;
            if (_param.type == SkillParams.TYPE.DRAG_DIR)
            {
                fellowCharacter.character = character;
                fellowCharacter.transform.position = character.GetCurPosition() + Vector3.up * 0.1f;
            }
            else if (_param.type == SkillParams.TYPE.DRAG_POINT)
            {
                fellowCharacter.character = character;
                fellowCharacter.transform.position = character.GetCurPosition() + Vector3.up * 0.1f;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        var p = eventData.position;
        
        var _param = character.GetSkillCDSkillParams(button.information.cdName);
        if (null == _param)
            return;
        if (_param.type == SkillParams.TYPE.DRAG_DIR)
        {
            Vector2 dir = (p - beginPos).normalized;
            if (holdingDrag == this)
            {
                fellowCharacter.transform.position = character.GetCurPosition() + Vector3.up * 0.1f;
 
                Vector3 forward = character.GetGameForward();
                Vector3 left = Vector3.Cross(forward, Vector3.up);
                fellowCharacter.forward  = skillDir = (-left * dir.x + forward * dir.y).normalized;
        
            }

        }
        else if (_param.type == SkillParams.TYPE.DRAG_POINT)
        {
            float len = Vector2.Distance(p , beginPos);
             
            if (len > pointOperaSize)
                len = pointOperaSize;
            float t = len / pointOperaSize;
            Vector2 dir = (p - beginPos).normalized * _param.skillRange *t;
 
            fellowCharacter.transform.position = character.GetCurPosition() + Vector3.up * 0.1f;

            Vector3 forward = character.GetGameForward();
            Vector3 left = Vector3.Cross(forward, Vector3.up);
            fellowCharacter.forward = skillDir = (-left * dir.x + forward * dir.y)  ;
        }
    }

    

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        var _param = character.GetSkillCDSkillParams(button.information.cdName);
        if (null == _param)
            return;
        VirtualInput.KeyCode bt = _param.button;
        VirtualInput.buttons[(int)bt] = true;
        VirtualInput.js_buttons[(int)bt] = true;

        if (holdingDrag == this)
        {
            VirtualInput.skillDir = skillDir;
            holdingDrag = null;
            fellowCharacter.gameObject.SetActive(false);
            fellowCharacter.rangeGamObject.SetActive(false);
        }
        count = 2;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //隔一帧重置按钮状态位.
        if (count > 0)
        {
            if (count == 1)
            {
                var _param = character.GetSkillCDSkillParams(button.information.cdName);
                if (null == _param)
                    return;
                VirtualInput.KeyCode bt = _param.button;
                VirtualInput.buttons[(int)bt] = false;
                VirtualInput.js_buttons[(int)bt] = false;

            }
            count--;
        }

    }

    }
