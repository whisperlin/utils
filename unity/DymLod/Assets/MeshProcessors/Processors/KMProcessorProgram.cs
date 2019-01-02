using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KMProcessorProgram : ScriptableObject {
	public string inAssetGUID;
	public string inMeshPath;	
	public string inContainerName; // Only used for descriptions... unreliable!
	
	public string inMeshDescription;
	public float inputTolerance = 0.0f;
	
	public bool unityOptimizeMesh = true;
	public bool bypass = false;
	
	public string targetPath = "replace";
	public bool importTriggeredByInspector = false;	
	public int importRevision = 0;
	
	public KrablMesh.Processor[] processors = new KrablMesh.Processor[0];
	
	public KrablMesh.MeshEdges ProcessToKrablMesh(UnityEngine.Mesh umesh, string platformID = null) {
		KrablMesh.MeshEdges kmesh = new KrablMesh.MeshEdges();
		inMeshDescription = MeshDescription(umesh);
		KrablMesh.ImportExport.UnityMeshToMeshEdges(umesh, kmesh, inputTolerance);
			
		if (bypass == false) {
			foreach (KrablMesh.Processor p in processors) {
				if (p.enabled == true) {
					p.SetBuildPlatform(platformID);
					//float t = Time.realtimeSinceStartup;
					p.Calculate(ref kmesh, this);
					// Debug.Log("Processor " + p.Name() + " took " + (Time.realtimeSinceStartup - t)*1000.0f + " ms.");
				}
			}
		}
		return kmesh;
	}
	
	public static string MeshDescription(UnityEngine.Mesh umesh) {
		string result = "" + umesh.vertexCount + " verts, " + umesh.triangles.Length/3 + " tris";
		if (umesh.subMeshCount > 1) {
			result += ", " + umesh.subMeshCount + " submeshes";
		}
		string sep = "   ";
		if (umesh.uv.Length > 0) { result += sep + "uv"; sep = ", "; }
		if (umesh.uv2.Length > 0) { result += sep + "uv2"; sep = ", "; }
 		if (umesh.colors.Length > 0) { result += sep + "colors"; sep = ", "; }
		if (umesh.bindposes.Length > 0) { result += sep + "skin"; sep = ", "; }
		return result;
	}
	
	public UnityEngine.Mesh Process(UnityEngine.Mesh umesh, string platformID = null) {
		if (bypass == false) {
			KrablMesh.MeshEdges kmesh = ProcessToKrablMesh(umesh, platformID);
			UnityEngine.Mesh result = new UnityEngine.Mesh();
			KrablMesh.ImportExport.MeshEdgesToUnityMesh(kmesh, result);
			return result;
		}
		return umesh;
	}
	
	public string descriptiveName() {
		if (inContainerName == null || inContainerName == "" || inMeshPath == null) return "";
		return inContainerName + "/" + inMeshPath;
	}
	
	public bool IsPlatformDependent() {
		foreach (KrablMesh.Processor p in processors) {
			if (p.enabled && p.IsPlatformDependent()) {
				return true;
			}
		}
		return false;
	}
}	
