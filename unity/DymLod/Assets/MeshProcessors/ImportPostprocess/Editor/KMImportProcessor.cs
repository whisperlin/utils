using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

class KmipHelper {
	public KmipHelper() {
		//Debug.Log("KMIPHELPER CREATED");
		EditorUserBuildSettings.activeBuildTargetChanged += KMImportProcessor.buildTargetChanged;
	}
	public void Dummy() {
	}
}

public class KMImportProcessor : AssetPostprocessor {	
	
	static KmipHelper dummy = new KmipHelper(); // Creating a static instance will happen very early when binaries are loaded.

	int importRevision = 0 ;
	
	void OnPostprocessModel (GameObject g) {
		dummy.Dummy(); // dummy call to silence the compiler. kmiphelper is needed to get messages if the build target changes
		importRevision = KMImportSettings.GetMasterImportRevision();
		
		// Collect all the meshes and their paths. -> to find duplicates that annoy me.
	//	List<Mesh> meshes = new List<Mesh>();
	//	List<string> meshnames = new List<string>();
		
		bool didProcess = false;
		foreach (MeshFilter mf in g.GetComponentsInChildren<MeshFilter>()) {
			Mesh mesh = mf.sharedMesh;
			if (_processMesh(g, mesh)) didProcess = true;
			if (mesh != null) {
				mf.sharedMesh = mesh;
			}
		}
		foreach (SkinnedMeshRenderer smr in g.GetComponentsInChildren<SkinnedMeshRenderer>()) {
			Mesh mesh = smr.sharedMesh;
			if (_processMesh(g, mesh)) didProcess = true;
			if (mesh != null) {
				smr.sharedMesh = mesh;
			}
		}
		
		// Write the revision to the revision database inside the Library folder.
		if (didProcess) {
			Dictionary<string, int> db = GetModelImportRevisionDictionary();
			db[AssetDatabase.AssetPathToGUID(assetPath)] = importRevision;
			StoreModelImportRevisionDictionary(db);
		}
	}
	
	static public string ProgressTitle(KMProcessorProgram prog) {
		return "Mesh Processors: " + prog.descriptiveName();
	}
	
	bool _processMesh(GameObject container, Mesh umesh) {
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5
		if (umesh.blendShapeCount > 0) {
			Debug.Log ("Krabl Mesh Processors can not yet process meshes with blendshapes.");
			return false;
		}
#endif

		string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
		//Debug.Log("Container guid: " + assetGUID);
		string meshPath = KrablMesh.UnityEditorUtils.MeshPathInAsset(umesh, container);
		//Debug.Log("Meshpath: " + meshPath);
		
		KMProcessorProgram program = KMImportSettings.ProgramForMesh(assetGUID, meshPath);
		if (program == null) return false;
		if (program.bypass) return true;
		
	//	Debug.Log("KMImportProcessor Process " + container.name + "/" + umesh.name + " PRG: " + ((program == null) ? "NULL" : program.ToString()), umesh);
		
		KrablMesh.Processor[] processors = program.processors;
		if (true) { //program.importTriggeredByInspector == true) {
			// Set up progress indicators
			program.importTriggeredByInspector = false;
			float numEnabled = 0.0f;
			for (int i = 0; i < processors.Length; ++i) if (processors[i].enabled) numEnabled += 1.0f;
			float progStep = (numEnabled > 0.0f) ? 1.0f/numEnabled : 1.0f;
			float progOffset = 0.0f;
			
			string title = ProgressTitle(program);
			for (int i = 0; i < processors.Length; ++i) {
				if (processors[i].enabled == true) {
					float offset = progOffset;
					processors[i].progressDelegate = delegate(string text, float val) {
						EditorUtility.DisplayProgressBar(title, text, val*progStep + offset);
					};
					progOffset += progStep;
				}
			}
		}
				
		Mesh processedMesh = program.Process(umesh, EditorUserBuildSettings.selectedBuildTargetGroup.ToString());
		processedMesh.name = umesh.name;
		if (program.unityOptimizeMesh) {
			;
		}
		EditorUtility.ClearProgressBar();
				
		// Store Revision to program
		program.importRevision = importRevision;
		EditorUtility.SetDirty(program);
		//AssetDatabase.SaveAssets();
		
		if (program.targetPath.Equals("replace")) {
			umesh.Clear();
			EditorUtility.CopySerialized(processedMesh, umesh);
		} else {
			_outputToAsset(processedMesh, program.targetPath);
		}
		GameObject.DestroyImmediate(processedMesh);
		
		return true;
	}
	
	private void _outputToAsset(Mesh mesh, string path) 
	{		
		// Make sure the folders exist
		string folderPath = Path.GetDirectoryName(path);
		
		KrablMesh.UnityEditorUtils.AssureAssetFolderExists(folderPath);
		path = Path.ChangeExtension(Path.Combine("Assets", path), "asset");
		//Debug.Log("Writing file to " + path);
		Mesh existingMesh = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;
		if (existingMesh != null) {
			existingMesh.Clear();
			EditorUtility.CopySerialized(mesh, existingMesh);
			AssetDatabase.SaveAssets();
		} else {
			AssetDatabase.CreateAsset(mesh, path);
		}
	}

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath) {		
		for (int i = 0; i < importedAssets.Length; ++i) {
			KMImportSettings kmis = AssetDatabase.LoadAssetAtPath(importedAssets[i], typeof(KMImportSettings)) as KMImportSettings;
			if (kmis != null) {
				KMImportSettings.SettingsReimported();
			}
		}
	}
		
	override public int GetPostprocessOrder() {
		// The later the better!
		return 100000000;
	}
	
	public static void buildTargetChanged() {
		//Debug.Log("Build Target Changed.");
		KMImportSettings.PlatformChanged();
	}
	
	static private string _revisionFilePath() {
		return Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library/krablmeshrevisions");
	}
	
	static public void StoreModelImportRevisionDictionary(Dictionary<string, int> db) {
		BinaryFormatter bf = new BinaryFormatter();
		using (FileStream fs = new FileStream(_revisionFilePath(), FileMode.Create)) {
			bf.Serialize(fs, db);
		}
	}
	
	static Dictionary<string, int> _revisionDB = null;
	
	static public Dictionary<string, int> GetModelImportRevisionDictionary() {
		if (_revisionDB == null) {			
			string path = _revisionFilePath();
			// Load db from file
			if (File.Exists(path)) {
					BinaryFormatter bf = new BinaryFormatter();
					using (FileStream fs = new FileStream(path, FileMode.Open)) {
					_revisionDB = bf.Deserialize(fs) as Dictionary<string, int>;
				}
			}
			if (_revisionDB == null) _revisionDB = new Dictionary<string, int>();
		}
		return _revisionDB;
	}
}