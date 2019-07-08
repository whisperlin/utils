using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityExtend.Editor.Builder;
using System.Text;
using System.Linq;

class PlotData {
    public string Name;
    public string Obj1;
    public string Material1;
    public string Obj2;
    public string Material2;
    public string Obj3;
    public string Material3;
    public string Obj4;
    public string Material4;
    public string Obj5;
    public string Material5;
}

class MemuItems {
    private static Dictionary<string, Dictionary<string, string>> plotData = null; //

    [MenuItem("Easy/FindIt!")]
    public static void FindIt() {
        string[] dp = AssetDatabase.GetDependencies("Assets/Scene/denglujiemian.unity");
        StreamWriter sr = new StreamWriter("D:/111.txt", false);
        foreach (var s in dp) {
            sr.Write(s);
            sr.Write("\r\n");
        }
        sr.Close();

    }
    [MenuItem("Easy/BuildWaterTest")]
    public static void BuildWaterTest() {
        BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle;
        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];

        var ret = AssetDatabase.GetDependencies(@"Assets\Props\Prefabs\Character\AAA.prefab");
        List<string> resources = new List<string>();
        foreach (var s in ret) {
            if (!s.EndsWith(".dll") && !s.EndsWith(".asset")) {
                resources.Add(s);
            }
        }

        AssetBundleBuild[] buildMap2 = new AssetBundleBuild[1];
        buildMap2[0].assetNames = resources.ToArray();
        buildMap2[0].assetBundleName = "CalmWater2";
        BuildPipeline.BuildAssetBundles("ABs", buildMap2, options, BuildTarget.StandaloneWindows);
        File.Copy("ABs\\CalmWater2", @"F:/hugenstar/程序/ylz/IronHeart/trunk/client_code/Assets/CalmWater2", true);

        string[] assets = new string[1];
        assets[0] = @"Assets\Props\Prefabs\Character\AAA.prefab";
        buildMap[0].assetNames = resources.ToArray();
        buildMap[0].assetBundleName = "CalmWater2";

        string[] assets2 = new string[1];
        assets2[0] = @"Assets\Test.unity";
        buildMap[1].assetNames = assets2;
        buildMap[1].assetBundleName = "testscene";
        BuildPipeline.BuildAssetBundles("ABs", buildMap, options, BuildTarget.StandaloneWindows);

