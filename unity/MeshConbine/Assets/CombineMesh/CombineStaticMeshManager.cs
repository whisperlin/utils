using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yw;

public class CombineStaticMeshManager  {

	public int  cellWidth = 300;//一百米一个块.
	GameObject root;
	//区域块.
	public class Block
	{
		public int  cellWidth ;//一百米一个块.
		public GameObject  root;
		float centerx;
		float centery;
		//按材质添加
		//晚点改掉这里。
		Dictionary<int,CombinedMesh> meshs = new Dictionary<int, CombinedMesh>(); 
		List<CombinedMesh> meshList = new List<CombinedMesh> ();

		public void AddMesh(MeshFilter mf,MeshRenderer mr)
		{
			Mesh mesh = mf.mesh;
			Material material = mr.sharedMaterial;
			CombinedMesh staticMesh;
			if (!meshs.TryGetValue (material.GetInstanceID (), out staticMesh)) {
				GameObject g = new GameObject ("DymCombMesh("+centerx.ToString()+","+centery.ToString()+")"+material.name);
				staticMesh = g.AddComponent<CombinedMesh> ();

				staticMesh.SetMat (material);
				if (null != g)
					g.transform.parent = root.transform;
				g.transform.position = new Vector3 (centerx,0, centery);

				staticMesh.bounds.center = g.transform.position;
				staticMesh.bounds.size = new Vector3(cellWidth,cellWidth,cellWidth);
				
				meshs [material.GetInstanceID ()] = staticMesh;
				meshList.Add (staticMesh);
			}
			staticMesh.AddMesh (mf, mr,false);
		}
		public void UpdateAllMeshs(Plane  [] cut) 
		{
			for (int i = 0, count = meshList.Count; i < count; i++) {
				meshList [i].UpdateMesh (cut);
			}
		}
		public void SetPosition(float x,float y)
		{
			centerx = x;
			centery = y;
		}
	}

	public class MeshData
	{
		public MeshFilter f;
		public MeshRenderer r;
		public Transform t;
	}
	public Dictionary<long,Block> blocks = new Dictionary<long, Block> ();
	public List<Block> blockList = new List<Block> ();
	public void UpdateAllMesh(Plane  [] cut)
	{
		for ( int i = 0, count = blocks.Count; i < count; i++) {
			blockList [i].UpdateAllMeshs (cut);
		}
	}
	 
	public void AddMesh(MeshFilter  f)
	{
		if (null == root)
			root = new GameObject ("CombineStaticMesh");
		GameObject g = f.gameObject;
		if (null == g)
			return;
		MeshRenderer r = g.GetComponent<MeshRenderer> ();
		if (null == r)
			return;
		int _x = ((int)g.transform.position.x)/cellWidth;
		int _y = ((int)g.transform.position.z)/cellWidth;
		long key = ParamHelper.doubleInt2long (_x,_y);
 
		Block block;
		if (!blocks.TryGetValue (key, out block)) {
			blocks [key] = block = new Block();
			block.root = root;
			block.cellWidth = cellWidth;
			blockList.Add (block);
			block.SetPosition ((0.5f+_x) * cellWidth ,(0.5f+_y)*cellWidth);
		}
		block.AddMesh (f, r);
	}

 
}
