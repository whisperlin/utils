using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MakePointBuilder : MonoBehaviour {

	[MenuItem("无限回廊/生成录路点")]
	static void BuildWayPoint () {
     
        Block [] blocks = GameObject.FindObjectsOfType<Block>();
        for (int i = 0; i < blocks.Length; i++)
        {
            Block block = blocks[i];
  
            block.meshFilters = block.gameObject.GetComponentsInChildren<MeshFilter>();
            block.meshRenders = block.gameObject.GetComponentsInChildren<MeshRenderer>();
            block.UpdateWayPoint();

        }
       
        
	}
	 
}
