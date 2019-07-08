using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityExtend.Editor.Builder;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Easy.Editor {
    public class AssetbundlesMenuItems {
        [MenuItem("Easy/AssetBundles/BuildResource")]
        static public void EasyBuildResource() {
            NewAssetBundleBuilder.Build();
            NewAssetBundleBuilder.BuildLocalVersion();
            EditorUtility.DisplayDialog("build", "build ok", "继续工作");
        }

        [MenuItem("Easy/AssetBundles/ClearNames")]
        static public void ClearNames() {
            var allAssets = AssetDatabase.GetAllAssetPaths().Where(path =>
                !(path.EndsWith(".dll"))
                && !(path.EndsWith(".cs"))
            ).ToArray();

            foreach (var filepath in allAssets) {
                if (filepath.EndsWith(".meta")) continue;

                var importer = AssetImporter.GetAtPath(filepath);
                if (importer == null) {
                    Debug.LogError("Not found: {0}" + filepath);
                    continue;
                }
                if (importer.assetBundleName != string.Empty) {
                    importer.assetBundleVariant = "";
                    importer.assetBundleName = "";
                }
            }
        }


        public static void PrefabZ2Zero(GameObject prefab) {
            var rectTransform = prefab.transform.GetComponent<RectTransform>();
            var anchoredPosition = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition3D = new Vector3(anchoredPosition.x, anchoredPosition.y, 0);

            foreach (Transform child in prefab.transform) {
                PrefabZ2Zero(child.gameObject);
            }
        }

        [MenuItem("Easy/Prefab/BatchProcessPrefab")]
        static public void BatchProcessPrefab() {
            string uiBuildDir = "Assets/UI/Builds";

            string[] dirs = Directory.GetDirectories(uiBuildDir, "*", SearchOption.AllDirectories);
            for (int i = 0; i < dirs.Length; i++) {
                string name = dirs[i].Replace('\\', '/');
                string[] files = Directory.GetFiles(name, "*prefab");
                if (files.Length == 0) {
                    continue;
                }

                foreach (var file in files) {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                    var newPrefab = GameObject.Instantiate(prefab);
                    PrefabZ2Zero(newPrefab);
                    PrefabUtility.ReplacePrefab(newPrefab, prefab, ReplacePrefabOptions.ConnectToPrefab);
                    GameObject.DestroyImmediate(newPrefab);
                }
            }
        }

        [MenuItem("Easy/Camera/CoderCamera")]
        static public void CoderCameraChange() {
            var cameraRoot = new GameObject();
            cameraRoot.name = "CameraRoot";

            var cameraMiddle = new GameObject();
            cameraMiddle.name = "CameraMiddle";

            var mainCamera = Camera.main;

            var cameraTransform = mainCamera.transform;
            var cameraRootTransform = cameraRoot.transform;
            var cameraMiddleTransform = cameraMiddle.transform;

            cameraRootTransform.rotation = cameraTransform.rotation;
            cameraRootTransform.position = cameraTransform.position;

            //middle
            cameraMiddleTransform.parent = cameraRootTransform;
            cameraMiddleTransform.localPosition = Vector3.zero;
            cameraMiddleTransform.localRotation = Quaternion.identity;

            //camera
            cameraTransform.parent = cameraMiddleTransform;
            cameraTransform.localRotation = Quaternion.identity;
            cameraTransform.localPosition = Vector3.zero;
        }

		[MenuItem("Easy/Prefab/Check ParticleSystem Lifetime")]
		static public void CheckParticleSystemLifetime() {
			var obj = Selection.activeGameObject;
			if(obj == null) {
				return;
			}

			var ps = obj.GetComponentsInChildren<ParticleSystem>();
			foreach(var i in ps) {
				var time = ComputeParticleSystemLifetime(i);
				if(time > 0 && time > i.duration) {
					//no set
					//i.duration = time;
				}
			}
		}

		static float ComputeParticleSystemLifetime(ParticleSystem ps) {
			if(ps.loop) {
				return -1;
			}

			var psr = ps.GetComponent<ParticleSystemRenderer>();
			if(ps.emission.enabled && psr.enabled) {
				if(ps.emission.type == ParticleSystemEmissionType.Time) {
					if(	ps.emission.rate.mode == ParticleSystemCurveMode.Constant &&
						ps.emission.rate.constant == 0 &&
						ps.emission.burstCount > 0) {
						ParticleSystem.Burst[] bs = new ParticleSystem.Burst[ps.emission.burstCount];
						ps.emission.GetBursts(bs);

						float max = 0;
						for(int i = 0, len = bs.Length; i < len; i++) {
							if(bs[i].time > max) {
								max = bs[i].time;
							}
						}

						return ps.startLifetime + max;
					}
				}
			}

			return -1;
		}
	}
}
