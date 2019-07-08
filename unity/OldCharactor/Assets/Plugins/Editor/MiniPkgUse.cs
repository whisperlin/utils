using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;


public class MiniPkgUse
{
    static public void WriteTxtFile(string _filePath, string sContent)
    {
        using (FileStream fs = new FileStream(_filePath, FileMode.OpenOrCreate))
        {
            byte[] ByteArray = System.Text.Encoding.UTF8.GetBytes(sContent);
            fs.Write(ByteArray, 0, ByteArray.Length);
            fs.Close();
        }
    }

    static string ReadTxtFile(string _filePath)
    {
        string result;
        using (FileStream fs = new FileStream(_filePath, FileMode.Open))
        {
            byte[] bytes = new byte[fs.Length];
            int r = fs.Read(bytes, 0, bytes.Length);
            result = Encoding.Default.GetString(bytes, 0, r);
        }
        return result;
    }

    public static string GetAssetName(string filePath)
    {
        string formatfilePath = filePath.Replace('\\', '/');
        int nStartIndex = formatfilePath.LastIndexOf('/');
        formatfilePath = formatfilePath.Substring(nStartIndex + 1);
        return formatfilePath;
    }

    [MenuItem("Assets/Find References", false, 10)]
    static private void Find()
    {
        FindDetail();
    }

    static private string[] GetPathFile(string sReadPath)
    {
        string path = sReadPath;
        string[] png_Files_Tmp = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        List<string> tmp = new List<string>();
        Dictionary<string, int> dicRefCnt = new Dictionary<string, int>();
        Dictionary<string, string> dicRefPath = new Dictionary<string, string>();

        foreach (var sPng in png_Files_Tmp)
        {
            if (!sPng.Contains(".meta"))
            {
                tmp.Add(sPng);
            }
        }

        string[] pngArray = new string[tmp.Count];
        tmp.CopyTo(pngArray);

        return pngArray;
    }

    static private void FindDetail()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;

        //找出所有资源
        List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
        string[] files1 = Directory.GetFiles("Assets/Props", "*.*", SearchOption.AllDirectories)
            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();

        string[] files2 = Directory.GetFiles("Assets/Scene_Processed", "*.*", SearchOption.AllDirectories)
            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();

        string[] Assetfiles = new string[files1.Length+ files2.Length];
        files1.CopyTo(Assetfiles, 0);
        files2.CopyTo(Assetfiles, files1.Length);


        string path1 = "Assets/Props/Art/Fx/Character/Skill/Texture";
        string[] png_Files1 = GetPathFile(path1);
        

        string path2 = "Assets/Props/Art/Fx/_Common/Texture";
        string[] png_Files2 = GetPathFile(path2);

        string[] png_Files = new string[png_Files1.Length + png_Files2.Length];
        png_Files1.CopyTo(png_Files, 0);
        png_Files2.CopyTo(png_Files, png_Files1.Length);
        
        List<string> tmp = new List<string>();
        Dictionary<string, int> dicRefCnt = new Dictionary<string, int>();
        Dictionary<string, string> dicRefPath = new Dictionary<string, string>();

