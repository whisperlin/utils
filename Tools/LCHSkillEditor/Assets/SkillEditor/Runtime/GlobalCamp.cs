using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CampInformation
{
    [Header("角色物理碰撞层")]
    [Range(0,31)]
    public int self;
    [Header("攻击物理碰撞层")]
    [Range(0, 31)]
    public int attack;
}
public class GlobalCamp : MonoBehaviour {
 
    public CampInformation[] campInformations = new CampInformation[0];
    public static CampInformation[]  globalCampInformations;

    public int jumpLayer = 20;
    void Awake() {
        globalCampInformations = campInformations;
        HashSet<int> usedChannels = new HashSet<int>();
        for (int i = 0; i < campInformations.Length; i++)
        {
            var c = campInformations[i];
            usedChannels.Add(c.self);
            usedChannels.Add(c.attack);
        }
        for (int i = 0; i < campInformations.Length; i++)
        {
            var c = campInformations[i];

            for (int j = 0; j < 32; j++)
            {
                if (!usedChannels.Contains(j))
                {
                    Physics.IgnoreLayerCollision(c.self, j, true);
                    Physics.IgnoreLayerCollision(c.attack, j, true);
                }
               
            }
 
            for (int j = 0; j < campInformations.Length; j++)
            {
                var c1 = campInformations[j];
                if (i == j)
                {
                   

                    //Physics.IgnoreLayerCollision(c.attack,c.self , true);
                    Physics.IgnoreLayerCollision(c.self, c.attack, true);
                    Physics.IgnoreLayerCollision(c.self, c.self, true);
                    Physics.IgnoreLayerCollision(c.attack, c.attack, true);
                }
                else
                {

                    Physics.IgnoreLayerCollision(c.self, c1.attack, false);//自身与攻击层碰撞
                    //Physics.IgnoreLayerCollision(c1.attack,c.self, false);//自身与攻击层碰撞
                    Physics.IgnoreLayerCollision(c.self, c1.self, true);//自身对方身体碰撞
                    //Physics.IgnoreLayerCollision( c1.self, c.self,true);
                    Physics.IgnoreLayerCollision(c.attack, c1.attack, true);//双方攻击层不碰撞。
                    //Physics.IgnoreLayerCollision(c1.attack, c.attack, true);//双方攻击层不碰撞。
                   
                }
            }
             
        }
        if (campInformations.Length > 0)
        {
            var c = campInformations[0];
            Physics.IgnoreLayerCollision(c.self, jumpLayer, false);
        }
        //jumpLayer
    }


}
