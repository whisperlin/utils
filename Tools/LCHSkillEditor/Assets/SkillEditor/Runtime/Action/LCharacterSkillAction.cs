using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCharacterHitData
{
    public int characterId;
    public HashSet<int> hittedObject = new HashSet<int>();
    public ObjDictionary value;
    public bool firstHit;//是否击中的第一个对象，用于产生动作停滞。
    public string cdName;
    public int skillState;
    public CdState cdState;
    public string effect = null;
    public GameObject effect_obj = null;
}
public class LCharacterSkillAction : LCharacterAction
{
    public string SkillId;
    public string cdName = "";//如果使用空，则默认使用SkillId
    public CdState cdState = CdState.NORMAL;
    public VirtualInput.KeyCode button;
    public int skillState = 0;//技能段数，多段技能使用。
    public bool isOnGround = true;
    protected LCHSkillData skillData;

    public bool HasLoaded
    {
        get
        {
            return hasLoaded;
        }
    }
    private bool hasLoaded = false;
    
    public static bool fixBeginPos = true;

    private ObjectContain role = new ObjectContain();
    private Dictionary<int,ObjectContain> objs = new Dictionary<int, ObjectContain>();
    private ObjectContain[] objList ;
    private Dictionary<int, AudioClip> clips = new Dictionary<int, AudioClip>();


    private Dictionary<string, SkillData> loadedSkillData = new Dictionary<string, SkillData>();
    private Vector3 baseGroundPos;
    private Vector3 beginPositon;
    private Quaternion beginLocalRot;
    private Vector3 beginLocalScale;


    public override bool isQualified(LCharacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information)
    {
        var pos0 = character.GetCurPosition();
        Vector3 pos;
        if (!information.getGroundHight(pos0, out pos))
        {
            Debug.LogError("has error");
            pos = character.GetCurPosition();
        }
        bool inAir = pos0.y - pos.y > 0.01f;
        if (VirtualInput.buttons[(int)button]  && (!isOnGround ||   !inAir )  && character.CanUsedSkill(cdName, skillState)  )
        {
            return true;
        }

        return false;
    }
    private  float curTime = 0f,lastTime = 0f ;
    public override IEnumerator onInit(LChatacterRecourceInterface loader, LChatacterInterface character, AddCoroutineFun fun)
    {
        Debug.Log("skill on init "+ SkillId);
        hasLoaded = false;
        CharactorLoadHandle handle0 = loader.LoadSkillDataFile(SkillId, fun);
        while (!handle0.isFinish)
            yield return null;
        skillData = (LCHSkillData)handle0.asset;
        //Debug.LogError("LoadSkillDataFile");
        role.type = 1;
        role.objId = -1;
        objs[role.objId] = role;
        objList = new ObjectContain[skillData.objs.Length];
        for (int i = 0, l = skillData.objs.Length; i < l; i++)
        {
            var o = skillData.objs[i];
            ObjectContain oc = new ObjectContain();
            oc.SetInformation(o);
            objs[o.id] = oc;
            objList[i] = oc;
            if (o.id == -1)//不加载主模型。
            {
                continue;
            }
            else if (o.type == 1)//对象(特效，模型...)。
            {
                //Debug.LogError("load " + oc.mod);
                var handle = loader.loadResource(oc.mod_name, oc.mod,fun);
                while (!handle.isFinish)
                {
                    yield return 1;
                }
                if (null != handle.asset)
                {
                    if (handle.asset == null)
                    {
                        Debug.LogError("找不到资源，是否没打包 ？ " + oc.mod);
                    }
                    else
                    {
                        oc.baseGameObject = (GameObject)handle.asset;
                        if (null != oc.baseGameObject)
                        {
                            oc.gameobject = GameObject.Instantiate(oc.baseGameObject);
                            oc.gameobject.SetActive(false);
                            oc.gameobject.name = o.name;
                            oc.gameobject.hideFlags = HideFlags.None;
                            oc.systems = oc.gameobject.GetComponentsInChildren<ParticleSystem>();
                        }
                           

                    }
                }
            }
            else if (o.type == 2)//(碰撞体)。
            {
                GameObject g = new GameObject();
                g.layer = character.GetAttackLayer();
                g.SetActive(false);
                g.AddComponent<BoxCollider>();
                g.hideFlags = HideFlags.DontSave;
                oc.gameobject = g;
                g.name = o.name;
            }
            else if (o.type == 3)//模型本身碰撞体。暂不连接。
            {
                continue;
            }
            else if (o.type == 4)//加载声音
            {
                GameObject g = new GameObject();
                g.name = o.name;
                oc.gameobject = g;
                loader.loadResource(oc.mod_name, oc.mod,fun);
            }
        }
        for (int i = 0, c0 = skillData.events.Count; i < c0; i++)
        {
            var _e = skillData.events[i];
            var contain = objs[_e.objId];
            contain.events.Add(_e);
        }

        for (int i = 0, c0 = skillData.channels.Count; i < c0; i++)
        {
            var _c = skillData.channels[i];
            var contain = objs[_c.objId];
            contain.channels.Add(_c);
        }

        hasLoaded = true;
        Debug.Log("skill on init finish "+ SkillId);
    }

    
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime = 0f;
        beginPositon = baseGroundPos = character.GetCurPosition();
        beginLocalRot = character.GetCurLocalRot();
        beginLocalScale = character.GetCurLoaclScale();
        lastTime = 0f;


