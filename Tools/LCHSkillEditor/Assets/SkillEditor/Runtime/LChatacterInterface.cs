using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public interface CharactorLoadHandle
{
    object asset {get;}
    bool isFinish { get; }
    string Error { get; }
}

public delegate void AddCoroutineFun(IEnumerator fun);
public interface LChatacterRecourceInterface
{
    void AddCoroutine(IEnumerator fun);
    void LoadSound(string name, string path);
    void PlaySound(string name, string path,Vector3 pos);
    //协程，还是绑定物体本身最安全。
    CharactorLoadHandle loadResource(string name,string path , AddCoroutineFun fun);
    void ReleaseResource(string path, string name);
    CharactorLoadHandle LoadSkillDataFile(string skillId , AddCoroutineFun fun);
}
public class CharactorLoadResultData  
{
    public object asset;
    public bool isFinish = false;
    public string error = "";
}
public class CharactorLoadResult : CharactorLoadHandle
{
    public CharactorLoadResult(CharactorLoadResultData data)
    {
        this.data = data;
    }
    CharactorLoadResultData data;
    public object asset
    {
        get
        {
            return data.asset;
        }
    }
    
    public bool isFinish
    {
        get
        {
            return data.isFinish;
        }
    }

    public string Error
    {
        get
        {
            return data.error;
        }
    }
}
public enum CdState
{
    NORMAL,//使用后就cd
    HIT,//前一段打中人，触发后续技能.
}
[System.Serializable]
public class CDData //cd技能数据
{
    public string skillId;//技能名
    public float cd;//cd时间
}

//技能参数，包括cd情况，技能释放方式等信息
public class SkillParams 
{
    
    public enum TYPE
    {
        CLICK,//按下施放.
        DRAG_DIR,//拖拽选择释放方向，
        DRAG_POINT,
        DRAG_TARGET,
        //后续添加拖拽选择释放目标，拖拽选择释放点等?
    }
    private int state = 0;
    public CDData[] cds;
    public float cd;
    public VirtualInput.KeyCode button;

    public TYPE type = TYPE.CLICK;//技能释放类型.
 
    public float skillRange = 1f;//技能释放距离
    public float skillWidth = 1f;//技能释放距离

    public float GetMaxCD()
    {
        return cds[state].cd;
    }
    public int State
    {
        get
        {
            return state;
        }
    }

    public bool isUsing = false;

    public void update()
    {
        cd = cd - Time.deltaTime;
 
        if (cd <= 0 && State > 0 )//后续技能用，比如放玩1技能，5秒内可以放2技能，使用2技能或者五秒过后，进入1技能cd状态。
        {
            cd = cds[0].cd;
            state = 0;
        }
    }
    public void updateState(int skillState)//一技能使用，或者命中，触发2技能。
    {
        if (skillState < cds.Length - 1)
        {
            state = skillState+1;
            //Debug.Log("updateState to " + state);
            cd = cds[state].cd;
        }
        else
        {
            state = 0;
            cd = cds[state].cd;
            //Debug.Log("updateState to " + state);
        }
        cd = cds[state].cd;
    }
    public bool CanUse(int state)
    {
        if (state == this.State)
        {
            if (state == 0)
                return cd < 0.00001f;
            else
                return cd > 0.00001f;
        }
        else
        {
            return false;
        }
    }

     
}
public enum LCharacterData
{
    TRANSFER,//跳转


    USER_KEY,//用户扩展键值ID。。
}

//角色的抽象接口
public interface LCharacterInterface
{
    //返回这个角色唯一的id,
    int GetId();
    void CrossFade(string anim_name,float time);
    Vector3 GetCurPosition();
    //获得移动的前向，根据游戏不同可能是相机的平面前向也可能是角色前向。
    Vector3  GetGameForward( );
    void SetCurPosition(Vector3 pos );
    void SetCurForward( Vector3 forward);
    Quaternion GetCurLocalRot();
    Vector3 GetCurLoaclScale();
    Animation GetRoleAnimation();//获得对象的主模型.
    Renderer[] GetRoleRender();
    int GetAttackLayer();
    bool IsDead();//是否死亡.
    int GetTargetId();
    void SetTargetId(int id);
    bool IsAI();
    void EnableAI();//绑定技能ai脚本会自动调用这个接口。
    int GetCamp();//
    void OnHit(Collider other, LCharacterColliderData data, Vector3 dir, ref LCharacterAction curAction, List<LCharacterAction> actionList , LChatacterInformationInterface information);

    SkillParams GetSkillCDSkillParams(string cdName );

    void AddParam(string cdName, SkillParams _params);
    void RemoveParam(string cdName);
    void UpdateSkillParams();
    void updateCDState(string cdName, int skillState);
    void Play(string animName);
    void ResetAndPlay(string animName);
    object GetData(int key);//key不存在返回null
    void SetData(int key,object value);
    void AddHaterd(int characterId, int v);
    void UpdateSkillRange(string cdName, float skillRange, float skillWidth);
    Vector3 GetCurForward();
    Vector3 GetCurXZForward();
    void SetAnimationLoop(string animName, bool v);
    void AddAction(LCharacterAction a);

    int GetHp();
    void SetHp(int hp);
    void OnHit(LCharacterHitData data);
    void DestroySelf();
 
}

//角色相关数据的抽象接口。
public interface LChatacterInformationInterface  {

    //尝试移动到某个位置.返回最近点可以移动位置
    Vector3 tryMove(Vector3 pos,Vector3 dir,bool fixToGround);

    bool getGroundHight(Vector3 pos,out Vector3 result );

    bool TryGetCharacter(int targetId, out LCharacterInterface chr);
 
    void AddCharacter(LCharacterInterface character);
    void RemoveCharacter(LCharacterInterface character);
    List<LCharacterInterface> GetAllCharacters();
    void slowly(float v, float slow_motion);
    Vector3 GetNewPointCanWalk(Vector3 pos);

    //获取数据接口，数据键值统一用int，避免unity字典产生c。可以用枚举做键值

}

