using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace LayaExport
{
	// Token: 0x0200000A RID: 10
	public class DataManager
	{
		// Token: 0x0600009B RID: 155 RVA: 0x000068C0 File Offset: 0x00004AC0
		public static void saveLayaAutoData()
		{
			DataManager.layaAutoGameObjectsList.Clear();
			if (DataManager.BatchMade && DataManager.Type == 1)
			{
				using (Dictionary<string, JSONObject>.Enumerator enumerator = DataManager.getLayaAutoSpriteNode().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, JSONObject> keyValuePair = enumerator.Current;
						Util.FileUtil.saveFile(DataManager.SAVEPATH + "/" + DataManager.cleanIllegalChar(keyValuePair.Key, true) + ".lh", keyValuePair.Value);
					}
					goto IL_CC;
				}
			}
			string fileName = "";
			if (DataManager.Type == 0)
			{
				fileName = DataManager.SAVEPATH + "/" + DataManager.sceneName + ".ls";
			}
			else if (DataManager.Type == 1)
			{
				fileName = DataManager.SAVEPATH + "/" + DataManager.sceneName + ".lh";
			}
			Util.FileUtil.saveFile(fileName, DataManager.getLayaAutoSceneNode());
			IL_CC:
			Debug.Log(" -- Exporting Data is Finished -- ");
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000069B4 File Offset: 0x00004BB4
		public static JSONObject getLayaAutoSceneNode()
		{
			JSONObject sceneNode = DataManager.getSceneNode();
			JSONObject field = sceneNode.GetField("props");
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			field.AddField("gameObjects", jsonobject);
			foreach (KeyValuePair<GameObject, string> keyValuePair in DataManager.layaAutoGameObjectsList[0])
			{
				JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject2.AddField("name", keyValuePair.Key.name);
				jsonobject2.AddField("renderSprite", keyValuePair.Value);
				jsonobject.Add(jsonobject2);
			}
			return sceneNode;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00006A64 File Offset: 0x00004C64
		public static Dictionary<string, JSONObject> getLayaAutoSpriteNode()
		{
			Dictionary<string, JSONObject> dictionary = new Dictionary<string, JSONObject>();
			Dictionary<string, JSONObject> dictionary2 = DataManager.saveSpriteNode();
			int num = 0;
			foreach (KeyValuePair<string, JSONObject> keyValuePair in dictionary2)
			{
				JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject.AddField("renderTree", keyValuePair.Value);
				JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject.AddField("gameObjects", jsonobject2);
				foreach (KeyValuePair<GameObject, string> keyValuePair2 in DataManager.layaAutoGameObjectsList[num++])
				{
					JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject3.AddField("name", keyValuePair2.Key.name);
					jsonobject3.AddField("renderSprite", keyValuePair2.Value);
					jsonobject2.Add(jsonobject3);
				}
				dictionary.Add(keyValuePair.Key, jsonobject);
			}
			return dictionary;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006B80 File Offset: 0x00004D80
		public static bool IsLayaAutoGameObjects(GameObject gameObject)
		{
			return gameObject.tag == "Laya3D" && !DataManager.layaAutoGameObjectsList[DataManager.LayaAutoGOListIndex].ContainsKey(gameObject);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006BB0 File Offset: 0x00004DB0
		public static bool IsCustomGameObject(GameObject gameObject)
		{
			bool result = false;
			if (gameObject.transform.parent != null)
			{
				return result;
			}
			if (gameObject.name == "pathFind")
			{
				DataManager.pathFindGameObject = gameObject;
				result = true;
			}
			return result;
		}
		public static string overlapSceneName = null;
		// Token: 0x060000A0 RID: 160 RVA: 0x00006BF0 File Offset: 0x00004DF0
		public static void getData(string lastname)
		{
			DataManager.sceneName = SceneManager.GetActiveScene().name;
			DataManager.sceneName = DataManager.cleanIllegalChar(DataManager.sceneName, true);
			if (DataManager.sceneName == "")
			{
				DataManager.sceneName = "layaScene";
			}
			string text;
			if (DataManager.CustomizeDirectory && DataManager.CustomizeDirectoryName != "")
			{
				DataManager.CustomizeDirectoryName = DataManager.cleanIllegalChar(DataManager.CustomizeDirectoryName, true);
				text = "/" + DataManager.CustomizeDirectoryName + lastname;
			}
			else
			{
				text = "/LayaScene_" + DataManager.sceneName + lastname;
			}
			DataManager.SAVEPATH += text;
			DataManager.ConvertOriginalTextureTypeList = new List<string>();
			if (DataManager.ConvertNonPNGAndJPG)
			{
				DataManager.ConvertOriginalTextureTypeList.Add(".tga");
				DataManager.ConvertOriginalTextureTypeList.Add(".TGA");
				DataManager.ConvertOriginalTextureTypeList.Add(".psd");
				DataManager.ConvertOriginalTextureTypeList.Add(".PSD");
				DataManager.ConvertOriginalTextureTypeList.Add(".gif");
				DataManager.ConvertOriginalTextureTypeList.Add(".GIF");
				DataManager.ConvertOriginalTextureTypeList.Add(".tif");
				DataManager.ConvertOriginalTextureTypeList.Add(".TIF");
				DataManager.ConvertOriginalTextureTypeList.Add(".bmp");
				DataManager.ConvertOriginalTextureTypeList.Add(".BMP");
				DataManager.ConvertOriginalTextureTypeList.Add(".exr");
				DataManager.ConvertOriginalTextureTypeList.Add(".EXR");
			}
			if (DataManager.ConvertOriginPNG)
			{
				DataManager.ConvertOriginalTextureTypeList.Add(".png");
				DataManager.ConvertOriginalTextureTypeList.Add(".PNG");
			}
			if (DataManager.ConvertOriginJPG)
			{
				DataManager.ConvertOriginalTextureTypeList.Add(".jpg");
				DataManager.ConvertOriginalTextureTypeList.Add(".JPG");
			}
			DataManager.directionalLightCurCount = 0;
			DataManager.pointLightCurCount = 0;
			DataManager.spotLightCurCount = 0;
			DataManager.recodeLayaJSFile(text + "/" + DataManager.sceneName);
			if (DataManager.LayaAuto)
			{
				DataManager.saveLayaAutoData();
			}
			else
			{
				DataManager.saveData();
				DataManager.lmCreate();
				DataManager.lmInfo.Clear();
			}
			if (DataManager.Conventional && DataManager.Platformindex == 0)
			{
				DataManager.NormalsaveTexturefiles();
			}
			if (DataManager.Android && DataManager.Platformindex == 2)
			{
				DataManager.Andriodfiles();
			}
			if (DataManager.Ios && DataManager.Platformindex == 1)
			{
				DataManager.IosTexturefiles();
			}
			DataManager.textureInfo.Clear();
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006E3C File Offset: 0x0000503C
		public static void saveData()
		{
			if (DataManager.BatchMade && DataManager.Type == 1)
			{
				using (Dictionary<string, JSONObject>.Enumerator enumerator = DataManager.saveSpriteNode().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, JSONObject> keyValuePair = enumerator.Current;
						Util.FileUtil.saveFile(DataManager.SAVEPATH + "/" + DataManager.cleanIllegalChar(keyValuePair.Key, true) + ".lh", keyValuePair.Value);
					}
					goto IL_C2;
				}
			}
			string fileName = "";
			if (DataManager.Type == 0)
			{
					fileName = DataManager.SAVEPATH + "/" + DataManager.sceneName + ".ls";
			}
			else if (DataManager.Type == 1)
			{
				if(overlapSceneName != null)
					fileName = DataManager.SAVEPATH + "/" + overlapSceneName + ".lh";
				else
					fileName = DataManager.SAVEPATH + "/" + DataManager.sceneName + ".lh";
			}
			Util.FileUtil.saveFile(fileName, DataManager.getSceneNode());
			IL_C2:
			Debug.Log(" -- Exporting Data is Finished -- ");
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00006F28 File Offset: 0x00005128
		public static JSONObject getSceneNode()
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject3.AddField("name", DataManager.sceneName);
			if (DataManager.Type == 0)
			{
				jsonobject.AddField("version", DataManager.LSVersion);
				jsonobject2.AddField("type", "Scene3D");
			}
			else if (DataManager.Type == 1)
			{
				jsonobject.AddField("version", DataManager.LHVersion);
				jsonobject2.AddField("type", "Sprite3D");
				jsonobject3.AddField("active", true);
			}
			jsonobject2.AddField("props", jsonobject3);
			jsonobject.AddField("data", jsonobject2);
			if (DataManager.Type == 0)
			{
				Material skybox = RenderSettings.skybox;
				if (skybox != null)
				{
					DataManager.getSkyBoxData(skybox, jsonobject3);
				}
				Color ambientLight = RenderSettings.ambientLight;
				JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject4.Add(ambientLight.r);
				jsonobject4.Add(ambientLight.g);
				jsonobject4.Add(ambientLight.b);
				jsonobject3.AddField("ambientColor", jsonobject4);
				if (RenderSettings.defaultReflectionMode == (UnityEngine.Rendering.DefaultReflectionMode)1)
				{
					DataManager.saveCubeMapFile(RenderSettings.customReflection, jsonobject3, false, null);
					jsonobject3.AddField("reflectionIntensity", RenderSettings.reflectionIntensity);
				}
				DataManager.saveLightMapFile(jsonobject3);
				jsonobject3.AddField("enableFog", RenderSettings.fog);
				jsonobject3.AddField("fogStart", RenderSettings.fogStartDistance);
				jsonobject3.AddField("fogRange", RenderSettings.fogEndDistance - RenderSettings.fogStartDistance);
				JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				Color fogColor = RenderSettings.fogColor;
				jsonobject5.Add(fogColor.r);
				jsonobject5.Add(fogColor.g);
				jsonobject5.Add(fogColor.b);
				jsonobject3.AddField("fogColor", jsonobject5);
			}
			else if (DataManager.Type == 1)
			{
				Vector3 vector = new Vector3(0f, 0f, 0f);
				Quaternion quaternion = new Quaternion(0f, 0f, 0f, -1f);
				Vector3 vector2;
				vector2 = new Vector3(1f, 1f, 1f);
				JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject6.Add(vector.x);
				jsonobject6.Add(vector.y);
				jsonobject6.Add(vector.z);
				jsonobject3.AddField("position", jsonobject6);
				JSONObject jsonobject7 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject7.Add(quaternion.x);
				jsonobject7.Add(quaternion.y);
				jsonobject7.Add(quaternion.z);
				jsonobject7.Add(quaternion.w);
				jsonobject3.AddField("rotation", jsonobject7);
				JSONObject jsonobject8 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject8.Add(vector2.x);
				jsonobject8.Add(vector2.y);
				jsonobject8.Add(vector2.z);
				jsonobject3.AddField("scale", jsonobject8);
			}
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			if (rootGameObjects.Length != 0)
			{
				JSONObject jsonobject9 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject2.AddField("child", jsonobject9);
				string gameObjectPath = DataManager.sceneName;
				Dictionary<GameObject, string> item = new Dictionary<GameObject, string>();
				DataManager.layaAutoGameObjectsList.Add(item);
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					DataManager.getGameObjectData(rootGameObjects[i].gameObject, gameObjectPath, jsonobject9, false);
				}
			}
			else
			{
				jsonobject2.AddField("child", new JSONObject(JSONObject.Type.ARRAY));
			}
			return jsonobject;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000727C File Offset: 0x0000547C
		public static Dictionary<string, JSONObject> saveSpriteNode()
		{
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			Dictionary<string, JSONObject> dictionary = new Dictionary<string, JSONObject>();
			if (rootGameObjects.Length != 0)
			{
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					DataManager.LayaAutoGOListIndex = i;
					Dictionary<GameObject, string> item = new Dictionary<GameObject, string>();
					DataManager.layaAutoGameObjectsList.Add(item);
					List<DataManager.ComponentType> list = DataManager.componentsOnGameObject(rootGameObjects[i]);
					DataManager.checkChildIsLegal(rootGameObjects[i], true);
					if ((rootGameObjects[i].activeInHierarchy || !DataManager.IgnoreNotActiveGameObject) && (list.Count > 1 || DataManager.curNodeHasLegalChild))
					{
						JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject.AddField("version", DataManager.LHVersion);
						JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject.AddField("data", jsonobject2);
						jsonobject2.AddField("type", "Sprite3D");
						JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject3.AddField("name", SceneManager.GetActiveScene().name);
						jsonobject3.AddField("active", true);
						jsonobject2.AddField("props", jsonobject3);
						Vector3 vector;
						vector = new Vector3(0f, 0f, 0f);
						Quaternion quaternion;
						quaternion = new Quaternion(0f, 0f, 0f, -1f);
						Vector3 vector2;
						vector2 = new Vector3(1f, 1f, 1f);
						JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
						jsonobject4.Add(vector.x);
						jsonobject4.Add(vector.y);
						jsonobject4.Add(vector.z);
						jsonobject3.AddField("position", jsonobject4);
						JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
						jsonobject5.Add(quaternion.x);
						jsonobject5.Add(quaternion.y);
						jsonobject5.Add(quaternion.z);
						jsonobject5.Add(quaternion.w);
						jsonobject3.AddField("rotation", jsonobject5);
						JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
						jsonobject6.Add(vector2.x);
						jsonobject6.Add(vector2.y);
						jsonobject6.Add(vector2.z);
						jsonobject3.AddField("scale", jsonobject6);
						JSONObject jsonobject7 = new JSONObject(JSONObject.Type.ARRAY);
						jsonobject2.AddField("child", jsonobject7);
						string gameObjectPath = DataManager.sceneName;
						DataManager.getGameObjectData(rootGameObjects[i].gameObject, gameObjectPath, jsonobject7, false);
						if (dictionary.ContainsKey(rootGameObjects[i].name))
						{
							dictionary.Add(rootGameObjects[i].name + dictionary.Count.ToString(), jsonobject);
						}
						else
						{
							dictionary.Add(rootGameObjects[i].name, jsonobject);
						}
					}
				}
			}
			return dictionary;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00007514 File Offset: 0x00005714
		public static void getGameObjectData(GameObject gameObject, string gameObjectPath, JSONObject parentsChildNodes, bool ignoreNullChild = false)
		{
			List<DataManager.ComponentType> list = DataManager.componentsOnGameObject(gameObject);
			DataManager.checkChildIsLegal(gameObject, true);
			if (!gameObject.activeSelf && DataManager.IgnoreNotActiveGameObject)
			{
				return;
			}
			if (DataManager.LayaAuto && DataManager.IsCustomGameObject(gameObject))
			{
				return;
			}
			if (list.Count < 1 && !DataManager.curNodeHasLegalChild)
			{
				return;
			}
			if (list.Count < 1 && ignoreNullChild)
			{
				return;
			}
			if (list.IndexOf(DataManager.ComponentType.DirectionalLight) != -1 && DataManager.directionalLightCurCount >= DataManager.directionalLightMaxCount)
			{
				return;
			}
			if (list.IndexOf(DataManager.ComponentType.PointLight) != -1 && DataManager.pointLightCurCount >= DataManager.pointLightMaxCount)
			{
				return;
			}
			if (list.IndexOf(DataManager.ComponentType.SpotLight) != -1 && DataManager.spotLightCurCount >= DataManager.spotLightMaxCount)
			{
				return;
			}
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			Vector3 localPosition = gameObject.transform.localPosition;
			Quaternion localRotation = gameObject.transform.localRotation;
			Vector3 localScale = gameObject.transform.localScale;
			string gameObjectPath2 = gameObjectPath;
			DataManager.getComponentsData(gameObject, jsonobject, jsonobject2, localPosition, localRotation, localScale, ref gameObjectPath2);
			DataManager.checkChildHasLocalParticle(gameObject, true);
			if (gameObject.transform.childCount > 0 && list.IndexOf(DataManager.ComponentType.Animator) == -1)
			{
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					DataManager.getGameObjectData(gameObject.transform.GetChild(i).gameObject, gameObjectPath2, jsonobject2, false);
				}
			}
			jsonobject.AddField("child", jsonobject2);
			parentsChildNodes.Add(jsonobject);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00007660 File Offset: 0x00005860
		public static void getSimpleGameObjectData(GameObject gameObject, string gameObjectPath, JSONObject parentsChildNodes)
		{
			List<DataManager.ComponentType> list = DataManager.componentsOnGameObject(gameObject);
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
				GameObject gameObject2 = componentsInChildren[i].gameObject;
				List<DataManager.ComponentType> list2 = DataManager.componentsOnGameObject(gameObject2);
				DataManager.checkChildIsLegal(gameObject2, true);
				if (!(gameObject2 == gameObject) && (gameObject2.activeInHierarchy || !DataManager.IgnoreNotActiveGameObject) && !(DataManager.selectParentbyType(gameObject2, DataManager.ComponentType.Animator) != null) && (list.IndexOf(DataManager.ComponentType.DirectionalLight) == -1 || DataManager.directionalLightCurCount < DataManager.directionalLightMaxCount))
				{
					if (list.IndexOf(DataManager.ComponentType.PointLight) != -1 && DataManager.pointLightCurCount >= DataManager.pointLightMaxCount)
					{
						return;
					}
					if (list.IndexOf(DataManager.ComponentType.SpotLight) != -1 && DataManager.spotLightCurCount >= DataManager.spotLightMaxCount)
					{
						return;
					}
					if (list2.Count > 1 || DataManager.curNodeHasLegalChild)
					{
						Matrix4x4 matrix4x = gameObject.transform.worldToLocalMatrix * gameObject2.transform.localToWorldMatrix;
						Vector3 position = matrix4x.GetColumn(3);
						Quaternion rotation = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
						Vector3 scale;
						scale = new Vector3(matrix4x.GetColumn(0).magnitude, matrix4x.GetColumn(1).magnitude, matrix4x.GetColumn(2).magnitude);
						MathUtil.Decompose(matrix4x.transpose, out scale, out rotation, out position);
						string text = gameObjectPath;
						DataManager.getComponentsData(gameObject2, jsonobject, jsonobject2, position, rotation, scale, ref text);
						jsonobject.AddField("child", jsonobject2);
						parentsChildNodes.Add(jsonobject);
					}
				}
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x0000780C File Offset: 0x00005A0C
		public static void getSkinGameobjectChilds(Dictionary<GameObject, JSONObject> existNode, GameObject gameObject, GameObject currentObject, string gameObjectPath, JSONObject parentsChildNodes, List<string> linkSprite = null)
		{
			List<DataManager.ComponentType> list = DataManager.componentsOnGameObject(currentObject);
			DataManager.checkChildIsLegal(currentObject, true);
			if (list.Count > 1 && currentObject.transform.parent.gameObject == gameObject)
			{
				JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
				Matrix4x4 matrix4x = gameObject.transform.worldToLocalMatrix * currentObject.transform.localToWorldMatrix;
				Vector3 position = matrix4x.GetColumn(3);
				Quaternion rotation = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
				Vector3 scale;
				scale = new Vector3(matrix4x.GetColumn(0).magnitude, matrix4x.GetColumn(1).magnitude, matrix4x.GetColumn(2).magnitude);
				MathUtil.Decompose(matrix4x.transpose, out scale, out rotation, out position);
				string text = gameObjectPath;
				DataManager.getComponentsData(currentObject, jsonobject, jsonobject2, position, rotation, scale, ref text);
				for (int i = 0; i < currentObject.transform.childCount; i++)
				{
					DataManager.getGameObjectData(currentObject.transform.GetChild(i).gameObject, gameObjectPath, jsonobject2, true);
				}
				jsonobject.AddField("child", jsonobject2);
				parentsChildNodes.Add(jsonobject);
				return;
			}
			if (list.Count <= 1)
			{
				int childCount = currentObject.transform.childCount;
				if (childCount == 0)
				{
					return;
				}
				for (int j = 0; j < childCount; j++)
				{
					DataManager.getSkinGameobjectChilds(existNode, gameObject, currentObject.transform.GetChild(j).gameObject, gameObjectPath, parentsChildNodes, linkSprite);
				}
				return;
			}
			else
			{
				GameObject gameObject2 = currentObject.transform.parent.gameObject;
				if (linkSprite != null && linkSprite.IndexOf(gameObject2.name) == -1)
				{
					linkSprite.Add(gameObject2.name);
				}
				if (!existNode.ContainsKey(gameObject2))
				{
					JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
					DataManager.getGameObjectData(currentObject, gameObjectPath, jsonobject3, true);
					JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
					Matrix4x4 matrix4x2 = gameObject.transform.worldToLocalMatrix * gameObject2.transform.localToWorldMatrix;
					Vector3 position2 = matrix4x2.GetColumn(3);
					Quaternion rotation2 = Quaternion.LookRotation(matrix4x2.GetColumn(2), matrix4x2.GetColumn(1));
					Vector3 scale2;
					scale2 = new Vector3(matrix4x2.GetColumn(0).magnitude, matrix4x2.GetColumn(1).magnitude, matrix4x2.GetColumn(2).magnitude);
					MathUtil.Decompose(matrix4x2.transpose, out scale2, out rotation2, out position2);
					string text2 = gameObjectPath;
					DataManager.getComponentsData(gameObject2, jsonobject4, jsonobject3, position2, rotation2, scale2, ref text2);
					jsonobject4.AddField("child", jsonobject3);
					parentsChildNodes.Add(jsonobject4);
					existNode.Add(gameObject2, jsonobject4);
					return;
				}
				JSONObject field = existNode[gameObject2].GetField("child");
				DataManager.getGameObjectData(currentObject, gameObjectPath, field, true);
				return;
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00007AEC File Offset: 0x00005CEC
		public static void getSkinAniGameObjectData(GameObject gameObject, string gameObjectPath, JSONObject parentsChildNodes, List<string> linkSprite = null)
		{
			Dictionary<GameObject, JSONObject> existNode = new Dictionary<GameObject, JSONObject>();
			int childCount = gameObject.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				DataManager.getSkinGameobjectChilds(existNode, gameObject, gameObject.transform.GetChild(i).gameObject, gameObjectPath, parentsChildNodes, linkSprite);
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00007B34 File Offset: 0x00005D34
		public static void getComponentsData(GameObject gameObject, JSONObject node, JSONObject child, Vector3 position, Quaternion rotation, Vector3 scale, ref string goPath)
		{
			List<DataManager.ComponentType> list = DataManager.componentsOnGameObject(gameObject);
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			node.AddField("type", "");
			if (list.IndexOf(DataManager.ComponentType.Transform) != -1)
			{
				node.SetField("type", "Sprite3D");
			}
			if (list.IndexOf(DataManager.ComponentType.PhysicsCollider) != -1)
			{
				node.SetField("type", "Sprite3D");
			}
			if (list.IndexOf(DataManager.ComponentType.Rigidbody3D) != -1)
			{
				node.SetField("type", "Sprite3D");
			}
			if (list.IndexOf(DataManager.ComponentType.Animator) != -1)
			{
				node.SetField("type", "Sprite3D");
			}
			if (list.IndexOf(DataManager.ComponentType.DirectionalLight) != -1)
			{
				node.SetField("type", "DirectionLight");
			}
			if (list.IndexOf(DataManager.ComponentType.PointLight) != -1)
			{
				node.SetField("type", "PointLight");
			}
			if (list.IndexOf(DataManager.ComponentType.SpotLight) != -1)
			{
				node.SetField("type", "SpotLight");
			}
			if (list.IndexOf(DataManager.ComponentType.Camera) != -1)
			{
				node.SetField("type", "Camera");
			}
			if (list.IndexOf(DataManager.ComponentType.MeshFilter) != -1)
			{
				node.SetField("type", "MeshSprite3D");
			}
			if (list.IndexOf(DataManager.ComponentType.MeshRenderer) != -1)
			{
				node.SetField("type", "MeshSprite3D");
			}
			if (list.IndexOf(DataManager.ComponentType.SkinnedMeshRenderer) != -1)
			{
				node.SetField("type", "SkinnedMeshSprite3D");
			}
			if (list.IndexOf(DataManager.ComponentType.ParticleSystem) != -1)
			{
				node.SetField("type", "ShuriKenParticle3D");
			}
			if (list.IndexOf(DataManager.ComponentType.Terrain) != -1)
			{
				if (DataManager.ConvertTerrainToMesh)
				{
					node.SetField("type", "MeshSprite3D");
				}
				else
				{
					node.SetField("type", "Terrain");
				}
			}
			if (list.IndexOf(DataManager.ComponentType.TrailRenderer) != -1)
			{
				node.SetField("type", "TrailSprite3D");
			}
			if (list.IndexOf(DataManager.ComponentType.LineRenderer) != -1)
			{
				node.SetField("type", "LineSprite3D");
			}
			node.AddField("props", jsonobject);
			StaticEditorFlags staticEditorFlags = GameObjectUtility.GetStaticEditorFlags(gameObject);
			jsonobject.AddField("name", gameObject.name);
			jsonobject.AddField("active", gameObject.activeSelf);
			jsonobject.AddField("isStatic", ((int)staticEditorFlags & 4) > 0);
			goPath = goPath + "/" + gameObject.name;
			if (gameObject.layer == 31)
			{
				Debug.LogWarning("LayaUnityPlugin : layer must less than 31 !");
			}
			else
			{
				jsonobject.AddField("layer", gameObject.layer);
			}
			node.AddField("components", jsonobject2);
			if (DataManager.IsLayaAutoGameObjects(gameObject))
			{
				DataManager.layaAutoGameObjectsList[DataManager.LayaAutoGOListIndex].Add(gameObject, goPath);
			}
			if (list.IndexOf(DataManager.ComponentType.Transform) != -1)
			{
				DataManager.getTransformComponentData(gameObject, jsonobject, position, rotation, scale);
			}
			if (list.IndexOf(DataManager.ComponentType.Rigidbody3D) != -1)
			{
				DataManager.getRigidbody3DComponentData(gameObject, jsonobject2);
			}
			if (list.IndexOf(DataManager.ComponentType.PhysicsCollider) != -1)
			{
				DataManager.getPhysicsColliderComponentData(gameObject, jsonobject2);
			}
			if (list.IndexOf(DataManager.ComponentType.Animator) != -1)
			{
				UnityEngine.Object avatar = gameObject.GetComponent<Animator>().avatar;
				List<string> linkSprite = new List<string>();
				if (avatar != null)
				{
					DataManager.getSkinAniGameObjectData(gameObject, goPath, child, linkSprite);
				}
				else
				{
					for (int i = 0; i < gameObject.transform.childCount; i++)
					{
						DataManager.getGameObjectData(gameObject.transform.GetChild(i).gameObject, goPath, child, false);
					}
				}
				DataManager.getAnimatorComponentData(gameObject, jsonobject2, linkSprite);
			}
			if (list.IndexOf(DataManager.ComponentType.DirectionalLight) != -1)
			{
				DataManager.getDirectionalLightComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.PointLight) != -1)
			{
				DataManager.getPointLightComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.SpotLight) != -1)
			{
				DataManager.getSpotLightComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.Camera) != -1)
			{
				DataManager.getCameraComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.MeshFilter) != -1)
			{
				DataManager.getMeshFilterComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.MeshRenderer) != -1)
			{
				DataManager.getMeshRendererComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.SkinnedMeshRenderer) != -1)
			{
				DataManager.getSkinnedMeshRendererComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.ParticleSystem) != -1)
			{
				DataManager.getParticleSystemComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.Terrain) != -1)
			{
				DataManager.getTerrainComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.TrailRenderer) != -1)
			{
				DataManager.getTrailRendererComponentData(gameObject, jsonobject);
			}
			if (list.IndexOf(DataManager.ComponentType.LineRenderer) != -1)
			{
				DataManager.getLineRendererComponentData(gameObject, jsonobject);
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00007F24 File Offset: 0x00006124
		public static void getTransformComponentData(GameObject gameObject, JSONObject props, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("position", jsonobject);
			props.AddField("rotation", jsonobject2);
			props.AddField("scale", jsonobject3);
			List<DataManager.ComponentType> list = DataManager.componentsOnGameObject(gameObject);
			jsonobject.Add(position.x * -1f);
			jsonobject.Add(position.y);
			jsonobject.Add(position.z);
			if (list.IndexOf(DataManager.ComponentType.Terrain) == -1)
			{
				if (list.IndexOf(DataManager.ComponentType.Camera) != -1 || list.IndexOf(DataManager.ComponentType.DirectionalLight) != -1 || list.IndexOf(DataManager.ComponentType.SpotLight) != -1)
				{
					rotation *= new Quaternion(0f, 1f, 0f, 0f);
				}
				jsonobject2.Add(rotation.x * -1f);
				jsonobject2.Add(rotation.y);
				jsonobject2.Add(rotation.z);
				jsonobject2.Add(rotation.w * -1f);
				jsonobject3.Add(scale.x);
				jsonobject3.Add(scale.y);
				jsonobject3.Add(scale.z);
				return;
			}
			jsonobject2.Add(0);
			jsonobject2.Add(0);
			jsonobject2.Add(0);
			jsonobject2.Add(-1);
			jsonobject3.Add(1);
			jsonobject3.Add(1);
			jsonobject3.Add(1);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00008080 File Offset: 0x00006280
		public static void getCameraComponentData(GameObject gameObject, JSONObject props)
		{
			Camera component = gameObject.GetComponent<Camera>();
			if (component.clearFlags == (UnityEngine.CameraClearFlags)1)
			{
				props.AddField("clearFlag", 1);
			}
			else if (component.clearFlags == (UnityEngine.CameraClearFlags)2 || component.clearFlags == (UnityEngine.CameraClearFlags)2)
			{
				props.AddField("clearFlag", 0);
			}
			else if (component.clearFlags == (UnityEngine.CameraClearFlags)3)
			{
				props.AddField("clearFlag", 2);
			}
			else
			{
				props.AddField("clearFlag", 3);
			}
			props.AddField("orthographic", component.orthographic);
			if (component.orthographic)
			{
				props.AddField("orthographicVerticalSize", component.orthographicSize * 2f);
			}
			else
			{
				props.AddField("fieldOfView", component.fieldOfView);
			}
			props.AddField("nearPlane", component.nearClipPlane);
			props.AddField("farPlane", component.farClipPlane);
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			Rect rect = component.rect;
			jsonobject.Add(rect.x);
			jsonobject.Add(rect.y);
			jsonobject.Add(rect.width);
			jsonobject.Add(rect.height);
			props.AddField("viewport", jsonobject);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			Color backgroundColor = component.backgroundColor;
			jsonobject2.Add(backgroundColor.r);
			jsonobject2.Add(backgroundColor.g);
			jsonobject2.Add(backgroundColor.b);
			jsonobject2.Add(backgroundColor.a);
			props.AddField("clearColor", jsonobject2);
			Skybox[] components = gameObject.GetComponents<Skybox>();
			if (components.Length != 0)
			{
				foreach (Skybox skybox in components)
				{
					if (skybox.enabled)
					{
						Material material = skybox.material;
						if (material != null)
						{
							DataManager.getSkyBoxData(material, props);
							return;
						}
					}
				}
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000823C File Offset: 0x0000643C
		public static void getDirectionalLightComponentData(GameObject gameObject, JSONObject props)
		{
			Light component = gameObject.GetComponent<Light>();
			props.AddField("intensity", component.intensity);
			switch (component.lightmapBakeType)
			{
			case LightmapBakeType.Mixed:
				props.AddField("lightmapBakedType", 1);
				goto IL_6F;
			case LightmapBakeType.Baked:
				props.AddField("lightmapBakedType", 2);
				goto IL_6F;
			case LightmapBakeType.Realtime:
				props.AddField("lightmapBakedType", 0);
				goto IL_6F;
			}
			props.AddField("lightmapBakedType", 0);
			IL_6F:
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			Color color = component.color;
			jsonobject.Add(color.r);
			jsonobject.Add(color.g);
			jsonobject.Add(color.b);
			props.AddField("color", jsonobject);
			DataManager.directionalLightCurCount++;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00008304 File Offset: 0x00006504
		public static void getPointLightComponentData(GameObject gameObject, JSONObject props)
		{
			Light component = gameObject.GetComponent<Light>();
			props.AddField("intensity", component.intensity);
			switch (component.lightmapBakeType)
			{
			case LightmapBakeType.Mixed:
				props.AddField("lightmapBakedType", 1);
				goto IL_6F;
			case LightmapBakeType.Baked:
				props.AddField("lightmapBakedType", 2);
				goto IL_6F;
			case LightmapBakeType.Realtime:
				props.AddField("lightmapBakedType", 0);
				goto IL_6F;
			}
			props.AddField("lightmapBakedType", 0);
			IL_6F:
			props.AddField("range", component.range);
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			Color color = component.color;
			jsonobject.Add(color.r);
			jsonobject.Add(color.g);
			jsonobject.Add(color.b);
			props.AddField("color", jsonobject);
			DataManager.pointLightCurCount++;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000083DC File Offset: 0x000065DC
		public static void getSpotLightComponentData(GameObject gameObject, JSONObject props)
		{
			Light component = gameObject.GetComponent<Light>();
			props.AddField("intensity", component.intensity);
			switch (component.lightmapBakeType)
			{
			case LightmapBakeType.Mixed:
				props.AddField("lightmapBakedType", 1);
				goto IL_6F;
			case LightmapBakeType.Baked:
				props.AddField("lightmapBakedType", 2);
				goto IL_6F;
			case LightmapBakeType.Realtime:
				props.AddField("lightmapBakedType", 0);
				goto IL_6F;
			}
			props.AddField("lightmapBakedType", 0);
			IL_6F:
			props.AddField("range", component.range);
			props.AddField("spotAngle", component.spotAngle);
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			Color color = component.color;
			jsonobject.Add(color.r);
			jsonobject.Add(color.g);
			jsonobject.Add(color.b);
			props.AddField("color", jsonobject);
			DataManager.spotLightCurCount++;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000084C4 File Offset: 0x000066C4
		public static void getMeshFilterComponentData(GameObject gameObject, JSONObject props)
		{
			Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			if (sharedMesh != null)
			{
				string str = DataManager.cleanIllegalChar(sharedMesh.name, true);
				string text = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(sharedMesh.GetInstanceID()).Split(new char[]
				{
					'.'
				})[0], false) + "-" + str;
				if (!DataManager.OptimizeMeshName)
				{
					text = string.Concat(new object[]
					{
						text,
						"[",
						sharedMesh.GetInstanceID(),
						"].lm"
					});
				}
				else
				{
					text += ".lm";
				}
				string key = DataManager.SAVEPATH + "/" + text;
				props.AddField("meshPath", text);
				bool enabled = gameObject.GetComponent<MeshRenderer>().enabled;
				props.AddField("enableRender", enabled);
				if (!DataManager.lmInfo.ContainsKey(key))
				{
					DataManager.lmInfo.Add(key, sharedMesh);
					return;
				}
			}
			else
			{
				Debug.LogWarning("LayaAir3D Warning(Code:1001) : " + gameObject.name + "'s MeshFilter Component Mesh data can't be null!");
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000085D4 File Offset: 0x000067D4
		public static void getMeshRendererComponentData(GameObject gameObject, JSONObject props)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			if (DataManager.Type == 0)
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component.lightmapIndex > -1)
				{
					props.AddField("lightmapIndex", component.lightmapIndex);
					props.AddField("lightmapScaleOffset", jsonobject);
					jsonobject.Add(component.lightmapScaleOffset.x);
					jsonobject.Add(component.lightmapScaleOffset.y);
					jsonobject.Add(component.lightmapScaleOffset.z);
					jsonobject.Add(component.lightmapScaleOffset.w);
				}
			}
			Material[] sharedMaterials = gameObject.GetComponent<MeshRenderer>().sharedMaterials;
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("materials", jsonobject2);
			DataManager.getLayaLmatData(gameObject, sharedMaterials, jsonobject2);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00008688 File Offset: 0x00006888
		public static void getSkinnedMeshRendererComponentData(GameObject gameObject, JSONObject props)
		{
			SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
			props.AddField("rootBone", component.rootBone ? component.rootBone.name : "");
			Bounds localBounds = component.localBounds;
			Vector3 center = localBounds.center;
			Vector3 vector;
			vector = new Vector3(-center.x, center.y, center.z);
			Vector3 extents = localBounds.extents;
			Vector3 vector2 = vector - extents;
			Vector3 vector3 = vector + extents;
			float val = Vector3.Distance(vector2, vector3) / 2f;
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			props.AddField("boundBox", jsonobject);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(vector2.x);
			jsonobject2.Add(vector2.y);
			jsonobject2.Add(vector2.z);
			jsonobject.AddField("min", jsonobject2);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject3.Add(vector3.x);
			jsonobject3.Add(vector3.y);
			jsonobject3.Add(vector3.z);
			jsonobject.AddField("max", jsonobject3);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
			props.AddField("boundSphere", jsonobject4);
			JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject5.Add(vector.x);
			jsonobject5.Add(vector.y);
			jsonobject5.Add(vector.z);
			jsonobject4.AddField("center", jsonobject5);
			jsonobject4.AddField("radius", val);
			Material[] sharedMaterials = component.sharedMaterials;
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("materials", jsonobject6);
			DataManager.getLayaLmatData(gameObject, sharedMaterials, jsonobject6);
			Mesh sharedMesh = component.sharedMesh;
			if (sharedMesh != null)
			{
				string str = DataManager.cleanIllegalChar(sharedMesh.name, true);
				string text = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(sharedMesh.GetInstanceID()).Split(new char[]
				{
					'.'
				})[0], false) + "-" + str;
				if (!DataManager.OptimizeMeshName)
				{
					text = string.Concat(new object[]
					{
						text,
						"[",
						sharedMesh.GetInstanceID(),
						"].lm"
					});
				}
				else
				{
					text += ".lm";
				}
				string savePath = DataManager.SAVEPATH + "/" + text;
				props.AddField("meshPath", text);
				DataManager.saveSkinLmFile(component, savePath);
				return;
			}
			Debug.LogWarning("LayaAir3D Warning(Code:1001) : " + gameObject.name + "'s MeshFilter Component Mesh data can't be null!");
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x0000891C File Offset: 0x00006B1C
		public static void getAnimatorComponentData(GameObject gameObject, JSONObject component, List<string> linkSprite)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject obj = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject.AddField("type", "Animator");
			Animator component2 = gameObject.GetComponent<Animator>();
			Avatar avatar = component2.avatar;
			if (avatar != null)
			{
				string assetPath = AssetDatabase.GetAssetPath(avatar.GetInstanceID());
				string text = string.Concat(new string[]
				{
					DataManager.cleanIllegalChar(assetPath.Split(new char[]
					{
						'.'
					})[0], false),
					"-",
					DataManager.cleanIllegalChar(gameObject.name, true),
					"-",
					avatar.name,
					".lav"
				});
				string fileName = DataManager.SAVEPATH + "/" + text;
				JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				DataManager.getLavData(gameObject, jsonobject5, gameObject);
				jsonobject4.AddField("version", "LAYAAVATAR:01");
				jsonobject4.AddField("rootNode", jsonobject5[0]);
				Util.FileUtil.saveFile(fileName, jsonobject4);
				jsonobject.AddField("avatar", jsonobject2);
				jsonobject2.AddField("path", text);
				jsonobject2.AddField("linkSprites", jsonobject3);
				for (int i = 0; i < linkSprite.Count; i++)
				{
					JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
					jsonobject6.Add(linkSprite[i]);
					jsonobject3.AddField(linkSprite[i], jsonobject6);
				}
			}
			jsonobject.AddField("layers", obj);
			DataManager.saveLaniData(gameObject, obj);
			if (component2.cullingMode == null)
			{
				jsonobject.AddField("cullingMode", 0);
			}
			else if (component2.cullingMode == (UnityEngine.AnimatorCullingMode)2)
			{
				jsonobject.AddField("cullingMode", 2);
			}
			else
			{
				jsonobject.AddField("cullingMode", 0);
			}
			jsonobject.AddField("playOnWake", true);
			component.Add(jsonobject);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00008AF4 File Offset: 0x00006CF4
		public static void getParticleSystemComponentData(GameObject gameObject, JSONObject props)
		{
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			ParticleSystemRenderer component2 = gameObject.GetComponent<ParticleSystemRenderer>();
			int val = 0;
			props.AddField("isPerformanceMode", true);
			props.AddField("duration", component.main.duration);
			props.AddField("looping", component.main.loop);
			props.AddField("prewarm", false);
			if (component.main.startDelay.mode.ToString() == "Constant")
			{
				val = 0;
			}
			else if (component.main.startDelay.mode.ToString() == "TwoConstants")
			{
				val = 1;
			}
			props.AddField("startDelayType", val);
			props.AddField("startDelay", component.main.startDelay.constant);
			props.AddField("startDelayMin", component.main.startDelay.constantMin);
			props.AddField("startDelayMax", component.main.startDelay.constantMax);
			if (component.main.startLifetime.mode.ToString() == "Constant")
			{
				val = 0;
			}
			else if (component.main.startLifetime.mode.ToString() == "Curves")
			{
				val = 1;
			}
			else if (component.main.startLifetime.mode.ToString() == "TwoConstants")
			{
				val = 2;
			}
			else if (component.main.startLifetime.mode.ToString() == "MinMaxCurves")
			{
				val = 3;
			}
			props.AddField("startLifetimeType", val);
			props.AddField("startLifetimeConstant", component.main.startLifetime.constant);
			props.AddField("startLifetimeConstantMin", component.main.startLifetime.constantMin);
			props.AddField("startLifetimeConstantMax", component.main.startLifetime.constantMax);
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("startLifetimeGradient", jsonobject);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
			if (component.main.startLifetime.curve != null)
			{
				for (int i = 0; i < component.main.startLifetime.curve.length; i++)
				{
					jsonobject3.Clear();
					jsonobject2.Add(jsonobject3);
					jsonobject3.AddField("key", component.main.startLifetime.curve[i].time);
					jsonobject3.AddField("value", component.main.startLifetime.curve[i].value);
				}
			}
			jsonobject.AddField("startLifetimes", jsonobject2);
			jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("startLifetimeGradientMin", jsonobject);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
			if (component.main.startLifetime.curveMin != null)
			{
				for (int i = 0; i < component.main.startLifetime.curveMin.length; i++)
				{
					jsonobject3.Clear();
					jsonobject2.Add(jsonobject3);
					jsonobject3.AddField("key", component.main.startLifetime.curveMin[i].time);
					jsonobject3.AddField("value", component.main.startLifetime.curveMin[i].value);
				}
			}
			jsonobject.AddField("startLifetimes", jsonobject2);
			jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("startLifetimeGradientMax", jsonobject);
			if (component.main.startLifetime.curveMax != null)
			{
				for (int i = 0; i < component.main.startLifetime.curveMax.length; i++)
				{
					jsonobject3.Clear();
					jsonobject2.Add(jsonobject3);
					jsonobject3.AddField("key", component.main.startLifetime.curveMax[i].time);
					jsonobject3.AddField("value", component.main.startLifetime.curveMax[i].value);
				}
			}
			jsonobject.AddField("startLifetimes", jsonobject2);
			if (component.main.startSpeed.mode.ToString() == "Constant")
			{
				val = 0;
			}
			else if (component.main.startSpeed.mode.ToString() == "Curve")
			{
				val = 1;
			}
			else if (component.main.startSpeed.mode.ToString() == "TwoConstants")
			{
				val = 2;
			}
			else if (component.main.startSpeed.mode.ToString() == "TwoCurves")
			{
				val = 3;
			}
			props.AddField("startSpeedType", val);
			props.AddField("startSpeedConstant", component.main.startSpeed.constant);
			props.AddField("startSpeedConstantMin", component.main.startSpeed.constantMin);
			props.AddField("startSpeedConstantMax", component.main.startSpeed.constantMax);
			if (component.main.startSizeX.mode.ToString() == "Constant")
			{
				val = 0;
			}
			else if (component.main.startSizeX.mode.ToString() == "Curve")
			{
				val = 1;
			}
			else if (component.main.startSizeX.mode.ToString() == "TwoConstants")
			{
				val = 2;
			}
			else if (component.main.startSizeX.mode.ToString() == "TwoCurves")
			{
				val = 3;
			}
			props.AddField("threeDStartSize", component.main.startSize3D);
			props.AddField("startSizeType", val);
			props.AddField("startSizeConstant", component.main.startSize.constant);
			props.AddField("startSizeConstantMin", component.main.startSize.constantMin);
			props.AddField("startSizeConstantMax", component.main.startSize.constantMax);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(component.main.startSizeX.constant);
			jsonobject2.Add(component.main.startSizeY.constant);
			jsonobject2.Add(component.main.startSizeZ.constant);
			props.AddField("startSizeConstantSeparate", jsonobject2);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(component.main.startSizeX.constantMin);
			jsonobject2.Add(component.main.startSizeY.constantMin);
			jsonobject2.Add(component.main.startSizeZ.constantMin);
			props.AddField("startSizeConstantMinSeparate", jsonobject2);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(component.main.startSizeX.constantMax);
			jsonobject2.Add(component.main.startSizeY.constantMax);
			jsonobject2.Add(component.main.startSizeZ.constantMax);
			props.AddField("startSizeConstantMaxSeparate", jsonobject2);
			props.AddField("threeDStartRotation", component.main.startRotation3D);
			if (component.main.startRotationX.mode.ToString() == "Constant")
			{
				val = 0;
			}
			else if (component.main.startRotationX.mode.ToString() == "Curve")
			{
				val = 1;
			}
			else if (component.main.startRotationX.mode.ToString() == "TwoConstants")
			{
				val = 2;
			}
			else if (component.main.startRotationX.mode.ToString() == "TwoCurves")
			{
				val = 3;
			}
			props.AddField("startRotationType", val);
			props.AddField("startRotationConstant", component.main.startRotation.constant * 180f / 3.14159274f);
			props.AddField("startRotationConstantMin", component.main.startRotation.constantMin * 180f / 3.14159274f);
			props.AddField("startRotationConstantMax", component.main.startRotation.constantMax * 180f / 3.14159274f);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(component.main.startRotationX.constant * 180f / 3.14159274f);
			jsonobject2.Add(-1f * (component.main.startRotationY.constant * 180f / 3.14159274f));
			jsonobject2.Add(-1f * (component.main.startRotationZ.constant * 180f / 3.14159274f));
			props.AddField("startRotationConstantSeparate", jsonobject2);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(component.main.startRotationX.constantMin * 180f / 3.14159274f);
			jsonobject2.Add(-1f * (component.main.startRotationY.constantMin * 180f / 3.14159274f));
			jsonobject2.Add(-1f * (component.main.startRotationZ.constantMin * 180f / 3.14159274f));
			props.AddField("startRotationConstantMinSeparate", jsonobject2);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(component.main.startRotationX.constantMax * 180f / 3.14159274f);
			jsonobject2.Add(-1f * (component.main.startRotationY.constantMax * 180f / 3.14159274f));
			jsonobject2.Add(-1f * (component.main.startRotationZ.constantMax * 180f / 3.14159274f));
			props.AddField("startRotationConstantMaxSeparate", jsonobject2);
			props.AddField("randomizeRotationDirection", component.main.randomizeRotationDirection * 180f / 3.14159274f);
			if (component.main.startColor.mode.ToString() == "Color")
			{
				val = 0;
			}
			else if (component.main.startColor.mode.ToString() == "Gradient")
			{
				val = 1;
			}
			else if (component.main.startColor.mode.ToString() == "TwoColors")
			{
				val = 2;
			}
			else if (component.main.startColor.mode.ToString() == "TwoGradients")
			{
				val = 3;
			}
			else if (component.main.startColor.mode.ToString() == "RandomColor")
			{
				val = 4;
			}
			props.AddField("startColorType", val);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(component.main.startColor.color.r);
			jsonobject2.Add(component.main.startColor.color.g);
			jsonobject2.Add(component.main.startColor.color.b);
			jsonobject2.Add(component.main.startColor.color.a);
			props.AddField("startColorConstant", jsonobject2);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(component.main.startColor.colorMin.r);
			jsonobject2.Add(component.main.startColor.colorMin.g);
			jsonobject2.Add(component.main.startColor.colorMin.b);
			jsonobject2.Add(component.main.startColor.colorMin.a);
			props.AddField("startColorConstantMin", jsonobject2);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(component.main.startColor.colorMax.r);
			jsonobject2.Add(component.main.startColor.colorMax.g);
			jsonobject2.Add(component.main.startColor.colorMax.b);
			jsonobject2.Add(component.main.startColor.colorMax.a);
			props.AddField("startColorConstantMax", jsonobject2);
			jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(Physics.gravity.x);
			jsonobject2.Add(Physics.gravity.y);
			jsonobject2.Add(Physics.gravity.z);
			props.AddField("gravity", jsonobject2);
			props.AddField("gravityModifier", component.main.gravityModifier.constant);
			if (component.main.simulationSpace.ToString() == "World")
			{
				val = 0;
			}
			else if (component.main.simulationSpace.ToString() == "Local")
			{
				val = 1;
			}
			props.AddField("simulationSpace", val);
			if (component.main.scalingMode.ToString() == "Hierarchy")
			{
				val = 0;
			}
			else if (component.main.scalingMode.ToString() == "Local")
			{
				val = 1;
			}
			else if (component.main.scalingMode.ToString() == "Shape")
			{
				val = 2;
			}
			props.AddField("scaleMode", val);
			props.AddField("playOnAwake", component.main.playOnAwake);
			props.AddField("maxParticles", component.main.maxParticles);
			props.AddField("autoRandomSeed", component.useAutoRandomSeed);
			props.AddField("randomSeed", (long)((ulong)component.randomSeed));
			if (component.emission.enabled)
			{
				JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
				props.AddField("emission", jsonobject4);
				jsonobject4.AddField("enable", component.emission.enabled);
				if (component.emission.rateOverTime.mode.ToString() == "Constant")
				{
					val = 0;
				}
				else if (component.emission.rateOverTime.mode.ToString() == "Curve")
				{
					val = 1;
				}
				else if (component.emission.rateOverTime.mode.ToString() == "TwoConstants")
				{
					val = 2;
				}
				else if (component.emission.rateOverTime.mode.ToString() == "TwoCurves")
				{
					val = 3;
				}
				jsonobject4.AddField("emissionRate", component.emission.rateOverTime.constant);
				jsonobject4.AddField("emissionRateTip", "Time");
				jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
				ParticleSystem.Burst[] array = new ParticleSystem.Burst[component.emission.burstCount];
				component.emission.GetBursts(array);
				for (int i = 0; i < array.Length; i++)
				{
					jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject3.AddField("time", array[i].time);
					jsonobject3.AddField("min", (int)array[i].minCount);
					jsonobject3.AddField("max", (int)array[i].maxCount);
					jsonobject2.Add(jsonobject3);
				}
				jsonobject4.AddField("bursts", jsonobject2);
			}
			if (component.shape.enabled)
			{
				JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
				props.AddField("shape", jsonobject4);
				jsonobject4.AddField("enable", component.shape.enabled);
				if (component.shape.shapeType.ToString() == "Sphere" || component.shape.shapeType.ToString() == "SphereShell")
				{
					val = 0;
				}
				else if (component.shape.shapeType.ToString() == "Hemisphere" || component.shape.shapeType.ToString() == "HemisphereShell")
				{
					val = 1;
				}
				else if (component.shape.shapeType.ToString() == "Cone" || component.shape.shapeType.ToString() == "ConeShell" || component.shape.shapeType.ToString() == "ConeBase" || component.shape.shapeType.ToString() == "ConeBaseShell" || component.shape.shapeType.ToString() == "ConeVolume" || component.shape.shapeType.ToString() == "ConeVolumeShell")
				{
					val = 2;
				}
				else if (component.shape.shapeType.ToString() == "Box")
				{
					val = 3;
				}
				else if (component.shape.shapeType.ToString() == "Mesh")
				{
					val = 4;
				}
				else if (component.shape.shapeType.ToString() == "MeshRenderer")
				{
					val = 5;
				}
				else if (component.shape.shapeType.ToString() == "SkinnedMeshRenderer")
				{
					val = 6;
				}
				else if (component.shape.shapeType.ToString() == "Circle" || component.shape.shapeType.ToString() == "CircleEdge")
				{
					val = 7;
				}
				else if (component.shape.shapeType.ToString() == "SingleSidedEdge")
				{
					val = 8;
				}
				jsonobject4.AddField("shapeType", val);
				jsonobject4.AddField("sphereRadius", component.shape.radius);
				if (component.shape.shapeType.ToString() == "SphereShell")
				{
					jsonobject4.AddField("sphereEmitFromShell", true);
				}
				else
				{
					jsonobject4.AddField("sphereEmitFromShell", false);
				}
				jsonobject4.AddField("sphereRandomDirection", component.shape.randomDirectionAmount);
				jsonobject4.AddField("hemiSphereRadius", component.shape.radius);
				if (component.shape.shapeType.ToString() == "HemisphereShell")
				{
					jsonobject4.AddField("hemiSphereEmitFromShell", true);
				}
				else
				{
					jsonobject4.AddField("hemiSphereEmitFromShell", false);
				}
				jsonobject4.AddField("hemiSphereRandomDirection", component.shape.randomDirectionAmount);
				jsonobject4.AddField("coneAngle", component.shape.angle);
				jsonobject4.AddField("coneRadius", component.shape.radius);
				jsonobject4.AddField("coneLength", component.shape.length);
				if (component.shape.shapeType.ToString() == "Cone")
				{
					val = 0;
				}
				else if (component.shape.shapeType.ToString() == "ConeShell")
				{
					val = 1;
				}
				else if (component.shape.shapeType.ToString() == "ConeVolume")
				{
					val = 2;
				}
				else if (component.shape.shapeType.ToString() == "ConeVolumeShell")
				{
					val = 3;
				}
				jsonobject4.AddField("coneEmitType", val);
				jsonobject4.AddField("coneRandomDirection", component.shape.randomDirectionAmount);
				jsonobject4.AddField("boxX", component.shape.box.x);
				jsonobject4.AddField("boxY", component.shape.box.y);
				jsonobject4.AddField("boxZ", component.shape.box.z);
				jsonobject4.AddField("boxRandomDirection", component.shape.randomDirectionAmount);
				jsonobject4.AddField("circleRadius", component.shape.radius);
				jsonobject4.AddField("circleArc", component.shape.arc);
				if (component.shape.shapeType.ToString() == "CircleEdge")
				{
					jsonobject4.AddField("circleEmitFromEdge", true);
				}
				else
				{
					jsonobject4.AddField("circleEmitFromEdge", false);
				}
				jsonobject4.AddField("circleRandomDirection", component.shape.randomDirectionAmount);
			}
			if (component.velocityOverLifetime.enabled)
			{
				JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
				props.AddField("velocityOverLifetime", jsonobject4);
				jsonobject4.AddField("enable", component.velocityOverLifetime.enabled);
				if (component.velocityOverLifetime.space.ToString() == "Local")
				{
					val = 0;
				}
				else if (component.velocityOverLifetime.space.ToString() == "World")
				{
					val = 1;
				}
				jsonobject4.AddField("space", val);
				jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject4.AddField("velocity", jsonobject);
				if (component.velocityOverLifetime.x.mode.ToString() == "Constant")
				{
					val = 0;
				}
				else if (component.velocityOverLifetime.x.mode.ToString() == "Curve")
				{
					val = 1;
				}
				else if (component.velocityOverLifetime.x.mode.ToString() == "TwoConstants")
				{
					val = 2;
				}
				else if (component.velocityOverLifetime.x.mode.ToString() == "TwoCurves")
				{
					val = 3;
				}
				jsonobject.AddField("type", val);
				JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject5.Add(component.velocityOverLifetime.x.constant * -1f);
				jsonobject5.Add(component.velocityOverLifetime.y.constant);
				jsonobject5.Add(component.velocityOverLifetime.z.constant);
				jsonobject.AddField("constant", jsonobject5);
				DataManager.saveParticleFrameData(component.velocityOverLifetime.x, jsonobject, "gradientX", "velocitys", 0, component.velocityOverLifetime.x.curveMultiplier, -1f);
				DataManager.saveParticleFrameData(component.velocityOverLifetime.y, jsonobject, "gradientY", "velocitys", 0, component.velocityOverLifetime.y.curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.velocityOverLifetime.z, jsonobject, "gradientZ", "velocitys", 0, component.velocityOverLifetime.z.curveMultiplier, 1f);
				jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject5.Add(component.velocityOverLifetime.x.constantMin * -1f);
				jsonobject5.Add(component.velocityOverLifetime.y.constantMin);
				jsonobject5.Add(component.velocityOverLifetime.z.constantMin);
				jsonobject.AddField("constantMin", jsonobject5);
				jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject5.Add(component.velocityOverLifetime.x.constantMax * -1f);
				jsonobject5.Add(component.velocityOverLifetime.y.constantMax);
				jsonobject5.Add(component.velocityOverLifetime.z.constantMax);
				jsonobject.AddField("constantMax", jsonobject5);
				DataManager.saveParticleFrameData(component.velocityOverLifetime.x, jsonobject, "gradientXMin", "velocitys", -1, component.velocityOverLifetime.x.curveMultiplier, -1f);
				DataManager.saveParticleFrameData(component.velocityOverLifetime.x, jsonobject, "gradientXMax", "velocitys", 1, component.velocityOverLifetime.x.curveMultiplier, -1f);
				DataManager.saveParticleFrameData(component.velocityOverLifetime.y, jsonobject, "gradientYMin", "velocitys", -1, component.velocityOverLifetime.y.curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.velocityOverLifetime.y, jsonobject, "gradientYMax", "velocitys", 1, component.velocityOverLifetime.y.curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.velocityOverLifetime.z, jsonobject, "gradientZMin", "velocitys", -1, component.velocityOverLifetime.z.curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.velocityOverLifetime.z, jsonobject, "gradientZMax", "velocitys", 1, component.velocityOverLifetime.z.curveMultiplier, 1f);
			}
			if (component.limitVelocityOverLifetime.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3040) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Limit_Velocity_Over_Lifetime Module !");
			}
			if (component.inheritVelocity.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3050) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Inherit_Velocity Module !");
			}
			if (component.forceOverLifetime.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3060) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Force_Over_Lifetime Module !");
			}
			if (component.colorOverLifetime.enabled)
			{
				JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
				props.AddField("colorOverLifetime", jsonobject4);
				jsonobject4.AddField("enable", component.colorOverLifetime.enabled);
				jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject4.AddField("color", jsonobject);
				if (component.colorOverLifetime.color.mode.ToString() == "Gradient")
				{
					val = 1;
				}
				else if (component.colorOverLifetime.color.mode.ToString() == "TwoGradients")
				{
					val = 3;
				}
				jsonobject.AddField("type", val);
				jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject2.Add(component.colorOverLifetime.color.color.r);
				jsonobject2.Add(component.colorOverLifetime.color.color.g);
				jsonobject2.Add(component.colorOverLifetime.color.color.b);
				jsonobject2.Add(component.colorOverLifetime.color.color.a);
				jsonobject.AddField("constant", jsonobject2);
				DataManager.saveParticleFrameData(component.colorOverLifetime.color.gradient, jsonobject, "gradient");
				jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject2.Add(component.colorOverLifetime.color.colorMin.r);
				jsonobject2.Add(component.colorOverLifetime.color.colorMin.g);
				jsonobject2.Add(component.colorOverLifetime.color.colorMin.b);
				jsonobject2.Add(component.colorOverLifetime.color.colorMin.a);
				jsonobject.AddField("constantMin", jsonobject2);
				jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject2.Add(component.colorOverLifetime.color.colorMax.r);
				jsonobject2.Add(component.colorOverLifetime.color.colorMax.g);
				jsonobject2.Add(component.colorOverLifetime.color.colorMax.b);
				jsonobject2.Add(component.colorOverLifetime.color.colorMax.a);
				jsonobject.AddField("constantMax", jsonobject2);
				DataManager.saveParticleFrameData(component.colorOverLifetime.color.gradientMin, jsonobject, "gradientMin");
				DataManager.saveParticleFrameData(component.colorOverLifetime.color.gradientMax, jsonobject, "gradientMax");
			}
			if (component.colorBySpeed.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3080) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Color_By_Speed Module !");
			}
			if (component.sizeOverLifetime.enabled)
			{
				JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
				props.AddField("sizeOverLifetime", jsonobject4);
				jsonobject4.AddField("enable", component.sizeOverLifetime.enabled);
				jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject4.AddField("size", jsonobject);
				if (component.sizeOverLifetime.size.mode.ToString() == "Curve")
				{
					val = 0;
				}
				else if (component.sizeOverLifetime.size.mode.ToString() == "TwoConstants")
				{
					val = 1;
				}
				else if (component.sizeOverLifetime.size.mode.ToString() == "TwoCurves")
				{
					val = 2;
				}
				jsonobject.AddField("type", val);
				jsonobject.AddField("separateAxes", component.sizeOverLifetime.separateAxes);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.size, jsonobject, "gradient", "sizes", 0, component.sizeOverLifetime.size.curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.x, jsonobject, "gradientX", "sizes", 0, component.sizeOverLifetime.x.curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.y, jsonobject, "gradientY", "sizes", 0, component.sizeOverLifetime.y.curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.z, jsonobject, "gradientZ", "sizes", 0, component.sizeOverLifetime.z.curveMultiplier, 1f);
				jsonobject.AddField("constantMin", component.sizeOverLifetime.size.constantMin);
				jsonobject.AddField("constantMax", component.sizeOverLifetime.size.constantMax);
				JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject5.Add(component.sizeOverLifetime.x.constantMin);
				jsonobject5.Add(component.sizeOverLifetime.y.constantMin);
				jsonobject5.Add(component.sizeOverLifetime.z.constantMin);
				jsonobject.AddField("constantMinSeparate", jsonobject5);
				jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject5.Add(component.sizeOverLifetime.x.constantMax);
				jsonobject5.Add(component.sizeOverLifetime.y.constantMax);
				jsonobject5.Add(component.sizeOverLifetime.z.constantMax);
				jsonobject.AddField("constantMaxSeparate", jsonobject5);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.size, jsonobject, "gradientMin", "sizes", -1, component.sizeOverLifetime.size.curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.size, jsonobject, "gradientMax", "sizes", 1, component.sizeOverLifetime.size.curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.x, jsonobject, "gradientXMin", "sizes", -1, 1f, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.x, jsonobject, "gradientXMax", "sizes", 1, 1f, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.y, jsonobject, "gradientYMin", "sizes", -1, 1f, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.y, jsonobject, "gradientYMax", "sizes", 1, 1f, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.z, jsonobject, "gradientZMin", "sizes", -1, 1f, 1f);
				DataManager.saveParticleFrameData(component.sizeOverLifetime.z, jsonobject, "gradientZMax", "sizes", 1, 1f, 1f);
			}
			if (component.sizeBySpeed.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3100) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Size_By_Speed Module !");
			}
			if (component.rotationOverLifetime.enabled)
			{
				JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
				props.AddField("rotationOverLifetime", jsonobject4);
				jsonobject4.AddField("enable", component.rotationOverLifetime.enabled);
				jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject4.AddField("angularVelocity", jsonobject);
				if (component.rotationOverLifetime.z.mode.ToString() == "Constant")
				{
					val = 0;
				}
				else if (component.rotationOverLifetime.z.mode.ToString() == "Curve")
				{
					val = 1;
				}
				else if (component.rotationOverLifetime.z.mode.ToString() == "TwoConstants")
				{
					val = 2;
				}
				else if (component.rotationOverLifetime.z.mode.ToString() == "TwoCurves")
				{
					val = 3;
				}
				jsonobject.AddField("type", val);
				jsonobject.AddField("separateAxes", component.rotationOverLifetime.separateAxes);
				jsonobject.AddField("constant", component.rotationOverLifetime.z.constant * 180f / 3.14159274f);
				jsonobject.AddField("constantMin", component.rotationOverLifetime.z.constantMin * 180f / 3.14159274f);
				jsonobject.AddField("constantMax", component.rotationOverLifetime.z.constantMax * 180f / 3.14159274f);
				JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject5.Add(component.rotationOverLifetime.x.constant * 180f / 3.14159274f);
				jsonobject5.Add(-1f * component.rotationOverLifetime.y.constant * 180f / 3.14159274f);
				jsonobject5.Add(-1f * component.rotationOverLifetime.z.constant * 180f / 3.14159274f);
				jsonobject.AddField("constantSeparate", jsonobject5);
				jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject5.Add(component.rotationOverLifetime.x.constantMin * 180f / 3.14159274f);
				jsonobject5.Add(component.rotationOverLifetime.y.constantMin * 180f / 3.14159274f);
				jsonobject5.Add(component.rotationOverLifetime.z.constantMin * 180f / 3.14159274f);
				jsonobject.AddField("constantMinSeparate", jsonobject5);
				jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject5.Add(component.rotationOverLifetime.x.constantMax * 180f / 3.14159274f);
				jsonobject5.Add(component.rotationOverLifetime.y.constantMax * 180f / 3.14159274f);
				jsonobject5.Add(component.rotationOverLifetime.z.constantMax * 180f / 3.14159274f);
				jsonobject.AddField("constantMaxSeparate", jsonobject5);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradient", "angularVelocitys", 0, component.rotationOverLifetime.z.curveMultiplier * 180f / 3.14159274f, 1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.x, jsonobject, "gradientX", "angularVelocitys", 0, component.rotationOverLifetime.x.curveMultiplier * 180f / 3.14159274f, 1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.y, jsonobject, "gradientY", "angularVelocitys", 0, component.rotationOverLifetime.y.curveMultiplier * 180f / 3.14159274f, -1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradientZ", "angularVelocitys", 0, component.rotationOverLifetime.z.curveMultiplier * 180f / 3.14159274f, -1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradientMin", "angularVelocitys", -1, component.rotationOverLifetime.z.curveMultiplier * 180f / 3.14159274f, 1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradientMax", "angularVelocitys", 1, component.rotationOverLifetime.z.curveMultiplier * 180f / 3.14159274f, 1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradientXMin", "angularVelocitys", -1, component.rotationOverLifetime.x.curveMultiplier * 180f / 3.14159274f, 1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradientXMax", "angularVelocitys", 1, component.rotationOverLifetime.x.curveMultiplier * 180f / 3.14159274f, 1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradientYMin", "angularVelocitys", -1, component.rotationOverLifetime.y.curveMultiplier * 180f / 3.14159274f, 1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradientYMax", "angularVelocitys", 1, component.rotationOverLifetime.y.curveMultiplier * 180f / 3.14159274f, 1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradientZMin", "angularVelocitys", -1, component.rotationOverLifetime.z.curveMultiplier * 180f / 3.14159274f, 1f);
				DataManager.saveParticleFrameData(component.rotationOverLifetime.z, jsonobject, "gradientZMax", "angularVelocitys", 1, component.rotationOverLifetime.z.curveMultiplier * 180f / 3.14159274f, 1f);
			}
			if (component.rotationBySpeed.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3120) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Rotation_By_Speed Module !");
			}
			if (component.externalForces.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3130) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support External_Forces Module !");
			}
			if (component.noise.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3140) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Noise Module !");
			}
			if (component.collision.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3150) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Collision Module !");
			}
			if (component.trigger.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3160) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Trigger Module !");
			}
			if (component.subEmitters.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3170) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support SubEmitters Module !");
			}
			if (component.textureSheetAnimation.enabled)
			{
				JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
				props.AddField("textureSheetAnimation", jsonobject4);
				jsonobject4.AddField("enable", component.textureSheetAnimation.enabled);
				jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject4.AddField("tiles", jsonobject2);
				jsonobject2.Add(component.textureSheetAnimation.numTilesX);
				jsonobject2.Add(component.textureSheetAnimation.numTilesY);
				float num = 0f;
				ParticleSystemAnimationType animation = component.textureSheetAnimation.animation;
				if (animation != null)
				{
					if (animation == (UnityEngine.ParticleSystemAnimationType)1)
					{
						val = 1;
						num = (float)component.textureSheetAnimation.numTilesX;
					}
					else
					{
						Debug.LogWarning("unknown type.");
					}
				}
				else
				{
					val = 0;
					num = (float)(component.textureSheetAnimation.numTilesX * component.textureSheetAnimation.numTilesY);
				}
				float curveMultiplier = num * component.textureSheetAnimation.frameOverTime.curveMultiplier;
				jsonobject4.AddField("type", val);
				jsonobject4.AddField("randomRow", component.textureSheetAnimation.useRandomRow);
				jsonobject4.AddField("rowIndex", component.textureSheetAnimation.rowIndex);
				jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject4.AddField("frame", jsonobject);
				if (component.textureSheetAnimation.frameOverTime.mode.ToString() == "Constant")
				{
					val = 0;
				}
				else if (component.textureSheetAnimation.frameOverTime.mode.ToString() == "Curve")
				{
					val = 1;
				}
				else if (component.textureSheetAnimation.frameOverTime.mode.ToString() == "TwoConstants")
				{
					val = 2;
				}
				else if (component.textureSheetAnimation.frameOverTime.mode.ToString() == "TwoCurves")
				{
					val = 3;
				}
				jsonobject.AddField("type", val);
				jsonobject.AddField("constant", component.textureSheetAnimation.frameOverTime.constant * num);
				DataManager.saveParticleFrameData(component.textureSheetAnimation.frameOverTime, jsonobject, "overTime", "frames", 0, curveMultiplier, 1f);
				jsonobject.AddField("constantMin", component.textureSheetAnimation.frameOverTime.constantMin * num);
				jsonobject.AddField("constantMax", component.textureSheetAnimation.frameOverTime.constantMax * num);
				DataManager.saveParticleFrameData(component.textureSheetAnimation.frameOverTime, jsonobject, "overTimeMin", "frames", -1, curveMultiplier, 1f);
				DataManager.saveParticleFrameData(component.textureSheetAnimation.frameOverTime, jsonobject, "overTimeMax", "frames", 1, curveMultiplier, 1f);
				jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject4.AddField("startFrame", jsonobject);
				jsonobject.AddField("type", 0);
				jsonobject.AddField("constant", component.textureSheetAnimation.startFrame.constant * (float)(component.textureSheetAnimation.numTilesX * component.textureSheetAnimation.numTilesY - 1));
				jsonobject.AddField("constantMin", component.textureSheetAnimation.startFrame.constantMin * (float)(component.textureSheetAnimation.numTilesX * component.textureSheetAnimation.numTilesY - 1));
				jsonobject.AddField("constantMax", component.textureSheetAnimation.startFrame.constantMax * (float)(component.textureSheetAnimation.numTilesX * component.textureSheetAnimation.numTilesY - 1));
				jsonobject4.AddField("cycles", component.textureSheetAnimation.cycleCount);
				if (component.textureSheetAnimation.enabled)
				{
					jsonobject4.AddField("enableUVChannels", 1);
				}
				else
				{
					jsonobject4.AddField("enableUVChannels", 0);
				}
				jsonobject4.AddField("enableUVChannelsTip", component.textureSheetAnimation.uvChannelMask.ToString());
			}
			if (component.lights.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3190) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Lights Module !");
			}
			if (component.trails.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3200) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support Trails Module !");
			}
			if (component.customData.enabled)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:3014) : GameObject(" + gameObject.name + ") ParticleSystem Componment can't support CustomData Module !");
			}
			int num2;
			if (component2.renderMode.ToString() == "Billboard")
			{
				num2 = 0;
			}
			else if (component2.renderMode.ToString() == "Stretch")
			{
				num2 = 1;
			}
			else if (component2.renderMode.ToString() == "HorizontalBillboard")
			{
				num2 = 2;
			}
			else if (component2.renderMode.ToString() == "VerticalBillboard")
			{
				num2 = 3;
			}
			else if (component2.renderMode.ToString() == "Mesh")
			{
				num2 = 4;
			}
			else
			{
				num2 = 0;
			}
			props.AddField("renderMode", num2);
			props.AddField("stretchedBillboardCameraSpeedScale", component2.cameraVelocityScale);
			props.AddField("stretchedBillboardSpeedScale", component2.velocityScale);
			props.AddField("stretchedBillboardLengthScale", component2.lengthScale);
			props.AddField("sortingFudge", component2.sortingFudge);
			Material sharedMaterial = gameObject.GetComponent<Renderer>().sharedMaterial;
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
			if (sharedMaterial != null)
			{
				string text = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(sharedMaterial.GetInstanceID()).Split(new char[]
				{
					'.'
				})[0], false) + ".lmat";
				string savePath = DataManager.SAVEPATH + "/" + text;
				JSONObject jsonobject7 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject7.AddField("type", "Laya.ShurikenParticleMaterial");
				jsonobject7.AddField("path", text);
				jsonobject6.Add(jsonobject7);
				props.AddField("material", jsonobject7);
				string name = sharedMaterial.shader.name;
				if (name != "LayaAir3D/ShurikenParticle")
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"LayaAir3D Warning(Code:2002) : ",
						gameObject.name,
						" dose's match ",
						name,
						" Shader, Must use ShurikenParticle Shader！"
					}));
				}
				DataManager.saveLayaEffectLmatFile(sharedMaterial, savePath, "ShurikenParticle");
			}
			if (num2 == 4)
			{
				Mesh mesh = gameObject.GetComponent<ParticleSystemRenderer>().mesh;
				if (mesh != null)
				{
					string str = DataManager.cleanIllegalChar(mesh.name, true);
					string text2 = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(mesh.GetInstanceID()).Split(new char[]
					{
						'.'
					})[0], false) + "-" + str + ".lm";
					string text3 = DataManager.SAVEPATH + "/" + text2;
					props.AddField("meshPath", text2);
					if (!File.Exists(text3))
					{
						DataManager.saveLmFile(mesh, text3);
						return;
					}
				}
				else
				{
					props.AddField("meshPath", "");
					Debug.LogWarning("LayaAir3D Warning(Code:1001) : " + gameObject.name + "'s MeshFilter Component Mesh data can't be null!");
				}
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000C38C File Offset: 0x0000A58C
		public static void getTerrainComponentData(GameObject gameObject, JSONObject props)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			Terrain component = gameObject.GetComponent<Terrain>();
			if (DataManager.ConvertTerrainToMesh)
			{
				DataManager.saveTerrainLmFile(gameObject, props, 3);
				DataManager.saveTerrainLmatFile(gameObject, props);
			}
			else
			{
				DataManager.saveTerrainData(DataManager.SAVEPATH, props, null);
			}
			if (component.lightmapIndex > -1)
			{
				props.AddField("lightmapIndex", component.lightmapIndex);
				props.AddField("lightmapScaleOffset", jsonobject);
				jsonobject.Add(component.lightmapScaleOffset.x);
				jsonobject.Add(component.lightmapScaleOffset.y);
				jsonobject.Add(component.lightmapScaleOffset.z);
				jsonobject.Add(component.lightmapScaleOffset.w);
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000C438 File Offset: 0x0000A638
		public static void getPhysicsColliderComponentData(GameObject gameObject, JSONObject component)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			component.Add(jsonobject);
			jsonobject.AddField("type", "PhysicsCollider");
			jsonobject.AddField("restitution", 0f);
			jsonobject.AddField("friction", 0.5f);
			jsonobject.AddField("rollingFriction", 0f);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject.AddField("shapes", jsonobject2);
			jsonobject.AddField("isTrigger", DataManager.getPhysicsShapesData(gameObject, jsonobject2));
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000C4B8 File Offset: 0x0000A6B8
		public static void getRigidbody3DComponentData(GameObject gameObject, JSONObject component)
		{
			Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			component.Add(jsonobject);
			jsonobject.AddField("type", "Rigidbody3D");
			jsonobject.AddField("mass", component2.mass);
			jsonobject.AddField("isKinematic", component2.isKinematic);
			jsonobject.AddField("restitution", 0f);
			jsonobject.AddField("friction", 0.5f);
			jsonobject.AddField("rollingFriction", 0f);
			jsonobject.AddField("linearDamping", 0);
			jsonobject.AddField("angularDamping", 0);
			jsonobject.AddField("overrideGravity", !component2.useGravity);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.Add(0);
			jsonobject2.Add(0);
			jsonobject2.Add(0);
			jsonobject.AddField("gravity", jsonobject2);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject.AddField("shapes", jsonobject3);
			jsonobject.AddField("isTrigger", DataManager.getPhysicsShapesData(gameObject, jsonobject3));
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000C5B8 File Offset: 0x0000A7B8
		public static void getTrailRendererComponentData(GameObject gameObject, JSONObject props)
		{
			TrailRenderer component = gameObject.GetComponent<TrailRenderer>();
			props.AddField("time", component.time);
			props.AddField("minVertexDistance", component.minVertexDistance);
			props.AddField("widthMultiplier", component.widthMultiplier);
			if (component.textureMode == null)
			{
				props.AddField("textureMode", 0);
			}
			else
			{
				props.AddField("textureMode", 1);
			}
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("widthCurve", jsonobject);
			Keyframe[] keys = component.widthCurve.keys;
			for (int i = 0; i < keys.Length; i++)
			{
				Keyframe keyframe = keys[i];
				JSONObject jsonobject2;
				if (i == 0 && keyframe.time != 0f)
				{
					jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject2.AddField("time", 0);
					jsonobject2.AddField("inTangent", 0);
					jsonobject2.AddField("outTangent", 0);
					jsonobject2.AddField("value", keyframe.value);
					jsonobject.Add(jsonobject2);
				}
				jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject2.AddField("time", keyframe.time);
				jsonobject2.AddField("inTangent", keyframe.inTangent);
				jsonobject2.AddField("outTangent", keyframe.inTangent);
				jsonobject2.AddField("value", keyframe.value);
				jsonobject.Add(jsonobject2);
				if (i == keys.Length - 1 && keyframe.time != 1f)
				{
					jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject2.AddField("time", 1);
					jsonobject2.AddField("inTangent", 0);
					jsonobject2.AddField("outTangent", 0);
					jsonobject2.AddField("value", keyframe.value);
					jsonobject.Add(jsonobject2);
				}
			}
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
			props.AddField("colorGradient", jsonobject3);
			Gradient colorGradient = component.colorGradient;
			if (colorGradient.mode == null)
			{
				jsonobject3.AddField("mode", 0);
			}
			else
			{
				jsonobject3.AddField("mode", 1);
			}
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject3.AddField("colorKeys", jsonobject4);
			GradientColorKey[] colorKeys = colorGradient.colorKeys;
			for (int j = 0; j < colorKeys.Length; j++)
			{
				GradientColorKey gradientColorKey = colorKeys[j];
				JSONObject jsonobject5;
				JSONObject jsonobject6;
				Color color;
				if (j == 0 && gradientColorKey.time != 0f)
				{
					jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject5.AddField("time", 0);
					jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
					jsonobject5.AddField("value", jsonobject6);
					color = gradientColorKey.color;
					jsonobject6.Add(color.r);
					jsonobject6.Add(color.g);
					jsonobject6.Add(color.b);
					jsonobject4.Add(jsonobject5);
				}
				jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject5.AddField("time", gradientColorKey.time);
				jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject5.AddField("value", jsonobject6);
				color = gradientColorKey.color;
				jsonobject6.Add(color.r);
				jsonobject6.Add(color.g);
				jsonobject6.Add(color.b);
				jsonobject4.Add(jsonobject5);
				if (j == colorKeys.Length - 1 && gradientColorKey.time != 1f)
				{
					jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject5.AddField("time", 1);
					jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
					jsonobject5.AddField("value", jsonobject6);
					color = gradientColorKey.color;
					jsonobject6.Add(color.r);
					jsonobject6.Add(color.g);
					jsonobject6.Add(color.b);
					jsonobject4.Add(jsonobject5);
				}
			}
			JSONObject jsonobject7 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject3.AddField("alphaKeys", jsonobject7);
			GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;
			for (int k = 0; k < alphaKeys.Length; k++)
			{
				GradientAlphaKey gradientAlphaKey = alphaKeys[k];
				JSONObject jsonobject8;
				if (k == 0 && gradientAlphaKey.time != 0f)
				{
					jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject8.AddField("time", 0);
					jsonobject8.AddField("value", gradientAlphaKey.alpha);
					jsonobject7.Add(jsonobject8);
				}
				jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject8.AddField("time", gradientAlphaKey.time);
				jsonobject8.AddField("value", gradientAlphaKey.alpha);
				jsonobject7.Add(jsonobject8);
				if (k == alphaKeys.Length - 1 && gradientAlphaKey.time != 1f)
				{
					jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject8.AddField("time", 1);
					jsonobject8.AddField("value", gradientAlphaKey.alpha);
					jsonobject7.Add(jsonobject8);
				}
			}
			Material[] sharedMaterials = component.sharedMaterials;
			JSONObject jsonobject9 = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("materials", jsonobject9);
			foreach (Material material in sharedMaterials)
			{
				if (!(material == null))
				{
					string text = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(material.GetInstanceID()).Split(new char[]
					{
						'.'
					})[0], false) + ".lmat";
					string savePath = DataManager.SAVEPATH + "/" + text;
					string name = material.shader.name;
					if (name != "LayaAir3D/Trail")
					{
						Debug.LogWarning(string.Concat(new string[]
						{
							"LayaAir3D Warning(Code:2003) : ",
							gameObject.name,
							" dose's match ",
							name,
							"Shader, Must use Trail Shader！"
						}));
					}
					JSONObject jsonobject10 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject10.AddField("type", "Laya.TrailMaterial");
					jsonobject10.AddField("path", text);
					jsonobject9.Add(jsonobject10);
					DataManager.saveLayaEffectLmatFile(material, savePath, "Trail");
				}
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000CB84 File Offset: 0x0000AD84
		public static void getLineRendererComponentData(GameObject gameObject, JSONObject props)
		{
			LineRenderer component = gameObject.GetComponent<LineRenderer>();
			props.AddField("useWorldSpace", component.useWorldSpace);
			props.AddField("widthMultiplier", component.widthMultiplier);
			if (component.textureMode == null)
			{
				props.AddField("textureMode", 0);
			}
			else
			{
				props.AddField("textureMode", 1);
			}
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			props.AddField("positions", jsonobject);
			jsonobject.AddField("size", component.positionCount);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject.AddField("values", jsonobject2);
			for (int i = 0; i < component.positionCount; i++)
			{
				JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject2.Add(jsonobject3);
				Vector3 position = component.GetPosition(i);
				jsonobject3.Add(-position.x);
				jsonobject3.Add(position.y);
				jsonobject3.Add(position.z);
			}
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("widthCurve", jsonobject4);
			Keyframe[] keys = component.widthCurve.keys;
			for (int j = 0; j < keys.Length; j++)
			{
				Keyframe keyframe = keys[j];
				JSONObject jsonobject5;
				if (j == 0 && keyframe.time != 0f)
				{
					jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject5.AddField("time", 0);
					jsonobject5.AddField("inTangent", 0);
					jsonobject5.AddField("outTangent", 0);
					jsonobject5.AddField("value", keyframe.value);
					jsonobject4.Add(jsonobject5);
				}
				jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject5.AddField("time", keyframe.time);
				jsonobject5.AddField("inTangent", keyframe.inTangent);
				jsonobject5.AddField("outTangent", keyframe.inTangent);
				jsonobject5.AddField("value", keyframe.value);
				jsonobject4.Add(jsonobject5);
				if (j == keys.Length - 1 && keyframe.time != 1f)
				{
					jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject5.AddField("time", 1);
					jsonobject5.AddField("inTangent", 0);
					jsonobject5.AddField("outTangent", 0);
					jsonobject5.AddField("value", keyframe.value);
					jsonobject4.Add(jsonobject5);
				}
			}
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.OBJECT);
			props.AddField("colorGradient", jsonobject6);
			Gradient colorGradient = component.colorGradient;
			if (colorGradient.mode == null)
			{
				jsonobject6.AddField("mode", 0);
			}
			else
			{
				jsonobject6.AddField("mode", 1);
			}
			JSONObject jsonobject7 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject6.AddField("colorKeys", jsonobject7);
			GradientColorKey[] colorKeys = colorGradient.colorKeys;
			for (int k = 0; k < colorKeys.Length; k++)
			{
				GradientColorKey gradientColorKey = colorKeys[k];
				JSONObject jsonobject8;
				JSONObject jsonobject9;
				Color color;
				if (k == 0 && gradientColorKey.time != 0f)
				{
					jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject8.AddField("time", 0);
					jsonobject9 = new JSONObject(JSONObject.Type.ARRAY);
					jsonobject8.AddField("value", jsonobject9);
					color = gradientColorKey.color;
					jsonobject9.Add(color.r);
					jsonobject9.Add(color.g);
					jsonobject9.Add(color.b);
					jsonobject7.Add(jsonobject8);
				}
				jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject8.AddField("time", gradientColorKey.time);
				jsonobject9 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject8.AddField("value", jsonobject9);
				color = gradientColorKey.color;
				jsonobject9.Add(color.r);
				jsonobject9.Add(color.g);
				jsonobject9.Add(color.b);
				jsonobject7.Add(jsonobject8);
				if (k == colorKeys.Length - 1 && gradientColorKey.time != 1f)
				{
					jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject8.AddField("time", 1);
					jsonobject9 = new JSONObject(JSONObject.Type.ARRAY);
					jsonobject8.AddField("value", jsonobject9);
					color = gradientColorKey.color;
					jsonobject9.Add(color.r);
					jsonobject9.Add(color.g);
					jsonobject9.Add(color.b);
					jsonobject7.Add(jsonobject8);
				}
			}
			JSONObject jsonobject10 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject6.AddField("alphaKeys", jsonobject10);
			GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;
			for (int l = 0; l < alphaKeys.Length; l++)
			{
				GradientAlphaKey gradientAlphaKey = alphaKeys[l];
				JSONObject jsonobject11;
				if (l == 0 && gradientAlphaKey.time != 0f)
				{
					jsonobject11 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject11.AddField("time", 0);
					jsonobject11.AddField("value", gradientAlphaKey.alpha);
					jsonobject10.Add(jsonobject11);
				}
				jsonobject11 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject11.AddField("time", gradientAlphaKey.time);
				jsonobject11.AddField("value", gradientAlphaKey.alpha);
				jsonobject10.Add(jsonobject11);
				if (l == alphaKeys.Length - 1 && gradientAlphaKey.time != 1f)
				{
					jsonobject11 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject11.AddField("time", 1);
					jsonobject11.AddField("value", gradientAlphaKey.alpha);
					jsonobject10.Add(jsonobject11);
				}
			}
			Material[] sharedMaterials = component.sharedMaterials;
			JSONObject jsonobject12 = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("materials", jsonobject12);
			foreach (Material material in sharedMaterials)
			{
				if (!(material == null))
				{
					string text = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(material.GetInstanceID()).Split(new char[]
					{
						'.'
					})[0], false) + ".lmat";
					string savePath = DataManager.SAVEPATH + "/" + text;
					string name = material.shader.name;
					if (name != "LayaAir3D/Line")
					{
						Debug.LogWarning(string.Concat(new string[]
						{
							"LayaAir3D Warning(Code:2004) : ",
							gameObject.name,
							" dose's match ",
							name,
							"Shader, Must use Line Shader！"
						}));
					}
					JSONObject jsonobject13 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject13.AddField("type", "Laya.LineMaterial");
					jsonobject13.AddField("path", text);
					jsonobject12.Add(jsonobject13);
					DataManager.saveLayaEffectLmatFile(material, savePath, "Line");
				}
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000D1D8 File Offset: 0x0000B3D8
		public static bool getPhysicsShapesData(GameObject gameObject, JSONObject shapes)
		{
			bool result = false;
			foreach (BoxCollider boxCollider in gameObject.GetComponents<BoxCollider>())
			{
				JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
				JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject.AddField("type", "BoxColliderShape");
				Vector3 center = boxCollider.center;
				jsonobject2.Add(-center.x);
				jsonobject2.Add(center.y);
				jsonobject2.Add(center.z);
				jsonobject.AddField("center", jsonobject2);
				Vector3 size = boxCollider.size;
				jsonobject3.Add(size.x);
				jsonobject3.Add(size.y);
				jsonobject3.Add(size.z);
				jsonobject.AddField("size", jsonobject3);
				if (boxCollider.isTrigger)
				{
					result = true;
				}
				shapes.Add(jsonobject);
			}
			foreach (SphereCollider sphereCollider in gameObject.GetComponents<SphereCollider>())
			{
				JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
				JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject4.AddField("type", "SphereColliderShape");
				Vector3 center2 = sphereCollider.center;
				jsonobject5.Add(-center2.x);
				jsonobject5.Add(center2.y);
				jsonobject5.Add(center2.z);
				jsonobject4.AddField("center", jsonobject5);
				jsonobject4.AddField("radius", sphereCollider.radius);
				if (sphereCollider.isTrigger)
				{
					result = true;
				}
				shapes.Add(jsonobject4);
			}
			foreach (CapsuleCollider capsuleCollider in gameObject.GetComponents<CapsuleCollider>())
			{
				JSONObject jsonobject6 = new JSONObject(JSONObject.Type.OBJECT);
				JSONObject jsonobject7 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject6.AddField("type", "CapsuleColliderShape");
				Vector3 center3 = capsuleCollider.center;
				jsonobject7.Add(center3.x);
				jsonobject7.Add(center3.y);
				jsonobject7.Add(center3.z);
				jsonobject6.AddField("center", jsonobject7);
				jsonobject6.AddField("radius", capsuleCollider.radius);
				jsonobject6.AddField("height", capsuleCollider.height);
				if (capsuleCollider.isTrigger)
				{
					result = true;
				}
				shapes.Add(jsonobject6);
			}
			foreach (MeshCollider meshCollider in gameObject.GetComponents<MeshCollider>())
			{
				JSONObject jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject8.AddField("type", "MeshColliderShape");
				Mesh sharedMesh = meshCollider.sharedMesh;
				if (sharedMesh != null)
				{
					string str = DataManager.cleanIllegalChar(sharedMesh.name, true);
					string text = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(sharedMesh.GetInstanceID()).Split(new char[]
					{
						'.'
					})[0], false) + "-" + str;
					if (!DataManager.OptimizeMeshName)
					{
						text = string.Concat(new object[]
						{
							text,
							"[",
							sharedMesh.GetInstanceID(),
							"].lm"
						});
					}
					else
					{
						text += ".lm";
					}
					jsonobject8.AddField("mesh", text);
					string key = DataManager.SAVEPATH + "/" + text;
					if (!DataManager.lmInfo.ContainsKey(key))
					{
						DataManager.lmInfo.Add(key, sharedMesh);
					}
				}
				else
				{
					Debug.LogWarning("LayaAir3D Warning(Code:1001) : " + gameObject.name + "'s MeshFilter Component Mesh data can't be null!");
				}
				if (meshCollider.isTrigger)
				{
					result = true;
				}
				shapes.Add(jsonobject8);
			}
			return result;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000D580 File Offset: 0x0000B780
		public static void getLayaLmatData(GameObject gameObject, Material[] materials, JSONObject materialsNode)
		{
			foreach (Material material in materials)
			{
				if (!(material == null))
				{
					string text = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(material.GetInstanceID()).Split(new char[]
					{
						'.'
					})[0], false) + ".lmat";
					string savePath = DataManager.SAVEPATH + "/" + text;
					string name = material.shader.name;
					if (name.Split(new char[]
					{
						'/'
					})[0] == "LayaAir3D")
					{
						if (name.Split(new char[]
						{
							'/'
						}).Length == 2)
						{
							JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
							materialsNode.Add(jsonobject);
							string text2 = name.Split(new char[]
							{
								'/'
							})[1];
							if (text2 == "BlinnPhong")
							{
								jsonobject.AddField("path", text);
								DataManager.saveLayaLmatFile(material, savePath, text2);
							}
							else if (text2 == "Unlit")
							{
								jsonobject.AddField("path", text);
								DataManager.saveLayaLmatFile(material, savePath, text2);
							}
							else if (text2 == "Effect")
							{
								jsonobject.AddField("path", text);
								DataManager.saveLayaEffectLmatFile(material, savePath, text2);
							}
							else if (text2 == "PBR(Standard)")
							{
								jsonobject.AddField("path", text);
								DataManager.saveLayaLmatFile(material, savePath, text2);
							}
							else if (text2 == "PBR(Specular)")
							{
								jsonobject.AddField("path", text);
								DataManager.saveLayaLmatFile(material, savePath, text2);
							}
							else
							{
								jsonobject.AddField("path", text);
								DataManager.saveLayaLmatFile(material, savePath, "BlinnPhong");
								Debug.LogWarning(string.Concat(new string[]
								{
									"LayaAir3D Warning(Code:2001) : ",
									gameObject.name,
									" dose's match ",
									name,
									"Shader!"
								}));
							}
						}
						else if (name.Split(new char[]{'/'}).Length == 3)
						{
							JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
							if (name.Split (new char[] {'/'}) [1] == "Addition") {
								string UserShaderName = name.Split(new char[] {'/'})[2];
								jsonobject2.AddField("type", "Laya."+UserShaderName+"Material");
								jsonobject2.AddField("path", text);
								materialsNode.Add(jsonobject2);
								DataManager.saveLayaParam(material, savePath,UserShaderName+"Material");
							}
							else if (name.Split(new char[]{'/'})[1] == "Water")
							{
								if (name.Split(new char[]
								{
									'/'
								})[2] == "Water (Primary)")
								{
									jsonobject2.AddField("type", "Laya.WaterPrimaryMaterial");
									jsonobject2.AddField("path", text);
									materialsNode.Add(jsonobject2);
									DataManager.saveLayaWaterLmatFile(material, savePath);
								}
								else
								{
									Debug.LogWarning(string.Concat(new string[]
									{
										"LayaAir3D Warning(Code:2001) : ",
										gameObject.name,
										" dose's match ",
										name,
										"Shader!"
									}));
								}
							}
							else
							{
								Debug.LogWarning(string.Concat(new string[]
								{
									"LayaAir3D Warning(Code:2001) : ",
									gameObject.name,
									" dose's match ",
									name,
									"Shader!"
								}));
							}

						}
					}
					else
					{
						JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject3.AddField("type", "Laya.BlinnPhongMaterial");
						jsonobject3.AddField("path", text);
						materialsNode.Add(jsonobject3);
						string shaderType = "BlinnPhong";
						DataManager.saveLayaLmatFile(material, savePath, shaderType);
						Debug.LogWarning("LayaAir3D Warning(Code:2000) : " + gameObject.name + " must use LayaAir3D shader!");
					}
				}
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000D8DC File Offset: 0x0000BADC
		public static void getLavData(GameObject gameObject, JSONObject parentsChildNodes, GameObject animatorGameObject)
		{
			DataManager.checkChildIsNotLegal(gameObject, true);
			if (!gameObject.activeSelf && DataManager.IgnoreNotActiveGameObject)
			{
				return;
			}
			if ((DataManager.selectParentbyType(gameObject, DataManager.ComponentType.Animator) != animatorGameObject || DataManager.componentsOnGameObject(gameObject).IndexOf(DataManager.ComponentType.Animator) != -1) && gameObject != animatorGameObject)
			{
				return;
			}
			if (!DataManager.curNodeHasNotLegalChild)
			{
				return;
			}
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
			Vector3 localPosition = gameObject.transform.localPosition;
			Quaternion localRotation = gameObject.transform.localRotation;
			Vector3 localScale = gameObject.transform.localScale;
			jsonobject2.AddField("name", gameObject.name);
			jsonobject.AddField("props", jsonobject2);
			jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.AddField("translate", jsonobject3);
			jsonobject3.Add(localPosition.x * -1f);
			jsonobject3.Add(localPosition.y);
			jsonobject3.Add(localPosition.z);
			jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.AddField("rotation", jsonobject4);
			jsonobject4.Add(localRotation.x * -1f);
			jsonobject4.Add(localRotation.y);
			jsonobject4.Add(localRotation.z);
			jsonobject4.Add(localRotation.w * -1f);
			jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.AddField("scale", jsonobject5);
			jsonobject5.Add(localScale.x);
			jsonobject5.Add(localScale.y);
			jsonobject5.Add(localScale.z);
			if (gameObject.transform.childCount > 0)
			{
				JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject.AddField("child", jsonobject6);
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					DataManager.getLavData(gameObject.transform.GetChild(i).gameObject, jsonobject6, animatorGameObject);
				}
			}
			else
			{
				jsonobject.AddField("child", new JSONObject(JSONObject.Type.ARRAY));
			}
			parentsChildNodes.Add(jsonobject);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000DADC File Offset: 0x0000BCDC
		public static void getSkyBoxData(Material skyBoxMaterial, JSONObject props)
		{
			string text = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(skyBoxMaterial.GetInstanceID()).Split(new char[]
			{
				'.'
			})[0], false) + ".lmat";
			string text2 = DataManager.SAVEPATH + "/" + text;
			if (skyBoxMaterial.shader.name == "Skybox/Cubemap")
			{
				JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				props.AddField("skyboxMaterial", jsonobject);
				jsonobject.AddField("type", "Laya.SkyBoxMaterial");
				jsonobject.AddField("path", text);
				if (!File.Exists(text2))
				{
					DataManager.saveLayaSkyBoxLmatFile(skyBoxMaterial, text2);
				}
			}
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000DB80 File Offset: 0x0000BD80
		public static void saveLmFile(Mesh mesh, string savePath)
		{
			string item = DataManager.cleanIllegalChar(mesh.name, true);
			ushort num = (ushort)mesh.subMeshCount;
			int num2 = (int)(num + 1);
			FileStream fileStream = Util.FileUtil.saveFile(savePath, null);
			ushort num3 = 0;
			string text = "";
			for (int i = 0; i < DataManager.VertexStructure.Length; i++)
			{
				DataManager.VertexStructure[i] = 0;
			}
			Vector3[] vertices = mesh.vertices;
			Vector3[] normals = mesh.normals;
			Color[] colors = mesh.colors;
			Vector2[] uv = mesh.uv;
			Vector2[] uv2 = mesh.uv2;
			Vector4[] tangents = mesh.tangents;
			if (vertices != null && vertices.Length != 0)
			{
				DataManager.VertexStructure[0] = 1;
				text += "POSITION";
				num3 += 12;
			}
			if (normals != null && normals.Length != 0 && !DataManager.IgnoreNormal)
			{
				DataManager.VertexStructure[1] = 1;
				text += ",NORMAL";
				num3 += 12;
			}
			if (colors != null && colors.Length != 0 && !DataManager.IgnoreColor)
			{
				DataManager.VertexStructure[2] = 1;
				text += ",COLOR";
				num3 += 16;
			}
			if (uv != null && uv.Length != 0 && !DataManager.IgnoreUV)
			{
				DataManager.VertexStructure[3] = 1;
				text += ",UV";
				num3 += 8;
			}
			if (uv2 != null && uv2.Length != 0 && !DataManager.IgnoreUV)
			{
				DataManager.VertexStructure[4] = 1;
				text += ",UV1";
				num3 += 8;
			}
			if (tangents != null && tangents.Length != 0 && !DataManager.IgnoreTangent)
			{
				DataManager.VertexStructure[6] = 1;
				text += ",TANGENT";
				num3 += 16;
			}
			int[] array = new int[(int)num];
			int[] array2 = new int[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				int[] indices = mesh.GetIndices(i);
				array[i] = indices[0];
				array2[i] = indices.Length;
			}
			long[] array3 = new long[(int)num];
			long[] array4 = new long[(int)num];
			long[] array5 = new long[(int)num];
			List<string> list = new List<string>();
			list.Add("MESH");
			list.Add("SUBMESH");
			string lmVersion = DataManager.LmVersion;
			Util.FileUtil.WriteData(fileStream, lmVersion);
			long position = fileStream.Position;
			long position2 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			long position3 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)num2
			});
			for (int i = 0; i < num2; i++)
			{
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
			}
			long position4 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new ushort[1]);
			long position5 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list.IndexOf("MESH")
			});
			list.Add(item);
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list.IndexOf(item)
			});
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				1
			});
			long position6 = fileStream.Position;
			for (int i = 0; i < 1; i++)
			{
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				list.Add(text);
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)list.IndexOf(text)
				});
			}
			long position7 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			long position8 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new ushort[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			long num4 = fileStream.Position - position5;
			for (int i = 0; i < (int)num; i++)
			{
				array3[i] = fileStream.Position;
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)list.IndexOf("SUBMESH")
				});
				Util.FileUtil.WriteData(fileStream, new ushort[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					1
				});
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				array4[i] = fileStream.Position;
				array5[i] = array4[i] - array3[i];
			}
			long position9 = fileStream.Position;
			for (int i = 0; i < list.Count; i++)
			{
				Util.FileUtil.WriteData(fileStream, list[i]);
			}
			long num5 = fileStream.Position - position9;
			long position10 = fileStream.Position;
			for (int j = 0; j < mesh.vertexCount; j++)
			{
				Vector3 vector = vertices[j];
				Util.FileUtil.WriteData(fileStream, new float[]
				{
					vector.x * -1f,
					vector.y,
					vector.z
				});
				if (DataManager.VertexStructure[1] == 1)
				{
					Vector3 vector2 = normals[j];
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						vector2.x * -1f,
						vector2.y,
						vector2.z
					});
				}
				if (DataManager.VertexStructure[2] == 1)
				{
					Color color = colors[j];
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						color.r,
						color.g,
						color.b,
						color.a
					});
				}
				if (DataManager.VertexStructure[3] == 1)
				{
					Vector2 vector3 = uv[j];
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						vector3.x,
						vector3.y * -1f + 1f
					});
				}
				if (DataManager.VertexStructure[4] == 1)
				{
					Vector2 vector4 = uv2[j];
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						vector4.x,
						vector4.y * -1f + 1f
					});
				}
				if (DataManager.VertexStructure[6] == 1)
				{
					Vector4 vector5 = tangents[j];
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						vector5.x * -1f,
						vector5.y,
						vector5.z,
						vector5.w
					});
				}
			}
			long num6 = fileStream.Position - position10;
			long position11 = fileStream.Position;
			int[] triangles = mesh.triangles;
			for (int j = 0; j < triangles.Length; j++)
			{
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)triangles[j]
				});
			}
			long num7 = fileStream.Position - position11;
			uint num8 = 0u;
			for (int i = 0; i < (int)num; i++)
			{
				fileStream.Position = array3[i] + 4L;
				uint num9;
				uint num10;
				uint num11;
				uint num12;
				if (num == 1)
				{
					num9 = 0u;
					num10 = (uint)(num6 / (long)((ulong)num3));
					num11 = 0u;
					num12 = (uint)(num7 / 2L);
				}
				else if (i == 0)
				{
					num9 = 0u;
					num10 = (uint)array[i + 1];
					num11 = num8;
					num12 = (uint)array2[i];
				}
				else if (i < (int)(num - 1))
				{
					num9 = (uint)array[i];
					num10 = (uint)(array[i + 1] - array[i]);
					num11 = num8;
					num12 = (uint)array2[i];
				}
				else
				{
					num9 = (uint)array[i];
					num10 = (uint)(num6 / (long)((ulong)num3) - (long)array[i]);
					num11 = num8;
					num12 = (uint)array2[i];
				}
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num9
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num10
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num11
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num12
				});
				num8 += num12;
				fileStream.Position += 2L;
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num11
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num12
				});
			}
			fileStream.Position = position6;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(position10 - position9)
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)num6
			});
			fileStream.Position = position7;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(position11 - position9)
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)num7
			});
			fileStream.Position = position4;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list.Count
			});
			long position12 = fileStream.Position;
			fileStream.Position = position3 + 2L;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)position5
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)num4
			});
			for (int i = 0; i < (int)num; i++)
			{
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					(uint)array3[i]
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					(uint)array5[i]
				});
			}
			fileStream.Position = position2;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)position9
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(position9 + num5 + num6 + num7 + array5[0])
			});
			fileStream.Close();
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000E4FC File Offset: 0x0000C6FC
		public static void saveSkinLmFile(SkinnedMeshRenderer skinnedMeshRenderer, string savePath)
		{
			Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
			int num = sharedMesh.GetIndices(0).Length;
			ushort num2 = 1;
			ushort num3 = (ushort)sharedMesh.subMeshCount;
			ushort num4 = 0;
			string text = "";
			string item = DataManager.cleanIllegalChar(sharedMesh.name, true);
			for (int i = 0; i < DataManager.VertexStructure.Length; i++)
			{
				DataManager.VertexStructure[i] = 0;
			}
			if (sharedMesh.vertices != null && sharedMesh.vertices.Length != 0)
			{
				DataManager.VertexStructure[0] = 1;
				text += "POSITION";
				num4 += 12;
			}
			if (sharedMesh.normals != null && sharedMesh.normals.Length != 0 && !DataManager.IgnoreNormal)
			{
				DataManager.VertexStructure[1] = 1;
				text += ",NORMAL";
				num4 += 12;
			}
			if (sharedMesh.colors != null && sharedMesh.colors.Length != 0 && !DataManager.IgnoreColor)
			{
				DataManager.VertexStructure[2] = 1;
				text += ",COLOR";
				num4 += 16;
			}
			if (sharedMesh.uv != null && sharedMesh.uv.Length != 0 && !DataManager.IgnoreUV)
			{
				DataManager.VertexStructure[3] = 1;
				text += ",UV";
				num4 += 8;
			}
			if (sharedMesh.uv2 != null && sharedMesh.uv2.Length != 0 && !DataManager.IgnoreUV)
			{
				DataManager.VertexStructure[4] = 1;
				text += ",UV1";
				num4 += 8;
			}
			if (sharedMesh.boneWeights != null && sharedMesh.boneWeights.Length != 0)
			{
				DataManager.VertexStructure[5] = 1;
				text += ",BLENDWEIGHT,BLENDINDICES";
				num4 += 32;
			}
			if (sharedMesh.tangents != null && sharedMesh.tangents.Length != 0 && !DataManager.IgnoreTangent)
			{
				DataManager.VertexStructure[6] = 1;
				text += ",TANGENT";
				num4 += 16;
			}
			List<Transform> list = new List<Transform>();
			for (int j = 0; j < skinnedMeshRenderer.bones.Length; j++)
			{
				Transform item2 = skinnedMeshRenderer.bones[j];
				if (list.IndexOf(item2) == -1)
				{
					list.Add(item2);
				}
			}
			List<DataManager.VertexData> list2 = new List<DataManager.VertexData>();
			List<DataManager.VertexData> list3 = new List<DataManager.VertexData>();
			List<DataManager.VertexData> list4 = new List<DataManager.VertexData>();
			int[] array = new int[3];
			List<int> list5 = new List<int>();
			List<List<int>>[] array2 = new List<List<int>>[(int)num3];
			List<int> list6 = new List<int>();
			List<int> list7 = new List<int>();
			List<int>[] array3 = new List<int>[(int)num3];
			for (int i = 0; i < (int)num3; i++)
			{
				int[] indices = sharedMesh.GetIndices(i);
				array2[i] = new List<List<int>>();
				array3[i] = new List<int>();
				for (int j = 0; j < indices.Length; j += 3)
				{
					for (int k = 0; k < 3; k++)
					{
						int num5 = j + k;
						int num6 = indices[num5];
						array[k] = -1;
						for (int l = 0; l < list3.Count; l++)
						{
							if (list3[l].index == num6)
							{
								array[k] = l;
								break;
							}
						}
						if (array[k] == -1)
						{
							DataManager.VertexData vertexData = DataManager.getVertexData(sharedMesh, num6);
							list4.Add(vertexData);
							for (int l = 0; l < 4; l++)
							{
								float num7 = vertexData.boneIndex[l];
								if (list6.IndexOf((int)num7) == -1 && list7.IndexOf((int)num7) == -1)
								{
									list7.Add((int)num7);
								}
							}
						}
					}
					if (list6.Count + list7.Count <= DataManager.MaxBoneCount)
					{
						for (int m = 0; m < list7.Count; m++)
						{
							list6.Add(list7[m]);
						}
						int num8 = 1;
						for (int m = 0; m < 3; m++)
						{
							if (array[m] == -1)
							{
								list5.Add(list3.Count - 1 + num8++ + list2.Count);
							}
							else
							{
								list5.Add(array[m] + list2.Count);
							}
						}
						for (int m = 0; m < list4.Count; m++)
						{
							list3.Add(list4[m]);
						}
					}
					else
					{
						array3[i].Add(j);
						array2[i].Add(list6);
						for (int m = 0; m < list3.Count; m++)
						{
							DataManager.VertexData vertexData = list3[m];
							for (int l = 0; l < 4; l++)
							{
								vertexData.boneIndex[l] = (float)list6.IndexOf((int)vertexData.boneIndex[l]);
							}
							list2.Add(vertexData);
						}
						j -= 3;
						list3 = new List<DataManager.VertexData>();
						list6 = new List<int>();
					}
					if (j + 3 == indices.Length)
					{
						array3[i].Add(indices.Length);
						array2[i].Add(list6);
						for (int m = 0; m < list3.Count; m++)
						{
							DataManager.VertexData vertexData = list3[m];
							for (int l = 0; l < 4; l++)
							{
								vertexData.boneIndex[l] = (float)list6.IndexOf((int)vertexData.boneIndex[l]);
							}
							list2.Add(vertexData);
						}
						list3 = new List<DataManager.VertexData>();
						list6 = new List<int>();
					}
					list7 = new List<int>();
					list4 = new List<DataManager.VertexData>();
				}
			}
			int[] array4 = new int[(int)num3];
			int[] array5 = new int[(int)num3];
			int num9 = 0;
			for (int i = 0; i < (int)num3; i++)
			{
				int[] indices2 = sharedMesh.GetIndices(i);
				array4[i] = list5[num9];
				array5[i] = indices2.Length;
				num9 += indices2.Length;
			}
			long num10 = 0L;
			long num11 = 0L;
			long num12 = 0L;
			long[] array6 = new long[(int)num3];
			long[] array7 = new long[(int)num3];
			long[] array8 = new long[(int)num3];
			List<string> list8 = new List<string>();
			list8.Add("MESH");
			list8.Add("SUBMESH");
			FileStream fileStream = Util.FileUtil.saveFile(savePath, null);
			string lmVersion = DataManager.LmVersion;
			Util.FileUtil.WriteData(fileStream, lmVersion);
			long position = fileStream.Position;
			long position2 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			long position3 = fileStream.Position;
			int num13 = (int)(num3 + 1);
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)num13
			});
			for (int i = 0; i < num13; i++)
			{
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
			}
			long position4 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new ushort[1]);
			long position5 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list8.IndexOf("MESH")
			});
			list8.Add(item);
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list8.IndexOf(item)
			});
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				num2
			});
			long position6 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			list8.Add(text);
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list8.IndexOf(text)
			});
			long position7 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			long position8 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list.Count
			});
			for (int i = 0; i < list.Count; i++)
			{
				list8.Add(list[i].name);
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)list8.IndexOf(list[i].name)
				});
			}
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			long num14 = fileStream.Position - position5;
			for (int i = 0; i < (int)num3; i++)
			{
				array6[i] = fileStream.Position;
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)list8.IndexOf("SUBMESH")
				});
				Util.FileUtil.WriteData(fileStream, new ushort[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)array2[i].Count
				});
				for (int j = 0; j < array2[i].Count; j++)
				{
					Util.FileUtil.WriteData(fileStream, new uint[1]);
					Util.FileUtil.WriteData(fileStream, new uint[1]);
					Util.FileUtil.WriteData(fileStream, new uint[1]);
					Util.FileUtil.WriteData(fileStream, new uint[1]);
				}
				array7[i] = fileStream.Position;
				array8[i] = array7[i] - array6[i];
			}
			long position9 = fileStream.Position;
			for (int i = 0; i < list8.Count; i++)
			{
				Util.FileUtil.WriteData(fileStream, list8[i]);
			}
			long num15 = fileStream.Position - position9;
			long position10 = fileStream.Position;
			for (int j = 0; j < list2.Count; j++)
			{
				DataManager.VertexData vertexData = list2[j];
				Vector3 vertice = vertexData.vertice;
				Util.FileUtil.WriteData(fileStream, new float[]
				{
					vertice.x * -1f,
					vertice.y,
					vertice.z
				});
				if (DataManager.VertexStructure[1] == 1)
				{
					Vector3 normal = vertexData.normal;
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						normal.x * -1f,
						normal.y,
						normal.z
					});
				}
				if (DataManager.VertexStructure[2] == 1)
				{
					Color color = vertexData.color;
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						color.r,
						color.g,
						color.b,
						color.a
					});
				}
				if (DataManager.VertexStructure[3] == 1)
				{
					Vector2 uv = vertexData.uv;
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						uv.x,
						uv.y * -1f + 1f
					});
				}
				if (DataManager.VertexStructure[4] == 1)
				{
					Vector2 uv2 = vertexData.uv2;
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						uv2.x,
						uv2.y * -1f
					});
				}
				if (DataManager.VertexStructure[5] == 1)
				{
					Vector4 boneWeight = vertexData.boneWeight;
					Vector4 boneIndex = vertexData.boneIndex;
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						boneWeight.x,
						boneWeight.y,
						boneWeight.z,
						boneWeight.w
					});
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						boneIndex.x,
						boneIndex.y,
						boneIndex.z,
						boneIndex.w
					});
				}
				if (DataManager.VertexStructure[6] == 1)
				{
					Vector4 tangent = vertexData.tangent;
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						tangent.x * -1f,
						tangent.y,
						tangent.z,
						tangent.w
					});
				}
			}
			long num16 = fileStream.Position - position10;
			long position11 = fileStream.Position;
			for (int j = 0; j < list5.Count; j++)
			{
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)list5[j]
				});
			}
			long num17 = fileStream.Position - position11;
			if (sharedMesh.bindposes != null && sharedMesh.bindposes.Length != 0)
			{
				Matrix4x4[] array9 = new Matrix4x4[sharedMesh.bindposes.Length];
				for (int i = 0; i < sharedMesh.bindposes.Length; i++)
				{
					array9[i] = sharedMesh.bindposes[i];
					array9[i] = array9[i].inverse;
					Vector3 vector;
					Quaternion quaternion;
					Vector3 vector2;
					MathUtil.Decompose(array9[i].transpose, out vector, out quaternion, out vector2);
					vector2.x *= -1f;
					quaternion.x *= -1f;
					quaternion.w *= -1f;
					array9[i] = Matrix4x4.TRS(vector2, quaternion, vector);
				}
				num10 = fileStream.Position;
				for (int i = 0; i < sharedMesh.bindposes.Length; i++)
				{
					Matrix4x4 matrix4x = array9[i];
					for (int j = 0; j < 16; j++)
					{
						Util.FileUtil.WriteData(fileStream, new float[]
						{
							matrix4x[j]
						});
					}
				}
				num11 = fileStream.Position;
				for (int i = 0; i < sharedMesh.bindposes.Length; i++)
				{
					Matrix4x4 inverse = array9[i].inverse;
					for (int j = 0; j < 16; j++)
					{
						Util.FileUtil.WriteData(fileStream, new float[]
						{
							inverse[j]
						});
					}
				}
				num12 = fileStream.Position;
				for (int i = 0; i < (int)num3; i++)
				{
					for (int j = 0; j < array2[i].Count; j++)
					{
						for (int k = 0; k < array2[i][j].Count; k++)
						{
							Util.FileUtil.WriteData(fileStream, new ushort[]
							{
								(ushort)array2[i][j][k]
							});
						}
					}
				}
				long position12 = fileStream.Position;
			}
			uint num18 = 0u;
			long num19 = num12 - position9;
			for (int i = 0; i < (int)num3; i++)
			{
				fileStream.Position = array6[i] + 4L;
				uint num20;
				uint num21;
				uint num22;
				uint num23;
				if (num3 == 1)
				{
					num20 = 0u;
					num21 = (uint)(num16 / (long)((ulong)num4));
					num22 = 0u;
					num23 = (uint)(num17 / 2L);
				}
				else if (i == 0)
				{
					num20 = 0u;
					num21 = (uint)array4[i + 1];
					num22 = num18;
					num23 = (uint)array5[i];
				}
				else if (i < (int)(num3 - 1))
				{
					num20 = (uint)array4[i];
					num21 = (uint)(array4[i + 1] - array4[i]);
					num22 = num18;
					num23 = (uint)array5[i];
				}
				else
				{
					num20 = (uint)array4[i];
					num21 = (uint)(num16 / (long)((ulong)num4) - (long)array4[i]);
					num22 = num18;
					num23 = (uint)array5[i];
				}
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num20
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num21
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num22
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num23
				});
				num18 += num23;
				fileStream.Position += 2L;
				int num24 = 0;
				for (int j = 0; j < array2[i].Count; j++)
				{
					Util.FileUtil.WriteData(fileStream, new uint[]
					{
						(uint)(num24 + (int)num22)
					});
					Util.FileUtil.WriteData(fileStream, new uint[]
					{
						(uint)(array3[i][j] - num24)
					});
					num24 = array3[i][j];
					Util.FileUtil.WriteData(fileStream, new uint[]
					{
						(uint)num19
					});
					Util.FileUtil.WriteData(fileStream, new uint[]
					{
						(uint)(array2[i][j].Count * 2)
					});
					num19 += (long)(array2[i][j].Count * 2);
				}
			}
			fileStream.Position = position6;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(position10 - position9)
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)num16
			});
			fileStream.Position = position7;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(position11 - position9)
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)num17
			});
			fileStream.Position = position8 + (long)((list.Count + 1) * 2);
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(num10 - position9)
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(num11 - num10)
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(num11 - position9)
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(num12 - num11)
			});
			fileStream.Position = position4;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list8.Count
			});
			long position13 = fileStream.Position;
			fileStream.Position = position3 + 2L;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)position5
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)num14
			});
			for (int i = 0; i < (int)num3; i++)
			{
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					(uint)array6[i]
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					(uint)array8[i]
				});
			}
			fileStream.Position = position2;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)position9
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(position9 + num15 + num16 + num17 + array8[0])
			});
			fileStream.Close();
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000F5F0 File Offset: 0x0000D7F0
		public static void saveTerrainLmFile(GameObject gameObject, JSONObject obj, int gameObjectType)
		{
			TerrainData terrainData = gameObject.GetComponent<Terrain>().terrainData;
			int num = terrainData.heightmapWidth;
			int num2 = terrainData.heightmapHeight;
			Vector3 size = terrainData.size;
			int terrainToMeshResolution = DataManager.TerrainToMeshResolution;
			Vector3 vector;
			vector = new Vector3(size.x / (float)(num - 1) * (float)terrainToMeshResolution, size.y, size.z / (float)(num2 - 1) * (float)terrainToMeshResolution);
			Vector2 vector2;
			vector2 = new Vector2(1f / (float)(num - 1), 1f / (float)(num2 - 1));
			float[,] heights = terrainData.GetHeights(0, 0, num, num2);
			num = (num - 1) / terrainToMeshResolution + 1;
			num2 = (num2 - 1) / terrainToMeshResolution + 1;
			Vector3[] array = new Vector3[num * num2];
			Vector3[] array2 = new Vector3[num * num2];
			Vector2[] array3 = new Vector2[num * num2];
			int[] array4 = new int[(num - 1) * (num2 - 1) * 6];
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					array[i * num + j] = Vector3.Scale(new Vector3((float)i, heights[j * terrainToMeshResolution, i * terrainToMeshResolution], (float)j), vector);
					array3[i * num + j] = Vector2.Scale(new Vector2((float)(j * terrainToMeshResolution), 1f - (float)(i * terrainToMeshResolution)), vector2) - new Vector2(0f, 1f / (float)(terrainData.heightmapHeight - 1));
					float x = array3[i * num + j].x;
					float y = array3[i * num + j].y;
					array3[i * num + j] = new Vector2(x * Mathf.Cos(1.57079637f) - y * Mathf.Sin(1.57079637f), x * Mathf.Sin(1.57079637f) + y * Mathf.Cos(1.57079637f));
				}
			}
			int num3 = 0;
			for (int k = 0; k < num2 - 1; k++)
			{
				for (int l = 0; l < num - 1; l++)
				{
					array4[num3++] = k * num + l;
					array4[num3++] = k * num + l + 1;
					array4[num3++] = (k + 1) * num + l;
					array4[num3++] = (k + 1) * num + l;
					array4[num3++] = k * num + l + 1;
					array4[num3++] = (k + 1) * num + l + 1;
				}
			}
			for (int m = 0; m < array.Length; m++)
			{
				List<Vector3> list = new List<Vector3>();
				for (int n = 0; n < array4.Length; n += 3)
				{
					if (array4[n] == m || array4[n + 1] == m || array4[n + 2] == m)
					{
						list.Add(array[array4[n]]);
						list.Add(array[array4[n + 1]]);
						list.Add(array[array4[n + 2]]);
					}
				}
				float num4 = 0f;
				List<float> list2 = new List<float>();
				List<Vector3> list3 = new List<Vector3>();
				for (int num5 = 0; num5 < list.Count; num5 += 3)
				{
					Vector3 vector3 = list[num5];
					Vector3 vector4 = list[num5 + 1];
					Vector3 vector5 = list[num5 + 2];
					float num6 = Mathf.Sqrt(Mathf.Pow(vector3.x - vector4.x, 2f) + Mathf.Pow(vector3.y - vector4.y, 2f) + Mathf.Pow(vector3.z - vector4.z, 2f));
					float num7 = Mathf.Sqrt(Mathf.Pow(vector3.x - vector5.x, 2f) + Mathf.Pow(vector3.y - vector5.y, 2f) + Mathf.Pow(vector3.z - vector5.z, 2f));
					float num8 = Mathf.Sqrt(Mathf.Pow(vector5.x - vector4.x, 2f) + Mathf.Pow(vector5.y - vector4.y, 2f) + Mathf.Pow(vector5.z - vector4.z, 2f));
					float num9 = (num6 + num7 + num8) / 2f;
					float num10 = Mathf.Sqrt(num9 * (num9 - num6) * (num9 - num7) * (num9 - num8));
					list2.Add(num10);
					num4 += num10;
					list3.Add(Vector3.Cross(vector3 - vector4, vector3 - vector5).normalized);
				}
				Vector3 vector6 = default(Vector3);
				for (int num11 = 0; num11 < list3.Count; num11++)
				{
					vector6 += list3[num11] * list2[num11] / num4;
				}
				array2[m] = vector6.normalized;
			}
			int num12 = 65534;
			List<List<DataManager.TerrainVertexData>> list4 = new List<List<DataManager.TerrainVertexData>>();
			List<DataManager.TerrainVertexData> list5 = new List<DataManager.TerrainVertexData>();
			list4.Add(list5);
			List<List<int>> list6 = new List<List<int>>();
			List<int> list7 = new List<int>();
			list6.Add(list7);
			List<int> list8 = new List<int>();
			for (int num13 = 0; num13 < array4.Length; num13++)
			{
				if (list5.Count == num12)
				{
					list5 = new List<DataManager.TerrainVertexData>();
					list4.Add(list5);
					list7 = new List<int>();
					list6.Add(list7);
					list8 = new List<int>();
				}
				int num14 = array4[num13];
				DataManager.TerrainVertexData item;
				item.vertice = array[num14];
				item.normal = array2[num14];
				item.uv = array3[num14];
				int num15 = list8.IndexOf(num14);
				if (num15 == -1)
				{
					list5.Add(item);
					list7.Add(list5.Count - 1);
					list8.Add(num14);
				}
				else
				{
					list7.Add(num15);
				}
			}
			int count = list4.Count;
			string text = DataManager.cleanIllegalChar("terrain_" + gameObject.name, true);
			string text2 = "terrain/" + text + ".lm";
			obj.AddField("meshPath", text2);
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			obj.AddField("materials", jsonobject);
			string str = DataManager.cleanIllegalChar("terrain_" + gameObject.name, true);
			string val = "terrain/" + str + ".lmat";
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject2.AddField("type", "Laya.ExtendTerrainMaterial");
			jsonobject2.AddField("path", val);
			for (int num16 = 0; num16 < count; num16++)
			{
				jsonobject.Add(jsonobject2);
			}
			string fileName = DataManager.SAVEPATH + "/" + text2;
			int num17 = 1 + count;
			ushort num18 = 32;
			string item2 = "POSITION,NORMAL,UV";
			long[] array5 = new long[count];
			long[] array6 = new long[count];
			long[] array7 = new long[count];
			long[] array8 = new long[count];
			long[] array9 = new long[count];
			List<string> list9 = new List<string>();
			list9.Add("MESH");
			list9.Add("SUBMESH");
			FileStream fileStream = Util.FileUtil.saveFile(fileName, null);
			string data = "LAYAMODEL:0301";
			Util.FileUtil.WriteData(fileStream, data);
			long position = fileStream.Position;
			long position2 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			long position3 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)num17
			});
			for (int num19 = 0; num19 < num17; num19++)
			{
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
			}
			long position4 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new ushort[1]);
			long position5 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list9.IndexOf("MESH")
			});
			list9.Add(text);
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list9.IndexOf(text)
			});
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list4.Count
			});
			list9.Add(item2);
			for (int num19 = 0; num19 < list4.Count; num19++)
			{
				array5[num19] = fileStream.Position;
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)list9.IndexOf(item2)
				});
			}
			long position6 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			long position7 = fileStream.Position;
			Util.FileUtil.WriteData(fileStream, new ushort[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			long num20 = fileStream.Position - position5;
			for (int num19 = 0; num19 < count; num19++)
			{
				array7[num19] = fileStream.Position;
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)list9.IndexOf("SUBMESH")
				});
				Util.FileUtil.WriteData(fileStream, new ushort[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					1
				});
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				Util.FileUtil.WriteData(fileStream, new uint[1]);
				array8[num19] = fileStream.Position;
				array9[num19] = array8[num19] - array7[num19];
			}
			long position8 = fileStream.Position;
			for (int num19 = 0; num19 < list9.Count; num19++)
			{
				Util.FileUtil.WriteData(fileStream, list9[num19]);
			}
			long num21 = fileStream.Position - position8;
			long position9 = fileStream.Position;
			for (int num19 = 0; num19 < list4.Count; num19++)
			{
				array6[num19] = fileStream.Position;
				List<DataManager.TerrainVertexData> list10 = list4[num19];
				for (int num22 = 0; num22 < list10.Count; num22++)
				{
					DataManager.TerrainVertexData terrainVertexData = list10[num22];
					Vector3 vertice = terrainVertexData.vertice;
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						vertice.x * -1f,
						vertice.y,
						vertice.z
					});
					Vector3 normal = terrainVertexData.normal;
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						normal.x * -1f,
						normal.y,
						normal.z
					});
					Vector2 uv = terrainVertexData.uv;
					Util.FileUtil.WriteData(fileStream, new float[]
					{
						uv.x,
						uv.y * -1f + 1f
					});
				}
			}
			long num23 = fileStream.Position - position9;
			long position10 = fileStream.Position;
			for (int num19 = 0; num19 < list6.Count; num19++)
			{
				List<int> list11 = list6[num19];
				for (int num22 = 0; num22 < list11.Count; num22++)
				{
					Util.FileUtil.WriteData(fileStream, new ushort[]
					{
						(ushort)list11[num22]
					});
				}
			}
			long num24 = fileStream.Position - position10;
			uint num25 = 0u;
			uint num26 = 0u;
			for (int num19 = 0; num19 < count; num19++)
			{
				fileStream.Position = array7[num19] + 2L;
				Util.FileUtil.WriteData(fileStream, new ushort[]
				{
					(ushort)num19
				});
				uint num27 = num25;
				uint count2 = (uint)list4[num19].Count;
				uint num28 = num26;
				uint count3 = (uint)list6[num19].Count;
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num27
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					count2
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num28
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					count3
				});
				num25 += count2;
				num26 += count3;
				fileStream.Position += 2L;
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					num28
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					count3
				});
			}
			for (int num19 = 0; num19 < list4.Count; num19++)
			{
				fileStream.Position = array5[num19];
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					(uint)(array6[num19] - position8)
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					(uint)(list4[num19].Count * (int)num18)
				});
			}
			fileStream.Position = position6;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(position10 - position8)
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)num24
			});
			fileStream.Position = position4;
			Util.FileUtil.WriteData(fileStream, new uint[1]);
			Util.FileUtil.WriteData(fileStream, new ushort[]
			{
				(ushort)list9.Count
			});
			long position11 = fileStream.Position;
			fileStream.Position = position3 + 2L;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)position5
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)num20
			});
			for (int num19 = 0; num19 < count; num19++)
			{
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					(uint)array7[num19]
				});
				Util.FileUtil.WriteData(fileStream, new uint[]
				{
					(uint)array9[num19]
				});
			}
			fileStream.Position = position2;
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)position8
			});
			Util.FileUtil.WriteData(fileStream, new uint[]
			{
				(uint)(position8 + num21 + num23 + num24 + array9[0])
			});
			fileStream.Close();
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00010458 File Offset: 0x0000E658
		public static void saveTerrainLmatFile(GameObject gameObject, JSONObject obj)
		{
			TerrainData terrainData = gameObject.GetComponent<Terrain>().terrainData;
			string text = DataManager.cleanIllegalChar("terrain_" + gameObject.name, true);
			string str = "terrain/" + text + ".lmat";
			string text2 = DataManager.SAVEPATH + "/" + str;
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject obj2 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject.AddField("version", DataManager.LmatVersion);
			jsonobject.AddField("props", jsonobject2);
			jsonobject2.AddField("type", "Laya.ExtendTerrainMaterial");
			jsonobject2.AddField("name", text);
			jsonobject2.AddField("renderStates", jsonobject3);
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject3.Add(jsonobject6);
			jsonobject6.AddField("cull", 2);
			jsonobject6.AddField("blend", 0);
			jsonobject6.AddField("srcBlend", 1);
			jsonobject6.AddField("dstBlend", 0);
			jsonobject6.AddField("depthWrite", true);
			jsonobject6.AddField("depthTest", 515);
			jsonobject2.AddField("alphaTest", false);
			jsonobject2.AddField("renderQueue", 1);
			jsonobject2.AddField("textures", jsonobject4);
			jsonobject2.AddField("vectors", jsonobject5);
			jsonobject2.AddField("defines", obj2);
			if (terrainData.alphamapTextures.Length > 0)
			{
				for (int i = 0; i < 1; i++)
				{
					JSONObject jsonobject7 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject7.AddField("name", "splatAlphaTexture");
					Color[] pixels = terrainData.alphamapTextures[i].GetPixels();
					int num = pixels.Length;
					int num2 = (int)Mathf.Sqrt((float)num);
					Texture2D texture2D = new Texture2D(num2, num2);
					Color[] array = new Color[num];
					for (int j = 0; j < num; j++)
					{
						array[j] = pixels[j];
						if (array[j].a == 0f)
						{
							array[j].a = 0.03125f;
						}
					}
					texture2D.SetPixels(array);
					texture2D.Apply();
					FileStream fileStream = File.Open(DataManager.SAVEPATH + "/terrain/splatAlphaTexture.png", FileMode.Create);
					new BinaryWriter(fileStream).Write(texture2D.EncodeToPNG());
					fileStream.Close();
					jsonobject7.AddField("path", "splatAlphaTexture.png");
					jsonobject4.Add(jsonobject7);
				}
			}
			int num3 = terrainData.splatPrototypes.Length;
			for (int k = 0; k < num3; k++)
			{
				SplatPrototype splatPrototype = terrainData.splatPrototypes[k];
				JSONObject jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject8.AddField("name", "diffuseTexture" + (k + 1));
				DataManager.saveTextureFile(jsonobject8, splatPrototype.texture, DataManager.Platformindex, DataManager.cleanIllegalChar(text2, false), null, "path");
				jsonobject4.Add(jsonobject8);
				JSONObject jsonobject9 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject9.AddField("name", "diffuseScaleOffset" + (k + 1));
				JSONObject jsonobject10 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject10.Add(terrainData.size.x / splatPrototype.tileSize.x);
				jsonobject10.Add(terrainData.size.z / splatPrototype.tileSize.y);
				jsonobject10.Add(splatPrototype.tileOffset.x);
				jsonobject10.Add(splatPrototype.tileOffset.y);
				jsonobject9.AddField("value", jsonobject10);
				jsonobject5.Add(jsonobject9);
			}
			JSONObject jsonobject11 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject11.AddField("name", "albedo");
			JSONObject jsonobject12 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject12.Add(1f);
			jsonobject12.Add(1f);
			jsonobject12.Add(1f);
			jsonobject12.Add(1f);
			jsonobject11.AddField("value", jsonobject12);
			jsonobject5.Add(jsonobject11);
			JSONObject jsonobject13 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject13.AddField("name", "ambientColor");
			JSONObject jsonobject14 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject14.Add(0f);
			jsonobject14.Add(0f);
			jsonobject14.Add(0f);
			jsonobject13.AddField("value", jsonobject14);
			jsonobject5.Add(jsonobject13);
			JSONObject jsonobject15 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject15.AddField("name", "diffuseColor");
			JSONObject jsonobject16 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject16.Add(1f);
			jsonobject16.Add(1f);
			jsonobject16.Add(1f);
			jsonobject15.AddField("value", jsonobject16);
			jsonobject5.Add(jsonobject15);
			JSONObject jsonobject17 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject17.AddField("name", "specularColor");
			JSONObject jsonobject18 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject18.Add(1f);
			jsonobject18.Add(1f);
			jsonobject18.Add(1f);
			jsonobject18.Add(8f);
			jsonobject17.AddField("value", jsonobject18);
			jsonobject5.Add(jsonobject17);
			Util.FileUtil.saveFile(text2, jsonobject);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00010994 File Offset: 0x0000EB94
		public static void saveLayaLmatFile(Material material, string savePath, string shaderType)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject.AddField("version", DataManager.LmatVersion);
			jsonobject.AddField("props", jsonobject2);
			if (shaderType == "BlinnPhong")
			{
				jsonobject2.AddField("type", "Laya.BlinnPhongMaterial");
			}
			else if (shaderType == "Unlit")
			{
				jsonobject2.AddField("type", "Laya.UnlitMaterial");
			}
			else if (shaderType == "Effect")
			{
				jsonobject2.AddField("type", "Laya.EffectMaterial");
			}
			else if (shaderType == "PBR(Standard)")
			{
				jsonobject2.AddField("type", "Laya.PBRStandardMaterial");
			}
			else if (shaderType == "PBR(Specular)")
			{
				jsonobject2.AddField("type", "Laya.PBRSpecularMaterial");
			}
			else
			{
				jsonobject2.AddField("type", "Laya.BlinnPhongMaterial");
			}
			List<string> list = material.shaderKeywords.ToList<string>();
			string name = material.name;
			jsonobject2.AddField("name", name);
			jsonobject2.AddField("renderStates", jsonobject3);
			JSONObject jsonobject7 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject3.Add(jsonobject7);
			if (material.HasProperty("_Cull"))
			{
				jsonobject7.AddField("cull", material.GetInt("_Cull"));
			}
			else
			{
				jsonobject7.AddField("cull", 2);
			}
			if (list.IndexOf("_ALPHABLEND_ON") != -1)
			{
				jsonobject7.AddField("blend", 1);
			}
			else
			{
				jsonobject7.AddField("blend", 0);
			}
			if (material.HasProperty("_SrcBlend"))
			{
				switch (material.GetInt("_SrcBlend"))
				{
				case 0:
					jsonobject7.AddField("srcBlend", 0);
					break;
				case 1:
					jsonobject7.AddField("srcBlend", 1);
					break;
				case 2:
					jsonobject7.AddField("srcBlend", 774);
					break;
				case 3:
					jsonobject7.AddField("srcBlend", 768);
					break;
				case 4:
					jsonobject7.AddField("srcBlend", 775);
					break;
				case 5:
					jsonobject7.AddField("srcBlend", 770);
					break;
				case 6:
					jsonobject7.AddField("srcBlend", 769);
					break;
				case 7:
					jsonobject7.AddField("srcBlend", 772);
					break;
				case 8:
					jsonobject7.AddField("srcBlend", 773);
					break;
				case 9:
					jsonobject7.AddField("srcBlend", 776);
					break;
				case 10:
					jsonobject7.AddField("srcBlend", 771);
					break;
				default:
					jsonobject7.AddField("srcBlend", 1);
					break;
				}
			}
			else
			{
				jsonobject7.AddField("srcBlend", 1);
			}
			if (material.HasProperty("_DstBlend"))
			{
				switch (material.GetInt("_DstBlend"))
				{
				case 0:
					jsonobject7.AddField("dstBlend", 0);
					break;
				case 1:
					jsonobject7.AddField("dstBlend", 1);
					break;
				case 2:
					jsonobject7.AddField("dstBlend", 774);
					break;
				case 3:
					jsonobject7.AddField("dstBlend", 768);
					break;
				case 4:
					jsonobject7.AddField("dstBlend", 775);
					break;
				case 5:
					jsonobject7.AddField("dstBlend", 770);
					break;
				case 6:
					jsonobject7.AddField("dstBlend", 769);
					break;
				case 7:
					jsonobject7.AddField("dstBlend", 772);
					break;
				case 8:
					jsonobject7.AddField("dstBlend", 773);
					break;
				case 9:
					jsonobject7.AddField("dstBlend", 776);
					break;
				case 10:
					jsonobject7.AddField("dstBlend", 771);
					break;
				default:
					jsonobject7.AddField("dstBlend", 0);
					break;
				}
			}
			else
			{
				jsonobject7.AddField("dstBlend", 0);
			}
			if (material.HasProperty("_ZWrite"))
			{
				if (material.GetInt("_ZWrite") == 1)
				{
					jsonobject7.AddField("depthWrite", true);
				}
				else
				{
					jsonobject7.AddField("depthWrite", false);
				}
			}
			else
			{
				jsonobject7.AddField("depthWrite", true);
			}
			if (material.HasProperty("_ZTest"))
			{
				switch (material.GetInt("_ZTest"))
				{
				case 0:
					jsonobject7.AddField("depthTest", 0);
					break;
				case 1:
					jsonobject7.AddField("depthTest", 512);
					break;
				case 2:
					jsonobject7.AddField("depthTest", 513);
					break;
				case 3:
					jsonobject7.AddField("depthTest", 514);
					break;
				case 4:
					jsonobject7.AddField("depthTest", 515);
					break;
				case 5:
					jsonobject7.AddField("depthTest", 516);
					break;
				case 6:
					jsonobject7.AddField("depthTest", 517);
					break;
				case 7:
					jsonobject7.AddField("depthTest", 518);
					break;
				case 8:
					jsonobject7.AddField("depthTest", 519);
					break;
				default:
					jsonobject7.AddField("depthTest", 0);
					break;
				}
			}
			else
			{
				jsonobject7.AddField("depthTest", 515);
			}
			if (material.HasProperty("_IsVertexColor"))
			{
				jsonobject2.AddField("enableVertexColor", material.GetInt("_IsVertexColor") != 0);
			}
			if (list.IndexOf("_ALPHATEST_ON") != -1)
			{
				jsonobject2.AddField("alphaTest", true);
			}
			else
			{
				jsonobject2.AddField("alphaTest", false);
			}
			if (material.HasProperty("_Cutoff"))
			{
				jsonobject2.AddField("alphaTestValue", material.GetFloat("_Cutoff"));
			}
			else
			{
				jsonobject2.AddField("alphaTestValue", 0.5f);
			}
			jsonobject2.AddField("renderQueue", material.renderQueue);
			if (material.HasProperty("_AlbedoIntensity"))
			{
				jsonobject2.AddField("albedoIntensity", material.GetFloat("_AlbedoIntensity"));
			}
			if (material.HasProperty("_Metallic"))
			{
				jsonobject2.AddField("metallic", material.GetFloat("_Metallic"));
			}
			if (material.HasProperty("_Glossiness"))
			{
				jsonobject2.AddField("smoothness", material.GetFloat("_Glossiness"));
			}
			if (material.HasProperty("_GlossMapScale"))
			{
				jsonobject2.AddField("smoothnessTextureScale", material.GetFloat("_GlossMapScale"));
			}
			if (material.HasProperty("_SmoothnessTextureChannel"))
			{
				jsonobject2.AddField("smoothnessSource", material.GetFloat("_SmoothnessTextureChannel"));
			}
			if (material.HasProperty("_BumpScale"))
			{
				jsonobject2.AddField("normalTextureScale", material.GetFloat("_BumpScale"));
			}
			if (material.HasProperty("_Parallax"))
			{
				jsonobject2.AddField("parallaxTextureScale", material.GetFloat("_Parallax"));
			}
			if (material.HasProperty("_OcclusionStrength"))
			{
				jsonobject2.AddField("occlusionTextureStrength", material.GetFloat("_OcclusionStrength"));
			}
			if (material.HasProperty("_Reflection"))
			{
				if ((double)material.GetFloat("_Reflection") == 1.0)
				{
					jsonobject2.AddField("enableReflection", true);
				}
				else
				{
					jsonobject2.AddField("enableReflection", false);
				}
			}
			if (material.HasProperty("_Emission"))
			{
				if ((double)material.GetFloat("_Emission") == 1.0)
				{
					jsonobject2.AddField("enableEmission", true);
				}
				else
				{
					jsonobject2.AddField("enableEmission", false);
				}
			}
			if (material.HasProperty("_MainTex"))
			{
				Texture2D texture2D = (Texture2D)material.GetTexture("_MainTex");
				if (texture2D != null)
				{
					JSONObject jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject8.AddField("name", "albedoTexture");
					DataManager.saveTextureFile(jsonobject8, texture2D, DataManager.Platformindex, savePath, name, "path");
					jsonobject4.Add(jsonobject8);
				}
				JSONObject jsonobject9 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject9.AddField("name", "tilingOffset");
				JSONObject jsonobject10 = new JSONObject(JSONObject.Type.ARRAY);
				Vector2 textureScale = material.GetTextureScale("_MainTex");
				Vector2 textureOffset = material.GetTextureOffset("_MainTex");
				jsonobject10.Add(textureScale.x);
				jsonobject10.Add(textureScale.y);
				jsonobject10.Add(textureOffset.x);
				jsonobject10.Add(textureOffset.y);
				jsonobject9.AddField("value", jsonobject10);
				jsonobject5.Add(jsonobject9);
			}
			if (material.HasProperty("_MetallicGlossMap"))
			{
				Texture2D texture2D2 = (Texture2D)material.GetTexture("_MetallicGlossMap");
				if (texture2D2 != null)
				{
					JSONObject jsonobject11 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject11.AddField("name", "metallicGlossTexture");
					DataManager.saveTextureFile(jsonobject11, texture2D2, DataManager.Platformindex, savePath, name, "path");
					jsonobject4.Add(jsonobject11);
				}
			}
			if (material.HasProperty("_Lighting"))
			{
				if ((double)material.GetFloat("_Lighting") == 0.0)
				{
					jsonobject2.AddField("enableLighting", true);
				}
				else
				{
					jsonobject2.AddField("enableLighting", false);
				}
			}
			if (!material.HasProperty("_Lighting") || (material.HasProperty("_Lighting") && (double)material.GetFloat("_Lighting") == 0.0))
			{
				if (material.HasProperty("_Shininess"))
				{
					jsonobject2.AddField("shininess", material.GetFloat("_Shininess"));
				}
				if (material.HasProperty("_SpecGlossMap"))
				{
					Texture2D texture2D3 = (Texture2D)material.GetTexture("_SpecGlossMap");
					if (texture2D3 != null)
					{
						JSONObject jsonobject12 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject12.AddField("name", "specularTexture");
						DataManager.saveTextureFile(jsonobject12, texture2D3, DataManager.Platformindex, savePath, name, "path");
						jsonobject4.Add(jsonobject12);
					}
				}
				if (material.HasProperty("_BumpMap"))
				{
					Texture2D texture2D4 = (Texture2D)material.GetTexture("_BumpMap");
					if (texture2D4 != null)
					{
						JSONObject jsonobject13 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject13.AddField("name", "normalTexture");
						DataManager.saveTextureFile(jsonobject13, texture2D4, DataManager.Platformindex, savePath, name, "path");
						jsonobject4.Add(jsonobject13);
					}
				}
				JSONObject jsonobject14 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject14.AddField("name", "specularColor");
				JSONObject jsonobject15 = new JSONObject(JSONObject.Type.ARRAY);
				if (material.HasProperty("_SpecColor"))
				{
					Color color = material.GetColor("_SpecColor");
					jsonobject15.Add(color.r);
					jsonobject15.Add(color.g);
					jsonobject15.Add(color.b);
					jsonobject14.AddField("value", jsonobject15);
					jsonobject5.Add(jsonobject14);
				}
			}
			if (material.HasProperty("_ParallaxMap"))
			{
				Texture2D texture2D5 = (Texture2D)material.GetTexture("_ParallaxMap");
				if (texture2D5 != null)
				{
					JSONObject jsonobject16 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject16.AddField("name", "parallaxTexture");
					DataManager.saveTextureFile(jsonobject16, texture2D5, DataManager.Platformindex, savePath, name, "path");
					jsonobject4.Add(jsonobject16);
				}
			}
			if (material.HasProperty("_OcclusionMap"))
			{
				Texture2D texture2D6 = (Texture2D)material.GetTexture("_OcclusionMap");
				if (texture2D6 != null)
				{
					JSONObject jsonobject17 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject17.AddField("name", "occlusionTexture");
					DataManager.saveTextureFile(jsonobject17, texture2D6, DataManager.Platformindex, savePath, name, "path");
					jsonobject4.Add(jsonobject17);
				}
			}
			if (material.HasProperty("_EmissionMap"))
			{
				Texture2D texture2D7 = (Texture2D)material.GetTexture("_EmissionMap");
				if (texture2D7 != null)
				{
					JSONObject jsonobject18 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject18.AddField("name", "emissionTexture");
					DataManager.saveTextureFile(jsonobject18, texture2D7, DataManager.Platformindex, savePath, name, "path");
					jsonobject4.Add(jsonobject18);
				}
			}
			JSONObject jsonobject19 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject19.AddField("name", "albedoColor");
			JSONObject jsonobject20 = new JSONObject(JSONObject.Type.ARRAY);
			if (material.HasProperty("_Color"))
			{
				Color color2 = material.GetColor("_Color");
				jsonobject20.Add(color2.r);
				jsonobject20.Add(color2.g);
				jsonobject20.Add(color2.b);
				jsonobject20.Add(color2.a);
				jsonobject19.AddField("value", jsonobject20);
				jsonobject5.Add(jsonobject19);
			}
			JSONObject jsonobject21 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject21.AddField("name", "emissionColor");
			JSONObject jsonobject22 = new JSONObject(JSONObject.Type.ARRAY);
			if (material.HasProperty("_EmissionColor"))
			{
				Color color3 = material.GetColor("_EmissionColor");
				jsonobject22.Add(color3.r);
				jsonobject22.Add(color3.g);
				jsonobject22.Add(color3.b);
				jsonobject22.Add(color3.a);
				jsonobject21.AddField("value", jsonobject22);
				jsonobject5.Add(jsonobject21);
			}
			if ((shaderType == "PBR(Standard)" || shaderType == "PBR(Specular)") && material.HasProperty("_Mode") && material.GetInt("_Mode") == 3)
			{
				jsonobject6.Add("ALPHAPREMULTIPLY");
			}
			if (shaderType == "Unlit")
			{
				if (material.HasProperty("_Mode") && material.GetInt("_Mode") == 3)
				{
					jsonobject6.Add("ADDTIVEFOG");
				}
				if (material.HasProperty("_EnableVertexColor") && material.GetInt("_EnableVertexColor") == 1)
				{
					jsonobject6.Add("ENABLEVERTEXCOLOR");
				}
			}
			jsonobject2.AddField("textures", jsonobject4);
			jsonobject2.AddField("vectors", jsonobject5);
			jsonobject2.AddField("defines", jsonobject6);
			Util.FileUtil.saveFile(savePath, jsonobject);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00011774 File Offset: 0x0000F974
		public static void saveLayaEffectLmatFile(Material material, string savePath, string shaderType)
		{
			string name = material.shader.name;
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject.AddField("version", DataManager.LmatVersion);
			jsonobject.AddField("props", jsonobject2);
			if (shaderType == "Trail")
			{
				jsonobject2.AddField("type", "Laya.TrailMaterial");
			}
			else if (shaderType == "Unlit")
			{
				jsonobject2.AddField("type", "Laya.LineMaterial");
			}
			else if (shaderType == "Effect")
			{
				jsonobject2.AddField("type", "Laya.EffectMaterial");
			}
			else if (shaderType == "ShurikenParticle")
			{
				jsonobject2.AddField("type", "Laya.ShurikenParticleMaterial");
			}
			else
			{
				jsonobject2.AddField("type", "Laya.EffectMaterial");
			}
			jsonobject2.AddField("renderStates", jsonobject3);
			JSONObject jsonobject7 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject3.Add(jsonobject7);
			List<string> list = material.shaderKeywords.ToList<string>();
			string name2 = material.name;
			jsonobject2.AddField("name", name2);
			jsonobject7.AddField("cull", 0);
			if (list.IndexOf("_ALPHABLEND_ON") != -1)
			{
				jsonobject7.AddField("blend", 1);
			}
			else
			{
				jsonobject7.AddField("blend", 0);
			}
			if (material.HasProperty("_SrcBlend"))
			{
				switch (material.GetInt("_SrcBlend"))
				{
				case 0:
					jsonobject7.AddField("srcBlend", 0);
					break;
				case 1:
					jsonobject7.AddField("srcBlend", 1);
					break;
				case 2:
					jsonobject7.AddField("srcBlend", 774);
					break;
				case 3:
					jsonobject7.AddField("srcBlend", 768);
					break;
				case 4:
					jsonobject7.AddField("srcBlend", 775);
					break;
				case 5:
					jsonobject7.AddField("srcBlend", 770);
					break;
				case 6:
					jsonobject7.AddField("srcBlend", 769);
					break;
				case 7:
					jsonobject7.AddField("srcBlend", 772);
					break;
				case 8:
					jsonobject7.AddField("srcBlend", 773);
					break;
				case 9:
					jsonobject7.AddField("srcBlend", 776);
					break;
				case 10:
					jsonobject7.AddField("srcBlend", 771);
					break;
				default:
					jsonobject7.AddField("srcBlend", 1);
					break;
				}
			}
			else
			{
				jsonobject7.AddField("srcBlend", 1);
			}
			if (material.HasProperty("_DstBlend"))
			{
				switch (material.GetInt("_DstBlend"))
				{
				case 0:
					jsonobject7.AddField("dstBlend", 0);
					break;
				case 1:
					jsonobject7.AddField("dstBlend", 1);
					break;
				case 2:
					jsonobject7.AddField("dstBlend", 774);
					break;
				case 3:
					jsonobject7.AddField("dstBlend", 768);
					break;
				case 4:
					jsonobject7.AddField("dstBlend", 775);
					break;
				case 5:
					jsonobject7.AddField("dstBlend", 770);
					break;
				case 6:
					jsonobject7.AddField("dstBlend", 769);
					break;
				case 7:
					jsonobject7.AddField("dstBlend", 772);
					break;
				case 8:
					jsonobject7.AddField("dstBlend", 773);
					break;
				case 9:
					jsonobject7.AddField("dstBlend", 776);
					break;
				case 10:
					jsonobject7.AddField("dstBlend", 771);
					break;
				default:
					jsonobject7.AddField("dstBlend", 0);
					break;
				}
			}
			else
			{
				jsonobject7.AddField("dstBlend", 0);
			}
			if (material.HasProperty("_ZWrite"))
			{
				if (material.GetInt("_ZWrite") == 1)
				{
					jsonobject7.AddField("depthWrite", true);
				}
				else
				{
					jsonobject7.AddField("depthWrite", false);
				}
			}
			else
			{
				jsonobject7.AddField("depthWrite", true);
			}
			if (material.HasProperty("_ZTest"))
			{
				switch (material.GetInt("_ZTest"))
				{
				case 0:
					jsonobject7.AddField("depthTest", 0);
					break;
				case 1:
					jsonobject7.AddField("depthTest", 512);
					break;
				case 2:
					jsonobject7.AddField("depthTest", 513);
					break;
				case 3:
					jsonobject7.AddField("depthTest", 514);
					break;
				case 4:
					jsonobject7.AddField("depthTest", 515);
					break;
				case 5:
					jsonobject7.AddField("depthTest", 516);
					break;
				case 6:
					jsonobject7.AddField("depthTest", 517);
					break;
				case 7:
					jsonobject7.AddField("depthTest", 518);
					break;
				case 8:
					jsonobject7.AddField("depthTest", 519);
					break;
				default:
					jsonobject7.AddField("depthTest", 0);
					break;
				}
			}
			else
			{
				jsonobject7.AddField("depthTest", 515);
			}
			if (list.IndexOf("_ALPHATEST_ON") != -1)
			{
				jsonobject2.AddField("alphaTest", true);
			}
			else
			{
				jsonobject2.AddField("alphaTest", false);
			}
			jsonobject2.AddField("renderQueue", 3000);
			if (material.HasProperty("_MainTex"))
			{
				Texture2D texture2D = (Texture2D)material.GetTexture("_MainTex");
				if (texture2D != null)
				{
					JSONObject jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject8.AddField("name", "texture");
					DataManager.saveTextureFile(jsonobject8, texture2D, DataManager.Platformindex, savePath, name2, "path");
					jsonobject4.Add(jsonobject8);
				}
				JSONObject jsonobject9 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject9.AddField("name", "tilingOffset");
				JSONObject jsonobject10 = new JSONObject(JSONObject.Type.ARRAY);
				Vector2 textureScale = material.GetTextureScale("_MainTex");
				Vector2 textureOffset = material.GetTextureOffset("_MainTex");
				jsonobject10.Add(textureScale.x);
				jsonobject10.Add(textureScale.y);
				jsonobject10.Add(textureOffset.x);
				jsonobject10.Add(textureOffset.y * -1f);
				jsonobject9.AddField("value", jsonobject10);
				jsonobject5.Add(jsonobject9);
			}
			JSONObject jsonobject11 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject11.AddField("name", "color");
			JSONObject jsonobject12 = new JSONObject(JSONObject.Type.ARRAY);
			if (material.HasProperty("_TintColor"))
			{
				Color color = material.GetColor("_TintColor");
				jsonobject12.Add(color.r);
				jsonobject12.Add(color.g);
				jsonobject12.Add(color.b);
				jsonobject12.Add(color.a);
				jsonobject11.AddField("value", jsonobject12);
				jsonobject5.Add(jsonobject11);
			}
			if (material.HasProperty("_Mode") && material.GetInt("_Mode") == 0)
			{
				jsonobject6.Add("ADDTIVEFOG");
			}
			jsonobject2.AddField("textures", jsonobject4);
			jsonobject2.AddField("vectors", jsonobject5);
			jsonobject2.AddField("defines", jsonobject6);
			Util.FileUtil.saveFile(savePath, jsonobject);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00011ED8 File Offset: 0x000100D8
		public static void saveLayaSkyBoxLmatFile(Material material, string savePath)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject.AddField("version", DataManager.LmatVersion);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject.AddField("props", jsonobject2);
			jsonobject2.AddField("type", "Laya.SkyBoxMaterial");
			string name = material.name;
			jsonobject2.AddField("name", name);
			if (material.HasProperty("_Exposure"))
			{
				jsonobject2.AddField("exposure", material.GetFloat("_Exposure"));
			}
			if (material.HasProperty("_Rotation"))
			{
				jsonobject2.AddField("rotation", material.GetFloat("_Rotation"));
			}
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.AddField("vectors", jsonobject3);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject4.AddField("name", "tintColor");
			JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
			if (material.HasProperty("_Tint"))
			{
				Color color = material.GetColor("_Tint");
				jsonobject5.Add(color.r);
				jsonobject5.Add(color.g);
				jsonobject5.Add(color.b);
				jsonobject5.Add(color.a);
				jsonobject4.AddField("value", jsonobject5);
				jsonobject3.Add(jsonobject4);
			}
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject2.AddField("textures", jsonobject6);
			if (material.HasProperty("_Tex"))
			{
				JSONObject jsonobject7 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject6.Add(jsonobject7);
				jsonobject7.AddField("name", "textureCube");
				DataManager.saveCubeMapFile((Cubemap)material.GetTexture("_Tex"), jsonobject7, true, savePath);
			}
			Util.FileUtil.saveFile(savePath, jsonobject);
		}
		public static  void saveLayaParam(Material material,string savePath,string matName)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject13 = new JSONObject(JSONObject.Type.ARRAY);

			JSONObject obj = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject.AddField("version", DataManager.LmatVersion);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject.AddField("props", jsonobject4);
			jsonobject4.AddField("type", "Laya."+matName);
			string name = material.name;
			jsonobject4.AddField("name", name);
			jsonobject4.AddField("renderQueue", material.renderQueue);

		 
			{
				List<string> list = material.shaderKeywords.ToList<string>();
				jsonobject4.AddField("renderStates", jsonobject13);
				JSONObject jsonobject7 = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject13.Add(jsonobject7);
				if (material.HasProperty("_Cull"))
				{
					jsonobject7.AddField("cull", material.GetInt("_Cull"));
				}
				else
				{
					jsonobject7.AddField("cull", 2);
				}
				if (list.IndexOf("_ALPHABLEND_ON") != -1)
				{
					jsonobject7.AddField("blend", 1);
				}
				else
				{
					jsonobject7.AddField("blend", 0);
				}
				if (material.HasProperty("_SrcBlend"))
				{
					switch (material.GetInt("_SrcBlend"))
					{
					case 0:
						jsonobject7.AddField("srcBlend", 0);
						break;
					case 1:
						jsonobject7.AddField("srcBlend", 1);
						break;
					case 2:
						jsonobject7.AddField("srcBlend", 774);
						break;
					case 3:
						jsonobject7.AddField("srcBlend", 768);
						break;
					case 4:
						jsonobject7.AddField("srcBlend", 775);
						break;
					case 5:
						jsonobject7.AddField("srcBlend", 770);
						break;
					case 6:
						jsonobject7.AddField("srcBlend", 769);
						break;
					case 7:
						jsonobject7.AddField("srcBlend", 772);
						break;
					case 8:
						jsonobject7.AddField("srcBlend", 773);
						break;
					case 9:
						jsonobject7.AddField("srcBlend", 776);
						break;
					case 10:
						jsonobject7.AddField("srcBlend", 771);
						break;
					default:
						jsonobject7.AddField("srcBlend", 1);
						break;
					}
				}
				else
				{
					jsonobject7.AddField("srcBlend", 1);
				}
				if (material.HasProperty("_DstBlend"))
				{
					switch (material.GetInt("_DstBlend"))
					{
					case 0:
						jsonobject7.AddField("dstBlend", 0);
						break;
					case 1:
						jsonobject7.AddField("dstBlend", 1);
						break;
					case 2:
						jsonobject7.AddField("dstBlend", 774);
						break;
					case 3:
						jsonobject7.AddField("dstBlend", 768);
						break;
					case 4:
						jsonobject7.AddField("dstBlend", 775);
						break;
					case 5:
						jsonobject7.AddField("dstBlend", 770);
						break;
					case 6:
						jsonobject7.AddField("dstBlend", 769);
						break;
					case 7:
						jsonobject7.AddField("dstBlend", 772);
						break;
					case 8:
						jsonobject7.AddField("dstBlend", 773);
						break;
					case 9:
						jsonobject7.AddField("dstBlend", 776);
						break;
					case 10:
						jsonobject7.AddField("dstBlend", 771);
						break;
					default:
						jsonobject7.AddField("dstBlend", 0);
						break;
					}
				}
				else
				{
					jsonobject7.AddField("dstBlend", 0);
				}
				if (material.HasProperty("_ZWrite"))
				{
					if (material.GetInt("_ZWrite") == 1)
					{
						jsonobject7.AddField("depthWrite", true);
					}
					else
					{
						jsonobject7.AddField("depthWrite", false);
					}
				}
				else
				{
					jsonobject7.AddField("depthWrite", true);
				}
				if (material.HasProperty("_ZTest"))
				{
					switch (material.GetInt("_ZTest"))
					{
					case 0:
						jsonobject7.AddField("depthTest", 0);
						break;
					case 1:
						jsonobject7.AddField("depthTest", 512);
						break;
					case 2:
						jsonobject7.AddField("depthTest", 513);
						break;
					case 3:
						jsonobject7.AddField("depthTest", 514);
						break;
					case 4:
						jsonobject7.AddField("depthTest", 515);
						break;
					case 5:
						jsonobject7.AddField("depthTest", 516);
						break;
					case 6:
						jsonobject7.AddField("depthTest", 517);
						break;
					case 7:
						jsonobject7.AddField("depthTest", 518);
						break;
					case 8:
						jsonobject7.AddField("depthTest", 519);
						break;
					default:
						jsonobject7.AddField("depthTest", 0);
						break;
					}
				}
				else
				{
					jsonobject7.AddField("depthTest", 515);
				}
			

				/*if (list.IndexOf("_ALPHATEST_ON") != -1)
				{
					jsonobject2.AddField("alphaTest", true);
				}
				else
				{
					jsonobject2.AddField("alphaTest", false);
				}
				*/
			}
;
			int c = ShaderUtil.GetPropertyCount (material.shader);
			for(int i = 0 ; i < c; i++ )
			{
				string pname = ShaderUtil.GetPropertyName(material.shader, i);
				var ptype = ShaderUtil.GetPropertyType (material.shader,i);
				switch (ptype) {
				case ShaderUtil.ShaderPropertyType.Color:
					{
						JSONObject jsonobject9 = new JSONObject(JSONObject.Type.OBJECT);
						JSONObject jsonobject10 = new JSONObject(JSONObject.Type.ARRAY);					
						Color color2 = material.GetColor(pname);
						jsonobject10.Add(color2.r);
						jsonobject10.Add(color2.g);
						jsonobject10.Add(color2.b);
						jsonobject10.Add(color2.a);
						jsonobject9.AddField ("name", pname);
						jsonobject9.AddField("value", jsonobject10);
						jsonobject3.Add(jsonobject9);
					}
					break;
				case ShaderUtil.ShaderPropertyType.Float:
					{
						jsonobject4.AddField(pname, material.GetFloat(pname));
					}
					break;
				case ShaderUtil.ShaderPropertyType.Range:
					{
						jsonobject4.AddField(pname, material.GetFloat(pname));
					}
					break;
				case ShaderUtil.ShaderPropertyType.TexEnv:
					{
						Texture2D texture2D = (Texture2D)material.GetTexture(pname);
						if (texture2D != null)
						{
							JSONObject jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
							jsonobject5.AddField("name", pname);
							DataManager.saveTextureFile(jsonobject5, texture2D, DataManager.Platformindex, savePath, name, "path");
							jsonobject2.Add(jsonobject5);
						}
						Vector2 textureScale = material.GetTextureScale(pname);
						Vector2 textureOffset = material.GetTextureOffset(pname);

						JSONObject jsonobject9 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject9.AddField("name", pname+"_ST");
						JSONObject jsonobject10 = new JSONObject(JSONObject.Type.ARRAY);
						jsonobject10.Add(textureScale.x);
						jsonobject10.Add(textureScale.y);
						jsonobject10.Add(textureOffset.x);
						jsonobject10.Add(textureOffset.y);
						jsonobject9.AddField("value", jsonobject10);
						jsonobject3.Add(jsonobject9);
					}
					break;
				case ShaderUtil.ShaderPropertyType.Vector:
					{
						JSONObject jsonobject9 = new JSONObject(JSONObject.Type.OBJECT);
						JSONObject jsonobject10 = new JSONObject(JSONObject.Type.ARRAY);	
						Vector4 color2 = material.GetVector(pname);
						jsonobject10.Add(color2.x);
						jsonobject10.Add(color2.y);
						jsonobject10.Add(color2.z);
						jsonobject10.Add(color2.w);
						jsonobject9.AddField ("name", pname);
						jsonobject9.AddField("value", jsonobject10);
						jsonobject3.Add(jsonobject9);

					}
					break;
				}	
			}
			jsonobject4.AddField("textures", jsonobject2);
			jsonobject4.AddField("vectors", jsonobject3);
			jsonobject4.AddField("defines", obj);
			Util.FileUtil.saveFile(savePath, jsonobject);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0001207C File Offset: 0x0001027C
		public static void saveLayaWaterLmatFile(Material material, string savePath)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject obj = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject.AddField("version", DataManager.LmatVersion);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject.AddField("props", jsonobject4);
			jsonobject4.AddField("type", "Laya.WaterPrimaryMaterial");
			string name = material.name;
			jsonobject4.AddField("name", name);
			if (material.HasProperty("_WaveScale"))
			{
				jsonobject4.AddField("waveScale", material.GetFloat("_WaveScale"));
			}
			if (material.HasProperty("_ColorControl"))
			{
				Texture2D texture2D = (Texture2D)material.GetTexture("_ColorControl");
				if (texture2D != null)
				{
					JSONObject jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject5.AddField("name", "mainTexture");
					DataManager.saveTextureFile(jsonobject5, texture2D, DataManager.Platformindex, savePath, name, "path");
					jsonobject2.Add(jsonobject5);
				}
			}
			if (material.HasProperty("_BumpMap"))
			{
				Texture2D texture2D2 = (Texture2D)material.GetTexture("_BumpMap");
				if (texture2D2 != null)
				{
					JSONObject jsonobject6 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject6.AddField("name", "normalTexture");
					DataManager.saveTextureFile(jsonobject6, texture2D2, DataManager.Platformindex, savePath, name, "path");
					jsonobject2.Add(jsonobject6);
				}
			}
			JSONObject jsonobject7 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject7.AddField("name", "horizonColor");
			JSONObject jsonobject8 = new JSONObject(JSONObject.Type.ARRAY);
			if (material.HasProperty("_horizonColor"))
			{
				Color color = material.GetColor("_horizonColor");
				jsonobject8.Add(color.r);
				jsonobject8.Add(color.g);
				jsonobject8.Add(color.b);
				jsonobject8.Add(color.a);
				jsonobject7.AddField("value", jsonobject8);
				jsonobject3.Add(jsonobject7);
			}
			JSONObject jsonobject9 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject9.AddField("name", "waveSpeed");
			JSONObject jsonobject10 = new JSONObject(JSONObject.Type.ARRAY);
			if (material.HasProperty("_WaveSpeed"))
			{
				Color color2 = material.GetColor("_WaveSpeed");
				jsonobject10.Add(-color2.r);
				jsonobject10.Add(color2.g);
				jsonobject10.Add(-color2.b);
				jsonobject10.Add(color2.a);
				jsonobject9.AddField("value", jsonobject10);
				jsonobject3.Add(jsonobject9);
			}
			jsonobject4.AddField("textures", jsonobject2);
			jsonobject4.AddField("vectors", jsonobject3);
			jsonobject4.AddField("defines", obj);
			Util.FileUtil.saveFile(savePath, jsonobject);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00012318 File Offset: 0x00010518
		public static void saveLightMapFile(JSONObject props)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			props.AddField("lightmaps", jsonobject);
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			if (lightmaps != null && lightmaps.Length != 0)
			{
				for (int i = 0; i < lightmaps.Length; i++)
				{
					JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
					Texture2D lightmapColor = lightmaps[i].lightmapColor;
					if (!(lightmapColor == null))
					{
						JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
						JSONObject jsonobject4 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject2.AddField("constructParams", jsonobject3);
						jsonobject2.AddField("propertyParams", jsonobject4);
						jsonobject3.Add(lightmapColor.width);
						jsonobject3.Add(lightmapColor.height);
						jsonobject3.Add(1);
						jsonobject3.Add(false);
						if (lightmapColor.filterMode == null)
						{
							jsonobject4.AddField("filterMode", 0);
						}
						else if (lightmapColor.filterMode == (UnityEngine.FilterMode)1)
						{
							jsonobject4.AddField("filterMode", 1);
						}
						else if (lightmapColor.filterMode == (UnityEngine.FilterMode)2)
						{
							jsonobject4.AddField("filterMode", 2);
						}
						else
						{
							jsonobject4.AddField("filterMode", 0);
						}
						if (lightmapColor.wrapMode == null)
						{
							jsonobject4.AddField("wrapModeU", 0);
							jsonobject4.AddField("wrapModeV", 0);
						}
						else if (lightmapColor.wrapMode == (UnityEngine.TextureWrapMode)1)
						{
							jsonobject4.AddField("wrapModeU", 1);
							jsonobject4.AddField("wrapModeV", 1);
						}
						else
						{
							jsonobject4.AddField("wrapModeU", 0);
							jsonobject4.AddField("wrapModeV", 0);
						}
						jsonobject4.AddField("anisoLevel", lightmapColor.anisoLevel);
						if (lightmapColor != null)
						{
							string assetPath = AssetDatabase.GetAssetPath(lightmapColor.GetInstanceID());
							if (string.IsNullOrEmpty(assetPath))
							{
								Debug.LogError("LayaAir:can't select Auto Generate checkbox with generate Lighting.");
							}
							else
							{
								jsonobject.Add(jsonobject2);
								string path = DataManager.SAVEPATH + "/" + Path.GetDirectoryName(assetPath);
								if (!Directory.Exists(path))
								{
									Directory.CreateDirectory(path);
								}
								string text = DataManager.SAVEPATH + "/" + assetPath;
								text = text.Substring(0, text.LastIndexOf(".")) + ".png";
								TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
								textureImporter.isReadable = true;
								textureImporter.textureCompression = 0;
								AssetDatabase.ImportAsset(assetPath);
								FileStream fileStream = File.Open(text, FileMode.Create, FileAccess.ReadWrite);
								new BinaryWriter(fileStream).Write(lightmapColor.EncodeToPNG());
								fileStream.Close();
								jsonobject2.AddField("path", assetPath.Split(new char[]
								{
									'.'
								})[0] + ".png");
							}
						}
					}
				}
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0001259C File Offset: 0x0001079C
		public static void saveTextureFile(JSONObject obj, Texture2D texture, int index, string MaterialPath = null, string materialName = null, string nodeName = "path")
		{
			if (!(texture != null))
			{
				obj.AddField(nodeName, "");
				return;
			}
			string assetPath = AssetDatabase.GetAssetPath(texture.GetInstanceID());
			string text = DataManager.SAVEPATH + "/" + Path.GetDirectoryName(assetPath);
			text = DataManager.cleanIllegalChar(text, false);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null)
			{
				return;
			}
			string text2 = DataManager.SAVEPATH + "/" + assetPath;
			text2 = text2.Substring(0, text2.LastIndexOf("."));
			TextureInfo textureInfo;
			if (!DataManager.textureInfo.ContainsKey(assetPath))
			{
				textureInfo = new TextureInfo();
				textureInfo.Path = assetPath;
				textureInfo.SavePath = text2;
				textureInfo.texture = texture;
				DataManager.textureInfo.Add(assetPath, textureInfo);
				if (texture.format == (UnityEngine.TextureFormat)3 || texture.format == (UnityEngine.TextureFormat)10 || texture.format == (UnityEngine.TextureFormat)28)
				{
					textureInfo.format = 0;
				}
				else if (texture.format == (UnityEngine.TextureFormat)4 || texture.format == (UnityEngine.TextureFormat)12 || texture.format == (UnityEngine.TextureFormat)29)
				{
					textureInfo.format = 1;
				}
				else
				{
					textureInfo.format = 1;
				}
				if (textureImporter.mipmapEnabled)
				{
					textureInfo.MipMap = texture.mipmapCount;
				}
				else
				{
					textureInfo.MipMap = 0;
				}
			}
			else
			{
				textureInfo = DataManager.textureInfo[assetPath];
			}
			switch (index)
			{
			case 0:
				if (textureInfo.format == 0)
				{
					text2 += ".jpg";
				}
				else
				{
					text2 += ".png";
				}
				text2 = DataManager.cleanIllegalChar(text2, false);
				break;
			case 1:
				text2 += ".pvr";
				break;
			case 2:
				if (textureInfo.format == 0)
				{
					text2 += ".ktx";
				}
				else if (textureInfo.format == 1)
				{
					text2 += ".png";
				}
				break;
			default:
				Debug.LogError("no format select");
				break;
			}
			textureInfo.SavePath = text2;
			if (!File.Exists(assetPath))
			{
				obj.AddField(nodeName, "");
				return;
			}
			string relativePath = Util.FileUtil.getRelativePath(MaterialPath, text2);
			obj.AddField(nodeName, relativePath);
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
			obj.AddField("constructParams", jsonobject);
			obj.AddField("propertyParams", jsonobject2);
			if (index == 1)
			{
				int val = Mathf.Max(texture.width, texture.height);
				jsonobject.Add(val);
				jsonobject.Add(val);
			}
			else
			{
				jsonobject.Add(texture.width);
				jsonobject.Add(texture.height);
			}
			if (textureInfo.format == 0)
			{
				switch (index)
				{
				case 0:
					jsonobject.Add(0);
					break;
				case 1:
					jsonobject.Add(11);
					break;
				case 2:
					jsonobject.Add(5);
					break;
				}
			}
			else if (textureInfo.format == 1)
			{
				switch (index)
				{
				case 0:
					jsonobject.Add(1);
					break;
				case 1:
					jsonobject.Add(12);
					break;
				case 2:
					jsonobject.Add(1);
					break;
				}
			}
			else
			{
				jsonobject.Add(1);
			}
			if (textureImporter != null)
			{
				jsonobject.Add(textureImporter.mipmapEnabled);
			}
			else
			{
				jsonobject.Add(false);
			}
			if (texture.filterMode == null)
			{
				jsonobject2.AddField("filterMode", 0);
			}
			else if (texture.filterMode == (UnityEngine.FilterMode)1)
			{
				jsonobject2.AddField("filterMode", 1);
			}
			else if (texture.filterMode == (UnityEngine.FilterMode)2)
			{
				jsonobject2.AddField("filterMode", 2);
			}
			else
			{
				jsonobject2.AddField("filterMode", 1);
			}
			if (texture.wrapMode == null)
			{
				jsonobject2.AddField("wrapModeU", 0);
				jsonobject2.AddField("wrapModeV", 0);
			}
			else if (texture.wrapMode == (UnityEngine.TextureWrapMode)1)
			{
				jsonobject2.AddField("wrapModeU", 1);
				jsonobject2.AddField("wrapModeV", 1);
			}
			else
			{
				jsonobject2.AddField("wrapModeU", 0);
				jsonobject2.AddField("wrapModeV", 0);
			}
			if (textureImporter != null)
			{
				jsonobject2.AddField("anisoLevel", texture.anisoLevel);
				return;
			}
			jsonobject2.AddField("anisoLevel", 0);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000129A4 File Offset: 0x00010BA4
		public static void saveCubeMapFile(Cubemap cubemap, JSONObject props, bool isMaterial = false, string materialPath = null)
		{
			if (cubemap == null)
			{
				return;
			}
			string text = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(cubemap.GetInstanceID()), false);
			text = text.Split(new char[]
			{
				'.'
			})[0];
			if (isMaterial)
			{
				string relativePath = Util.FileUtil.getRelativePath(materialPath, DataManager.SAVEPATH + "/" + text + ".ltc");
				props.AddField("path", relativePath);
			}
			else
			{
				props.AddField("reflectionTexture", text + ".ltc");
			}
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			string str = text.Substring(text.LastIndexOf("/") + 1);
			jsonobject.AddField("front", str + "_PositiveZ.png");
			jsonobject.AddField("back", str + "_NegativeZ.png");
			jsonobject.AddField("left", str + "_PositiveX.png");
			jsonobject.AddField("right", str + "_NegativeX.png");
			jsonobject.AddField("up", str + "_PositiveY.png");
			jsonobject.AddField("down", str + "_NegativeY.png");
			text = DataManager.SAVEPATH + "/" + text + ".ltc";
			Util.FileUtil.saveFile(text, jsonobject);
			try
			{
				Color[] pixels = cubemap.GetPixels((UnityEngine.CubemapFace)0);
				Color[] pixels2 = cubemap.GetPixels((UnityEngine.CubemapFace)1);
				Color[] pixels3 = cubemap.GetPixels((UnityEngine.CubemapFace)2);
				Color[] pixels4 = cubemap.GetPixels((UnityEngine.CubemapFace)3);
				Color[] pixels5 = cubemap.GetPixels((UnityEngine.CubemapFace)4);
				Color[] pixels6 = cubemap.GetPixels((UnityEngine.CubemapFace)5);
				Texture2D cubeMapTextureData = DataManager.getCubeMapTextureData(pixels);
				Texture2D cubeMapTextureData2 = DataManager.getCubeMapTextureData(pixels2);
				Texture2D cubeMapTextureData3 = DataManager.getCubeMapTextureData(pixels3);
				Texture2D cubeMapTextureData4 = DataManager.getCubeMapTextureData(pixels4);
				Texture2D cubeMapTextureData5 = DataManager.getCubeMapTextureData(pixels5);
				Texture2D cubeMapTextureData6 = DataManager.getCubeMapTextureData(pixels6);
				text = text.Substring(0, text.LastIndexOf('.'));
				File.WriteAllBytes(text + "_PositiveX.png", cubeMapTextureData.EncodeToPNG());
				File.WriteAllBytes(text + "_NegativeX.png", cubeMapTextureData2.EncodeToPNG());
				File.WriteAllBytes(text + "_PositiveY.png", cubeMapTextureData3.EncodeToPNG());
				File.WriteAllBytes(text + "_NegativeY.png", cubeMapTextureData4.EncodeToPNG());
				File.WriteAllBytes(text + "_PositiveZ.png", cubeMapTextureData5.EncodeToPNG());
				File.WriteAllBytes(text + "_NegativeZ.png", cubeMapTextureData6.EncodeToPNG());
			}
			catch (Exception ex)
			{
				ex.ToString();
				Debug.LogWarning("LayaAir3D Warning(Code:2006) : " + cubemap.name + "must set can read!");
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00012C24 File Offset: 0x00010E24
		public static void saveLaniData(GameObject gameObject, JSONObject obj)
		{
			List<DataManager.ComponentType> list = DataManager.componentsOnGameObject(gameObject);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("UnityEngine.GameObject", "");
			dictionary.Add("UnityEngine.Transform", "transform");
			dictionary.Add("UnityEngine.MeshRenderer", "meshRenderer");
			dictionary.Add("UnityEngine.SkinnedMeshRenderer", "skinnedMeshRenderer");
			dictionary.Add("UnityEngine.ParticleSystemRenderer", "particleRenderer");
			dictionary.Add("UnityEngine.TrailRenderer", "trailRenderer");
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2.Add("m_IsActive", "active");
			dictionary2.Add("m_LocalPosition", "localPosition");
			dictionary2.Add("m_LocalRotation", "localRotation");
			dictionary2.Add("m_LocalScale", "localScale");
			dictionary2.Add("localEulerAnglesRaw", "localRotationEuler");
			dictionary2.Add("material", "material");
			dictionary2.Add("m_Enabled", "enable");
			Dictionary<string, byte> dictionary3 = new Dictionary<string, byte>();
			dictionary3.Add("m_LocalPosition", 12);
			dictionary3.Add("m_LocalRotation", 16);
			dictionary3.Add("m_LocalScale", 12);
			dictionary3.Add("localEulerAnglesRaw", 12);
			Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
			dictionary4.Add("m_LocalPosition", 3);
			dictionary4.Add("m_LocalRotation", 4);
			dictionary4.Add("m_LocalScale", 3);
			dictionary4.Add("localEulerAnglesRaw", 3);
			List<string> list2 = new List<string>();
			list2.Add("x");
			list2.Add("y");
			list2.Add("z");
			List<string> list3 = new List<string>();
			list3.Add("x");
			list3.Add("y");
			list3.Add("z");
			list3.Add("w");
			new List<string>().Add("value");
			Dictionary<string, List<string>> dictionary5 = new Dictionary<string, List<string>>();
			dictionary5.Add("m_LocalPosition", list2);
			dictionary5.Add("m_LocalRotation", list3);
			dictionary5.Add("m_LocalScale", list2);
			dictionary5.Add("localEulerAnglesRaw", list2);
			List<ushort> list4 = new List<ushort>();
			list4.Add(12);
			list4.Add(16);
			AnimatorController animatorController = (AnimatorController)gameObject.GetComponent<Animator>().runtimeAnimatorController;
			if (animatorController == null)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:1002) : " + gameObject.name + "'s Animator Compoment must have a Controller!");
				return;
			}
			AnimatorControllerLayer[] layers = animatorController.layers;
			int num = layers.Length;
			for (int i = 0; i < num; i++)
			{
				AnimatorControllerLayer animatorControllerLayer = layers[i];
				ChildAnimatorState[] states = animatorControllerLayer.stateMachine.states;
				JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
				jsonobject.AddField("name", animatorControllerLayer.name);
				jsonobject.AddField("weight", animatorControllerLayer.defaultWeight);
				if (animatorControllerLayer.blendingMode == null)
				{
					jsonobject.AddField("blendingMode", 0);
				}
				else if (animatorControllerLayer.blendingMode == (UnityEditor.Animations.AnimatorLayerBlendingMode)1)
				{
					jsonobject.AddField("blendingMode", 1);
				}
				else
				{
					jsonobject.AddField("blendingMode", 0);
				}
				JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
				jsonobject.AddField("states", jsonobject2);
				obj.Add(jsonobject);
				for (int j = 0; j < states.Length; j++)
				{
					AnimatorState state = states[j].state;
					JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject3.AddField("name", state.name);
					jsonobject2.Add(jsonobject3);
					AnimationClip animationClip = state.motion as AnimationClip;
					List<double> list5 = new List<double>();
					List<string> list6 = new List<string>();
					list6.Add("ANIMATIONS");
					if (animationClip != null)
					{
						string name = gameObject.name;
						int num2 = (int)animationClip.frameRate;
						string text = DataManager.cleanIllegalChar(animationClip.name, true);
						list6.Add(text);
						string text2 = DataManager.cleanIllegalChar(AssetDatabase.GetAssetPath(animationClip.GetInstanceID()).Split(new char[]
						{
							'.'
						})[0], false) + "-" + text + ".lani";
						string fileName = DataManager.SAVEPATH + "/" + text2;
						jsonobject3.AddField("clipPath", text2);
						EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animationClip);
						AnimationClipCurveData[] array = new AnimationClipCurveData[curveBindings.Length];
						for (int k = 0; k < curveBindings.Length; k++)
						{
							array[k] = new AnimationClipCurveData(curveBindings[k]);
							array[k].curve = AnimationUtility.GetEditorCurve(animationClip, curveBindings[k]);
						}
						for (int l = 0; l < array.Length; l++)
						{
							Keyframe[] keys = array[l].curve.keys;
							for (int m = 0; m < keys.Length; m++)
							{
								double item = Math.Round((double)keys[m].time, 3);
								if (list5.IndexOf(item) == -1)
								{
									list5.Add(item);
								}
							}
						}
						list5.Sort();
						List<string> list7 = new List<string>();
						List<DataManager.CustomAnimationClipCurveData> list8 = new List<DataManager.CustomAnimationClipCurveData>();
						foreach (AnimationClipCurveData animationClipCurveData in array)
						{
							DataManager.CustomAnimationCurve curve;
							curve.keys = animationClipCurveData.curve.keys;
							DataManager.CustomAnimationClipCurveData item2;
							item2.curve = curve;
							item2.path = animationClipCurveData.path;
							item2.propertyName = animationClipCurveData.propertyName;
							item2.type = animationClipCurveData.type;
							list8.Add(item2);
						}
						List<DataManager.CustomAnimationClipCurveData> list9 = new List<DataManager.CustomAnimationClipCurveData>();
						List<DataManager.CustomAnimationClipCurveData> list10 = new List<DataManager.CustomAnimationClipCurveData>();
						foreach (AnimationClipCurveData animationClipCurveData2 in array)
						{
							string path = animationClipCurveData2.path;
							string propertyName = animationClipCurveData2.propertyName;
							if (dictionary.ContainsKey(animationClipCurveData2.type.ToString()))
							{
								string a = dictionary[animationClipCurveData2.type.ToString()];
								if (a == "meshRenderer" || a == "skinnedMeshRenderer" || a == "particleRenderer" || a == "trailRenderer" || a == "" || a == "")
								{
									DataManager.CustomAnimationCurve curve2;
									curve2.keys = animationClipCurveData2.curve.keys;
									DataManager.CustomAnimationClipCurveData item3;
									item3.curve = curve2;
									item3.path = animationClipCurveData2.path;
									item3.propertyName = animationClipCurveData2.propertyName;
									item3.type = animationClipCurveData2.type;
									list10.Add(item3);
								}
								else
								{
									string text3 = propertyName.Substring(0, propertyName.LastIndexOf('.'));
									text3 = propertyName.Substring(0, propertyName.LastIndexOf('.'));
									string item4 = text3 + "|" + path;
									if (list7.IndexOf(item4) == -1)
									{
										list7.Add(item4);
										list9 = new List<DataManager.CustomAnimationClipCurveData>();
										for (int num4 = 0; num4 < dictionary5[text3].Count; num4++)
										{
											string b = text3 + "." + dictionary5[text3][num4];
											for (int num5 = 0; num5 < list8.Count; num5++)
											{
												if (list8[num5].propertyName == b && list8[num5].path == path)
												{
													list9.Add(list8[num5]);
													list8.RemoveAt(list8.IndexOf(list8[num5]));
												}
											}
										}
										if (dictionary5[text3].Count != list9.Count)
										{
											List<DataManager.CustomAnimationClipCurveData> list11 = new List<DataManager.CustomAnimationClipCurveData>();
											for (int num6 = 0; num6 < dictionary5[text3].Count; num6++)
											{
												string text4 = text3 + "." + dictionary5[text3][num6];
												bool flag = false;
												for (int num7 = 0; num7 < list9.Count; num7++)
												{
													if (list9[num7].propertyName == text4)
													{
														flag = true;
														list11.Add(list9[num7]);
													}
												}
												if (!flag)
												{
													DataManager.CustomAnimationCurve curve3;
													curve3.keys = new Keyframe[0];
													DataManager.CustomAnimationClipCurveData item5;
													item5.path = list9[0].path;
													item5.propertyName = text4;
													item5.type = list9[0].type;
													item5.curve = curve3;
													list11.Add(item5);
												}
											}
											list9 = list11;
										}
										List<double> list12 = new List<double>();
										for (int num8 = 0; num8 < list9.Count; num8++)
										{
											Keyframe[] keys2 = list9[num8].curve.keys;
											for (int num9 = 0; num9 < keys2.Length; num9++)
											{
												bool flag2 = false;
												for (int num10 = 0; num10 < list12.Count; num10++)
												{
													if (Math.Round(list12[num10], 3) == Math.Round((double)keys2[num9].time, 3))
													{
														flag2 = true;
													}
												}
												if (!flag2)
												{
													list12.Add((double)keys2[num9].time);
												}
											}
										}
										list12.Sort();
										List<Keyframe> list13 = new List<Keyframe>();
										for (int num11 = 0; num11 < list12.Count; num11++)
										{
											Keyframe item6 = default(Keyframe);
											item6.inTangent = float.NaN;
											item6.outTangent = float.NaN;
											item6.time = (float)list12[num11];
											item6.value = float.NaN;
											list13.Add(item6);
										}
										for (int num12 = 0; num12 < list9.Count; num12++)
										{
											List<Keyframe> list14 = list9[num12].curve.keys.ToList<Keyframe>();
											List<Keyframe> list15 = new List<Keyframe>();
											for (int num13 = 0; num13 < list12.Count; num13++)
											{
												bool flag3 = false;
												for (int num14 = 0; num14 < list14.Count; num14++)
												{
													if (Math.Round((double)list14[num14].time, 3) == Math.Round(list12[num13], 3))
													{
														flag3 = true;
														list15.Add(list14[num14]);
													}
												}
												if (!flag3)
												{
													list15.Add(list13[num13]);
												}
											}
											for (int num15 = 0; num15 < list12.Count; num15++)
											{
												if (float.IsNaN(list15[num15].value))
												{
													bool flag4 = false;
													bool flag5 = false;
													int index = -1;
													int index2 = -1;
													for (int num16 = num15 - 1; num16 >= 0; num16--)
													{
														if (!float.IsNaN(list15[num16].value))
														{
															flag4 = true;
															index = num16;
															break;
														}
													}
													for (int num17 = num15 + 1; num17 < list12.Count; num17++)
													{
														if (!float.IsNaN(list15[num17].value))
														{
															flag5 = true;
															index2 = num17;
															break;
														}
													}
													if (flag4 && flag5)
													{
														float num18 = list15[index2].time - list15[index].time;
														float t = (float)((list12[num15] - list12[index]) / (list12[index2] - list12[index]));
														float outTangent;
														float value = MathUtil.Interpolate((float)list12[index], (float)list12[index2], list15[index].value, list15[index2].value, list15[index].outTangent * num18, list15[index2].inTangent * num18, t, out outTangent);
														Keyframe value2 = default(Keyframe);
														value2.inTangent = (value2.outTangent = outTangent);
														value2.value = value;
														value2.time = (float)list12[num15];
														list15[num15] = value2;
													}
													else if (flag4 && !flag5)
													{
														Keyframe value3 = default(Keyframe);
														value3.inTangent = (value3.outTangent = 0f);
														value3.value = list15[index].value;
														value3.time = (float)list12[num15];
														list15[num15] = value3;
													}
													else if (!flag4 && flag5)
													{
														Keyframe value4 = default(Keyframe);
														value4.inTangent = (value4.outTangent = 0f);
														value4.value = list15[index2].value;
														value4.time = (float)list12[num15];
														list15[num15] = value4;
													}
													else
													{
														Debug.LogWarning(string.Concat(new string[]
														{
															gameObject.name,
															"'s Animator ",
															gameObject.name,
															"/",
															list9[num12].path,
															" ",
															list9[num12].propertyName,
															" keyFrame data can't be null!"
														}));
													}
												}
											}
											DataManager.CustomAnimationCurve curve4;
											curve4.keys = list15.ToArray();
											DataManager.CustomAnimationClipCurveData value5;
											value5.curve = curve4;
											value5.path = list9[num12].path;
											value5.propertyName = list9[num12].propertyName;
											value5.type = list9[num12].type;
											list9[num12] = value5;
										}
										for (int num19 = 0; num19 < list9.Count; num19++)
										{
											list10.Add(list9[num19]);
										}
									}
								}
							}
						}
						List<DataManager.AniNodeData> list16 = new List<DataManager.AniNodeData>();
						int num20 = 0;
						for (int num21 = 0; num21 < list10.Count; num21 += num20)
						{
							DataManager.CustomAnimationClipCurveData customAnimationClipCurveData = list10[num21];
							List<ushort> list17 = new List<ushort>();
							string[] array2 = customAnimationClipCurveData.path.Split(new char[]
							{
								'/'
							});
							for (int num22 = 0; num22 < array2.Length; num22++)
							{
								if (list6.IndexOf(array2[num22]) == -1)
								{
									list6.Add(array2[num22]);
								}
								list17.Add((ushort)list6.IndexOf(array2[num22]));
							}
							DataManager.AniNodeData aniNodeData;
							aniNodeData.pathLength = (ushort)list17.Count;
							aniNodeData.pathIndex = list17;
							string text5 = dictionary[customAnimationClipCurveData.type.ToString()];
							if (list6.IndexOf(text5) == -1)
							{
								list6.Add(text5);
							}
							aniNodeData.conpomentTypeIndex = (ushort)list6.IndexOf(text5);
							string[] array3 = customAnimationClipCurveData.propertyName.Split(new char[]
							{
								'.'
							});
							List<ushort> list18 = new List<ushort>();
							string text6 = array3[0];
							text6 = dictionary2[text6];
							if (text5 == "transform")
							{
								if (list6.IndexOf(text6) == -1)
								{
									list6.Add(text6);
								}
								list18.Add((ushort)list6.IndexOf(text6));
								aniNodeData.propertyNameLength = 1;
								aniNodeData.propertyNameIndex = list18;
							}
							else if (text5 == "meshRenderer" || text5 == "skinnedMeshRenderer" || text5 == "particleRenderer" || text5 == "trailRenderer" || text5 == "")
							{
								if (array3.Length == 1)
								{
									if (list6.IndexOf(text6) == -1)
									{
										list6.Add(text6);
									}
									list18.Add((ushort)list6.IndexOf(text6));
									aniNodeData.propertyNameLength = 1;
									aniNodeData.propertyNameIndex = list18;
								}
								else if (array3.Length == 2)
								{
									if (list6.IndexOf(text6) == -1)
									{
										list6.Add(text6);
									}
									list18.Add((ushort)list6.IndexOf(text6));
									string item7 = array3[1];
									if (list6.IndexOf(item7) == -1)
									{
										list6.Add(item7);
									}
									list18.Add((ushort)list6.IndexOf(item7));
									aniNodeData.propertyNameLength = 2;
									aniNodeData.propertyNameIndex = list18;
								}
								else if (array3.Length == 3)
								{
									if (list6.IndexOf(text6) == -1)
									{
										list6.Add(text6);
									}
									list18.Add((ushort)list6.IndexOf(text6));
									string text7 = array3[1];
									text7 += array3[2].ToUpper();
									if (list6.IndexOf(text7) == -1)
									{
										list6.Add(text7);
									}
									list18.Add((ushort)list6.IndexOf(text7));
									aniNodeData.propertyNameLength = 2;
									aniNodeData.propertyNameIndex = list18;
								}
								else
								{
									aniNodeData.propertyNameLength = 0;
									aniNodeData.propertyNameIndex = list18;
									Debug.LogWarning("LayaAir3D : " + gameObject.name + " Animation attribute length overbounds!");
								}
							}
							else
							{
								aniNodeData.propertyNameLength = 0;
								aniNodeData.propertyNameIndex = list18;
								Debug.LogWarning("LayaAir3D : " + gameObject.name + " Animation attribute length overbounds!");
							}
							if (array3[0] == "m_LocalPosition")
							{
								aniNodeData.type = 1;
							}
							else if (array3[0] == "m_LocalRotation")
							{
								aniNodeData.type = 2;
							}
							else if (array3[0] == "m_LocalScale")
							{
								aniNodeData.type = 3;
							}
							else if (array3[0] == "localEulerAnglesRaw")
							{
								aniNodeData.type = 4;
							}
							else
							{
								aniNodeData.type = 0;
							}
							try
							{
								num20 = dictionary4[array3[0]];
							}
							catch (Exception ex)
							{
								ex.ToString();
								num20 = 1;
							}
							List<DataManager.AniNodeFrameData> list19 = new List<DataManager.AniNodeFrameData>();
							Keyframe[] keys3 = customAnimationClipCurveData.curve.keys;
							for (int num23 = 0; num23 < keys3.Length; num23++)
							{
								float time = keys3[num23].time;
								DataManager.AniNodeFrameData item8;
								item8.startTimeIndex = (ushort)list5.IndexOf(Math.Round((double)time, 3));
								List<float> list20 = new List<float>();
								List<float> list21 = new List<float>();
								List<float> list22 = new List<float>();
								int num24 = 0;
								for (int num25 = num21; num25 < num21 + num20; num25++)
								{
									Keyframe keyframe = list10[num25].curve.keys[num23];
									if (text6 == "localPosition")
									{
										if (num24 == 0)
										{
											list20.Add(keyframe.value * -1f);
											list21.Add(keyframe.inTangent * -1f);
											list22.Add(keyframe.outTangent * -1f);
										}
										else
										{
											list20.Add(keyframe.value);
											list21.Add(keyframe.inTangent);
											list22.Add(keyframe.outTangent);
										}
									}
									else if (text6 == "localRotation")
									{
										if (num24 == 0 || num24 == 3)
										{
											list20.Add(keyframe.value * -1f);
											list21.Add(keyframe.inTangent * -1f);
											list22.Add(keyframe.outTangent * -1f);
										}
										else
										{
											list20.Add(keyframe.value);
											list21.Add(keyframe.inTangent);
											list22.Add(keyframe.outTangent);
										}
									}
									else if (text6 == "localRotationEuler")
									{
										if (list.IndexOf(DataManager.ComponentType.Camera) != -1)
										{
											if (num24 == 0)
											{
												list20.Add(keyframe.value * -1f);
												list21.Add(keyframe.inTangent * -1f);
												list22.Add(keyframe.outTangent * -1f);
											}
											else if (num24 == 1)
											{
												list20.Add(180f - keyframe.value);
												list21.Add(keyframe.inTangent * -1f);
												list22.Add(keyframe.outTangent * -1f);
											}
											else
											{
												list20.Add(keyframe.value);
												list21.Add(keyframe.inTangent);
												list22.Add(keyframe.outTangent);
											}
										}
										else if (num24 == 1 || num24 == 2)
										{
											list20.Add(keyframe.value * -1f);
											list21.Add(keyframe.inTangent * -1f);
											list22.Add(keyframe.outTangent * -1f);
										}
										else
										{
											list20.Add(keyframe.value);
											list21.Add(keyframe.inTangent);
											list22.Add(keyframe.outTangent);
										}
									}
									else
									{
										list20.Add(keyframe.value);
										list21.Add(keyframe.inTangent);
										list22.Add(keyframe.outTangent);
									}
									num24++;
								}
								item8.valueNumbers = list20;
								item8.inTangentNumbers = list21;
								item8.outTangentNumbers = list22;
								list19.Add(item8);
							}
							aniNodeData.keyFrameCount = (ushort)keys3.Length;
							aniNodeData.aniNodeFrameDatas = list19;
							list16.Add(aniNodeData);
						}
						FileStream fileStream = Util.FileUtil.saveFile(fileName, null);
						string laniVersion = DataManager.LaniVersion;
						Util.FileUtil.WriteData(fileStream, laniVersion);
						long position = fileStream.Position;
						Util.FileUtil.WriteData(fileStream, new uint[1]);
						Util.FileUtil.WriteData(fileStream, new uint[1]);
						long position2 = fileStream.Position;
						int num26 = 1;
						Util.FileUtil.WriteData(fileStream, new ushort[]
						{
							(ushort)num26
						});
						for (int num27 = 0; num27 < num26; num27++)
						{
							Util.FileUtil.WriteData(fileStream, new uint[1]);
							Util.FileUtil.WriteData(fileStream, new uint[1]);
						}
						long position3 = fileStream.Position;
						Util.FileUtil.WriteData(fileStream, new uint[1]);
						Util.FileUtil.WriteData(fileStream, new ushort[1]);
						long position4 = fileStream.Position;
						Util.FileUtil.WriteData(fileStream, new ushort[]
						{
							(ushort)list6.IndexOf("ANIMATIONS")
						});
						Util.FileUtil.WriteData(fileStream, new ushort[]
						{
							(ushort)list5.Count
						});
						for (int num28 = 0; num28 < list5.Count; num28++)
						{
							Util.FileUtil.WriteData(fileStream, new float[]
							{
								(float)list5[num28]
							});
						}
						Util.FileUtil.WriteData(fileStream, new ushort[]
						{
							(ushort)list6.IndexOf(text)
						});
						float num29 = (list5.Count == 0) ? 0f : ((float)list5[list5.Count - 1]);
						Util.FileUtil.WriteData(fileStream, new float[]
						{
							num29
						});
						Util.FileUtil.WriteData(fileStream, new bool[]
						{
							animationClip.isLooping
						});
						Util.FileUtil.WriteData(fileStream, new ushort[]
						{
							(ushort)num2
						});
						Util.FileUtil.WriteData(fileStream, new ushort[]
						{
							(ushort)list16.Count
						});
						for (int num30 = 0; num30 < list16.Count; num30++)
						{
							DataManager.AniNodeData aniNodeData = list16[num30];
							Util.FileUtil.WriteData(fileStream, new byte[]
							{
								aniNodeData.type
							});
							Util.FileUtil.WriteData(fileStream, new ushort[]
							{
								aniNodeData.pathLength
							});
							for (int num31 = 0; num31 < (int)aniNodeData.pathLength; num31++)
							{
								Util.FileUtil.WriteData(fileStream, new ushort[]
								{
									aniNodeData.pathIndex[num31]
								});
							}
							Util.FileUtil.WriteData(fileStream, new ushort[]
							{
								aniNodeData.conpomentTypeIndex
							});
							Util.FileUtil.WriteData(fileStream, new ushort[]
							{
								aniNodeData.propertyNameLength
							});
							for (int num32 = 0; num32 < (int)aniNodeData.propertyNameLength; num32++)
							{
								Util.FileUtil.WriteData(fileStream, new ushort[]
								{
									aniNodeData.propertyNameIndex[num32]
								});
							}
							Util.FileUtil.WriteData(fileStream, new ushort[]
							{
								aniNodeData.keyFrameCount
							});
							for (int num33 = 0; num33 < (int)aniNodeData.keyFrameCount; num33++)
							{
								Util.FileUtil.WriteData(fileStream, new ushort[]
								{
									aniNodeData.aniNodeFrameDatas[num33].startTimeIndex
								});
								List<float> valueNumbers = aniNodeData.aniNodeFrameDatas[num33].valueNumbers;
								List<float> inTangentNumbers = aniNodeData.aniNodeFrameDatas[num33].inTangentNumbers;
								List<float> outTangentNumbers = aniNodeData.aniNodeFrameDatas[num33].outTangentNumbers;
								for (int num34 = 0; num34 < inTangentNumbers.Count; num34++)
								{
									Util.FileUtil.WriteData(fileStream, new float[]
									{
										inTangentNumbers[num34]
									});
								}
								for (int num35 = 0; num35 < outTangentNumbers.Count; num35++)
								{
									Util.FileUtil.WriteData(fileStream, new float[]
									{
										outTangentNumbers[num35]
									});
								}
								for (int num36 = 0; num36 < valueNumbers.Count; num36++)
								{
									Util.FileUtil.WriteData(fileStream, new float[]
									{
										valueNumbers[num36]
									});
								}
							}
						}
						AnimationEvent[] events = animationClip.events;
						int num37 = events.Length;
						Util.FileUtil.WriteData(fileStream, new short[]
						{
							(short)num37
						});
						for (int num38 = 0; num38 < num37; num38++)
						{
							AnimationEvent animationEvent = events[num38];
							Util.FileUtil.WriteData(fileStream, new float[]
							{
								animationEvent.time
							});
							string functionName = animationEvent.functionName;
							if (list6.IndexOf(functionName) == -1)
							{
								list6.Add(functionName);
							}
							Util.FileUtil.WriteData(fileStream, new ushort[]
							{
								(ushort)list6.IndexOf(functionName)
							});
							ushort num39 = 3;
							Util.FileUtil.WriteData(fileStream, new ushort[]
							{
								num39
							});
							for (int num40 = 0; num40 < 1; num40++)
							{
								Util.FileUtil.WriteData(fileStream, new byte[]
								{
									2
								});
								Util.FileUtil.WriteData(fileStream, new float[]
								{
									animationEvent.floatParameter
								});
								Util.FileUtil.WriteData(fileStream, new byte[]
								{
									1
								});
								Util.FileUtil.WriteData(fileStream, new int[]
								{
									animationEvent.intParameter
								});
								Util.FileUtil.WriteData(fileStream, new byte[]
								{
									3
								});
								string stringParameter = animationEvent.stringParameter;
								if (list6.IndexOf(stringParameter) == -1)
								{
									list6.Add(stringParameter);
								}
								Util.FileUtil.WriteData(fileStream, new ushort[]
								{
									(ushort)list6.IndexOf(stringParameter)
								});
							}
						}
						long position5 = fileStream.Position;
						for (int num41 = 0; num41 < list6.Count; num41++)
						{
							Util.FileUtil.WriteData(fileStream, list6[num41]);
						}
						long position6 = fileStream.Position;
						fileStream.Position = position3 + 4L;
						Util.FileUtil.WriteData(fileStream, new ushort[]
						{
							(ushort)list6.Count
						});
						fileStream.Position = position2 + 2L + 4L;
						Util.FileUtil.WriteData(fileStream, new uint[]
						{
							(uint)(position5 - position4)
						});
						fileStream.Position = position;
						Util.FileUtil.WriteData(fileStream, new uint[]
						{
							(uint)position5
						});
						Util.FileUtil.WriteData(fileStream, new uint[]
						{
							(uint)(position6 - position5)
						});
						fileStream.Close();
					}
				}
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000146F4 File Offset: 0x000128F4
		public static void saveTerrainData(string savePath, JSONObject obj, GameObject gameObject = null)
		{
			LayaTerrainExporter.ExportAllTerrian(savePath, obj);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00014700 File Offset: 0x00012900
		public static void saveParticleFrameData(ParticleSystem.MinMaxCurve minMaxCurve, JSONObject obj, string str1, string str2, int type, float curveMultiplier, float convert)
		{
			AnimationCurve animationCurve;
			if (type == -1)
			{
				animationCurve = minMaxCurve.curveMin;
			}
			else if (type == 1)
			{
				animationCurve = minMaxCurve.curveMax;
			}
			else
			{
				animationCurve = minMaxCurve.curve;
			}
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			obj.AddField(str1, jsonobject);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
			if (animationCurve != null && animationCurve.length != 0)
			{
				int length = animationCurve.length;
				for (int i = 0; i < length; i++)
				{
					Keyframe keyframe = animationCurve[i];
					if (i == 0)
					{
						jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject3.AddField("key", 0f);
						jsonobject3.AddField("value", keyframe.value * curveMultiplier * convert);
						jsonobject2.Add(jsonobject3);
						if (keyframe.time - DataManager.precision > 0f && (double)keyframe.time < 0.5)
						{
							jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
							jsonobject3.AddField("key", keyframe.time);
							jsonobject3.AddField("value", keyframe.value * curveMultiplier * convert);
							jsonobject2.Add(jsonobject3);
						}
					}
					if (i != 0 && i != length - 1)
					{
						jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject3.AddField("key", keyframe.time);
						jsonobject3.AddField("value", keyframe.value * curveMultiplier * convert);
						jsonobject2.Add(jsonobject3);
					}
					if (i == length - 1)
					{
						if (keyframe.time + DataManager.precision < 1f && (double)keyframe.time >= 0.5)
						{
							jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
							jsonobject3.AddField("key", keyframe.time);
							jsonobject3.AddField("value", keyframe.value * curveMultiplier * convert);
							jsonobject2.Add(jsonobject3);
						}
						jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject3.AddField("key", 1f);
						jsonobject3.AddField("value", keyframe.value * curveMultiplier * convert);
						jsonobject2.Add(jsonobject3);
					}
				}
			}
			jsonobject.AddField(str2, jsonobject2);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00014914 File Offset: 0x00012B14
		public static void saveParticleFrameData(Gradient gradient, JSONObject obj, string str)
		{
			if (gradient == null)
			{
				return;
			}
			GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
			GradientColorKey[] colorKeys = gradient.colorKeys;
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			obj.AddField(str, jsonobject);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
			if (alphaKeys != null && alphaKeys.Length != 0)
			{
				int num = alphaKeys.Length;
				for (int i = 0; i < num; i++)
				{
					GradientAlphaKey gradientAlphaKey = alphaKeys[i];
					if (i == 0)
					{
						jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject3.AddField("key", 0f);
						jsonobject3.AddField("value", gradientAlphaKey.alpha);
						jsonobject2.Add(jsonobject3);
						if (gradientAlphaKey.time - DataManager.precision > 0f && (double)gradientAlphaKey.time < 0.5)
						{
							jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
							jsonobject3.AddField("key", gradientAlphaKey.time);
							jsonobject3.AddField("value", gradientAlphaKey.alpha);
							jsonobject2.Add(jsonobject3);
						}
					}
					if (i != 0 && i != num - 1)
					{
						jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject3.AddField("key", gradientAlphaKey.time);
						jsonobject3.AddField("value", gradientAlphaKey.alpha);
						jsonobject2.Add(jsonobject3);
					}
					if (i == num - 1)
					{
						if (gradientAlphaKey.time + DataManager.precision < 1f && (double)gradientAlphaKey.time >= 0.5)
						{
							jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
							jsonobject3.AddField("key", gradientAlphaKey.time);
							jsonobject3.AddField("value", gradientAlphaKey.alpha);
							jsonobject2.Add(jsonobject3);
						}
						jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject3.AddField("key", 1f);
						jsonobject3.AddField("value", gradientAlphaKey.alpha);
						jsonobject2.Add(jsonobject3);
					}
				}
			}
			jsonobject.AddField("alphas", jsonobject2);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
			if (colorKeys != null && colorKeys.Length != 0)
			{
				int num2 = colorKeys.Length;
				for (int j = 0; j < num2; j++)
				{
					GradientColorKey gradientColorKey = colorKeys[j];
					if (j == 0)
					{
						jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject5.AddField("key", 0f);
						jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
						jsonobject6.Add(gradientColorKey.color.r);
						jsonobject6.Add(gradientColorKey.color.g);
						jsonobject6.Add(gradientColorKey.color.b);
						jsonobject5.AddField("value", jsonobject6);
						jsonobject4.Add(jsonobject5);
						if (gradientColorKey.time - DataManager.precision > 0f && (double)gradientColorKey.time < 0.5)
						{
							jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
							jsonobject5.AddField("key", gradientColorKey.time);
							jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
							jsonobject6.Add(gradientColorKey.color.r);
							jsonobject6.Add(gradientColorKey.color.g);
							jsonobject6.Add(gradientColorKey.color.b);
							jsonobject5.AddField("value", jsonobject6);
							jsonobject4.Add(jsonobject5);
						}
					}
					if (j != 0 && j != num2 - 1)
					{
						jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject5.AddField("key", gradientColorKey.time);
						jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
						jsonobject6.Add(gradientColorKey.color.r);
						jsonobject6.Add(gradientColorKey.color.g);
						jsonobject6.Add(gradientColorKey.color.b);
						jsonobject5.AddField("value", jsonobject6);
						jsonobject4.Add(jsonobject5);
					}
					if (j == num2 - 1)
					{
						if (gradientColorKey.time + DataManager.precision < 1f && (double)gradientColorKey.time >= 0.5)
						{
							jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
							jsonobject5.AddField("key", gradientColorKey.time);
							jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
							jsonobject6.Add(gradientColorKey.color.r);
							jsonobject6.Add(gradientColorKey.color.g);
							jsonobject6.Add(gradientColorKey.color.b);
							jsonobject5.AddField("value", jsonobject6);
							jsonobject4.Add(jsonobject5);
						}
						jsonobject5 = new JSONObject(JSONObject.Type.OBJECT);
						jsonobject5.AddField("key", 1f);
						jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
						jsonobject6.Add(gradientColorKey.color.r);
						jsonobject6.Add(gradientColorKey.color.g);
						jsonobject6.Add(gradientColorKey.color.b);
						jsonobject5.AddField("value", jsonobject6);
						jsonobject4.Add(jsonobject5);
					}
				}
			}
			jsonobject.AddField("rgbs", jsonobject4);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00014E0C File Offset: 0x0001300C
		public static Texture2D getCubeMapTextureData(Color[] color)
		{
			int num = 0;
			int num3;
			int num2 = num3 = (int)Mathf.Sqrt((float)color.Length);
			Texture2D texture2D = new Texture2D(num3, num2);
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num3; j++)
				{
					texture2D.SetPixel(j, num2 - 1 - i, color[num++]);
				}
			}
			return texture2D;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00014E6C File Offset: 0x0001306C
		public static DataManager.VertexData getVertexData(Mesh mesh, int index)
		{
			DataManager.VertexData result;
			result.index = index;
			result.vertice = mesh.vertices[index];
			if (DataManager.VertexStructure[1] == 1)
			{
				result.normal = mesh.normals[index];
			}
			else
			{
				result.normal = default(Vector3);
			}
			if (DataManager.VertexStructure[2] == 1)
			{
				result.color = mesh.colors[index];
			}
			else
			{
				result.color = default(Color);
			}
			if (DataManager.VertexStructure[3] == 1)
			{
				result.uv = mesh.uv[index];
			}
			else
			{
				result.uv = default(Vector2);
			}
			if (DataManager.VertexStructure[4] == 1)
			{
				result.uv2 = mesh.uv2[index];
			}
			else
			{
				result.uv2 = default(Vector2);
			}
			if (DataManager.VertexStructure[5] == 1)
			{
				BoneWeight boneWeight = mesh.boneWeights[index];
				result.boneWeight.x = boneWeight.weight0;
				result.boneWeight.y = boneWeight.weight1;
				result.boneWeight.z = boneWeight.weight2;
				result.boneWeight.w = boneWeight.weight3;
				result.boneIndex.x = (float)boneWeight.boneIndex0;
				result.boneIndex.y = (float)boneWeight.boneIndex1;
				result.boneIndex.z = (float)boneWeight.boneIndex2;
				result.boneIndex.w = (float)boneWeight.boneIndex3;
			}
			else
			{
				result.boneWeight = default(Vector4);
				result.boneIndex = default(Vector4);
			}
			if (DataManager.VertexStructure[6] == 1)
			{
				result.tangent = mesh.tangents[index];
			}
			else
			{
				result.tangent = default(Vector4);
			}
			return result;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00015044 File Offset: 0x00013244
		public static bool isHasChildByType(GameObject gameObject, DataManager.ComponentType type, bool onlySon, bool isCheckParent)
		{
			GameObject gameObject2 = gameObject;
			if (isCheckParent)
			{
				gameObject2 = gameObject.transform.parent.gameObject;
			}
			List<GameObject> list = new List<GameObject>();
			DataManager.selectChildByType(gameObject2, type, list, onlySon);
			return list.Count > 0;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00015084 File Offset: 0x00013284
		public static void selectChildByType(GameObject gameObject, DataManager.ComponentType type, List<GameObject> selectGameObjects, bool onlySon)
		{
			if (gameObject.transform.childCount > 0)
			{
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
					if (DataManager.componentsOnGameObject(gameObject2).IndexOf(type) != -1)
					{
						selectGameObjects.Add(gameObject2);
					}
					if (!onlySon)
					{
						DataManager.selectChildByType(gameObject2, type, selectGameObjects, onlySon);
					}
				}
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000150EC File Offset: 0x000132EC
		public static GameObject selectParentbyType(GameObject gameObject, DataManager.ComponentType type)
		{
			if (gameObject.transform.parent == null)
			{
				return null;
			}
			GameObject gameObject2 = gameObject.transform.parent.gameObject;
			if (DataManager.componentsOnGameObject(gameObject2).IndexOf(type) != -1)
			{
				return gameObject2;
			}
			return DataManager.selectParentbyType(gameObject2, type);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00015138 File Offset: 0x00013338
		public static bool isHasLegalChild(GameObject gameObject)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
				if (DataManager.componentsOnGameObject(gameObject2).Count > 1 && gameObject2.activeInHierarchy)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00015188 File Offset: 0x00013388
		public static void checkChildHasLocalParticle(GameObject gameObject, bool isTopNode)
		{
			if (isTopNode)
			{
				DataManager.curNodeHasLocalParticleChild = false;
			}
			if (gameObject.transform.childCount > 0)
			{
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
					if (DataManager.componentsOnGameObject(gameObject2).IndexOf(DataManager.ComponentType.ParticleSystem) != -1 && gameObject2.GetComponent<ParticleSystem>().main.scalingMode.ToString() == "Local")
					{
						DataManager.curNodeHasLocalParticleChild = true;
					}
					DataManager.checkChildHasLocalParticle(gameObject2, false);
				}
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00015220 File Offset: 0x00013420
		public static void checkChildIsNotLegal(GameObject gameObject, bool isTopNode)
		{
			if (isTopNode)
			{
				DataManager.curNodeHasNotLegalChild = false;
			}
			if (DataManager.componentsOnGameObject(gameObject).Count <= 1)
			{
				DataManager.curNodeHasNotLegalChild = true;
			}
			if (gameObject.transform.childCount > 0)
			{
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
					if (DataManager.componentsOnGameObject(gameObject2).Count <= 1)
					{
						DataManager.curNodeHasNotLegalChild = true;
					}
					DataManager.checkChildIsNotLegal(gameObject2, false);
				}
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0001529C File Offset: 0x0001349C
		public static void checkChildIsLegal(GameObject gameObject, bool isTopNode)
		{
			if (isTopNode)
			{
				DataManager.curNodeHasLegalChild = false;
			}
			if (gameObject.transform.childCount > 0)
			{
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
					if (DataManager.componentsOnGameObject(gameObject2).Count >= 1)
					{
						DataManager.curNodeHasLegalChild = true;
					}
					DataManager.checkChildIsLegal(gameObject2, false);
				}
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00015304 File Offset: 0x00013504
		public static bool checkChildBoneIsLegal(Transform root, Transform bone, Transform skinBone, int count)
		{
			if (root == bone)
			{
				return true;
			}
			for (int i = 0; i < count; i++)
			{
				if (skinBone == root)
				{
					return false;
				}
				if (bone == skinBone)
				{
					return true;
				}
				skinBone = skinBone.parent;
			}
			return false;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00015348 File Offset: 0x00013548
		public static List<DataManager.ComponentType> componentsOnGameObject(GameObject gameObject)
		{
			List<DataManager.ComponentType> list = new List<DataManager.ComponentType>();
			Camera component = gameObject.GetComponent<Camera>();
			Light component2 = gameObject.GetComponent<Light>();
			MeshFilter component3 = gameObject.GetComponent<MeshFilter>();
			MeshRenderer component4 = gameObject.GetComponent<MeshRenderer>();
			SkinnedMeshRenderer component5 = gameObject.GetComponent<SkinnedMeshRenderer>();
			UnityEngine.Object component6 = gameObject.GetComponent<Animation>();
			Animator component7 = gameObject.GetComponent<Animator>();
			ParticleSystem component8 = gameObject.GetComponent<ParticleSystem>();
			Terrain component9 = gameObject.GetComponent<Terrain>();
			BoxCollider component10 = gameObject.GetComponent<BoxCollider>();
			SphereCollider component11 = gameObject.GetComponent<SphereCollider>();
			CapsuleCollider component12 = gameObject.GetComponent<CapsuleCollider>();
			MeshCollider component13 = gameObject.GetComponent<MeshCollider>();
			UnityEngine.Object component14 = gameObject.GetComponent<Rigidbody>();
			UnityEngine.Object component15 = gameObject.GetComponent<TrailRenderer>();
			UnityEngine.Object component16 = gameObject.GetComponent<LineRenderer>();
			list.Add(DataManager.ComponentType.Transform);
			if (component16 != null)
			{
				list.Add(DataManager.ComponentType.LineRenderer);
			}
			if (component15 != null)
			{
				list.Add(DataManager.ComponentType.TrailRenderer);
			}
			if (component14 != null)
			{
				list.Add(DataManager.ComponentType.Rigidbody3D);
			}
			else if (component10 != null || component11 != null || component12 != null || component13 != null)
			{
				list.Add(DataManager.ComponentType.PhysicsCollider);
			}
			if (component6 != null)
			{
				Debug.LogWarning("LayaAir3D Warning(Code:0000) : " + gameObject.name + " can't use Animation Componment!");
			}
			if (component7 != null)
			{
				list.Add(DataManager.ComponentType.Animator);
			}
			if (component != null)
			{
				list.Add(DataManager.ComponentType.Camera);
			}
			if (component2 != null)
			{
				if (component2.type ==(UnityEngine.LightType) 1)
				{
					list.Add(DataManager.ComponentType.DirectionalLight);
				}
				else if (component2.type == (UnityEngine.LightType)2)
				{
					list.Add(DataManager.ComponentType.PointLight);
				}
				else if (component2.type == null)
				{
					list.Add(DataManager.ComponentType.SpotLight);
				}
			}
			if (component3 != null)
			{
				if (component == null)
				{
					list.Add(DataManager.ComponentType.MeshFilter);
					if (component4 == null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " need a MeshRenderer ComponentType !");
					}
				}
				else if (component != null)
				{
					Debug.LogWarning("LayaAir3D : " + gameObject.name + " Camera and MeshFilter can't exist at the same time !");
				}
			}
			if (component4 != null)
			{
				if (component == null)
				{
					list.Add(DataManager.ComponentType.MeshRenderer);
					if (component3 == null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " need a meshFilter ComponentType !");
					}
				}
				else if (component != null)
				{
					Debug.LogWarning("LayaAir3D : " + gameObject.name + " Camera and MeshRenderer can't exist at the same time !");
				}
			}
			if (component5 != null)
			{
				if (component == null && component3 == null && component4 == null)
				{
					list.Add(DataManager.ComponentType.SkinnedMeshRenderer);
				}
				else
				{
					if (component != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " Camera and SkinnedMeshRenderer can't exist at the same time !");
					}
					if (component3 != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " MeshFilter and SkinnedMeshRenderer can't exist at the same time !");
					}
					if (component4 != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " MeshRenderer and SkinnedMeshRenderer can't exist at the same time !");
					}
				}
			}
			if (component8 != null)
			{
				if (component == null && component3 == null && component4 == null && component5 == null)
				{
					list.Add(DataManager.ComponentType.ParticleSystem);
				}
				else
				{
					if (component != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " Camera and ParticleSystem can't exist at the same time !");
					}
					if (component3 != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " MeshFilter and ParticleSystem can't exist at the same time !");
					}
					if (component4 != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " MeshRenderer and ParticleSystem can't exist at the same time !");
					}
					if (component5 != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " SkinnedMeshRenderer and ParticleSystem can't exist at the same time !");
					}
				}
			}
			if (component9 != null)
			{
				if (component == null && component3 == null && component4 == null && component5 == null && component8 == null)
				{
					list.Add(DataManager.ComponentType.Terrain);
				}
				else
				{
					if (component != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " Camera and Terrain can't exist at the same time !");
					}
					if (component3 != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " MeshFilter and Terrain can't exist at the same time !");
					}
					if (component4 != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " MeshRenderer and Terrain can't exist at the same time !");
					}
					if (component5 != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " SkinnedMeshRenderer and Terrain can't exist at the same time !");
					}
					if (component8 != null)
					{
						Debug.LogWarning("LayaAir3D : " + gameObject.name + " ParticleSystem and Terrain can't exist at the same time !");
					}
				}
			}
			return list;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000157E4 File Offset: 0x000139E4
		public static bool frameDataIsEquals(DataManager.FrameData f1, DataManager.FrameData f2)
		{
			Vector3 localPosition = f1.localPosition;
			Quaternion localRotation = f1.localRotation;
			Vector3 localScale = f1.localScale;
			Vector3 localPosition2 = f2.localPosition;
			Quaternion localRotation2 = f2.localRotation;
			Vector3 localScale2 = f2.localScale;
			return MathUtil.isSimilar(localPosition.x, localPosition2.x) && MathUtil.isSimilar(localPosition.y, localPosition2.y) && MathUtil.isSimilar(localPosition.z, localPosition2.z) && MathUtil.isSimilar(localRotation.x, localRotation2.x) && MathUtil.isSimilar(localRotation.y, localRotation2.y) && MathUtil.isSimilar(localRotation.z, localRotation2.z) && MathUtil.isSimilar(localRotation.w, localRotation2.w) && MathUtil.isSimilar(localScale.x, localScale2.x) && MathUtil.isSimilar(localScale.y, localScale2.y) && MathUtil.isSimilar(localScale.z, localScale2.z);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000158F0 File Offset: 0x00013AF0
		public static string cleanIllegalChar(string str, bool heightLevel)
		{
			str = str.Replace("<", "_");
			str = str.Replace(">", "_");
			str = str.Replace("\"", "_");
			str = str.Replace("|", "_");
			str = str.Replace("?", "_");
			str = str.Replace("*", "_");
			str = str.Replace("#", "_");
			if (heightLevel)
			{
				str = str.Replace("/", "_");
				str = str.Replace(":", "_");
			}
			return str;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000159A4 File Offset: 0x00013BA4
		public static bool findStrsInCurString(string curString, List<string> strs)
		{
			int num = curString.Length - 4;
			for (int i = 0; i < strs.Count; i++)
			{
				if (curString.IndexOf(strs[i]) == num)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000159E0 File Offset: 0x00013BE0
		public static void recodeLayaJSFile(string pathName)
		{
			FileStream fileStream = new FileStream(Application.dataPath + "/StreamingAssets/LayaDemo/LayaAir3DSample.js", FileMode.Create, FileAccess.ReadWrite);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.WriteLine("(function(global){");
			streamWriter.WriteLine("var Laya3D = global.Laya3D;");
			streamWriter.WriteLine("var Laya = global.Laya;");
			streamWriter.WriteLine("Laya3D.init(0, 0);");
			streamWriter.WriteLine("Laya.stage.scaleMode = Laya.Stage.SCALE_FULL;");
			streamWriter.WriteLine("Laya.stage.screenMode = Laya.Stage.SCREEN_NONE;");
			streamWriter.WriteLine("Laya.Stat.show();");
			if (DataManager.Type == 0)
			{
				streamWriter.WriteLine("Laya.Scene3D.load('res" + pathName + ".ls', Laya.Handler.create(null, function(scene){Laya.stage.addChild(scene); }));");
			}
			else
			{
				streamWriter.WriteLine("var scene = Laya.stage.addChild(new Laya.Scene3D());");
				streamWriter.WriteLine("Laya.Sprite3D.load('res" + pathName + ".lh', Laya.Handler.create(null, function(sprite){scene.addChild(sprite); }));");
			}
			streamWriter.WriteLine("})(this);");
			streamWriter.Close();
			fileStream.Close();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00015AB0 File Offset: 0x00013CB0
		public static long GetTimeStamp()
		{
			return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00015AE4 File Offset: 0x00013CE4
		public static void NormalsaveTexturefiles()
		{
			foreach (KeyValuePair<string, TextureInfo> keyValuePair in DataManager.textureInfo)
			{
				string path = keyValuePair.Value.Path;
				string savePath = keyValuePair.Value.SavePath;
				Texture2D texture = keyValuePair.Value.texture;
				TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
				textureImporter.isReadable = true;
				textureImporter.textureCompression = 0;
				AssetDatabase.ImportAsset(path);
				if (texture.format == (UnityEngine.TextureFormat)10 || texture.format == (UnityEngine.TextureFormat)12 || texture.format == (UnityEngine.TextureFormat)28 || texture.format == (UnityEngine.TextureFormat)29)
				{
					Debug.LogError("LayaAie: Texture " + textureImporter.assetPath + " can't Readable,maybe you should cancel  Override for PC,MAC&Linux Standalone  checkbox.");
				}
				else if (keyValuePair.Value.format == 0)
				{
					File.WriteAllBytes(savePath, texture.EncodeToJPG());
				}
				else
				{
					File.WriteAllBytes(savePath, texture.EncodeToPNG());
				}
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00015BEC File Offset: 0x00013DEC
		public static void IosTexturefiles()
		{
			GameObject gameObject = SceneManager.GetActiveScene().GetRootGameObjects()[0];
			if (gameObject.GetComponent<HTTPClient>() != null)
			{
				DataManager.httpclient = gameObject.GetComponent<HTTPClient>();
			}
			else
			{
				DataManager.httpclient = gameObject.AddComponent<HTTPClient>();
			}
			HTTPClient.extension = "pvr";
			foreach (KeyValuePair<string, TextureInfo> keyValuePair in DataManager.textureInfo)
			{
				string path = keyValuePair.Value.Path;
				string savePath = keyValuePair.Value.SavePath;
				HTTPClient.MipMap = keyValuePair.Value.MipMap;
				Texture2D texture = keyValuePair.Value.texture;
				int num = Mathf.Max(texture.width, texture.height);
				HTTPClient.OtherSetting = string.Concat(new string[]
				{
					" -m ",
					keyValuePair.Value.MipMap.ToString(),
					" -r ",
					num.ToString(),
					",",
					num.ToString(),
					" -q pvrtcfastest "
				});
				if (keyValuePair.Value.format == 0)
				{
					HTTPClient.format = "PVRTC1_4_RGB";
				}
				else
				{
					HTTPClient.format = "PVRTC1_4";
				}
				HTTPClient._texture = path;
				DataManager.httpclient.textureInfo(savePath);
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00015D5C File Offset: 0x00013F5C
		public static void Andriodfiles()
		{
			GameObject gameObject = SceneManager.GetActiveScene().GetRootGameObjects()[0];
			if (gameObject.GetComponent<HTTPClient>() != null)
			{
				DataManager.httpclient = gameObject.GetComponent<HTTPClient>();
			}
			else
			{
				DataManager.httpclient = gameObject.AddComponent<HTTPClient>();
			}
			HTTPClient.extension = "ktx";
			foreach (KeyValuePair<string, TextureInfo> keyValuePair in DataManager.textureInfo)
			{
				if (keyValuePair.Value.format == 0)
				{
					string path = keyValuePair.Value.Path;
					string savePath = keyValuePair.Value.SavePath;
					HTTPClient.format = "ETC1";
					HTTPClient._texture = path;
					HTTPClient.MipMap = keyValuePair.Value.MipMap;
					HTTPClient.OtherSetting = " -m " + keyValuePair.Value.MipMap.ToString() + "-q etcfast ";
					DataManager.httpclient.textureInfo(savePath);
				}
				else
				{
					string path2 = keyValuePair.Value.Path;
					string savePath2 = keyValuePair.Value.SavePath;
					Texture2D texture = keyValuePair.Value.texture;
					TextureImporter textureImporter = AssetImporter.GetAtPath(path2) as TextureImporter;
					textureImporter.isReadable = true;
					textureImporter.textureCompression = 0;
					AssetDatabase.ImportAsset(path2);
					if (keyValuePair.Value.format == 0)
					{
						File.WriteAllBytes(savePath2, texture.EncodeToJPG());
					}
					else
					{
						File.WriteAllBytes(savePath2, texture.EncodeToPNG());
					}
				}
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00015EE8 File Offset: 0x000140E8
		public static void lmCreate()
		{
			foreach (KeyValuePair<string, Mesh> keyValuePair in DataManager.lmInfo)
			{
				DataManager.saveLmFile(keyValuePair.Value, keyValuePair.Key);
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00015F48 File Offset: 0x00014148
		public static void SwitchToLayaShader()
		{
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			if (rootGameObjects.Length != 0)
			{
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					DataManager.SwitchToLayaShader(rootGameObjects[i].gameObject);
				}
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00015F84 File Offset: 0x00014184
		public static void SwitchToLayaShader(GameObject gameObject)
		{
			List<DataManager.ComponentType> list = DataManager.componentsOnGameObject(gameObject);
			Shader shader = Shader.Find("LayaAir3D/BlinnPhong");
			Shader shader2 = Shader.Find("LayaAir3D/ShurikenParticle");
			Shader shader3 = Shader.Find("LayaAir3D/Trail");
			if (list.IndexOf(DataManager.ComponentType.MeshRenderer) != -1)
			{
				foreach (Material material in gameObject.GetComponent<MeshRenderer>().sharedMaterials)
				{
					if (!(material == null))
					{
						material.shader = shader;
						DataManager.onChangeLayaBlinnPhong(material);
					}
				}
			}
			if (list.IndexOf(DataManager.ComponentType.SkinnedMeshRenderer) != -1)
			{
				foreach (Material material2 in gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials)
				{
					if (!(material2 == null))
					{
						material2.shader = shader;
						DataManager.onChangeLayaBlinnPhong(material2);
					}
				}
			}
			if (list.IndexOf(DataManager.ComponentType.ParticleSystem) != -1)
			{
				Material sharedMaterial = gameObject.GetComponent<Renderer>().sharedMaterial;
				if (sharedMaterial != null)
				{
					sharedMaterial.shader = shader2;
					DataManager.onChangeLayaParticle(sharedMaterial);
				}
			}
			if (list.IndexOf(DataManager.ComponentType.TrailRenderer) != -1)
			{
				Material sharedMaterial2 = gameObject.GetComponent<Renderer>().sharedMaterial;
				if (sharedMaterial2 != null)
				{
					sharedMaterial2.shader = shader3;
					DataManager.onChangeLayaParticle(sharedMaterial2);
				}
			}
			if (gameObject.transform.childCount > 0)
			{
				for (int k = 0; k < gameObject.transform.childCount; k++)
				{
					DataManager.SwitchToLayaShader(gameObject.transform.GetChild(k).gameObject);
				}
			}
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000160F0 File Offset: 0x000142F0
		public static void onChangeLayaBlinnPhong(Material material)
		{
			switch (material.GetInt("_Mode"))
			{
			case 0:
				material.SetInt("_AlphaTest", 0);
				material.SetInt("_AlphaBlend", 0);
				material.SetInt("_SrcBlend", 1);
				material.SetInt("_DstBlend", 0);
				material.SetInt("_ZWrite", 1);
				material.SetInt("_ZTest", 4);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("EnableAlphaCutoff");
				material.EnableKeyword("EnableLighting");
				material.renderQueue = 2000;
				return;
			case 1:
				material.SetInt("_AlphaTest", 1);
				material.SetInt("_AlphaBlend", 0);
				material.SetInt("_SrcBlend", 1);
				material.SetInt("_DstBlend", 0);
				material.SetInt("_ZWrite", 1);
				material.SetInt("_ZTest", 4);
				material.EnableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.EnableKeyword("EnableAlphaCutoff");
				material.EnableKeyword("EnableLighting");
				material.renderQueue = 2450;
				return;
			case 2:
				material.SetInt("_AlphaTest", 0);
				material.SetInt("_AlphaBlend", 1);
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 10);
				material.SetInt("_ZWrite", 0);
				material.SetInt("_ZTest", 4);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("EnableAlphaCutoff");
				material.EnableKeyword("EnableLighting");
				material.renderQueue = 3000;
				return;
			default:
				material.SetInt("_AlphaTest", 0);
				material.SetInt("_AlphaBlend", 0);
				material.SetInt("_SrcBlend", 1);
				material.SetInt("_DstBlend", 0);
				material.SetInt("_ZWrite", 1);
				material.SetInt("_ZTest", 4);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("EnableAlphaCutoff");
				material.EnableKeyword("EnableLighting");
				material.renderQueue = 2000;
				return;
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00016320 File Offset: 0x00014520
		public static void onChangeLayaParticle(Material material)
		{
			int @int = material.GetInt("_Mode");
			if (@int == 0)
			{
				material.SetInt("_AlphaTest", 0);
				material.SetInt("_AlphaBlend", 1);
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 1);
				material.SetInt("_ZWrite", 0);
				material.SetInt("_ZTest", 4);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.renderQueue = 3000;
				return;
			}
			if (@int != 1)
			{
				material.SetInt("_AlphaTest", 0);
				material.SetInt("_AlphaBlend", 1);
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 10);
				material.SetInt("_ZWrite", 0);
				material.SetInt("_ZTest", 4);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.renderQueue = 3000;
				return;
			}
			material.SetInt("_AlphaTest", 0);
			material.SetInt("_AlphaBlend", 1);
			material.SetInt("_SrcBlend", 5);
			material.SetInt("_DstBlend", 10);
			material.SetInt("_ZWrite", 0);
			material.SetInt("_ZTest", 4);
			material.DisableKeyword("_ALPHATEST_ON");
			material.EnableKeyword("_ALPHABLEND_ON");
			material.renderQueue = 3000;
		}

		// Token: 0x0400005C RID: 92
		private static List<Dictionary<GameObject, string>> layaAutoGameObjectsList = new List<Dictionary<GameObject, string>>();

		// Token: 0x0400005D RID: 93
		private static int LayaAutoGOListIndex = 0;

		// Token: 0x0400005E RID: 94
		public static GameObject pathFindGameObject;

		// Token: 0x0400005F RID: 95
		public static string VERSION = "2.0.0 beta";

		// Token: 0x04000060 RID: 96
		public static bool LayaAuto = false;

		// Token: 0x04000061 RID: 97
		public static int Type;

		// Token: 0x04000062 RID: 98
		public static bool IgnoreUV;

		// Token: 0x04000063 RID: 99
		public static bool IgnoreColor;

		// Token: 0x04000064 RID: 100
		public static bool IgnoreNormal;

		// Token: 0x04000065 RID: 101
		public static bool IgnoreTangent;

		// Token: 0x04000066 RID: 102
		public static bool ConvertNonPNGAndJPG;

		// Token: 0x04000067 RID: 103
		public static bool ConvertOriginPNG;

		// Token: 0x04000068 RID: 104
		public static bool ConvertOriginJPG;

		// Token: 0x04000069 RID: 105
		public static bool ConvertToPNG;

		// Token: 0x0400006A RID: 106
		public static bool ConvertToJPG;

		// Token: 0x0400006B RID: 107
		public static float ConvertQuality;

		// Token: 0x0400006C RID: 108
		public static bool ConvertTerrainToMesh;

		// Token: 0x0400006D RID: 109
		public static int TerrainToMeshResolution;

		// Token: 0x0400006E RID: 110
		public static bool IgnoreNotActiveGameObject;

		// Token: 0x0400006F RID: 111
		public static bool BatchMade;

		// Token: 0x04000070 RID: 112
		public static bool CustomizeDirectory;

		// Token: 0x04000071 RID: 113
		public static string CustomizeDirectoryName;

		// Token: 0x04000072 RID: 114
		public static string SAVEPATH;

		// Token: 0x04000073 RID: 115
		public static bool OptimizeMeshName;

		// Token: 0x04000074 RID: 116
		public static float ScaleFactor;

		// Token: 0x04000075 RID: 117
		public static bool Ios;

		// Token: 0x04000076 RID: 118
		public static bool Android;

		// Token: 0x04000077 RID: 119
		public static bool Conventional;

		// Token: 0x04000078 RID: 120
		public static int Platformindex;

		// Token: 0x04000079 RID: 121
		private static bool curNodeHasLegalChild;

		// Token: 0x0400007A RID: 122
		private static bool curNodeHasNotLegalChild;

		// Token: 0x0400007B RID: 123
		private static bool curNodeHasLocalParticleChild;

		// Token: 0x0400007C RID: 124
		private static int MaxBoneCount = 24;

		// Token: 0x0400007D RID: 125
		private static float precision = 0.01f;

		// Token: 0x0400007E RID: 126
		private static int[] VertexStructure = new int[7];

		// Token: 0x0400007F RID: 127
		private static string sceneName;

		// Token: 0x04000080 RID: 128
		private static List<string> ConvertOriginalTextureTypeList;

		// Token: 0x04000081 RID: 129
		private static int directionalLightMaxCount = 1;

		// Token: 0x04000082 RID: 130
		private static int directionalLightCurCount = 0;

		// Token: 0x04000083 RID: 131
		private static int pointLightMaxCount = 1;

		// Token: 0x04000084 RID: 132
		private static int pointLightCurCount = 0;

		// Token: 0x04000085 RID: 133
		private static int spotLightMaxCount = 1;

		// Token: 0x04000086 RID: 134
		private static int spotLightCurCount = 0;

		// Token: 0x04000087 RID: 135
		public static Dictionary<string, TextureInfo> textureInfo = new Dictionary<string, TextureInfo>();

		// Token: 0x04000088 RID: 136
		public static Dictionary<string, Mesh> lmInfo = new Dictionary<string, Mesh>();

		// Token: 0x04000089 RID: 137
		public static GameObject httpscrip;

		// Token: 0x0400008A RID: 138
		public static HTTPClient httpclient;

		// Token: 0x0400008B RID: 139
		private static string LSVersion = "LAYASCENE3D:01";

		// Token: 0x0400008C RID: 140
		private static string LHVersion = "LAYAHIERARCHY:01";

		// Token: 0x0400008D RID: 141
		private static string LmVersion = "LAYAMODEL:0400";

		// Token: 0x0400008E RID: 142
		private static string LmatVersion = "LAYAMATERIAL:02";

		// Token: 0x0400008F RID: 143
		private static string LaniVersion = "LAYAANIMATION:03";

		// Token: 0x02000019 RID: 25
		public enum ComponentType
		{
			// Token: 0x040000D5 RID: 213
			Transform,
			// Token: 0x040000D6 RID: 214
			Camera,
			// Token: 0x040000D7 RID: 215
			DirectionalLight,
			// Token: 0x040000D8 RID: 216
			PointLight,
			// Token: 0x040000D9 RID: 217
			SpotLight,
			// Token: 0x040000DA RID: 218
			MeshFilter,
			// Token: 0x040000DB RID: 219
			MeshRenderer,
			// Token: 0x040000DC RID: 220
			SkinnedMeshRenderer,
			// Token: 0x040000DD RID: 221
			Animator,
			// Token: 0x040000DE RID: 222
			ParticleSystem,
			// Token: 0x040000DF RID: 223
			Terrain,
			// Token: 0x040000E0 RID: 224
			PhysicsCollider,
			// Token: 0x040000E1 RID: 225
			Rigidbody3D,
			// Token: 0x040000E2 RID: 226
			TrailRenderer,
			// Token: 0x040000E3 RID: 227
			LineRenderer
		}

		// Token: 0x0200001A RID: 26
		public struct FrameData
		{
			// Token: 0x040000E4 RID: 228
			public Vector3 localPosition;

			// Token: 0x040000E5 RID: 229
			public Quaternion localRotation;

			// Token: 0x040000E6 RID: 230
			public Vector3 localScale;
		}

		// Token: 0x0200001B RID: 27
		public struct VertexData
		{
			// Token: 0x040000E7 RID: 231
			public int index;

			// Token: 0x040000E8 RID: 232
			public Vector3 vertice;

			// Token: 0x040000E9 RID: 233
			public Vector3 normal;

			// Token: 0x040000EA RID: 234
			public Color color;

			// Token: 0x040000EB RID: 235
			public Vector2 uv;

			// Token: 0x040000EC RID: 236
			public Vector2 uv2;

			// Token: 0x040000ED RID: 237
			public Vector4 boneWeight;

			// Token: 0x040000EE RID: 238
			public Vector4 boneIndex;

			// Token: 0x040000EF RID: 239
			public Vector4 tangent;
		}

		// Token: 0x0200001C RID: 28
		public struct TerrainVertexData
		{
			// Token: 0x040000F0 RID: 240
			public Vector3 vertice;

			// Token: 0x040000F1 RID: 241
			public Vector3 normal;

			// Token: 0x040000F2 RID: 242
			public Vector2 uv;
		}

		// Token: 0x0200001D RID: 29
		public struct AniNodeFrameData
		{
			// Token: 0x040000F3 RID: 243
			public ushort startTimeIndex;

			// Token: 0x040000F4 RID: 244
			public List<float> inTangentNumbers;

			// Token: 0x040000F5 RID: 245
			public List<float> outTangentNumbers;

			// Token: 0x040000F6 RID: 246
			public List<float> valueNumbers;
		}

		// Token: 0x0200001E RID: 30
		public struct AniNodeData
		{
			// Token: 0x040000F7 RID: 247
			public byte type;

			// Token: 0x040000F8 RID: 248
			public ushort pathLength;

			// Token: 0x040000F9 RID: 249
			public List<ushort> pathIndex;

			// Token: 0x040000FA RID: 250
			public ushort conpomentTypeIndex;

			// Token: 0x040000FB RID: 251
			public ushort propertyNameLength;

			// Token: 0x040000FC RID: 252
			public List<ushort> propertyNameIndex;

			// Token: 0x040000FD RID: 253
			public ushort keyFrameCount;

			// Token: 0x040000FE RID: 254
			public List<DataManager.AniNodeFrameData> aniNodeFrameDatas;
		}

		// Token: 0x0200001F RID: 31
		public struct CustomAnimationCurve
		{
			// Token: 0x040000FF RID: 255
			public Keyframe[] keys;
		}

		// Token: 0x02000020 RID: 32
		public struct CustomAnimationClipCurveData
		{
			// Token: 0x04000100 RID: 256
			public DataManager.CustomAnimationCurve curve;

			// Token: 0x04000101 RID: 257
			public string path;

			// Token: 0x04000102 RID: 258
			public string propertyName;

			// Token: 0x04000103 RID: 259
			public Type type;
		}
	}
}
