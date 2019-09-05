using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelHelper {

    public static Dictionary<LCHChannelType, string> channels = new Dictionary<LCHChannelType, string>(){
        {LCHChannelType.PosX,"角色/位移/X" },
        {LCHChannelType.PosY,"角色/位移/Y" },
        {LCHChannelType.PosZ,"角色/位移/Z" },
    };
}