        /*for (int i = 0, c0 = objList.Length; i < c0; i++)
        {
            var o0 = objList[i];
            if (o0.type == 1  )
            {
                if (null != o0.baseGameObject)
                {
                    Debug.LogError("bg "+o0.baseGameObject + " g "+o0.gameobject );
                }
            }
        }*/

        //Debug.Log("begin skill");
    }
    
    

    public static void CmpAnimationPos(float curTime,ObjectContain contain, LChatacterInterface character, LChatacterInformationInterface information)
    {
        contain.ResetTransformData();
        for (int i = 0, c0 = contain.channels.Count; i < c0; i++)
        {
            var channel = contain.channels[i];

            LCHChannelType t = (LCHChannelType)channel.type;
            switch (t)
            {
                case LCHChannelType.PosX:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.pos += new Vector3(v, 0f, 0f);
                        break;
                    }
                case LCHChannelType.PosY:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.pos += new Vector3(0f, v, 0f);
                        break;
                    }
                case LCHChannelType.PosZ:
                    {
                        LCHChannelData.debug = true;
                        float v = channel.GetLerpValue(curTime);
                        LCHChannelData.debug = false;
                        contain.pos += new Vector3(0f, 0f, v);
                        break;
                    }

                case LCHChannelType.RotY:
                    {

                        contain.rot = channel.GetSlerpYValue(curTime);
                        break;
                    }

                case LCHChannelType.ScaleX:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.scale = new Vector3(v, contain.scale.y, contain.scale.z);
                        break;
                    }
                case LCHChannelType.ScaleY:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.scale = new Vector3(contain.scale.x, v, contain.scale.z);
                        break;
                    }
                case LCHChannelType.ScaleZ:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.scale = new Vector3(contain.scale.x, contain.scale.y, v);
                        break;
                    }
            }
        }

    }


    public static void SetRolePos(float curTime, ObjectContain contain, LChatacterInterface character, LChatacterInformationInterface information,
        ref Vector3 beginGroundLinePos,
        Vector3 beginLocalScale,
        Quaternion beginLocalRot,
        bool fixBeginPos = true)
    {

        if (null != contain.gameobject)
        {
            //初始位置
            var oldPos = contain.gameobject.transform.position;
            contain.gameobject.transform.position = beginGroundLinePos;
            contain.gameobject.transform.localScale = beginLocalScale;
            contain.gameobject.transform.localRotation = beginLocalRot;

            //位置转向缩放
            contain.gameobject.transform.position = contain.gameobject.transform.localToWorldMatrix.MultiplyPoint(contain.pos);
            contain.gameobject.transform.localRotation *= contain.rot;
            contain.gameobject.transform.localScale = MathfHelper.Vector3Mul(beginLocalScale, contain.scale);


            //旧技能位置在水平线上的位置.
            var old_h_pos = oldPos;
            old_h_pos.y = beginGroundLinePos.y;

            //新技能位置在水平线上的位置
            var new_h_pos = contain.gameobject.transform.position;
            new_h_pos.y = beginGroundLinePos.y;
            var _dir = new_h_pos - old_h_pos;

            if (fixBeginPos)
            {
                //技能贴地运动
                var new_h_pos1 = information.tryMove(old_h_pos, _dir, true);
                var _delta = new_h_pos1 - new_h_pos;

                beginGroundLinePos = new Vector3(beginGroundLinePos.x + _delta.x, new_h_pos1.y, beginGroundLinePos.z + _delta.y);//修正技能水平线高度

                contain.gameobject.transform.position = new Vector3(new_h_pos1.x, contain.gameobject.transform.position.y, new_h_pos1.z);
            }
            else
            {
                //技能贴地运动
                new_h_pos = information.tryMove(old_h_pos, _dir, true);
                beginGroundLinePos = new Vector3(beginGroundLinePos.x, new_h_pos.y, beginGroundLinePos.z);//修正技能水平线高度

                contain.gameobject.transform.position = new Vector3(new_h_pos.x, contain.gameobject.transform.position.y, new_h_pos.z);
            }
        }
        
    }

    public static Transform FindInChildrenIncludingInactive(Transform t, string name)
    {

        for (int i = 0; i < t.childCount; i++)
        {
            Transform t2 = t.GetChild(i);
            //Debug.Log(t2.gameObject.name + "= " + name + " " + (name == t2.gameObject.name));
            if (t2.gameObject.name == name)
                return t2;
            Transform found = FindInChildrenIncludingInactive(t.GetChild(i), name);
            if (found != null)
                return found;
        }

        return null;  //couldn't find crap
    }
     
    public static void SetObjectPos(float curTime, ObjectContain contain, Dictionary<int, ObjectContain> objs , LChatacterInterface character, LChatacterInformationInterface information,
         Vector3 beginPositon,
        Vector3 beginLocalScale,
        Quaternion beginLocalRot )
    {
        //beginPositon
        if (contain.type == 1 || contain.type == 2 || contain.type == 4)
        {
            ObjectContain role = objs[-1];
            if (contain.bind == 1 && contain.gameobject != null && contain.gameobject.transform.parent == null && role.gameobject != null)
            {
                contain.gameobject.transform.parent = role.gameobject.transform;
            }
            else if (contain.bind == 2 && contain.gameobject != null && contain.gameobject.transform.parent == null && role.gameobject != null)
            {
                contain.gameobject.transform.parent = FindInChildrenIncludingInactive(role.gameobject.transform, contain.bindName);
            }
            else if (contain.bind == 0 && contain.gameobject != null  )
            {
                contain.gameobject.transform.parent = null;
                contain.gameobject.transform.position = beginPositon;
                contain.gameobject.transform.localRotation = beginLocalRot;
                contain.gameobject.transform.localScale = Vector3.one;
            }
            if (contain.bind == 0)
            {
                contain.gameobject.transform.position = contain.gameobject.transform.localToWorldMatrix.MultiplyPoint(contain.pos);
                contain.gameobject.transform.localRotation *= contain.rot;
            }
            else
            {
                contain.gameobject.transform.localPosition = contain.pos;
                contain.gameobject.transform.localRotation = contain.rot;
                contain.gameobject.transform.localScale = contain.scale;
            }
            contain.gameobject.transform.localScale = contain.scale;
        }
        //type3 不改位移
        else if (contain.type == 3)
        {
            if (contain.gameobject == null)
            {
                if (contain.bind == 2)
                {
                    ObjectContain c1 = objs[-1];
                    if (null != c1)
                    {
                        var t = FindInChildrenIncludingInactive(c1.gameobject.transform, contain.bindName);
                        contain.gameobject = t.gameObject;
                    }
                }
                else if (contain.bind == 1)
                {
                    ObjectContain c1 = objs[contain.bindObjId];
                    contain.gameobject = c1.gameobject;
                }
            }
        }
    }
    public static void CmpObjectEvent(float curTime,float lastTime, LCHEventChannelData _e, ObjectContain contain, LChatacterInterface character, LChatacterInformationInterface information,
        Dictionary<int, ObjectContain> objs
        ,CdState cdState
        , string cdName
        , int skillState
         
        )
    {
        LCHChannelType t = (LCHChannelType)_e.type;
        ObjDictionary value;
        float _time;
        if (t == LCHChannelType.Object)
        {
            int res = _e.TryGetKeyFrameRunTime(curTime, lastTime, out value, out _time);
            if (res == 1)
            {
                if (null != contain.gameobject)
                {
                    bool enable = value.GetValue<bool>("enable", true);
                    if (_e.objId == -1)
                    {
                        //因為主對象直接設置 gameObject的Enable,所有腳本都不工作了.
                        if (null != contain.roleRenders)
                        {
                            for (int n = 0; n < contain.roleRenders.Length; n++)
                            {
                                contain.roleRenders[n].enabled = enable;
                            }
                        }
                    }
                    else
                    {
                        
                        
                        if (enable)
                        {
                            contain.gameobject.SetActive(false);
                            contain.gameobject.SetActive(true);
                            /*contain.gameobject.SetActive(enable);
                            for (int i = 0, l = contain.systems.Length; i < l; i++)
                            {
                                //Debug.Log(contain.systems[i].isStopped);
                                contain.systems[i].gameObject.SetActive(false);
                                contain.systems[i].gameObject.SetActive(true);
                                contain.systems[i].Play(true);
                            }*/
                             
                        }
                        else
                        {
                            contain.gameobject.SetActive(enable);
                        }
                         
                        //Debug.LogError("_e.objId  " + _e.objId + " " + enable);
                        //contain.gameobject.SetActive(enable);
                    }

                    if (enable)
                    {
                        if (contain.type == 1)
                        {
                            string anim_name = value.GetValue<string>("anim", "");
                            if (anim_name.Length > 0)
                            {
                                if (null == contain.animaction)
                                {
                                    contain.animaction = contain.gameobject.GetComponent<Animation>();
                                }
                                if (null != contain.animaction)
                                {
                                    bool cf = value.GetValue<bool>("crossFade", true);
                                    //crossFade
                                    contain.animaction[anim_name].time = _time;
                                    if (cf)
                                    {
                                        contain.animaction.CrossFade(anim_name, 0.05f);
                                    }
                                    else
                                    {
                                        contain.animaction[anim_name].time=0f;//特么受击不这么干不重置。。。
                                        contain.animaction.Play(anim_name );
                                    }
                                   
                                }
                            }
                        }
                    }
                }
            }
            else if (res == 0)
            {
                 
                if (_e.objId != -1)//非主模型，一开始隐藏
                {
                    if (null != contain.gameobject)
                    {
                        if (contain.objId != -1)
                            contain.gameobject.SetActive(false);
                         
                    }
                }
            }
        }
        if (t == LCHChannelType.Event)
        {
            int res = _e.TryGetKeyFrameRunTime(curTime, lastTime, out value, out _time);
            if (res == 1)
            {
                if (contain.type == 2 || contain.type == 3)
                {
                    if (null == contain.collider)
                    {
                        if (null != contain.gameobject)
                        {
                            contain.collider = contain.gameobject.GetComponent<Collider>();
                            contain.gameobject.layer = character.GetAttackLayer();
                            if (null != contain.collider)
                            {
                                contain.collider.isTrigger = true;
                            }
                        }
                    }
                }
                if (null != contain.gameobject)
                {
                    bool enable = value.GetValue<bool>("enable", true);
                    contain.gameobject.SetActive(enable);
                    if (contain.collider)
                    {
                        contain.collider.enabled = enable;
                        if (enable)
                        {
                            if (null == contain.hitData)
                            {
                                LCharacterHitDataCmp hdc = contain.gameobject.GetComponent<LCharacterHitDataCmp>();
                                if (null == hdc)
                                {
                                    hdc = contain.gameobject.AddComponent<LCharacterHitDataCmp>();
                                }
                                contain.hitData = hdc.data;
                            }
                           
                            contain.hitData.type = "hit";
                            LCharacterHitData hitData = new LCharacterHitData();

                            hitData.hittedObject.Clear();
                            hitData.value = value;
                            hitData.characterId = character.GetId();
                            hitData.firstHit = true;

                            hitData.cdName = cdName;
                            hitData.skillState = skillState;
                            hitData.cdState = cdState;

                            int effectId = value.GetValueInt("hit_effect", -2);
                            if (effectId != -2 && objs.ContainsKey(effectId))
                            {
                                var ct = objs[effectId];
                                hitData.effect = ct.mod;
                                hitData.effect_obj = ct.baseGameObject;

                            }
                            else
                            {
                                hitData.effect = null;
                                hitData.effect_obj = null;
                            }
                            contain.hitData.data = hitData;

                        }
                    }
                }
            }
            else if (res == 0)
            {
                if (null != contain.gameobject)
                {
                    contain.gameobject.SetActive(false);
                }
            }
        }
    }
    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        
        if (!hasLoaded)
            return;

        role.ResetTransformData();
        for (int i = 0, c0 = objList.Length; i < c0; i++)
        {
            var o0 = objList[i];
            o0.ResetTransformData();
            
        }
       
        CmpAnimationPos(curTime,role,character,information );
        SetRolePos(curTime, role, character, information, ref baseGroundPos, beginLocalScale, beginLocalRot, true);
        for (int i1 = 0, c1 = objList.Length; i1 < c1; i1++)
        {
            var contain = objList[i1];
            CmpAnimationPos(curTime, contain, character, information );
        }
        //因为模型不是技能系统负责加载，所以最后一刻才负责绑定。
        if (null == role.gameobject)
        {
            role.gameobject = character.GetRoleAnimation().gameObject;
            role.roleRenders = character.GetRoleRender();
        }

         
        //避免调用字典foreach导致每帧产生 GC Allow
        for (int i = 0, c0 = objList.Length; i < c0; i++)
        {
            var contain = objList[i];
            SetObjectPos(curTime, contain, objs, character, information,
             beginPositon,
            beginLocalScale,
            beginLocalRot);
        }

        {
            var contain = role;
            for (int i = 0, c0 = contain.events.Count; i < c0; i++)
            {
                var _e = contain.events[i];
                CmpObjectEvent(curTime, lastTime, _e, contain, character, information,objs ,cdState,cdName,skillState );
            }
        }
        for (int ii = 0, c1 = objList.Length; ii < c1; ii++)
        {
            var contain = objList[ii];
            for (int i = 0, c0 = contain.events.Count; i < c0; i++)
            {
                var _e = contain.events[i];
                CmpObjectEvent(curTime, lastTime, _e, contain, character, information, objs, cdState, cdName, skillState );
            }
        }
         //计算物体事件，比如播放动作，声音等。
            for (int i = 0, c0 = skillData.events.Count; i < c0; i++)
        {
            var _e = skillData.events[i];
            var contain = objs[_e.objId];
            

        }
        lastTime = curTime;
    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime += Time.deltaTime;
        if (skillData != null && skillData.maxLength < curTime)
        {
            return true;
        }
        return false;
    }


    public override void onRelease()
    {
        for (int i = 0, c0 = objList.Length; i < c0; i++)
        {
            var o0 = objList[i];
            if (o0.objId != -1)
            {
                if (o0.objId != -1)
                {
                    if (null != o0.gameobject)
                    {
                        GameObject.DestroyImmediate(o0.gameobject);
                    }
                }
            }
        }
    }
    public override void endAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        if (cdState != CdState.HIT)
            character.updateCDState(cdName, skillState);
        for (int i = 0, c0 = objList.Length; i < c0; i++)
        {
            var o0 = objList[i];
            if (o0.objId != -1)
            {
                if (  o0.type == 2 || o0.type == 3)
                {
                    if (null != o0.gameobject)
                    {
                        o0.gameobject.SetActive(false);
                    }
                }
            }
        }
    }
}