        //File.Copy("ABs\\CalmWater", @"F:/hugenstar/程序/ylz/IronHeart/trunk/client_code/Assets/CalmWater", true);
        File.Copy("ABs\\testscene", @"F:/hugenstar/程序/ylz/IronHeart/trunk/client_code/Assets/testscene", true);
        EditorUtility.DisplayDialog("build", "build ok", "继续工作");
    }
    [MenuItem("Easy/NavMesh/Bake")]
    public static void NewBake() {
        UnityExtend.Editor.Builder.NavMeshBuilder.NewBake(
                0.3f, /* cellSize */
                0.02f, /*  cellHeight  */
                2.0f, /* agentHeight */
                0.5f, /* agentRadius */
                0.9f, /* agentMaxClimb */
                45.0f, /* agentMaxSlope */
                -1f, /* regionMinSize */
                20f, /* regionMergeSize */
                12.0f, /* edgeMaxLen */
                1.3f, /* edgeMaxError */
                6.0f, /* vertsPerPoly */
                6.0f, /* detailSampleDist */
                1.0f /* detailSampleMaxError */
            );
        UnityExtend.Editor.Builder.NavMeshBuilder.ShowExportObj();
        //UnityExtend.Editor.Builder.NavMeshBuilder.ShowExportObj();
    }

    [MenuItem("Easy/NavMesh/Bake2")]
    public static void NewBake2() {

        //UnityExtend.Editor.Builder2.NavMeshBuilder.TestNav();

        UnityExtend.Editor.Builder.NavMeshBuilder.NewBake(
            0.3f, /* cellSize */
            0.02f, /*  cellHeight  */
            2.0f, /* agentHeight */
            0.5f, /* agentRadius */
            0.9f, /* agentMaxClimb */
            45.0f, /* agentMaxSlope */
            20f, /* regionMinSize */
            10f, /* regionMergeSize */
            12.0f, /* edgeMaxLen */
            1.3f, /* edgeMaxError */
            6.0f, /* vertsPerPoly */
            6.0f, /* detailSampleDist */
            1.0f /* detailSampleMaxError */
        );

        //      UnityExtend.Editor.Builder.NavMeshBuilder.ShowExportObj();

        UnityExtend.Editor.Builder.NavMeshBuilder.ShowExportObj();
    }

    class ExtMesh {
        public MeshFilter Original;

        public int Area;
        public Vector3[] Vertices;
        public int[] Triangles;

        public Bounds Bounds;

        public Matrix4x4 Matrix;
        public Vector3 Scale;
    }

    static void GetSceneMeshes(List<ExtMesh> meshes) {
        var filters = GameObject.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];
        var filteredFilters = new List<MeshFilter>(filters.Length / 3);

        var mask = StaticEditorFlags.NavigationStatic | StaticEditorFlags.LightmapStatic;
        var scene = EditorSceneManager.GetActiveScene();

        for (int i = 0; i < filters.Length; i++) {
            MeshFilter filter = filters[i];
            Renderer render = filter.GetComponent<Renderer>();

            if (render == null) {
                continue;
            }

            if (filter.sharedMesh == null) {
                continue;
            }

            var flags = GameObjectUtility.GetStaticEditorFlags(filter.gameObject);
            if ((flags & ~mask) != 0) {
                GameObjectUtility.SetStaticEditorFlags(filter.gameObject, flags & mask);
                EditorUtility.SetDirty(filter.gameObject);
                EditorSceneManager.MarkSceneDirty(scene);
            }

            if (!GameObjectUtility.AreStaticEditorFlagsSet(filter.gameObject, StaticEditorFlags.NavigationStatic)) {
                continue;
            }

            if (GameObjectUtility.GetNavMeshArea(filter.gameObject) == 0) {
                if (filter.gameObject.GetComponent<MeshCollider>() == null) {
                    var c = filter.gameObject.AddComponent<MeshCollider>();
                    c.sharedMesh = filter.sharedMesh;
                    EditorUtility.SetDirty(filter.gameObject);
                    EditorSceneManager.MarkSceneDirty(scene);
                }
            }

            filteredFilters.Add(filter);
        }


        var cachedVertices = new Dictionary<Mesh, Vector3[]>();
        var cachedTris = new Dictionary<Mesh, int[]>();

        for (int i = 0; i < filteredFilters.Count; i++) {
            MeshFilter filter = filteredFilters[i];

            Renderer render = filter.GetComponent<Renderer>();

            Mesh mesh = filter.sharedMesh;
            var smesh = new ExtMesh();
            smesh.Matrix = render.localToWorldMatrix;
            smesh.Original = filter;
            smesh.Scale = filter.gameObject.transform.localScale;

            if (cachedVertices.ContainsKey(mesh)) {
                smesh.Vertices = cachedVertices[mesh];
                smesh.Triangles = cachedTris[mesh];
            }
            else {
                smesh.Vertices = mesh.vertices;
                smesh.Triangles = mesh.triangles;
                var normal = mesh.normals;
                var uv = mesh.uv;
                cachedVertices[mesh] = smesh.Vertices;
                cachedTris[mesh] = smesh.Triangles;
            }

            smesh.Bounds = render.bounds;
            smesh.Area = GameObjectUtility.GetNavMeshArea(filter.gameObject);

            meshes.Add(smesh);
        }
    }

    static string ConvertMeshesToString(List<ExtMesh> meshes) {
        Mesh test = new Mesh();
        StringBuilder vBuilder = new StringBuilder();
        StringBuilder fBuilder = new StringBuilder();

        int vsCount = 0;
        int trisCount = 0;
        for (int m = 0; m < meshes.Count; m++) {
            vsCount += meshes[m].Vertices.Length;
            trisCount += meshes[m].Triangles.Length;
        }

        int vsOffset = 0;
        for (int m = 0; m < meshes.Count; ++m) {
            var mesh = meshes[m];

            var scale = mesh.Scale;
            Matrix4x4 matrix = mesh.Matrix;
            Vector3[] vs = mesh.Vertices;
            int[] tris = mesh.Triangles;
            int trisLength = tris.Length;

            for (int i = 0; i < vs.Length; i++) {
                var v = matrix.MultiplyPoint3x4(vs[i]);
                vBuilder.AppendFormat("v {0} {1} {2}\n", -v.x, v.y, v.z);
                /*
                    scale.x >= 0 ? -v.x : v.x,
                    scale.y >= 0 ? v.y : -v.y,
                    scale.z >= 0 ? v.z : -v.z);
                */
                //aryVertices.Add(v);
            }

            for (int i = 0; i < tris.Length; i += 3) {
                fBuilder.AppendFormat("f {0} {1} {2}\n", tris[i] + 1 + vsOffset, tris[i + 2] + 1 + vsOffset, tris[i + 1] + 1 + vsOffset);

                //aryIndices.Add(tris[i]  + vsOffset);
                //aryIndices.Add(tris[i + 1]  + vsOffset);
                //aryIndices.Add(tris[i + 2]  + vsOffset);

                if (mesh.Area == 1) {
                    fBuilder.AppendFormat("#!\n");
                }
            }

            vsOffset += vs.Length;
        }

        return vBuilder.ToString() + fBuilder.ToString();
    }

    static Mesh ConvertStringToMesh(string data) {
        //List<string> lines = new List<string>();
        var lines = data.Split('\n');

        //using (StringReader sr = new StringReader(data)) {
        //    string line;
        //    while ((line = sr.ReadLine()) != null) {
        //        lines.Add(line);
        //    }
        //}

        var mesh = new Mesh();

        List<Vector3> aryVertices = new List<Vector3>();
        List<int> aryIndices = new List<int>();

        for (int i = 0; i < lines.Length; ++i) {
            var row = lines[i];
            if (row.Length == 0 || row[0] == '#') {
                continue;
            }

            if (row[0] == 'v' && row[1] != 'n' && row[1] != 't') {
                var results = row.Split(' ');
                System.Diagnostics.Trace.Assert(results.Length == 4);
                aryVertices.Add(new Vector3(-float.Parse(results[1]), float.Parse(results[2]), float.Parse(results[3])));
            }

            if (row[0] == 'f') {
                var results = row.Split(' ');
                System.Diagnostics.Trace.Assert(results.Length == 4);
                aryIndices.Add(int.Parse(results[1]) - 1);
                aryIndices.Add(int.Parse(results[3]) - 1);
                aryIndices.Add(int.Parse(results[2]) - 1);
            }

        }

        mesh.vertices = aryVertices.ToArray();
        mesh.triangles = aryIndices.ToArray();

        return mesh;
    }


    [MenuItem("Easy/NavMesh/GenNav")]
    public static void ExportObj() {
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;
        UnityExtend.Editor.Builder.NavMeshBuilder.ExportObj(ref min, ref max, true, true);
    }

    [MenuItem("Easy/NavMesh/ShowExportObj")]
    public static void ShowExportObj() {
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;
        UnityExtend.Editor.Builder.NavMeshBuilder.ShowExportObj();
    }

    private static string[] GetFiles(string path, string searchPattern, bool topFile = false) {
        List<string> results = new List<string>();

        string[] multipleFilters = searchPattern.Split('|');
        SearchOption opt = SearchOption.AllDirectories;
        if (topFile) {
            opt = SearchOption.TopDirectoryOnly;
        }

        foreach (string filter in multipleFilters) {
            results.AddRange(Directory.GetFiles(path, filter, opt));
        }

        for (var i = 0; i < results.Count; ++i) {
            results[i] = results[i].Replace("\\", "/");
        }

        return results.ToArray();
    }

    private static string[] GetDirectories(string path, bool topDir) {
        SearchOption opt = SearchOption.AllDirectories;
        if (topDir) {
            opt = SearchOption.TopDirectoryOnly;
        }

        string[] dirs = Directory.GetDirectories(path, "*", opt);

        string[] result = new string[dirs.Length];

        for (var i = 0; i < dirs.Length; ++i) {
            result[i] = dirs[i].Replace("\\", "/");
        }

        return result;
    }

    [MenuItem("Assets/Build Selected Effect")]
    private static void BuildSelectedEffect() {
        var selected = Selection.activeObject;
        var effectPath = AssetDatabase.GetAssetPath(selected);
        if (!effectPath.StartsWith("Assets/Props/Prefabs/Fx/")) {
            EditorUtility.DisplayDialog("错误", "不是有效的特效路径", "继续工作");
            return;
        }

        var sourcePath = "Assets/Props/Prefabs/Fx";
        var targetPath = string.Format("{0}_Processed", sourcePath);
        _buildEffect(effectPath, targetPath, sourcePath);
        EditorUtility.DisplayDialog("build", "build ok", "继续工作");
        return;
    }

    [MenuItem("Assets/自己控制纹理属性")]
    private static void CustomTexture() {
        var selected = Selection.activeObject;
        var path = AssetDatabase.GetAssetPath(selected);
        var dir = Path.GetDirectoryName(path);
        var fileName = Path.GetFileName(path);
        var lsName = Path.Combine(dir, "." + fileName + ".ls");
        AssetImporter importer = AssetImporter.GetAtPath(path);
        if (importer != null) {
            importer.assetBundleName = "";
        }

        if (File.Exists(lsName)) {
            return;
        }
        File.Create(lsName).Close();

    }


    private static void _buildEffect(string prefabPath, string targetPath, string sourcePath) {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        var instance = GameObject.Instantiate(prefab);
        var processTo = string.Format("{0}{1}", targetPath, prefabPath.Substring(sourcePath.Length,
            prefabPath.Length - sourcePath.Length - ".prefab".Length)
            );

        var processToPath = Path.GetDirectoryName(processTo);

        if (!Directory.Exists(processToPath)) {
            Directory.CreateDirectory(processToPath);
        }

        PrefabUtility.DisconnectPrefabInstance(instance);
        PrefabUtility.CreatePrefab(string.Format("{0}_h.prefab", processTo), instance);
        var trans = instance.transform.FindChild("Node2");
        if (trans != null) {
            GameObject.DestroyImmediate(trans.gameObject);
        }

        PrefabUtility.DisconnectPrefabInstance(instance);

        PrefabUtility.CreatePrefab(string.Format("{0}_m.prefab", processTo), instance);
        trans = instance.transform.FindChild("Node1");
        if (trans != null) {
            GameObject.DestroyImmediate(trans.gameObject);
        }

        PrefabUtility.DisconnectPrefabInstance(instance);

        PrefabUtility.CreatePrefab(string.Format("{0}_l.prefab", processTo), instance);

        GameObject.DestroyImmediate(instance);

    }

    [MenuItem("Easy/Effect/BuildEffect")]
    public static void BuildEffect() {
        var sourcePath = "Assets/Props/Prefabs/Fx";
        var targetPath = string.Format("{0}_Processed", sourcePath);
        var paths = GetDirectories(sourcePath, false);
        for (int i = 0; i < paths.Length; ++i) {
            var path = paths[i];

            var prefabs = GetFiles(path, "*.prefab", true);
            var processToPath = string.Format("{0}{1}", targetPath, path.Substring(sourcePath.Length));

            if (!Directory.Exists(processToPath)) {
                Directory.CreateDirectory(processToPath);
            }

            for (int j = 0; j < prefabs.Length; ++j) {
                _buildEffect(prefabs[j], targetPath, sourcePath);
            }

        }

        EditorUtility.DisplayDialog("build", "build ok", "继续工作");
    }

    [MenuItem("Easy/Scene/BuildScene")]
    public static void BuildScene() {
        SceneBuilder.BuildScene();
        SetMapTerrainTextureIsEnable(true);
        EditorUtility.DisplayDialog("build", "build ok", "继续工作");
    }
    public static void SetMapTerrainTextureIsEnable(bool isEnable)
    {
        var terrainMaterialDescs= Transform.FindObjectsOfType<EasyExtend.Scene.TerrainMaterialDesc>();
        foreach(EasyExtend.Scene.TerrainMaterialDesc errainMaterialDesc in terrainMaterialDescs)
        {
            var mat = errainMaterialDesc.transform.GetComponent<MeshRenderer>().material;
            var texture2D = mat.GetTexture("_Control") as Texture2D;

            string path = AssetDatabase.GetAssetPath(texture2D);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (!textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
            } 
        }

    }
    [MenuItem("Easy/Scene/CopyScene")]
    public static void CopyScene() {
        SceneBuilder.BuildScene(false);
        EditorUtility.DisplayDialog("build", "build ok", "继续工作");
    }


    [MenuItem("Easy/Scene/检查材质")]
    public static void CheckMaterial() {

        Dictionary<string, string> dictError = new Dictionary<string, string>();

        var renders = GameObject.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
        for (int i = 0; i < renders.Length; i++) {

            Renderer render = renders[i];

            if (render == null) {
                continue;
            }


            var material = render.sharedMaterial;

            if (material == null || material.shader == null) {
                continue;
            }
            if (material.shader.name.StartsWith("Mobile/") || material.shader.name.StartsWith("Legacy")) {
                dictError[material.name] = material.shader.name;
            }
        }

        foreach (var p in dictError) {
            Debug.LogErrorFormat("material error {0} ==> {1}", p.Key, p.Value);
        }

        if (dictError.Count == 0) {
            EditorUtility.DisplayDialog("检查材质", "恭喜未发现有问题!", "继续工作");
        }
        else {
            EditorUtility.DisplayDialog("检查材质", "完成, 发现错误. 结果请看Console窗口的输出!", "继续工作");
        }

    }


    [MenuItem("Easy/Scene/切换为低显")]
    public static void ChangeToLowMaterial() {
        ChangeToLowMaterial(false);
    }
    [MenuItem("Easy/Scene/切换为低显(不检查)")]
    public static void ChangeToLowMaterialNoCheck() {
        ChangeToLowMaterial(true);
    }


    public static void ChangeToLowMaterial(bool skipCheck) {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene == null) {
            Debug.LogError("scene == null");
            return;
        }

        if (string.IsNullOrEmpty(scene.path)) {
            Debug.LogError("scene.path == null");
            return;
        }

        string saveToPath = Path.GetDirectoryName(scene.path);
        saveToPath = Path.Combine(saveToPath, scene.name);
        saveToPath = Path.Combine(saveToPath, ".material.mxt");

        Dictionary<string, Material> dictName2Material = new Dictionary<string, Material>();

        var renders = GameObject.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
        for (int i = 0; i < renders.Length; i++) {
            Renderer render = renders[i];

            if (render == null) {
                continue;
            }


            var material = render.sharedMaterial;

            if (material == null || material.shader == null) {
                continue;
            }

            if (!skipCheck) {
                if (material.shader.name.StartsWith("Mobile/") || material.shader.name.StartsWith("Legacy")) {
                    EditorUtility.DisplayDialog("切换为低显", "材质不干净, 不能完成切换. 请先进行检查材质工作!", "继续工作");
                    return;
                }
            }


            if (dictName2Material.ContainsKey(material.name) && dictName2Material[material.name].shader.name != material.shader.name) {
                EditorUtility.DisplayDialog("切换为低显", "有重名材质, 目前尚未支持. 请检查: " + material.shader.name, "继续工作");
                return;
            }

            if (dictName2Material.ContainsKey(material.name) && dictName2Material[material.name] != material) {
                EditorUtility.DisplayDialog("切换为低显", "发现有非共享的材质. 请检查: " + material.shader.name, "继续工作");
                return;
            }

            dictName2Material[material.name] = material;
        }

        using (StreamWriter sr = new StreamWriter(saveToPath, false)) {
            foreach (var p in dictName2Material) {
                var oldName = p.Value.shader.name;

                if (!oldName.StartsWith("YuLongZhi") || oldName.EndsWith("Terrain")) {
                    continue;
                }

                Shader shader = null;
                if (oldName.Contains("Cutout") || oldName.Contains("Tree")) {
                    shader = Shader.Find("Legacy Shaders/Transparent/Cutout/Diffuse");
                }
                else {
                    shader = Shader.Find("Mobile/Diffuse");
                }

                Debug.Assert(shader != null);
                p.Value.shader = shader;

                sr.Write(string.Format("{0},{1}\n", p.Key, oldName));
            }
        }

        EditorUtility.DisplayDialog("切换为低显", "完成", "继续工作");
    }

    private static string readAllText(string path) {
        var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        var sr = new StreamReader(fs);

        using (fs) {
            return sr.ReadToEnd();
        }
    }

    [MenuItem("Easy/Scene/还原为高显")]
    public static void RevertMaterial() {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene == null) {
            Debug.LogError("scene == null");
            return;
        }

        if (string.IsNullOrEmpty(scene.path)) {
            Debug.LogError("scene.path == null");
            return;
        }

        string saveToPath = Path.GetDirectoryName(scene.path);
        saveToPath = Path.Combine(saveToPath, scene.name);
        saveToPath = Path.Combine(saveToPath, ".material.mxt");

        if (!File.Exists(saveToPath)) {
            EditorUtility.DisplayDialog("还原为高显", "中间文件不存在", "继续工作");
            return;
        }

        var txt = readAllText(saveToPath);

        Dictionary<string, string> dictMaterial2Shader = new Dictionary<string, string>();


        using (StringReader sr = new StringReader(txt)) {
            string line;
            while ((line = sr.ReadLine()) != null) {
                if (!string.IsNullOrEmpty(line)) {
                    var idx = line.IndexOf(',');
                    var key = line.Substring(0, idx);
                    var value = line.Substring(idx + 1);

                    dictMaterial2Shader[key] = value;
                }
            }
        }

        Dictionary<string, Material> dictName2Material = new Dictionary<string, Material>();

        var renders = GameObject.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
        for (int i = 0; i < renders.Length; i++) {
            Renderer render = renders[i];

            if (render == null) {
                continue;
            }


            var material = render.sharedMaterial;

            if (material == null || material.shader == null) {
                continue;
            }

            if (dictMaterial2Shader.ContainsKey(material.name)) {
                dictName2Material[material.name] = material;
            }
        }

        foreach (var p in dictName2Material) {
            var shader = Shader.Find(dictMaterial2Shader[p.Key]);
            Debug.Assert(shader);
            p.Value.shader = shader;
        }

        EditorUtility.DisplayDialog("还原为高显", "完成", "继续工作");
        return;
    }

    //static AnimationClip BuildAnimationClip(DirectoryInfo dictorys) {
    //    string animationName = dictorys.Name;
    //    //查找所有图片，因为我找的测试动画是.jpg 
    //    FileInfo[] images = dictorys.GetFiles("*.jpg");
    //    AnimationClip clip = new AnimationClip();
    //    AnimationUtility.SetAnimationType(clip, ModelImporterAnimationType.Generic);
    //    EditorCurveBinding curveBinding = new EditorCurveBinding();
    //    curveBinding.type = typeof(SpriteRenderer);
    //    curveBinding.path = "";
    //    curveBinding.propertyName = "m_Sprite";
    //    ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[images.Length];
    //    //动画长度是按秒为单位，1/10就表示1秒切10张图片，根据项目的情况可以自己调节
    //    float frameTime = 1 / 10f;
    //    for (int i = 0; i < images.Length; i++) {
    //        Sprite sprite = Resources.LoadAssetAtPath<Sprite>(DataPathToAssetPath(images[i].FullName));
    //        keyFrames[i] = new ObjectReferenceKeyframe();
    //        keyFrames[i].time = frameTime * i;
    //        keyFrames[i].value = sprite;
    //    }
    //    //动画帧率，30比较合适
    //    clip.frameRate = 30;

    private static void _doSplitSelectedAnim(System.String modelPath, System.String prefabPath = null) {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();


        //var prefabs = GetFiles(path, "*.prefab", true);


        ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(modelPath);
        SerializedObject serializedObject = new SerializedObject(modelImporter);

        var animationCompression = serializedObject.FindProperty("m_AnimationCompression");
        animationCompression.intValue = (int)ModelImporterAnimationCompression.Optimal;

        serializedObject.ApplyModifiedProperties();
        AssetDatabase.WriteImportSettingsIfDirty(modelPath);
        AssetDatabase.Refresh();

        if (prefabPath == null) {
            prefabPath = modelPath + "@model.prefab";
        }

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        var anim = prefab.GetComponent<Animation>();

        Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();

        bool hasIdle = false;
        if (anim) {
            var path = modelPath + "_Anim";
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            foreach (AnimationState state in anim) {
                if (!state.name.Contains("Idle")) {
                    AnimationClip placeClip = new AnimationClip();
                    EditorUtility.CopySerialized(state.clip, placeClip);

                    var targetPath = path + "/" + state.name + ".anim";
                    AssetDatabase.CreateAsset(placeClip, targetPath);
                    clips.Add(state.name, placeClip);
                }
                else {
                    hasIdle = true;
                }
            }
        }


        if (hasIdle) {
            animationCompression.intValue = (int)ModelImporterAnimationCompression.Off;

            serializedObject.ApplyModifiedProperties();
            AssetDatabase.WriteImportSettingsIfDirty(modelPath);

            prefab = null;
            anim = null;
            modelImporter = null;
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();

            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            anim = prefab.GetComponent<Animation>();

            if (anim) {
                var path = modelPath + "_Anim";
                foreach (AnimationState state in anim) {
                    if (state.name.Contains("Idle")) {
                        AnimationClip placeClip = new AnimationClip();
                        EditorUtility.CopySerialized(state.clip, placeClip);

                        var targetPath = path + "/" + state.name + ".anim";
                        AssetDatabase.CreateAsset(placeClip, targetPath);
                        clips.Add(state.name, placeClip);
                    }
                }
            }

            animationCompression.intValue = (int)ModelImporterAnimationCompression.Optimal;

            serializedObject.ApplyModifiedProperties();
            AssetDatabase.WriteImportSettingsIfDirty(modelPath);
            AssetDatabase.Refresh();
        }

        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);

        GameObject ins = GameObject.Instantiate(prefab);
        PrefabUtility.DisconnectPrefabInstance(ins);

        anim = ins.GetComponent<Animation>();

        List<AnimationClip> aryClips = new List<AnimationClip>();

        AnimationClip defClip = null;
        foreach (var p in clips) {
            anim.RemoveClip(p.Key);
            //p.Value.name = p.Key;
            aryClips.Add(p.Value);

            if (anim.clip.name == p.Key) {
                defClip = p.Value;
            }
        }

        AnimationUtility.SetAnimationClips(anim, aryClips.ToArray());
        anim.clip = defClip;

        PrefabUtility.CreatePrefab(prefabPath, ins);

        GameObject.DestroyImmediate(ins, true);

        //AssetDatabase.CreateAsset(newObject, modelPath + "@model.prefab");
    }

    [MenuItem("Assets/拆分模型")]
    private static void SplitSelectedAnim() {
        var selected = Selection.activeObject;
        string path;

        if (selected == null) {
            path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            var prefabs = GetFiles(path, "*.FBX", true);

            foreach (var modelPath in prefabs) {
                _doSplitSelectedAnim(modelPath);
            }
        }
        else {
            path = AssetDatabase.GetAssetPath(selected);
            if (!path.EndsWith(".FBX")) {
                EditorUtility.DisplayDialog("拆分动画", "选中对象并非FBX", "继续工作");
                return;
            }
            _doSplitSelectedAnim(path);
        }

        EditorUtility.DisplayDialog("拆分动画", "处理完毕", "继续工作");

        return;
    }

    class AnimSplit {
        public string Name;
        public string FBXPath;
        public int Compression;
    }

    static private string AnimPath = "Assets/Props/Art/Character/Role/_Anim";
    static private string AnimRoot = "Assets/Props/Art/Character/Role/Animation/";

    private static bool _doSplitSelectedActorAnim(string targetName, System.String modelPath, List<AnimSplit> animInfos) {
        var targetPath = AnimPath;
        if (!Directory.Exists(targetPath)) {
            Directory.CreateDirectory(targetPath);
        }


        animInfos.Sort(delegate (AnimSplit a, AnimSplit b) {
            return a.Compression.CompareTo(b.Compression);
        });

        Dictionary<int, List<AnimSplit>> animByCompression = new Dictionary<int, List<AnimSplit>>();
        Dictionary<string, AnimSplit> animByName = new Dictionary<string, AnimSplit>();

        foreach (var animInfo in animInfos) {
            List<AnimSplit> target = null;
            if (!animByCompression.TryGetValue(animInfo.Compression, out target)) {
                target = new List<AnimSplit>();
                animByCompression.Add(animInfo.Compression, target);
            }
            target.Add(animInfo);
            animByName.Add(animInfo.Name, animInfo);
        }

        var keys = animByCompression.Keys.ToList();
        keys.Sort();

        AssetDatabase.RemoveUnusedAssetBundleNames();

        string _modelPath = AnimRoot + modelPath;
        ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(_modelPath);
		if (modelImporter == null) {
			EditorUtility.DisplayDialog("拆分动画", "无法加载FBX: " + _modelPath, "继续工作");
			return false;
		}
        SerializedObject serializedObject = new SerializedObject(modelImporter);

        foreach (var key in keys) {
            var animationCompression = serializedObject.FindProperty("m_AnimationCompression");
            animationCompression.intValue = key;

            serializedObject.ApplyModifiedProperties();
            AssetDatabase.WriteImportSettingsIfDirty(_modelPath);
            AssetDatabase.Refresh();

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_modelPath);
            var anim = prefab.GetComponent<Animation>();
            if (anim == null) {
                EditorUtility.DisplayDialog("拆分动画", "无法发现动画对象", "继续工作");
                return false;
            }

            foreach (AnimationState state in anim) {
                AnimSplit info = null;
                if (!animByName.TryGetValue(state.name, out info)) {
                    continue;
                }
                if (info.Compression != key) {
                    continue;
                }

                animByName.Remove(state.name);

                AnimationClip placeClip = new AnimationClip();
                EditorUtility.CopySerialized(state.clip, placeClip);

                var outPath = targetPath + "/" + modelPath;
                if(!Directory.Exists(outPath)) {
                    Directory.CreateDirectory(outPath);
                }

                outPath = outPath + "/" + state.name + ".anim";
                AssetDatabase.CreateAsset(placeClip, outPath);
            }

        }

        bool result = true;
        foreach (var p in animByName) {
            EditorUtility.DisplayDialog("拆分动画", "动画导出失败: " + p.Key + " from " + p.Value.FBXPath, "继续工作");
            result = false;
        }

        AssetDatabase.Refresh();
        return result;
    }

    [MenuItem("Assets/拆分主角动画")]
    private static void SplitSelectedActorAnim() {
        var selected = Selection.activeObject;
        if (selected == null) {
            EditorUtility.DisplayDialog("错误", "请选中对应动画配置表", "继续工作");
            return;
        }
        var path = AssetDatabase.GetAssetPath(selected);
        //Assets/Config/Animation/mengjiang.csv
        if (!path.StartsWith("Assets/Config/Animation") || !path.EndsWith(".csv")) {
            EditorUtility.DisplayDialog("错误", "请选中对应动画配置表", "继续工作");
            return;
        }

        string animationName = Path.GetFileNameWithoutExtension(path);
        var loader = new CSVLoader();
        var text = readAllText(path);
        if (!loader.Load(text)) {
            EditorUtility.DisplayDialog("错误", "加载动画表失败: " + path, "继续工作");
        }

        var nameCol = loader.GetColumn("Name");
        var fbxCol = loader.GetColumn("FBXPath");
        var compCol = loader.GetColumn("Compression");
        var defaultCol = loader.GetColumn("IsDefault");

        string defaultAnimName = null;
        List<string> anims = new List<string>();

        Dictionary<string, List<AnimSplit>> infos = new Dictionary<string, List<AnimSplit>>();
        for (int row = 0; row < loader.RowCount(); row++) {
            var animInfo = new AnimSplit();
            animInfo.Name = nameCol.GetString(row);
            animInfo.FBXPath = fbxCol.GetString(row);
            animInfo.Compression = compCol.GetInteger32(row);

            List<AnimSplit> subAnimInfos = null;
            if (!infos.TryGetValue(animInfo.FBXPath, out subAnimInfos)) {
                subAnimInfos = new List<AnimSplit>();
                infos.Add(animInfo.FBXPath, subAnimInfos);
            }

            if (defaultAnimName == null || defaultCol.GetBoolean(row)) {
                defaultAnimName = animInfo.Name;
            }

            subAnimInfos.Add(animInfo);
            anims.Add(animInfo.Name);
        }


        bool result = true;
        foreach (var p in infos) {
            if (!_doSplitSelectedActorAnim(animationName, p.Key, p.Value)) {
                result = false;
            }
        }

        if (result && defaultAnimName != null) {
            EditorUtility.DisplayDialog("拆分主角动画", "已完成", "继续工作");
        }
        else {
            EditorUtility.DisplayDialog("错误", "切分失败", "继续工作");
        }

        return;
    }


    static private string MatRoot = "Assets/Props/Art/Character/Role/Mesh";
    static private string PrefabRoot = "Assets/Props/Prefabs/Character/Role/";

    private static void _doCreateActorPrefab(string name, Dictionary<string, AnimationClip> clips, string defaultClipName, string mesh, string mat) {
        //1, 加载模型
        var meshObject = AssetDatabase.LoadAssetAtPath<GameObject>(MatRoot + "/" + mesh);
        meshObject = GameObject.Instantiate(meshObject);
        var animComp = meshObject.GetComponent<Animation>();
        if(animComp == null) {
            animComp = meshObject.AddComponent<Animation>();
        }

        AnimationUtility.SetAnimationClips(animComp, clips.Values.ToArray());
        animComp.clip = clips[defaultClipName];

        //2, 设置材质
        var render = meshObject.GetComponentInChildren<Renderer>();
        if(render == null) {
            EditorUtility.DisplayDialog("错误", "mesh里面未包含render: " + mesh, "继续工作");
            return;
        }

        var matObj = AssetDatabase.LoadAssetAtPath<Material>(MatRoot + "/" + mat);

        if(matObj == null) {
            EditorUtility.DisplayDialog("错误", "加载材质错误: " + mat, "继续工作");
            return;
        }

        render.material = matObj;

        //给加上阴影
        var shadow = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Props/Art/Character/_Common/shadow/shadow.prefab");

        var shadowObj = GameObject.Instantiate(shadow);
        shadowObj.name = shadow.name;
        shadowObj.transform.parent = meshObject.transform;

        meshObject.name = name;

        //3, 保存预制件
        PrefabUtility.CreatePrefab(PrefabRoot + name + ".prefab", meshObject);

        //4, 销毁对象
        GameObject.DestroyImmediate(shadowObj, true);
        GameObject.DestroyImmediate(meshObject, true);  
    }

    private static Dictionary<string, AnimationClip> loadAnimClips(string name, ref string defaultAnimName) {
        var path = "Assets/Config/Animation/" + name + ".csv";

        var loader = new CSVLoader();
        var text = readAllText(path);
        if (!loader.Load(text)) {
            EditorUtility.DisplayDialog("错误", "加载动画表失败: " + path, "继续工作");
            return null;
        }


        var nameCol = loader.GetColumn("Name");
        var fbxCol = loader.GetColumn("FBXPath");
        var defaultCol = loader.GetColumn("IsDefault");

        Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();
        Dictionary<string, List<string>> infos = new Dictionary<string, List<string>>();

        for (int row = 0; row < loader.RowCount(); row++) {
            var fbxPath = fbxCol.GetString(row);
            var aniClipName = nameCol.GetString(row);

            List<string> subAnimNames = null;
            if (!infos.TryGetValue(fbxPath, out subAnimNames)) {
                subAnimNames = new List<string>();
                infos.Add(fbxPath, subAnimNames);
            }
            
            if (defaultAnimName == null || defaultCol.GetBoolean(row)) {
                defaultAnimName = aniClipName;
            }
            subAnimNames.Add(aniClipName);
        }

        foreach(var p in infos) {
            var animRootPath = AnimPath + "/" + p.Key;
            foreach(var pp in p.Value) {
                var animPath = animRootPath + "/" + pp + ".anim";
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
                if(clip == null) {
                    EditorUtility.DisplayDialog("错误", "加载动画切片失败: " + animPath, "继续工作");
                    return null;
                }

                clips.Add(pp, clip);
            }
        }

        return clips;
    }

    [MenuItem("Assets/创建主角预制件")]
    private static void CreateActorPrefab() {
        var selected = Selection.activeObject;
        if (selected == null) {
            EditorUtility.DisplayDialog("错误", "请选中对应预制件配置表", "继续工作");
            return;
        }
        var path = AssetDatabase.GetAssetPath(selected);
        //Assets/Config/Animation/mengjiang.csv
        if (!path.StartsWith("Assets/Config/Prefab") || !path.EndsWith(".csv")) {
            EditorUtility.DisplayDialog("错误", "请选中对应预制件配置表", "继续工作");
            return;
        }

        var loader = new CSVLoader();
        var text = readAllText(path);
        if (!loader.Load(text)) {
            EditorUtility.DisplayDialog("错误", "加载预制件配置失败: " + path, "继续工作");
            return;
        }

        var nameCol = loader.GetColumn("Name");
        var animCol = loader.GetColumn("Animation");
        var matCol = loader.GetColumn("Material");
        var meshCol = loader.GetColumn("Mesh");


        Dictionary<string, Dictionary<string, AnimationClip>> allClips = new Dictionary<string, Dictionary<string, AnimationClip>>();
        Dictionary<string, string> defaultClipNames = new Dictionary<string, string>();

        for (int row = 0; row < loader.RowCount(); row++) {
            string animName = animCol.GetString(row);
            string defaultName = null;

            Dictionary<string, AnimationClip> clips = null;
            if (!allClips.TryGetValue(animName, out clips)) {
                clips = loadAnimClips(animName, ref defaultName);

                if(clips != null) {
                    defaultClipNames.Add(animName, defaultName);
                    allClips.Add(animName, clips);
                }
                else {
                    EditorUtility.DisplayDialog("错误", "无法加载对应的动画切片: " + animName, "继续工作");
                }
            }
            else {
                defaultClipNames.TryGetValue(animName, out defaultName);
            }

            _doCreateActorPrefab(
                nameCol.GetString(row),
                clips,
                defaultName,
                meshCol.GetString(row),
                matCol.GetString(row)
            );
        }

        EditorUtility.DisplayDialog("创建主角预制件", "完成", "继续工作");
    }

    [MenuItem("Assets/创建剧情动画预制件")]
    private static void CreatePlotPrefab() {
        var loader = new CSVLoader();
        var text = readAllText("Assets/Config/Prefab/juqing.csv");
        if (!loader.Load(text)) {
            EditorUtility.DisplayDialog("错误", "加载预制件配置失败:Assets/Config/Prefab/juqing.csv ", "继续工作");
            return;
        }

        if (plotData == null) {
            plotData = new Dictionary<string, Dictionary<string, string>>();
            LoadPlotData(loader);
        }

        var selected = Selection.activeObject;
        if (selected == null) {
            EditorUtility.DisplayDialog("错误", "请选中对应预制件配置表", "继续工作");
            return;
        }

        string path;
        path = AssetDatabase.GetAssetPath(selected);
        if (!path.EndsWith(".FBX")) {
            EditorUtility.DisplayDialog("拆分动画", "选中对象并非FBX", "继续工作");
            return;
        }

        if (!path.Contains("PlotAnim")) {
            EditorUtility.DisplayDialog("拆分动画", "非剧情目录", "继续工作");
            return;
        }

        string prefabPath = path.Replace("Art", "Prefabs").Replace(".FBX", ".prefab");
        _doSplitSelectedAnim(path, prefabPath);

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        string fileName = System.IO.Path.GetFileName(path);
        Dictionary<string, string> processMat;
        if (plotData.TryGetValue(fileName, out processMat)) {
            var dicIter = processMat.GetEnumerator();
            while (dicIter.MoveNext()) {
                var objTf = prefab.transform.Find(dicIter.Current.Key);
                if (objTf != null) {
                    var mat = AssetDatabase.LoadAssetAtPath<Material>(dicIter.Current.Value);
                    if(mat != null){
                        var mesh = objTf.GetComponent<SkinnedMeshRenderer>();
                        if(mesh){
                            mesh.materials[0] = mat;
                        }
                    }
                }
            }
        }

        PrefabUtility.CreatePrefab(prefabPath, prefab);
        GameObject.DestroyImmediate(prefab, true);  
        
        EditorUtility.DisplayDialog("创建剧情动画预制件", "完成", "继续工作");
    }

    [MenuItem("Assets/设置特效层级")]
    private static void SetEffectSortOrder() {
        var selected = Selection.activeObject;
        var effectPath = AssetDatabase.GetAssetPath(selected);
        if (!effectPath.StartsWith("Assets/Props/Prefabs/Fx/")) {
            EditorUtility.DisplayDialog("错误", "不是有效的特效路径", "继续工作");
            return;
        }

        //var loader = new CSVLoader();
        //var text = readAllText("Assets/Config/Effect/materialMapIds.csv");
        //if (!loader.Load(text)) {
        //    EditorUtility.DisplayDialog("错误", "加载特效层级配置失败:Assets/Config/Effect/materialMapIds.csv", "继续工作");
        //    return;
        //}

        //var nameCol = loader.GetColumn("Name");
        //var sortOrder = loader.GetColumn("SortOrder");

        Dictionary<string, int> mapIds = new Dictionary<string, int>();

        //for (int row = 0; row < loader.RowCount(); row++) {
        //    string name = nameCol.GetString(row);
        //    int mapSortOrder = sortOrder.GetInteger32(row);
        //    mapIds[name] = mapSortOrder;
        //}

        string[] dirs = Directory.GetDirectories(effectPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < dirs.Length; i++) {
            string name = dirs[i].Replace('\\', '/');
            string[] files = Directory.GetFiles(name, "*prefab");
            if (files.Length == 0) {
                continue;
            }

            int setOrder = 0;

            foreach (var file in files) {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                var newPrefab = GameObject.Instantiate(prefab);
                var ps = newPrefab.GetComponent<ParticleSystemRenderer>();
                var mat = ps.material;

                var renderers = newPrefab.GetComponentsInChildren<Renderer>();
                for (int j = 0; j < renderers.Length; ++j) {
                    var render = renderers[i];
                    
                    int mapSortOrder = 0;
                    if (mapIds.TryGetValue(render.material.name, out mapSortOrder)) {
                        render.sortingOrder = mapSortOrder;
                    } else {
                        render.sortingOrder = setOrder;
                        mapIds[render.material.name] = render.sortingOrder;
                        ++setOrder;
                    }     
                }

                PrefabUtility.ReplacePrefab(newPrefab, prefab, ReplacePrefabOptions.ConnectToPrefab);
                GameObject.DestroyImmediate(newPrefab);
            }
        }
    }

    private static void LoadPlotData(CSVLoader loader) {
        int rows = loader.RowCount();
        var factory = DataReader<PlotData>.Build(loader);
        for (int i = 0; i < rows; ++i) {
            PlotData info = factory.Parse(loader, i);
            var value = new Dictionary<string, string>();
            plotData[info.Name] = value;

            if (!string.IsNullOrEmpty(info.Obj1)) {
                value[info.Obj1] = info.Material1;
            }

            if (!string.IsNullOrEmpty(info.Obj2)) {
                value[info.Obj2] = info.Material2;
            }

            if (!string.IsNullOrEmpty(info.Obj3)) {
                value[info.Obj3] = info.Material3;
            }

            if (!string.IsNullOrEmpty(info.Obj4)) {
                value[info.Obj4] = info.Material4;
            }

            if (!string.IsNullOrEmpty(info.Obj5)) {
                value[info.Obj5] = info.Material5;
            }
        }
    }
}

