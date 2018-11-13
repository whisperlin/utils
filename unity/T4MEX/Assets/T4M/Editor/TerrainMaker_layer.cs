using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public partial class TerrainMaker : EditorWindow
{
    float GetHighMean(short []highData)
    {
        float totalHigh = 0;
        for(int i=0;i<highData.Length;i++)
        {
            totalHigh += (float)highData[i];
        }

        return totalHigh / highData.Length;
    }

    float GetHighVariance(short[] highData, float mean, out float []highPresentData)
    {
        highPresentData = new float[highData.Length];
        float maxDiv = float.MinValue;
        for(int i=0;i<highData.Length;i++)
        {
            float high = (float)highData[i];
            float highDiv = (high - mean);
            highPresentData[i] = highDiv;

            float absHighDiv = Mathf.Abs(highDiv);
            if (absHighDiv > maxDiv)
                maxDiv = absHighDiv;
        }

        return maxDiv;
    }

    public Material Control_Material = null;
    public Material Target_Material = null;

    public int LayerUsed = 4;

    public struct PixelLayer
    {
        public float layer;
        public float weight;
    }

    string mPath = "../../../resource/FileStream/terrain.raw";

    public static Texture2DArray CreateTextureArray2D(Texture2D []layerTextures, string TexturePath)
    {
        int layerCount = layerTextures.Length;
        int layerTextureSize = layerTextures[0].width;
        string formatString = layerTextures[0].format.ToString();
        Texture2DArray textureArray = new Texture2DArray(layerTextureSize, layerTextureSize,
            layerCount, layerTextures[0].format, true);


        for (int i = 0; i < layerTextures.Length; i++)
        {
			//Debug.Log (layerTextures [i].width.ToString()  + "_" +layerTextures [i].height.ToString());
            Graphics.CopyTexture(layerTextures[i], 0, textureArray, i);
        }

        textureArray.filterMode = FilterMode.Bilinear;
        textureArray.wrapMode = TextureWrapMode.Repeat;
        //textureArray.Apply();
        
        AssetDatabase.CreateAsset(textureArray, TexturePath);
        return textureArray;
    }



    void CreateNormalMap()
    {
        int layerCount = LayerUsed * 2;

        Texture2D[] layerTextures = new Texture2D[layerCount];

        for (int i = 0; i < layerCount; i++)
        {
            if (i < LayerUsed)
            {
                string splat = "_Splat" + i;
                layerTextures[i] = Control_Material.GetTexture(splat) as Texture2D;
            }
            else
            {
                int iIndex = i + (LayerUsed == 3 ? 1 : 0);
                string splat = "_Splat" + iIndex;
                layerTextures[i] = Control_Material.GetTexture(splat) as Texture2D;
            }
        }

        for (int i = 0; i < layerCount; i++)
        {
            string src_path = (AssetDatabase.GetAssetPath(layerTextures[i]));
            if (src_path.EndsWith(".png"))
            {
                layerTextures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(src_path.Replace(".png", "_N.png"));
            }
        }

        Texture2D Controller1 = Control_Material.GetTexture("_Control") as Texture2D;
        string TexturePath = AssetDatabase.GetAssetPath(Controller1);
        string fileName = Path.GetFileName(TexturePath);
        string path = TexturePath.Replace(fileName, "TextureArray_normal.asset");
        Texture2DArray normalArray = CreateTextureArray2D(layerTextures, path);

        Target_Material.SetTexture("_Splat_Normals", normalArray);
    }

    void CreateLayerTexture()
    {
        int layerCount = LayerUsed * 2;
        Texture2D Controller1 = Control_Material.GetTexture("_Control") as Texture2D;
        Texture2D Controller2 = Control_Material.GetTexture("_Control2") as Texture2D;

        Texture2D[] layerTextures = new Texture2D[layerCount];

        for (int i = 0; i < layerCount; i++)
        {
            if (i < LayerUsed)
            {
                string splat = "_Splat" + i;
                layerTextures[i] = Control_Material.GetTexture(splat) as Texture2D;
            }
            else
            {
                int iIndex = i + (LayerUsed == 3 ? 1 : 0);
                string splat = "_Splat" + iIndex;
                layerTextures[i] = Control_Material.GetTexture(splat) as Texture2D;
            }
        }


        if (Controller1 != null && Controller2 != null)
        {
            PixelLayer[] pixel_layers = new PixelLayer[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                pixel_layers[i] = new PixelLayer();
            }

            Color[] pix_ctrl1 = Controller1.GetPixels();
            Color[] pix_ctrl2 = Controller2.GetPixels();

            Color[] IndexTexture = new Color[Controller1.width * Controller1.height];
            Color[] WeightTexture = new Color[Controller1.width * Controller1.height];

            Color[] IndexTexture1 = new Color[Controller1.width * Controller1.height];
            Color[] WeightTexture1 = new Color[Controller1.width * Controller1.height];

            int lastWeightMorethan20 = 0;

            for (int i = 0; i < pix_ctrl1.Length; i++)
            {
                float total = 0;
                for (int j = 0; j < layerCount; j++)
                {
                    pixel_layers[j].layer = (float)j;
                    if (j < LayerUsed)
                        pixel_layers[j].weight = pix_ctrl1[i][j % LayerUsed];
                    else
                        pixel_layers[j].weight = pix_ctrl2[i][j % LayerUsed];

                    total += pixel_layers[j].weight;
                }

                System.Array.Sort(pixel_layers, delegate (PixelLayer l1, PixelLayer l2)
                {
                    if (l1.weight > l2.weight)
                        return -1;
                    else if (l1.weight == l2.weight)
                        return 0;

                    return 1;
                });

                IndexTexture[i].r = pixel_layers[0].layer / 10.0f;
                IndexTexture[i].g = pixel_layers[1].layer / 10.0f;
                IndexTexture[i].b = pixel_layers[2].layer / 10.0f;
                IndexTexture[i].a = pixel_layers[3].layer / 10.0f;

                IndexTexture1[i].r = pixel_layers[4].layer / 10.0f;
                IndexTexture1[i].g = pixel_layers[5].layer / 10.0f;
                IndexTexture1[i].b = pixel_layers[6].layer / 10.0f;
                IndexTexture1[i].a = pixel_layers[7].layer / 10.0f;


                WeightTexture[i].r = pixel_layers[0].weight;
                WeightTexture[i].g = pixel_layers[1].weight;
                WeightTexture[i].b = pixel_layers[2].weight;
                WeightTexture[i].a = pixel_layers[3].weight;

                WeightTexture1[i].r = pixel_layers[4].weight;
                WeightTexture1[i].g = pixel_layers[5].weight;
                WeightTexture1[i].b = pixel_layers[6].weight;
                WeightTexture1[i].a = pixel_layers[7].weight;

                float present = WeightTexture1[i].r / total;
                if (present > 0.1)
                {
                    lastWeightMorethan20++;
                }
            }

            Debug.LogFormat("{0} larger then 20%", lastWeightMorethan20);

            Texture2D text_index = new Texture2D(Controller1.width, Controller1.height, TextureFormat.RGBA32, false);
            Texture2D text_weight = new Texture2D(Controller1.width, Controller1.height, TextureFormat.RGBA32, false);

            Texture2D text_index1 = new Texture2D(Controller1.width, Controller1.height, TextureFormat.RGBA32, false);
            Texture2D text_weight1 = new Texture2D(Controller1.width, Controller1.height, TextureFormat.RGBA32, false);

            text_index.SetPixels(IndexTexture);
            text_weight.SetPixels(WeightTexture);

            text_index1.SetPixels(IndexTexture1);
            text_weight1.SetPixels(WeightTexture1);

            text_index.Apply();
            text_weight.Apply();

            text_index1.Apply();
            text_weight1.Apply();

            byte[] text_index_Pix = text_index.EncodeToPNG();
            byte[] text_weight_Pix = text_weight.EncodeToPNG();

            string TexturePath = AssetDatabase.GetAssetPath(Controller1);
            string fileName = Path.GetFileName(TexturePath);

            string indexPath = TexturePath.Replace(fileName, "IndexTexture.png");
            File.WriteAllBytes(indexPath, text_index_Pix);

            string weightPath = TexturePath.Replace(fileName, "WeightTexture.png");
            File.WriteAllBytes(weightPath, text_weight_Pix);

            text_index_Pix = text_index1.EncodeToPNG();
            text_weight_Pix = text_weight1.EncodeToPNG();

            indexPath = TexturePath.Replace(fileName, "IndexTexture1.png");
            File.WriteAllBytes(indexPath, text_index_Pix);

            weightPath = TexturePath.Replace(fileName, "WeightTexture1.png");
            File.WriteAllBytes(weightPath, text_weight_Pix);

            string path = TexturePath.Replace(fileName, "TextureArray.asset");
            Texture2DArray texLayers = CreateTextureArray2D(layerTextures, path);
            Target_Material.SetTexture("_Splats", texLayers);

            for(int i=0;i<layerCount;i++)
            {
                string texName = "_Splat" + i;
                Vector2 vTiling = Control_Material.GetTextureScale(texName);
                Target_Material.SetFloat(texName + "_ST", vTiling.x);
            }

            Target_Material.SetFloat("_NoLODDistance", Control_Material.GetFloat("_NoLODDistance"));
            Target_Material.SetFloat("_Normal_Idensity", Control_Material.GetFloat("_Normal_Idensity"));
            Target_Material.SetFloat("_Detail_Normal_Idensity", Control_Material.GetFloat("_Detail_Normal_Idensity"));
            Target_Material.SetColor("_color", Control_Material.GetColor("_color"));
            Target_Material.SetTexture("_Lightmap", Control_Material.GetTexture("_Lightmap"));
            Target_Material.SetFloat("_Lightmap_Scale", Control_Material.GetFloat("_Lightmap_Scale"));
        }
    }

    public void OnLayerGUI()
    {
        mPath = GUILayout.TextField(mPath);

        Control_Material = EditorGUILayout.ObjectField(Control_Material, typeof(Material)) as Material;
        Target_Material = EditorGUILayout.ObjectField(Target_Material, typeof(Material)) as Material;

        LayerUsed = EditorGUILayout.IntField(LayerUsed);

        

        if (GUILayout.Button("Terrain normal layer"))
        {
            CreateNormalMap();
        }

        if (GUILayout.Button("Terrain layer"))
        {
            CreateLayerTexture();
        }
    }
}
