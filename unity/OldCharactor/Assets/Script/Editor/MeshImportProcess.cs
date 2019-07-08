using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class MeshImportProcess : AssetPostprocessor {
    void OnPreprocessModel() {
        var importer = assetImporter as ModelImporter;
        var path = importer.assetPath;

		importer.importMaterials = false;
		importer.materialSearch = ModelImporterMaterialSearch.Everywhere;
		importer.meshCompression = ModelImporterMeshCompression.Medium;

		if(	path.StartsWith("Assets/Props/Art/Character/Weapon") ||
			path.StartsWith("Assets/Props/Art/Item")) {
			importer.animationType = ModelImporterAnimationType.None;
		}

		if(	path.StartsWith("Assets/Props/Art/Character/Generals") ||
			path.StartsWith("Assets/Props/Art/Character/Monster") ||
			path.StartsWith("Assets/Props/Art/Character/NPC") ||
			path.StartsWith("Assets/Props/Art/Character/Role") ||
            path.StartsWith("Assets/Props/Art/Character/Ride") ||
            path.StartsWith("Assets/Props/Art/Character/Pet") ||
            path.StartsWith("Assets/Props/Art/Character/Wing") ||
            path.StartsWith("Assets/Props/Art/Character/HuiZhang") ||
            path.StartsWith("Assets/Props/Art/Character/Hero") ||
            path.StartsWith("Assets/Props/Art/Other") 
            ){
			importer.animationType = ModelImporterAnimationType.Legacy;

            if(!path.Contains("Idle.") && !path.StartsWith("Assets/Props/Art/Character/Ride") && !path.StartsWith("Assets/Props/Art/Character/Role")) {
                importer.animationCompression = ModelImporterAnimationCompression.Optimal;
            }


            if (path.StartsWith("Assets/Props/Art/Other/PlotAnim")) {
                importer.animationCompression = ModelImporterAnimationCompression.KeyframeReduction;
            }

                var clip_txt_path = path.Substring(0, path.Length - 4) + "_clips.txt";
            if(File.Exists(clip_txt_path)) {
                var clips = new List<ModelImporterClipAnimation>();

                var lines = File.ReadAllLines(clip_txt_path);
                for(int i = 0; i < lines.Length; i++) { 
                    var line = lines[i];
                    if(!string.IsNullOrEmpty(line)) {
                        var values = line.Split(':');
                        if(values.Length >= 2) { 
                            int fps = 30;

                            var name = values[0];
                            var range = values[1];
                            if(values.Length >= 3) { 
                                var fps_str = values[2];
                                if(fps_str != null && fps_str.Length > 3) { 
                                    fps = System.Convert.ToInt32(fps_str.Substring(3));
                                }
                            }

                            var ranges = range.Split(new string[] {"-"}, System.StringSplitOptions.RemoveEmptyEntries);
                            if(ranges.Length == 2) { 
                                var begin = ranges[0];
                                var end = ranges[1];
                                
                                var clip = new ModelImporterClipAnimation();
                                clip.name = name;
                                clip.firstFrame = System.Convert.ToSingle(begin);
                                clip.lastFrame = System.Convert.ToSingle(end);
								clip.wrapMode = DecideWrapMode(clip.name);

								clips.Add(clip);
                            }
                        }
                    }
                }

                if(clips.Count > 0) { 
                    importer.clipAnimations = clips.ToArray();
                }
            }
        }
    }

    void OnPostprocessModel(GameObject obj) {
		var importer = assetImporter as ModelImporter;
		var path = importer.assetPath;

		var rs = obj.GetComponentsInChildren<Renderer>();
		for(int i = 0; i < rs.Length; i++) {
			var mats = new Material[1];
			mats[0] = null;
			rs[i].sharedMaterials = mats;
		}

		if(	path.StartsWith("Assets/Props/Art/Character/Monster") ||
			path.StartsWith("Assets/Props/Art/Character/NPC")) {
			var clip_txt_path = path.Substring(0, path.Length - 4) + "_clips.txt";
			if(!File.Exists(clip_txt_path)) {
				var clips = importer.defaultClipAnimations;
				foreach(var i in clips) {
					i.wrapMode = DecideWrapMode(i.name);
				}
				importer.clipAnimations = clips;
			}
		}
	}

	static WrapMode DecideWrapMode(string name) {
		if(	name.Contains("Idle")
			|| name.Contains("Run")
			|| name.Contains("Loop")) {
			return WrapMode.Loop;
		} else {
			return WrapMode.ClampForever;
		}
	}

	static void CheckDir(string file, bool asset) {
        if(asset) { 
            file = Application.dataPath + "/../" + file;
        }

        var dir = new FileInfo(file).Directory.FullName;

        if(!Directory.Exists(dir)) { 
            Directory.CreateDirectory(dir);
        }
    }
}
