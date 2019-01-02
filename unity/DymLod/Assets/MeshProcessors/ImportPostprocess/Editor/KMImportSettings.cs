using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class KMImportSettings : ScriptableObject {	
	public KMProcessorProgram[] programs = new KMProcessorProgram[0]; // All the import programs
	// This is public because it needs to be serialized to get the correct references.
	// Making it public also makes debugging easier.
	public int masterImportRevision = 1;
		
	public void OnEnable() {
		if (_instance == null) {
			_instance = this;
		}
	}

	// Static stuff... this is a singleton.	
	static KMImportSettings _instance =  null;
	
	// static stuff
	static string assetPath = "Assets/MeshProcessors/MeshImportProcessorSettings.asset"; // TODO: make this configurable?
	
	static void _assureInstance() {
		if (_instance == null) {
			_instance = AssetDatabase.LoadAssetAtPath(assetPath, typeof(KMImportSettings)) as KMImportSettings;
			if (_instance == null) {
				// Create a new file. 
				// Maybe this could also search for moved files in the future.
				_instance = ScriptableObject.CreateInstance<KMImportSettings>();
				AssetDatabase.CreateAsset(_instance, assetPath);
			//	AssetDatabase.SaveAssets(); // maybe this crashes on first launch?
				
				// searching would be similar to this:
			/*	// Search for renamed prefab in project (maybe there's a more efficient way?)
				string[] paths = AssetDatabase.GetAllAssetPaths();
				foreach (string p in paths) {
					GameObject candidate = AssetDatabase.LoadAssetAtPath(p, typeof(GameObject)) as GameObject;
					if (candidate != null && candidate.GetComponent<KMImportSettingsRoot>() != null) {
						prefabPath = p;
						return candidate;
					}
				} */
			}
		} 
	}
	
	public static bool DoDebug() { return false; }
				
	public static int GetMasterImportRevision() {
		 _assureInstance();
		return _instance.masterImportRevision;
	}
	
	public static int IncrementMasterImportRevision() {
		_assureInstance();
		_instance.masterImportRevision++;
		EditorUtility.SetDirty(_instance);
		return _instance.masterImportRevision;
	}
	
	public static KMProcessorProgram ProgramForMesh(string assetGUID, string meshPath) {	
		_assureInstance();
		KMProcessorProgram[] programs = _instance.programs;
		for (int i = 0; i < programs.Length; ++i) {
			KMProcessorProgram prog = programs[i];
			if (prog != null && prog.inAssetGUID.Equals(assetGUID) && prog.inMeshPath.Equals(meshPath)) {
				return prog;
			}
		}
		return null;
	}
	
	public static KMProcessorProgram CreateProgram(string assetGUID, string meshPath, UnityEngine.Mesh mesh) {
		KMProcessorProgram prog = ScriptableObject.CreateInstance<KMProcessorProgram>();
		string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
		prog.inMeshPath = meshPath;
		prog.inContainerName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
		prog.inAssetGUID = assetGUID;
		prog.inMeshDescription = KMProcessorProgram.MeshDescription(mesh); // number of verts/tris etc.
		
		prog.name = "KMProcessorProgram";
		if (prog.descriptiveName() != null) {
			prog.name += "[" + prog.descriptiveName() + "]";
		}		
		return prog;
	}
	
	public static void AddProgram(KMProcessorProgram prog) {
		_assureInstance();
		if (prog == null) return;
		
		HideFlags hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		
		prog.hideFlags = hideFlags;
		AssetDatabase.AddObjectToAsset(prog, _instance); // storing to the file.
		// Store all the processors to the asset file as well.
		for (int i = 0; i < prog.processors.Length; ++i) {
			prog.processors[i].hideFlags = hideFlags;
			AssetDatabase.AddObjectToAsset(prog.processors[i], _instance);
		}
		ArrayUtility.Add<KMProcessorProgram>(ref _instance.programs, prog);
		
	// this import sometimes takes very long (7 sec)... not sure why but it's a reason not to do it.
		//	AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_instance)); // Needed? It does lead to the subassets showing up in case they are not hidden.
		// Save assets leads to the same problem
	//	AssetDatabase.SaveAssets(); 
	}
	
	public static void RemoveProgram(KMProcessorProgram prog) {
		_assureInstance();
		if (prog == null) return;
		// Remove from the array
		ArrayUtility.Remove<KMProcessorProgram>(ref _instance.programs, prog);
		// Destroy all the store processors
		for (int i = 0; i < prog.processors.Length; ++i) {
			DestroyImmediate(prog.processors[i], true);
		}
		// Destroy the program
		DestroyImmediate(prog, true);
		// AssetDatabase.SaveAssets(); // Needed? sometimes it is crazy slow
	}
	
	
	public static void PlatformChanged() {
		_assureInstance();
		KMProcessorProgram[] programs = _instance.programs;
		for (int i = 0; i < programs.Length; ++i) {
			if (programs[i].IsPlatformDependent()) {
				AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(programs[i].inAssetGUID));
			}			
		}
	}
	
	public static void SettingsReimported() {
		_assureInstance();
		float tm = 0.0f;
		if (KMImportSettings.DoDebug()) {
			tm = Time.realtimeSinceStartup;
			Debug.Log("Mesh Processor Settings were reimported.");
		}
		
		Dictionary<string, int> revisionDB = KMImportProcessor.GetModelImportRevisionDictionary();
		
		KMProcessorProgram[] programs = _instance.programs;
		for (int i = 0; i < programs.Length; ++i) {
			KMProcessorProgram prog = programs[i];
			if (prog != null) {
				int modelImportRevision = 0; // this is the revision number of the import on the running machine
				// prog.importRevision is the revision stored inside the importer asset
				// They need to be the same in order for the mesh to have the correct state.
				revisionDB.TryGetValue(prog.inAssetGUID, out modelImportRevision);
				//Debug.Log("P: " + path + " Rev New: " + rev);
				// If the importsettings changed on a different machine,
				// we need to reimport the changed assets
				if (modelImportRevision != prog.importRevision) {	 
					string path = AssetDatabase.GUIDToAssetPath(prog.inAssetGUID);
					if (path != null && path != "") {
						AssetDatabase.ImportAsset(path);
					}
				}
			}
		}
		if (KMImportSettings.DoDebug()) Debug.Log("Settings reimported took " + (Time.realtimeSinceStartup - tm)*1000.0f + " ms.");
	}
}
    