        int startIndex = 0;
        string[] sAllTexts = new string[Assetfiles.Length];
        EditorApplication.update = delegate()
        {
            if (startIndex >= Assetfiles.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;
                Debug.Log("查找成功");

                string sContent = "";
                string sRefDesc = "";
                List<string> result = new List<string>();
                foreach (var item in png_Files)
                {
                    string sName = GetAssetName(item);
                    if (!tmp.Contains(item))
                    {
                        result.Add(sName);
                        sContent = sContent + sName + "\n";
                        if (!AssetDatabase.DeleteAsset(item))
                        {
                            Debug.LogError("DeleteAsset fail:" + item);
                        }
                    }
                    else
                    {
//                            sRefDesc = sRefDesc + sName + ": " + dicRefCnt[item] + "\n";
//                             if(dicRefCnt[item] <= 1)
//                             {
                            sRefDesc = sRefDesc + dicRefPath[item].Replace('\\', '/') + ",\n";
//                             }
                            
                    }
                }

                sContent = sContent + result.Count.ToString();

                //WriteTxtFile("Assets/Mat_RefPNG.txt", sRefDesc);
                //RemoUselessMat(sRefDesc, Assetfiles);
                return;
            }

            string sText = sAllTexts[startIndex];
            string AssetName = Assetfiles[startIndex];
            string sAssetContent = File.ReadAllText(AssetName);
            startIndex++;
            int nCheckIdx = 0;
            while (nCheckIdx < png_Files.Length)
            {
                string pngPath = png_Files[nCheckIdx];
                string guid = AssetDatabase.AssetPathToGUID(pngPath);
                nCheckIdx++;
                if (guid != "" && Regex.IsMatch(sAssetContent, guid))
                {
                    tmp.Add(pngPath);
                    int temp = 0;
                    if (dicRefCnt.TryGetValue(pngPath, out temp))
                    {
                        dicRefCnt[pngPath] = temp + 1;
                    }
                    else
                    {
                        dicRefCnt[pngPath] = 1;
                        dicRefPath[pngPath] = AssetName;
                    }
                        
                    //Debug.Log(AssetName, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(AssetName)));
                }
            };

            bool isCancel = EditorUtility.DisplayCancelableProgressBar("资源查找中", AssetName, (float)startIndex / (float)Assetfiles.Length);
            if (isCancel)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;
                Debug.Log("处理被终止");
            }
        };
    }

    [MenuItem("Assets/Find References", true)]
    static private bool VFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    static private void RemoUselessMat(string sContent, string[] Assetfiles)
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;

        var sMatArray = Regex.Split(sContent, ",\n");

//         string path = AssetDatabase.GetAssetPath(Selection.activeObject);
//         string sContent = ReadTxtFile(path);
// 
//         //找出所有资源
//         List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".asset" };
//         string[] files1 = Directory.GetFiles("Assets/Props", "*.*", SearchOption.AllDirectories)
//             .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
// 
//         string[] files2 = Directory.GetFiles("Assets/Scene_Processed", "*.*", SearchOption.AllDirectories)
//             .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
// 
//         string[] Assetfiles = new string[files1.Length + files2.Length];
//         files1.CopyTo(Assetfiles, 0);
//         files2.CopyTo(Assetfiles, files1.Length);

        int startIndex = 0;
        string[] sAllTexts = new string[Assetfiles.Length];
        List<string> UsefulMat = new List<string>();
        EditorApplication.update = delegate()
        {
            if (startIndex >= Assetfiles.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;
                Debug.Log("删除无用材质成功");
                string sUseLessRefDesc = "";
                foreach (var sItem in sMatArray)
                {
                    if(!UsefulMat.Contains(sItem))
                    {
                        sUseLessRefDesc = sUseLessRefDesc + sItem.Replace('\\', '/') + ",\n";
                        if (!AssetDatabase.DeleteAsset(sItem))
                        {
                            Debug.LogError("DeleteAsset fail:" + sItem);
                        }
                    }
                }

                WriteTxtFile("Assets/" + "UselessMat_Result.txt", sUseLessRefDesc);
                return;
            }

            string sText = sAllTexts[startIndex];
            string AssetName = Assetfiles[startIndex];
            string sAssetContent = File.ReadAllText(AssetName);
            startIndex++;
            int nCheckIdx = 0;
            while (nCheckIdx < sMatArray.Length)
            {
                string matPath = sMatArray[nCheckIdx];
                string guid = AssetDatabase.AssetPathToGUID(matPath);
                nCheckIdx++;
                if (guid != "" && Regex.IsMatch(sAssetContent, guid))
                {
                    UsefulMat.Add(matPath);
                }
            };

            bool isCancel = EditorUtility.DisplayCancelableProgressBar("资源查找中", AssetName, (float)startIndex / (float)Assetfiles.Length);
            if (isCancel)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;
                Debug.Log("处理被终止");
            }
        };
    }
}
