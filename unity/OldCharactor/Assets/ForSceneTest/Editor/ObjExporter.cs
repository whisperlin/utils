using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;

class MeshInst {
	public GameObject obj;
	public Material[] mats;
	public Texture2D[] texs;
	public Mesh mesh;
	public Vector3[] vertices;
	public Vector2[] uv;
	public Vector3[] normals;
	public List<int[]> submeshs;
	public int startIndex;
}

public class ObjExporter {
	[MenuItem("Easy/Scene/Export Obj")]
	static void ExportObj() {
		var obj = Selection.activeGameObject;
		if(obj == null) {
			return;
		}

		List<MeshInst> meshs = new List<MeshInst>();

		var ms = obj.GetComponentsInChildren<MeshRenderer>();
		for(int i = 0; i < ms.Length; i++) {
			var mats = ms[i].sharedMaterials;
			var mf = ms[i].GetComponent<MeshFilter>();
			if(mf != null && mf.sharedMesh != null) {
				MeshInst inst = new MeshInst();
				inst.obj = ms[i].gameObject;
				inst.mats = mats;
				inst.texs = new Texture2D[mats.Length];
				for(int j = 0; j < mats.Length; j++) {
					if(mats[j] != null) {
						inst.texs[j] = mats[j].mainTexture as Texture2D;
					}
				}
				inst.mesh = mf.sharedMesh;
				inst.vertices = inst.mesh.vertices;
				inst.uv = inst.mesh.uv;
				inst.normals = inst.mesh.normals;

				inst.submeshs = new List<int[]>();
				for(int j = 0; j < inst.mesh.subMeshCount; j++) {
					inst.submeshs.Add(inst.mesh.GetIndices(j));
				}

				meshs.Add(inst);
			}
		}

		StringBuilder sb = new StringBuilder();

		for(int i = 0; i < meshs.Count; i++) {
			var inst = meshs[i];
			for(int j = 0; j < inst.vertices.Length; j++) {
				var pos = inst.vertices[j];
				pos = inst.obj.transform.localToWorldMatrix.MultiplyPoint3x4(pos);
				sb.AppendFormat("v {0} {1} {2}\r\n", -pos.x, pos.y, pos.z);
			}
			if(i == 0) {
				inst.startIndex = 0;
			} else {
				inst.startIndex = meshs[i - 1].startIndex + meshs[i - 1].vertices.Length;
			}
		}

		for(int i = 0; i < meshs.Count; i++) {
			var inst = meshs[i];
			for(int j = 0; j < inst.uv.Length; j++) {
				var uv = inst.uv[j];
				sb.AppendFormat("vt {0} {1}\r\n", uv.x, uv.y);
			}
		}

		for(int i = 0; i < meshs.Count; i++) {
			var inst = meshs[i];
			for(int j = 0; j < inst.normals.Length; j++) {
				var nor = inst.normals[j];
				sb.AppendFormat("vn {0} {1} {2}\r\n", -nor.x, nor.y, nor.z);
			}
		}

		sb.Append("mtllib scene.mtl\r\n");

		for(int i = 0; i < meshs.Count; i++) {
			var inst = meshs[i];
			for(int j = 0; j < inst.submeshs.Count; j++) {
				var indices = inst.submeshs[j];

				sb.AppendFormat("g group_{0}_{1}\r\n", i, j);
				sb.AppendFormat("usemtl mat_{0}_{1}\r\n", i, j);

				for(int k = 0; k < indices.Length; k += 3) {
					var i0 = indices[k];
					var i1 = indices[k + 2];
					var i2 = indices[k + 1];
					sb.AppendFormat("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\r\n", inst.startIndex + i0 + 1, inst.startIndex + i1 + 1, inst.startIndex + i2 + 1);
				}
			}
		}

		if(!Directory.Exists("scene_obj_export")) {
			Directory.CreateDirectory("scene_obj_export");
		}

		File.WriteAllText("Assets/scene.obj", sb.ToString());

		sb = new StringBuilder();

		for(int i = 0; i < meshs.Count; i++) {
			var inst = meshs[i];

			for(int j = 0; j < inst.texs.Length; j++) {
				if(inst.texs[j] != null) {
					var path = AssetDatabase.GetAssetPath(inst.texs[j]);
					var name = new FileInfo(path).Name;
					if(!File.Exists("scene_obj_export/" + name)) {
						File.Copy(path, "scene_obj_export/" + name, false);
					}

					sb.AppendFormat("newmtl mat_{0}_{1}\r\n", i, j);
					sb.Append("Kd 1.000 1.000 1.000\r\n");
					sb.AppendFormat("map_Kd {0}\r\n", name);
				}
			}
		}

		File.WriteAllText("Assets/scene.mtl", sb.ToString());

		Debug.Log("export done.");
	}
}
