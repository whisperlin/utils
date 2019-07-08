using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEditor.Events;

public class UIAuxiliaryFun {

    #region 按钮菜单
    [MenuItem("UIUtil/SetButtonEffect")]
    public static void SetButtonEffect()
    {
        if (!BSelectedObject())
            return;

        Object[] labels = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);
        foreach (Object item in labels)
        {
            var go = item as GameObject;
            if (go == null)
                continue;
            SetButtonAnimStyle(go, UnityEngine.UI.Selectable.Transition.Animation, null);
            EditorUtility.SetDirty(item);//重要
        }
    }


    [MenuItem("UIUtil/ButtonStyle/关闭按钮")]
    public static void SetButtonCloseStyle() {
        SetButtonStyle("btnClose", 60, 62, Image.Type.Simple, 0, string.Empty, string.Empty);
        SetButtonEffect();
    }

    [MenuItem("UIUtil/ButtonStyle/一级按钮")]
    public static void SetButton1Style() {
        SetButtonStyle("btnBg1", 250, 88, Image.Type.Sliced, 0, string.Empty, string.Empty);
    }

    [MenuItem("UIUtil/ButtonStyle/二级按钮(黄)")]
    public static void SetButton2YellowStyle() {
        SetButtonStyle("btnYellow", 250, 74, Image.Type.Sliced, 32, "faf7f7", "935021");
    }
    [MenuItem("UIUtil/ButtonStyle/二级按钮(蓝)")]
    public static void SetButton2BlueStyle() {
        SetButtonStyle("btnBlue", 250, 74, Image.Type.Sliced, 32, "f7f7fa", "393765");
    }

    [MenuItem("UIUtil/ButtonStyle/三级按钮")]
    
    #endregion

    #region 按钮样式
    public static void SetButton3Style() {
        SetButtonStyle("btnBg3", 130, 52, Image.Type.Sliced,26,"f6f6fc","1f244a");
    }
    public static void SetButtonStyle(string btnImg,int width, int height,
        Image.Type imageType, int fontSize, string fontColor, string fontShadowColor) {

        if (!BSelectedObject())
            return;

        Object[] btnCons = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);
        foreach (Object item in btnCons) {
            var go = item as GameObject;
            if (go == null)
                continue;

            var butImg = go.GetComponent<Image>();
            if (butImg == null)
                butImg = go.AddComponent<Image>();
            butImg.type = imageType;
            butImg.raycastTarget = true;
            Object[] spriteObjects = AssetDatabase.LoadAllAssetsAtPath("Assets/UI/Textures/Shared/Shared.png");

            foreach (Object obj in spriteObjects)//遍历小图集
		    {
                if (obj.name == btnImg) {
                    butImg.sprite = obj as Sprite;
                    break;
                }
            }

            if (fontSize > 0){
                var tChild = go.transform.FindChild("Text");
                if (tChild != null) { 
                    var txtChild = tChild.gameObject.GetComponent<Text>();
                    if (txtChild != null) {
                        txtChild.fontSize = fontSize;
                        if (fontColor != null && fontColor != string.Empty) {
                            txtChild.color = ChangeFontWindow.hexToColor(fontColor);
                        }
                        if (fontShadowColor != null && fontShadowColor != string.Empty) {
                            var btnShadow = tChild.gameObject.GetComponent<Shadow>();
                            if (btnShadow == null)
                                btnShadow = tChild.gameObject.AddComponent<Shadow>();
                            btnShadow.effectDistance = new Vector2(2,-2);
                            btnShadow.effectColor = ChangeFontWindow.hexToColor(fontShadowColor);
                        }
                    }
                }
            }

            RectTransform imgRec = butImg.rectTransform;
            imgRec.pivot = Vector2.one * 0.5f;
            imgRec.sizeDelta = new Vector2(width, height);

            SetButtonAnimStyle(go, UnityEngine.UI.Selectable.Transition.ColorTint, butImg);
            
            EditorUtility.SetDirty(item);//重要
        }
        AssetDatabase.Refresh();
    }


    private static void SetButtonAnimStyle(GameObject go, UnityEngine.UI.Selectable.Transition animType, Image butImg) {
        var but = go.GetComponent<Button>();
        if (but == null)
            but = go.AddComponent<Button>();
        but.transition = animType;

        if (animType == Selectable.Transition.Animation) {
            but.animationTriggers.normalTrigger = "Normal";
            but.animationTriggers.highlightedTrigger = "Highlighted";
            but.animationTriggers.pressedTrigger = "Pressed";
            but.animationTriggers.disabledTrigger = "Disabled";

            var ani = go.GetComponent<Animator>();
            if (ani == null)
                ani = go.AddComponent<Animator>();

            if (ani.runtimeAnimatorController == null) {
                var runa = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/UI/Builds/Anim/AnimButtonScale.controller");
                ani.runtimeAnimatorController = runa;
            }
        } else {
            but.targetGraphic = butImg;
            ColorBlock colorBlock = new ColorBlock();
            colorBlock.normalColor = new Color32(255, 255, 255, 255);
            colorBlock.highlightedColor = new Color32(245, 245, 245, 255);
            colorBlock.pressedColor = new Color32(200, 200, 200, 255);
            colorBlock.disabledColor = new Color32(200, 200, 200, 128);
            colorBlock.colorMultiplier = 1;
            colorBlock.fadeDuration = 0.1f;
            but.colors = colorBlock;
        }
    }

    #endregion

    #region 图片空引用检测

    //////////////UI检测 替换成 NoDrawingRayCast////////////////////
    [MenuItem("UIUtil/CheckUI/CheckImageNullToNoDrawingRayCast")]
    public static void CheckImageNullToNoDrawingRayCast() {
        CheckImage(false, true);
    }

    [MenuItem("UIUtil/CheckUI/CheckImageNull")]
    public static void CheckImageNull() {
        CheckImage(true, false);
    }
    public static void CheckImage(bool isNull,bool isAlpha) {
        string[] filePaths = Directory.GetFiles("Assets/UI/Builds/", "*.prefab", SearchOption.AllDirectories);
        foreach (var filePath in filePaths) {
            var fileName = Path.GetFileName(filePath);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            bool isDirty = false;
            bool isExist = false;
            Component[] Componets = go.GetComponentsInChildren(typeof(Image), true);
            StringBuilder sb = new StringBuilder("->child:");
            foreach (var compomnt in Componets) {
                var imgGo = compomnt.gameObject;
                var image = compomnt as Image;
                if (image != null ) {
                    if (isNull) {
                        if (image.sprite == null || image.enabled == false) {
                            sb.AppendLine(compomnt.name);
                            isExist = true;
                        }
                    }
                    if (isAlpha) {
                        if (image.color.a == 0) {
                            isDirty = true;
                            isExist = true;
                            sb.AppendLine(compomnt.name);
                            GameObject.DestroyImmediate(image, true);
                            imgGo.AddComponent<NoDrawingRayCast>();
                            EditorUtility.SetDirty(imgGo);
                        }
                    }
                }
            }
            if (isExist) {
                Debug.LogError("ImageNull.Path:" + go.name + sb.ToString());
            }

            if (isDirty) {                
                EditorUtility.SetDirty(go);
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
    #endregion


    #region 点击事件检测
    [MenuItem("Assets/DisableRayCastTarget")]
    public static void DisableAssetRayCastTargets() {
        if (!BSelectedObject()) 
            return;

        Object[] gos = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);
        foreach (Object item in gos) {
            var go = item as GameObject;
            if (go == null)
                continue;
            DisableRayCast(go);
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("UIUtil/CheckUI/DisableRayCastTargets")]
    public static void DisableRayCastTargets() {
        string[] filePaths = Directory.GetFiles("Assets/UI/Builds/", "*.prefab", SearchOption.AllDirectories);
        foreach (var filePath in filePaths) {
            var fileName = Path.GetFileName(filePath);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            DisableRayCast(go);
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    public static void DisableRayCast(GameObject go) {
        bool isDirty = false;
        StringBuilder sb = new StringBuilder("->child:");

        Component[] Componets = go.GetComponentsInChildren(typeof(Image), true);
        foreach (var compomnt in Componets) {
            var imgGo = compomnt.gameObject;
            var image = compomnt as Image;
            if (image != null) {
                var btn = imgGo.GetComponent<Button>();
                var btnG = imgGo.GetComponent<Toggle>();
                if (image.raycastTarget && btn == null && btnG == null) {
                    isDirty = true;
                    sb.AppendLine(compomnt.name);
                    image.raycastTarget = false;
                    EditorUtility.SetDirty(imgGo);
                } else if (!image.raycastTarget && (btn != null || btnG != null))
                {
                    isDirty = true;
                    image.raycastTarget = true;
                    EditorUtility.SetDirty(imgGo);
                }
            }
        }

        Componets = go.GetComponentsInChildren(typeof(Text), true);
        foreach (var compomnt in Componets) {
            var imgGo = compomnt.gameObject;
            var text = compomnt as Text;
            if (text != null) {
                var btn = imgGo.GetComponent<Button>();
                var btnG = imgGo.GetComponent<Toggle>();
                if (text.raycastTarget && btn == null && btnG == null) {
                    isDirty = true;
                    sb.AppendLine(compomnt.name);
                    text.raycastTarget = false;
                    EditorUtility.SetDirty(imgGo);
                } else if (!text.raycastTarget && (btn != null || btnG != null)) {
                    isDirty = true;
                    text.raycastTarget = true;
                    EditorUtility.SetDirty(imgGo);
                }
            }
        }

        if (isDirty) {
            Debug.LogError("DisableRayCast.Path:" + go.name + sb.ToString());
            EditorUtility.SetDirty(go);
        }
    }
    #endregion

    #region 引用检测
    [MenuItem("Assets/FindTextureReferences")]
    public static void FindSpriteReferences() {
        if (!BSelectedObject())
            return;
        
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        bool bSprite = Selection.activeObject is Sprite;
        Dictionary<string, List<string>> dicSprite = new Dictionary<string, List<string>>();
        var nGuid = AssetDatabase.AssetPathToGUID(path);
        var texturePic = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texturePic == null) {
            Debug.LogError("未选择任何图集");
            return;
        }
        EditorUtility.DisplayProgressBar("Find Reference", "Finding...", 0);

        string sNameSelect = texturePic.name;
        if (bSprite) {
            dicSprite.Add(Selection.activeObject.name, null);
        } else {            
            Object[] spriteObjects = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var sprite in spriteObjects) {
                dicSprite.Add(sprite.name, null);
            }
        }

        bool isRefTexture = false;
        string[] filePaths = Directory.GetFiles("Assets/UI/Builds/", "*.prefab", SearchOption.AllDirectories);
        try {
            int i = 0;
            foreach (var filePath in filePaths) {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);                
                var paths = AssetDatabase.GetDependencies(new[] { filePath });
                foreach (var strpath in paths) {
                    var sGuid = AssetDatabase.AssetPathToGUID(strpath);                    
                    if (nGuid == sGuid) {
                        isRefTexture = true;

                        AddSpriteName(sNameSelect, dicSprite, filePath);
                        Component[] Componets = go.GetComponentsInChildren(typeof(Image), true);
                        foreach (var compomnt in Componets) {
                            var image = compomnt as Image;
                            if (image && image.mainTexture && image.mainTexture.name == sNameSelect && image.sprite) {
                                AddSpriteName(image.sprite.name, dicSprite, go.name + "\\" + image.name);
                            }
                        }
                        break;
                    }
                }
                i++;
                EditorUtility.DisplayProgressBar("Find Reference", filePath, i / filePaths.Length);
            }
        }catch (System.Exception e) {
            Debug.Log(e.Message);
        } finally {
            EditorUtility.ClearProgressBar();
        }
        StringBuilder sbre = new StringBuilder("references->child:");
        StringBuilder sbno = new StringBuilder(sNameSelect+ "prefab未引用的图,请务删除动态加载的图片->child:\n");

        if (isRefTexture) {
            var iter = dicSprite.GetEnumerator();
            int refNoCount = 0;
            int refCount = 0;
            while (iter.MoveNext()) {
                List<string> refSpriteList = iter.Current.Value;
                if (refSpriteList != null && refSpriteList.Count > 0) {
                    refCount++;
                    sbre.AppendLine(iter.Current.Key + ":引用次资源:");
                    foreach (string refPath in refSpriteList) {
                        sbre.AppendLine(iter.Current.Key + ":" + refPath);
                    }
                } else {
                    refNoCount++;
                    sbno.AppendLine(iter.Current.Key + ":引用次数 == 0");
                }
            }
            if (refNoCount > 0)//没引用的输出
                Debug.LogError(sbno.ToString());
            else if(bSprite)
                Debug.Log(sbre.ToString());
            else
                Debug.Log("没有冗余图片");
        } else {
            Debug.Log("图集未被引用");
        }
    }
    #endregion


    #region 引用检测
    [MenuItem("Assets/PrintTextureReferencesWithoutShared2")]
    public static void PrintTextureReferencesWithoutShared2()
    {
        if (!BSelectedObject())
            return;

        var selectSourcePath = AssetDatabase.GetAssetPath(Selection.activeObject);
        bool bSprite = Selection.activeObject is Sprite;
        Dictionary<string, List<string>> selectSourceListDic = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> prefabUseImagDic = new Dictionary<string, List<string>>();
        Dictionary<string, string> share2SourceListDic = new Dictionary<string, string>();
        var sSelectSourceGuid = AssetDatabase.AssetPathToGUID(selectSourcePath);
        var texturePic = AssetDatabase.LoadAssetAtPath<Texture2D>(selectSourcePath);
        if (texturePic == null)
        {
            Debug.LogError("未选择任何图集");
            return;
        }
        EditorUtility.DisplayProgressBar("Find Reference", "Finding...", 0);

        string sNameSelect = texturePic.name;
        if (bSprite)
        {
            selectSourceListDic.Add(Selection.activeObject.name, null);
        }
        else
        {
            Object[] spriteObjects = AssetDatabase.LoadAllAssetsAtPath(selectSourcePath);
            foreach (var sprite in spriteObjects)
            {
                selectSourceListDic.Add(sprite.name, null);
            }

            spriteObjects = AssetDatabase.LoadAllAssetsAtPath("Assets/UI/Textures/Shared2/Shared2.png");
            foreach (var sprite in spriteObjects)
            {
                share2SourceListDic.Add(sprite.name, null);
            }
        }

        string[] allPefabPaths = Directory.GetFiles("Assets/UI/Builds/", "*.prefab", SearchOption.AllDirectories);
        try
        {
            int i = 0;
            foreach (var prefabPath in allPefabPaths)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                var prefabDependency = AssetDatabase.GetDependencies(new[] { prefabPath });
                foreach (var dependencySourcePath in prefabDependency)
                {
                    var sDependencySourceGuid = AssetDatabase.AssetPathToGUID(dependencySourcePath);
                    if (sSelectSourceGuid == sDependencySourceGuid)
                    {
                        Component[] Componets = prefab.GetComponentsInChildren(typeof(Image), true);
                        foreach (var compomnt in Componets)
                        {
                            var image = compomnt as Image;
                            if (image && image.mainTexture && image.mainTexture.name == sNameSelect && image.sprite)
                            {
                                //预制体用到的子节点用到的图片
                                //检测该图片是否位于Shard2中
                                string sKey = image.name + " " + image.sprite.name;
                                if (share2SourceListDic.ContainsKey(image.sprite.name) == false)
                                    sKey = sKey + " NeedCopy!";
                                List<string> refSpriteList = null;
                                if (prefabUseImagDic.ContainsKey(prefabPath) == false)
                                {
                                    refSpriteList = new List<string>();
                                    prefabUseImagDic[prefabPath] = refSpriteList;
                                }
                                else
                                {
                                    refSpriteList = prefabUseImagDic[prefabPath];
                                }
                                refSpriteList.Add(sKey);
                            }
                        }
                        break;
                    }
                }
                i++;
                EditorUtility.DisplayProgressBar("Find Reference", prefabPath, i / allPefabPaths.Length);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        var iter = prefabUseImagDic.GetEnumerator();
        Debug.Log("检索结果如下====================:");
        StringBuilder sbre = new StringBuilder("");
        int nCount = 0;
        int nMaxCount = 255;//过长的Log显示不了
        while (iter.MoveNext())
        {
            List<string> refSpriteList = iter.Current.Value;
            if (refSpriteList != null && refSpriteList.Count > 0)
            {
                sbre.AppendLine("PrefabName==="+iter.Current.Key);
                foreach (string refPath in refSpriteList)
                {
                    nCount++;
                    sbre.AppendLine(refPath);
                    if (nCount >= nMaxCount)
                    {
                        nCount = 0;
                        Debug.Log(sbre.ToString());
                        sbre = new StringBuilder("");
                    }
                }
            }
            else
            {
                //sbre.AppendLine(iter.Current.Key + ":引用次数 == 0");
            }
        }
        Debug.Log(sbre.ToString());
    }
    #endregion

    #region util方法
    static bool BSelectedObject() {
        //获取所有UILabel组件
        if (Selection.objects == null || Selection.objects.Length == 0) {
            Debug.Log("没选择对象");
            return false;
        }
        return true;
    }

    static void AddSpriteName(string spriteName, Dictionary<string, List<string>> dicSprite, string value) {
        if (!dicSprite.ContainsKey(spriteName))
            return;
        List<string> refSpriteList = dicSprite[spriteName];
        if (refSpriteList == null) { 
            refSpriteList = new List<string>();
            dicSprite[spriteName] = refSpriteList;
        }
        refSpriteList.Add(value);
    }
    #endregion

    #region 添加声音
    [MenuItem("UIUtil/CheckUI/给选中的Go添加Click声音")]
    public static void AddGameObjectSound() {
        if (!BSelectedObject())
            return;
        StringBuilder sb = new StringBuilder("->child:");
        Object[] labels = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);
        foreach (Object item in labels) {
            var go = item as GameObject;
            if (go == null)
                continue;
            SetButtonAnimStyle(go, UnityEngine.UI.Selectable.Transition.Animation, null);
            EditorUtility.SetDirty(item);//重要

            AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/UI/click.wav");
            Button btn = go.GetComponent<Button>();
            AddAudioSource(btn, audioClip, sb);
        }
    }

    [MenuItem("Assets/给选择的prefabButton添加Click声音")]
    public static void AddButtonSound() {
        if (!BSelectedObject())
            return;

        Object[] gos = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);
        foreach (Object item in gos) {
            var go = item as GameObject;
            if (go == null)
                continue;
            AddPrefabButtonSound(go);
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("UIUtil/CheckUI/给所有的UIButton添加Click声音")]
    public static void AddButtonSounds() {
        string[] filePaths = Directory.GetFiles("Assets/UI/Builds/", "*.prefab", SearchOption.AllDirectories);
        foreach (var filePath in filePaths) {
            var fileName = Path.GetFileName(filePath);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            AddPrefabButtonSound(go);
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    static void AddAudioSource(Button compomnt, AudioClip audioClip, StringBuilder sb) {
        var imgGo = compomnt.gameObject;
        Button btn = compomnt;
        UISound uiSound = imgGo.GetComponent<UISound>();
        if (uiSound == null) {
            uiSound = imgGo.AddComponent<UISound>();
            sb.AppendLine(compomnt.name);
        }
        uiSound.Clip = audioClip;
        AudioSource audio = imgGo.GetComponent<AudioSource>();
//         if (audio == null) {
//             audio = imgGo.AddComponent<AudioSource>();
//             sb.AppendLine(compomnt.name);
//         }

        if (audio.clip == null) {
            //audio.clip = audioClip;
            audio.playOnAwake = false;
        } else if (audio.playOnAwake) {
            audio.playOnAwake = false;
        }
//         UnityEngine.UI.Button.ButtonClickedEvent btClickEvent = btn.onClick;
//         int PersCount = btClickEvent.GetPersistentEventCount();
//         if (PersCount == 0) {
//             UnityEventTools.AddPersistentListener(btClickEvent);
//         }
//         UnityEventTools.RegisterPersistentListener(btClickEvent, 0, audio.Play);

        EditorUtility.SetDirty(imgGo);
    }

    public static void AddPrefabButtonSound(GameObject go) {
        bool isDirty = false;
        StringBuilder sb = new StringBuilder("->child:");
        AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/UI/click.wav");
        Component[] Componets = go.GetComponentsInChildren(typeof(Button), true);
        foreach (Component compomnt in Componets) {
            isDirty = true;
            AddAudioSource(compomnt as Button, audioClip, sb);           
        }

        if (isDirty) {
            Debug.LogError("AddPrefabButtonSound.Path:" + go.name + sb.ToString());
            EditorUtility.SetDirty(go);
        } else { 
            Debug.LogError("未找到没有设置声音的按钮");
        }
    }
    #endregion

    [MenuItem("UIUtil/YLZComponents")]
    private static void ShowYLZComponentsWindow() {

        UIComponentsWindow cw = EditorWindow.GetWindow<UIComponentsWindow>(true, "添加游戏自定义UI组件");

    }
}



//////////////windows////////////////
//组件窗口
class UIComponentsWindow : EditorWindow {

    private void OnGUI() {

        //GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        //string assetPath = Application.dataPath;
        string[] filePaths = Directory.GetFiles("Assets/UI/Builds/Common/", "*.prefab");
        foreach (var filePath in filePaths) {
            var fileName = Path.GetFileName(filePath);
            if (GUILayout.Button(fileName, GUILayout.Width(200))) {
                if (Selection.gameObjects == null || Selection.gameObjects.Length == 0) {
                    Debug.Log("没选择对象");
                    return;
                }

                foreach (var goSelect in Selection.gameObjects) {
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                    GameObject goInstance = GameObject.Instantiate(go);
                    goInstance.SetActive(true);
                    goInstance.transform.SetParent(goSelect.transform);
                    goInstance.transform.localPosition = Vector3.zero;
                    break;
                }
            }
        }
        GUILayout.EndVertical();
        //GUILayout.EndHorizontal();

    }
}