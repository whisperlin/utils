using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//角色的抽象接口
public interface LChatacterInterface
{
    void CrossFade(string anim_name);
    LChatacterAction GetRootAction();
    LChatacterAction GetCurAction();
    void SetCurAction(LChatacterAction action);
}
//角色相关数据的抽象接口。
public interface LChatacterInformationInterface  {

    //尝试移动到某个位置.返回最近点可以移动位置
    Vector3 tryMove(Vector3 pos);
    //获取数据接口，数据键值统一用int，避免unity字典产生c。可以用枚举做键值
    int getDataInt(int i);
    int getDataFloat(int i);
}

