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

//角色的抽象接口
public interface LChatacterInterface
{
    //返回这个角色唯一的id,
    int GetId();
    void CrossFade(string anim_name);
    Vector3 GetCurPosition();
    //获得移动的前向，根据游戏不同可能是相机的平面前向也可能是角色前向。
    void GetForward(out Vector3 forward);
    void SetCurPosition(Vector3 pos );
    void SetCurForward( Vector3 forward);
    Quaternion GetCurLocalRot();
    Vector3 GetCurLoaclScale();
    Animation GetRoleAnimation();//获得对象的主模型.
    Renderer[] GetRoleRender();
    int GetAttackLayer();
}
//角色相关数据的抽象接口。
public interface LChatacterInformationInterface  {

    //尝试移动到某个位置.返回最近点可以移动位置
    Vector3 tryMove(Vector3 pos,Vector3 dir,bool fixToGround);
    Vector3 getGroundHight(Vector3 pos );
    //获取数据接口，数据键值统一用int，避免unity字典产生c。可以用枚举做键值

}

