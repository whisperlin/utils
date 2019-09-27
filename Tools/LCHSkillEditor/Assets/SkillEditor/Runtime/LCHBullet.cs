using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCHBullet : MonoBehaviour
{

    public enum TYPE
    {
        NORMAL,//中了穿透过去
        FELLOW,
        DISTORY,//中了就消失
    }
    public float speed = 10f;
    public TYPE type = TYPE.NORMAL;
    public int count = 1;

    public float reduce = 1f; //技能击中角色后  curReduce = curReduce*reduce; 当curReduce<maxReduce 时 当curReduce = maxReduce; 
    public float maxReduce = 0.6f;
    public float curReduce = 1f;//当前伤害消减

    public float maxTime = 2f;
    public float curTime = 0f;
    public int curTargetId;
    bool destoryNextFrame = false;

    public void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime|| destoryNextFrame)
        {
            GameObject.DestroyImmediate(gameObject,false);
            return;

        }
        var _delta = Time.deltaTime* speed;

        
        if (type == TYPE.NORMAL)
        {
            transform.position += gameObject.transform.forward * _delta;
        }
        else if (type == TYPE.FELLOW)
        {
            LCharacterInterface chr;
            if (CharacterBase.information.TryGetCharacter(curTargetId, out chr))
            {
                Vector3 pos =  chr.GetCurPosition();
                float d = Vector3.Distance(pos,transform.position);
                //Debug.Log("d = "+d + "  "+ _delta);
                if (d > _delta)
                {
                    Vector3 forward = (pos- transform.position).normalized;
                   
                    transform.position = transform.position + forward* _delta;
                }
                else
                {
                    transform.position = pos;
                    destoryNextFrame = true;
                }
            }
        }
    }
}
