using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class LayaTerrainExporter
{
	// Token: 0x0600000E RID: 14 RVA: 0x00002BEC File Offset: 0x00000DEC
	public static void ExportAllTerrian(string savePath, JSONObject obj)
	{
		LayaTerrainExporter.m_saveLocation = savePath + "/terrain";
		foreach (Terrain terrain in Terrain.activeTerrains)
		{
			LayaTerrainExporter.Clean();
			LayaTerrainExporter.m_terrain = terrain;
			obj.AddField("dataPath", "terrain/" + LayaTerrainExporter.m_terrain.name.ToLower() + ".lt");
			LayaTerrainExporter.ExportTerrain();
		}
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002C58 File Offset: 0x00000E58
	public static void saveLightMapData(JSONObject obj)
	{
		if (LayaTerrainExporter.m_terrain != null && LayaTerrainExporter.m_terrain.lightmapIndex > -1)
		{
			obj.AddField("lightmapIndex", LayaTerrainExporter.m_terrain.lightmapIndex);
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			obj.AddField("lightmapScaleOffset", jsonobject);
			jsonobject.Add(LayaTerrainExporter.m_terrain.lightmapScaleOffset.x);
			jsonobject.Add(LayaTerrainExporter.m_terrain.lightmapScaleOffset.y);
			jsonobject.Add(LayaTerrainExporter.m_terrain.lightmapScaleOffset.z);
			jsonobject.Add(-LayaTerrainExporter.m_terrain.lightmapScaleOffset.w);
		}
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002D00 File Offset: 0x00000F00
	private static void ExportTerrain()
	{
		if (!Directory.Exists(LayaTerrainExporter.m_saveLocation))
		{
			Directory.CreateDirectory(LayaTerrainExporter.m_saveLocation);
		}
		if (LayaTerrainExporter.m_terrain.terrainData == null)
		{
			Debug.LogWarning("LayaAir3D : " + LayaTerrainExporter.m_terrain.name + "'s TerrainData can't find!");
			return;
		}
		JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
		jsonobject.AddField("version", "1.0");
		jsonobject.AddField("cameraCoordinateInverse", LayaTerrainExporter.cameraCoordinateInverse);
		float num = LayaTerrainExporter.m_terrain.terrainData.size.x / (float)(LayaTerrainExporter.m_terrain.terrainData.heightmapWidth - 1);
		jsonobject.AddField("gridSize", num);
		if ((LayaTerrainExporter.m_terrain.terrainData.heightmapWidth - 1) % LayaTerrainExporter.CHUNK_GRID_NUM != 0)
		{
			Debug.LogError("高度图的宽减去一必须是" + LayaTerrainExporter.CHUNK_GRID_NUM + "的倍数");
			return;
		}
		if ((LayaTerrainExporter.m_terrain.terrainData.heightmapHeight - 1) % LayaTerrainExporter.CHUNK_GRID_NUM != 0)
		{
			Debug.LogError("高度图的高减去一必须是" + LayaTerrainExporter.CHUNK_GRID_NUM + "的倍数");
			return;
		}
		int num2 = (LayaTerrainExporter.m_terrain.terrainData.heightmapWidth - 1) / LayaTerrainExporter.CHUNK_GRID_NUM;
		int num3 = (LayaTerrainExporter.m_terrain.terrainData.heightmapHeight - 1) / LayaTerrainExporter.CHUNK_GRID_NUM;
		jsonobject.AddField("chunkNumX", num2);
		jsonobject.AddField("chunkNumZ", num3);
		JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
		jsonobject2.AddField("numX", LayaTerrainExporter.m_terrain.terrainData.heightmapWidth);
		jsonobject2.AddField("numZ", LayaTerrainExporter.m_terrain.terrainData.heightmapHeight);
		jsonobject2.AddField("bitType", 16);
		jsonobject2.AddField("value", LayaTerrainExporter.m_terrain.terrainData.size.y);
		jsonobject2.AddField("url", LayaTerrainExporter.m_terrain.name.ToLower() + "_heightmap.thdata");
		jsonobject.AddField("heightData", jsonobject2);
		JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
		jsonobject.AddField("material", jsonobject3);
		JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
		jsonobject4.Add(0f);
		jsonobject4.Add(0f);
		jsonobject4.Add(0f);
		jsonobject3.AddField("ambient", jsonobject4);
		JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
		jsonobject5.Add(1f);
		jsonobject5.Add(1f);
		jsonobject5.Add(1f);
		jsonobject3.AddField("diffuse", jsonobject5);
		JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
		jsonobject6.Add(0.2f);
		jsonobject6.Add(0.2f);
		jsonobject6.Add(0.2f);
		jsonobject6.Add(32f);
		jsonobject3.AddField("specular", jsonobject6);
		JSONObject jsonobject7 = new JSONObject(JSONObject.Type.ARRAY);
		jsonobject.AddField("detailTexture", jsonobject7);
		int num4 = LayaTerrainExporter.m_terrain.terrainData.splatPrototypes.Length;
		for (int i = 0; i < num4; i++)
		{
			JSONObject jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
			SplatPrototype splatPrototype = LayaTerrainExporter.m_terrain.terrainData.splatPrototypes[i];
			jsonobject8.AddField("diffuse", splatPrototype.texture.name.ToLower() + ".jpg");
			if (splatPrototype.normalMap != null)
			{
				jsonobject8.AddField("normal", splatPrototype.normalMap.name.ToLower() + ".jpg");
			}
			JSONObject jsonobject9 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject9.Add(splatPrototype.tileSize.x / num);
			jsonobject9.Add(splatPrototype.tileSize.y / num);
			jsonobject8.AddField("scale", jsonobject9);
			JSONObject jsonobject10 = new JSONObject(JSONObject.Type.ARRAY);
			jsonobject10.Add(splatPrototype.tileOffset.x);
			jsonobject10.Add(splatPrototype.tileOffset.y);
			jsonobject8.AddField("offset", jsonobject10);
			jsonobject7.Add(jsonobject8);
		}
		LayaTerrainExporter.m_chunkInfoNode = new JSONObject(JSONObject.Type.ARRAY);
		jsonobject.AddField("chunkInfo", LayaTerrainExporter.m_chunkInfoNode);
		float[,] heightsData = LayaTerrainExporter.GetHeightsData();
		Texture2D texture2D = LayaTerrainExporter.ExportNormal(LayaTerrainExporter.calcNormalOfTriangle(heightsData, num2 * LayaTerrainExporter.LEAF_GRID_NUM * num3 * LayaTerrainExporter.LEAF_GRID_NUM * 2, num));
		JSONObject jsonobject11 = new JSONObject(JSONObject.Type.ARRAY);
		jsonobject11.Add(texture2D.name.ToLower() + ".png");
		jsonobject.AddField("normalMap", jsonobject11);
		int num5 = 0;
		int num6 = 0;
		TextureFormat format = 0;
		for (int j = 0; j < num3; j++)
		{
			for (int k = 0; k < num2; k++)
			{
				num4 = LayaTerrainExporter.m_terrain.terrainData.alphamapTextures.Length;
				LayaTerrainExporter.m_chunkNode = new JSONObject(JSONObject.Type.OBJECT);
				LayaTerrainExporter.m_alphamapArrayNode = new JSONObject(JSONObject.Type.ARRAY);
				LayaTerrainExporter.m_detailIDArrayNode = new JSONObject(JSONObject.Type.ARRAY);
				LayaTerrainExporter.m_splatIndex = 0;
				for (int l = 0; l < num4; l++)
				{
					Texture2D texture2D2 = LayaTerrainExporter.m_terrain.terrainData.alphamapTextures[l];
					if (texture2D2.width % num2 != 0)
					{
						Debug.LogError("Control Texture(alpha map) 的宽必须是" + num2 + "的倍数");
						return;
					}
					if (texture2D2.height % num3 != 0)
					{
						Debug.LogError("Control Texture(alpha map) 的高必须是" + num3 + "的倍数");
						return;
					}
					num5 = texture2D2.width / num2;
					num6 = texture2D2.height / num3;
					int num7 = k * num5;
					int num8 = texture2D2.height - (j + 1) * num6;
					Color[] pixels = texture2D2.GetPixels(num7, num8, num5, num6);
					format = texture2D2.format;
					LayaTerrainExporter.MergeAlphaMap(string.Concat(new object[]
					{
						LayaTerrainExporter.m_terrain.name.ToLower(),
						"_splatalpha{0}_",
						k,
						"_",
						j,
						LayaTerrainExporter.m_debug ? ".jpg" : ".png"
					}), format, num5, num6, pixels);
				}
				LayaTerrainExporter.AfterMergeAlphaMap(string.Concat(new object[]
				{
					LayaTerrainExporter.m_terrain.name.ToLower(),
					"_splatalpha{0}_",
					k,
					"_",
					j,
					LayaTerrainExporter.m_debug ? ".jpg" : ".png"
				}), format, num5, num6);
				LayaTerrainExporter.m_chunkNode.AddField("normalMap", 0);
			}
			LayaTerrainExporter.ExportSplat();
			LayaTerrainExporter.ExportHeightmap16(heightsData);
			LayaTerrainExporter.ExportAlphamap();
		}
		JSONObject jsonobject12 = new JSONObject(JSONObject.Type.ARRAY);
		for (int m = 0; m < LayaTerrainExporter.m_alphamapDataList.Count; m++)
		{
			jsonobject12.Add(LayaTerrainExporter.m_alphamapDataList[m].Key.ToLower());
		}
		jsonobject.AddField("alphaMap", jsonobject12);
		LayaTerrainExporter.saveData(jsonobject);
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00003404 File Offset: 0x00001604
	private static void Clean()
	{
		LayaTerrainExporter.m_splatIndex = 0;
		LayaTerrainExporter.m_alphaIndex = 0;
		LayaTerrainExporter.m_channelIndex = 0;
		LayaTerrainExporter.m_alphaBlock = null;
		LayaTerrainExporter.m_detailID = null;
		LayaTerrainExporter.m_alphamapDataList.Clear();
		LayaTerrainExporter.m_terrain = null;
		LayaTerrainExporter.m_alphamapArrayNode = null;
		LayaTerrainExporter.m_detailIDArrayNode = null;
		LayaTerrainExporter.m_chunkInfoNode = null;
		LayaTerrainExporter.m_alphaBlockName = "";
	}

	// Token: 0x06000012 RID: 18 RVA: 0x0000345C File Offset: 0x0000165C
	private static void SaveAlphaMap(string fileName, TextureFormat format, int width, int height)
	{
		if (LayaTerrainExporter.m_alphaIndex <= 0 || !LayaTerrainExporter.IsAlphaMapEmpty(LayaTerrainExporter.m_alphaBlock))
		{
			int num = LayaTerrainExporter.GetAlphaMapCached(LayaTerrainExporter.m_alphaBlock, LayaTerrainExporter.m_alphamapDataList);
			if (num == -1)
			{
				Texture2D texture2D = new Texture2D(width, height, format, false);
				Color[] array = new Color[LayaTerrainExporter.m_alphaBlock.Length];
				for (int i = 0; i < LayaTerrainExporter.m_alphaBlock.Length; i++)
				{
					float r = LayaTerrainExporter.m_alphaBlock[i].r;
					float g = LayaTerrainExporter.m_alphaBlock[i].g;
					float b = LayaTerrainExporter.m_alphaBlock[i].b;
					float a = LayaTerrainExporter.m_alphaBlock[i].a;
					array[i].r = g;
					array[i].g = b;
					array[i].b = a;
					float num2 = r + g + b + a;
					array[i].a = ((num2 > 1f) ? 1f : num2);
				}
				texture2D.SetPixels(array);
				texture2D.Apply();
				string text = string.Format(fileName, LayaTerrainExporter.m_alphaIndex).ToLower();
				File.WriteAllBytes(LayaTerrainExporter.m_saveLocation + "/" + text, LayaTerrainExporter.m_debug ? texture2D.EncodeToJPG() : texture2D.EncodeToPNG());
				LayaTerrainExporter.m_alphamapDataList.Add(new KeyValuePair<string, Color[]>(text, LayaTerrainExporter.m_alphaBlock));
				num = LayaTerrainExporter.m_alphamapDataList.Count - 1;
			}
			LayaTerrainExporter.m_alphamapArrayNode.Add(num);
			LayaTerrainExporter.m_detailIDArrayNode.Add(LayaTerrainExporter.m_detailID);
		}
		LayaTerrainExporter.m_alphaIndex++;
		LayaTerrainExporter.m_alphaBlock = null;
		LayaTerrainExporter.m_channelIndex = 0;
	}

	// Token: 0x06000013 RID: 19 RVA: 0x0000360C File Offset: 0x0000180C
	private static void AfterMergeAlphaMap(string fileName, TextureFormat format, int width, int height)
	{
		if (LayaTerrainExporter.m_alphaBlock != null)
		{
			LayaTerrainExporter.SaveAlphaMap(fileName, format, width, height);
		}
		LayaTerrainExporter.m_alphaIndex = 0;
		LayaTerrainExporter.m_alphaBlockName = "";
		LayaTerrainExporter.m_chunkNode.AddField("alphaMap", LayaTerrainExporter.m_alphamapArrayNode);
		LayaTerrainExporter.m_chunkNode.AddField("detailID", LayaTerrainExporter.m_detailIDArrayNode);
		LayaTerrainExporter.m_chunkInfoNode.Add(LayaTerrainExporter.m_chunkNode);
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00003670 File Offset: 0x00001870
	private static void MergeAlphaMap(string fileName, TextureFormat format, int width, int height, Color[] color)
	{
		for (int i = 0; i < 4; i++)
		{
			if (LayaTerrainExporter.m_alphaBlock == null)
			{
				LayaTerrainExporter.m_detailID = new JSONObject(JSONObject.Type.ARRAY);
				LayaTerrainExporter.m_alphaBlockName = fileName;
				LayaTerrainExporter.m_alphaBlock = new Color[color.Length];
				for (int j = 0; j < LayaTerrainExporter.m_alphaBlock.Length; j++)
				{
					LayaTerrainExporter.m_alphaBlock[j] = new Color(0f, 0f, 0f, 0f);
				}
			}
			bool flag = true;
			for (int k = 0; k < color.Length; k++)
			{
				float num = color[k][i];
				LayaTerrainExporter.m_alphaBlock[k][LayaTerrainExporter.m_channelIndex] = num;
				if (num != 0f)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				LayaTerrainExporter.m_detailID.Add(LayaTerrainExporter.m_splatIndex);
				LayaTerrainExporter.m_channelIndex++;
				if (LayaTerrainExporter.m_channelIndex > 3)
				{
					LayaTerrainExporter.SaveAlphaMap(LayaTerrainExporter.m_alphaBlockName, format, width, height);
				}
			}
			LayaTerrainExporter.m_splatIndex++;
		}
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00003770 File Offset: 0x00001970
	private static bool IsAlphaMapEmpty(Color[] color)
	{
		for (int i = 0; i < color.Length; i++)
		{
			if (color[i] != Color.clear)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x000037A4 File Offset: 0x000019A4
	private static int GetAlphaMapCached(Color[] color, List<KeyValuePair<string, Color[]>> alphamapDataList)
	{
		for (int i = 0; i < alphamapDataList.Count; i++)
		{
			bool flag = true;
			for (int j = 0; j < color.Length; j++)
			{
				if (color[j] != alphamapDataList[i].Value[j])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00003800 File Offset: 0x00001A00
	private static void ExportHeightmap16(float[,] heightsData)
	{
		byte[] array = new byte[LayaTerrainExporter.m_terrain.terrainData.heightmapWidth * LayaTerrainExporter.m_terrain.terrainData.heightmapHeight * 2];
		float num = 65536f;
		int num2 = 0;
		for (int i = 0; i < LayaTerrainExporter.m_terrain.terrainData.heightmapHeight; i++)
		{
			for (int j = 0; j < LayaTerrainExporter.m_terrain.terrainData.heightmapWidth; j++)
			{
				byte[] bytes = BitConverter.GetBytes((ushort)Mathf.Clamp(Mathf.RoundToInt(heightsData[i, j] * num), 0, 65535));
				array[num2 * 2] = bytes[0];
				array[num2 * 2 + 1] = bytes[1];
				num2++;
			}
		}
		FileStream fileStream = new FileStream(LayaTerrainExporter.m_saveLocation + "/" + LayaTerrainExporter.m_terrain.name.ToLower() + "_heightmap.thdata", FileMode.Create);
		fileStream.Write(array, 0, array.Length);
		fileStream.Close();
	}

	// Token: 0x06000018 RID: 24 RVA: 0x000038E8 File Offset: 0x00001AE8
	private static void ExportAlphamap()
	{
		int num = LayaTerrainExporter.m_terrain.terrainData.alphamapTextures.Length;
		for (int i = 0; i < num; i++)
		{
			Texture2D texture2D = LayaTerrainExporter.m_terrain.terrainData.alphamapTextures[i];
			FileStream fileStream = File.Open(LayaTerrainExporter.m_saveLocation + "/" + texture2D.name.ToLower() + ".png", FileMode.Create);
			new BinaryWriter(fileStream).Write(texture2D.EncodeToPNG());
			fileStream.Close();
		}
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00003960 File Offset: 0x00001B60
	private static float[,] GetHeightsData()
	{
		float[,] heights = LayaTerrainExporter.m_terrain.terrainData.GetHeights(0, 0, LayaTerrainExporter.m_terrain.terrainData.heightmapWidth, LayaTerrainExporter.m_terrain.terrainData.heightmapHeight);
		float[,] array = new float[LayaTerrainExporter.m_terrain.terrainData.heightmapWidth, LayaTerrainExporter.m_terrain.terrainData.heightmapHeight];
		for (int i = LayaTerrainExporter.m_terrain.terrainData.heightmapHeight - 1; i >= 0; i--)
		{
			for (int j = 0; j < LayaTerrainExporter.m_terrain.terrainData.heightmapWidth; j++)
			{
				array[LayaTerrainExporter.m_terrain.terrainData.heightmapHeight - 1 - i, j] = heights[i, j];
			}
		}
		return array;
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00003A1C File Offset: 0x00001C1C
	private static void ExportSplat()
	{
		int num = LayaTerrainExporter.m_terrain.terrainData.splatPrototypes.Length;
		for (int i = 0; i < num; i++)
		{
			SplatPrototype splatPrototype = LayaTerrainExporter.m_terrain.terrainData.splatPrototypes[i];
			Texture2D texture2D = splatPrototype.texture;
			string assetPath = AssetDatabase.GetAssetPath(texture2D.GetInstanceID());
			TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			textureImporter.isReadable = true;
			textureImporter.textureCompression = 0;
			AssetDatabase.ImportAsset(assetPath);
			FileStream fileStream = File.Open(LayaTerrainExporter.m_saveLocation + "/" + texture2D.name.ToLower() + ".jpg", FileMode.Create);
			new BinaryWriter(fileStream).Write(texture2D.EncodeToJPG());
			fileStream.Close();
			if (splatPrototype.normalMap != null)
			{
				texture2D = splatPrototype.normalMap;
				string assetPath2 = AssetDatabase.GetAssetPath(texture2D.GetInstanceID());
				TextureImporter textureImporter2 = AssetImporter.GetAtPath(assetPath2) as TextureImporter;
				textureImporter2.isReadable = true;
				textureImporter2.textureCompression = 0;
				AssetDatabase.ImportAsset(assetPath2);
				FileStream fileStream2 = File.Open(LayaTerrainExporter.m_saveLocation + "/" + texture2D.name.ToLower() + ".jpg", FileMode.Create);
				new BinaryWriter(fileStream2).Write(texture2D.EncodeToJPG());
				fileStream2.Close();
			}
		}
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00003B44 File Offset: 0x00001D44
	private static Vector3[] calcNormalOfTriangle(float[,] heightsData, int size, float girdSize)
	{
		Vector3[] array = new Vector3[(LayaTerrainExporter.m_terrain.terrainData.heightmapWidth - 1) * (LayaTerrainExporter.m_terrain.terrainData.heightmapHeight - 1) * 2];
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		float[] array2 = new float[LayaTerrainExporter.m_terrain.terrainData.heightmapWidth * LayaTerrainExporter.m_terrain.terrainData.heightmapHeight];
		float num = 65536f;
		int num2 = 0;
		for (int i = 0; i < LayaTerrainExporter.m_terrain.terrainData.heightmapHeight; i++)
		{
			for (int j = 0; j < LayaTerrainExporter.m_terrain.terrainData.heightmapWidth; j++)
			{
				ushort num3 = (ushort)Mathf.Clamp(Mathf.RoundToInt(heightsData[i, j] * num), 0, 65535);
				array2[num2] = (float)num3 * 1f / 32766f * LayaTerrainExporter.m_terrain.terrainData.size.y * 0.5f;
				num2++;
			}
		}
		int heightmapWidth = LayaTerrainExporter.m_terrain.terrainData.heightmapWidth;
		int heightmapHeight = LayaTerrainExporter.m_terrain.terrainData.heightmapHeight;
		int num4 = 0;
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x.SetTRS(Vector3.zero, Quaternion.AngleAxis(180f, Vector3.up), Vector3.one);
		Matrix4x4 matrix4x2 = default(Matrix4x4);
		matrix4x2.SetTRS(new Vector3(0f, 0f, LayaTerrainExporter.m_terrain.terrainData.size.y), Quaternion.identity, Vector3.one);
		Matrix4x4 matrix4x3 = matrix4x2 * matrix4x;
		for (int k = 0; k < heightmapHeight - 1; k++)
		{
			for (int l = 0; l < heightmapWidth - 1; l++)
			{
				vector.x = (float)l * girdSize;
				vector.y = array2[(k + 1) * heightmapWidth + l];
				vector.z = (float)(k + 1) * girdSize;
				if (LayaTerrainExporter.cameraCoordinateInverse)
				{
					vector = matrix4x3.MultiplyPoint(vector);
				}
				vector2.x = (float)(l + 1) * girdSize;
				vector2.y = array2[(k + 1) * heightmapWidth + l + 1];
				vector2.z = (float)(k + 1) * girdSize;
				if (LayaTerrainExporter.cameraCoordinateInverse)
				{
					vector2 = matrix4x3.MultiplyPoint(vector2);
				}
				vector3.x = (float)l * girdSize;
				vector3.y = array2[k * heightmapWidth + l];
				vector3.z = (float)k * girdSize;
				if (LayaTerrainExporter.cameraCoordinateInverse)
				{
					vector3 = matrix4x3.MultiplyPoint(vector3);
				}
				vector4.x = (float)(l + 1) * girdSize;
				vector4.y = array2[k * heightmapWidth + l + 1];
				vector4.z = (float)k * girdSize;
				if (LayaTerrainExporter.cameraCoordinateInverse)
				{
					vector4 = matrix4x3.MultiplyPoint(vector4);
				}
				Vector3 vector5 = Vector3.Cross(vector - vector3, vector4 - vector3);
				vector5.Normalize();
				array[num4] = vector5;
				num4++;
				Vector3 vector6 = Vector3.Cross(vector4 - vector2, vector - vector2);
				vector6.Normalize();
				array[num4] = vector6;
				num4++;
			}
		}
		return array;
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00003E7C File Offset: 0x0000207C
	private static Vector3 calcVertextNorml1(int x, int z, Vector3[] normalTriangle, int terrainXNum, int terrainZNum)
	{
		int num = z - 1;
		int num2 = x - 1;
		int[] array = new int[]
		{
			(num * terrainXNum + num2) * 2 + 1,
			(z * terrainXNum + num2) * 2,
			(z * terrainXNum + num2) * 2 + 1,
			(z * terrainXNum + x) * 2,
			(num * terrainXNum + x) * 2 + 1,
			(num * terrainXNum + x) * 2
		};
		if (num < 0)
		{
			array[0] = (array[4] = (array[5] = -1));
		}
		if (z >= terrainZNum)
		{
			array[1] = -1;
			array[2] = -1;
			array[3] = -1;
		}
		if (num2 < 0)
		{
			array[0] = (array[1] = (array[2] = -1));
		}
		if (x >= terrainXNum)
		{
			array[3] = -1;
			array[4] = -1;
			array[5] = -1;
		}
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			int num7 = array[i];
			if (array[i] >= 0)
			{
				num3 += normalTriangle[num7].x;
				num4 += normalTriangle[num7].y;
				num5 += normalTriangle[num7].z;
				num6 += 1f;
			}
		}
		Vector3 result;
		result = new Vector3(num3 / num6, num4 / num6, num5 / num6);
		result.Normalize();
		return result;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00003FE0 File Offset: 0x000021E0
	private static Texture2D ExportNormal(Vector3[] normalTriangle)
	{
		Color[] array = new Color[LayaTerrainExporter.m_terrain.terrainData.heightmapWidth * LayaTerrainExporter.m_terrain.terrainData.heightmapHeight];
		int num = 0;
		for (int i = LayaTerrainExporter.m_terrain.terrainData.heightmapHeight - 1; i >= 0; i--)
		{
			for (int j = 0; j < LayaTerrainExporter.m_terrain.terrainData.heightmapWidth; j++)
			{
				Vector3 vector = LayaTerrainExporter.calcVertextNorml1(j, i, normalTriangle, LayaTerrainExporter.m_terrain.terrainData.heightmapWidth - 1, LayaTerrainExporter.m_terrain.terrainData.heightmapHeight - 1);
				vector.x = (vector.x + 1f) * 0.5f;
				vector.y = (vector.y + 1f) * 0.5f;
				vector.z = (vector.z + 1f) * 0.5f;
				array[num] = new Color(vector.x, vector.y, vector.z, 1f);
				num++;
			}
		}
		Texture2D texture2D = new Texture2D(LayaTerrainExporter.m_terrain.terrainData.heightmapWidth, LayaTerrainExporter.m_terrain.terrainData.heightmapHeight, (UnityEngine.TextureFormat)4, false);
		texture2D.SetPixels(array);
		texture2D.Apply();
		texture2D.name = LayaTerrainExporter.m_terrain.name.ToLower() + "_normalMap";
		File.WriteAllBytes(LayaTerrainExporter.m_saveLocation + "/" + texture2D.name + ".png", texture2D.EncodeToPNG());
		return texture2D;
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00004178 File Offset: 0x00002378
	public static void saveData(JSONObject node)
	{
		string value = node.Print(true);
		StreamWriter streamWriter = new StreamWriter(new FileStream(LayaTerrainExporter.m_saveLocation + "/" + LayaTerrainExporter.m_terrain.name.ToLower() + ".lt", FileMode.Create, FileAccess.Write));
		streamWriter.Write(value);
		streamWriter.Close();
	}

	// Token: 0x04000027 RID: 39
	private static int CHUNK_GRID_NUM = 64;

	// Token: 0x04000028 RID: 40
	private static int LEAF_GRID_NUM = 32;

	// Token: 0x04000029 RID: 41
	private static int m_splatIndex = 0;

	// Token: 0x0400002A RID: 42
	private static int m_alphaIndex = 0;

	// Token: 0x0400002B RID: 43
	private static int m_channelIndex = 0;

	// Token: 0x0400002C RID: 44
	private static Color[] m_alphaBlock = null;

	// Token: 0x0400002D RID: 45
	private static JSONObject m_detailID = null;

	// Token: 0x0400002E RID: 46
	private static List<KeyValuePair<string, Color[]>> m_alphamapDataList = new List<KeyValuePair<string, Color[]>>();

	// Token: 0x0400002F RID: 47
	private static Terrain m_terrain;

	// Token: 0x04000030 RID: 48
	private static string m_saveLocation;

	// Token: 0x04000031 RID: 49
	private static JSONObject m_alphamapArrayNode = null;

	// Token: 0x04000032 RID: 50
	private static JSONObject m_detailIDArrayNode = null;

	// Token: 0x04000033 RID: 51
	private static JSONObject m_chunkNode = null;

	// Token: 0x04000034 RID: 52
	private static JSONObject m_chunkInfoNode = null;

	// Token: 0x04000035 RID: 53
	private static string m_alphaBlockName = "";

	// Token: 0x04000036 RID: 54
	private static bool m_debug = false;

	// Token: 0x04000037 RID: 55
	private static bool cameraCoordinateInverse = true;
}
