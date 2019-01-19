using UnityEngine;
using UnityEditor;
using System.Xml;

public partial class LayaAir3D : EditorWindow
{
	//partial
    private static string version;

    enum ConfigEnum
    {
        config1 = 0,
        config2 = 1,
        config3 = 2,
        config4 = 3,
        config5 = 4
    }

    enum TargetTextureTypeEnum
    {
        jpg = 0,
        png = 1
    }

    enum TerrainToMeshResolution
    {
        VeryHeight = 2,
        Height = 4,
        Medium = 8,
        Low = 16,
        VeryLow = 32
    }

    private static ConfigEnum configEnum;
    public static int curConfigIndex = 1;
    private static int lastConfigIndex;

    private static int FirstlevelMenu;

    private static Vector2 ScrollPosition;

    private static bool MeshSetting;
    private static bool IgnoreVerticesUV;
    private static bool IgnoreVerticesNormal;
    private static bool IgnoreVerticesTangent;
    private static bool IgnoreVerticesColor;

    private static bool TextureSetting;
    private static bool ConvertTexture;
    private static bool ConvertNonPNGAndJPG;
    private static bool ConvertOriginPNG;
    private static bool ConvertOriginJPG;
    private static TargetTextureTypeEnum targetTextureTypeEnum;
    private static float ConvertQuality = 50.0f;
    private static float ConvertMaxQuality = 100.0f;

    private static bool TerrainSetting;
    private static bool ConvertTerrain;
    private static TerrainToMeshResolution terrainToMeshResolution;

    private static bool GameObjectSetting;
  
    private static bool IgnoreNotActiveGameObject;
 
    private static bool BatchMade;

    private static bool OtherSetting;
    //private static bool CoverOriginalFile;
    private static bool CustomizeDirectory;
    private static string CustomizeDirectoryName = "";
    private static bool AutoSave;

    //CompressTexture
    //miner
    private static bool CompressTexture;
    public static bool Android = false;
    public static bool Ios = false;
    private static bool Conventional = true;
    //——————————

    private static string SAVEPATH = "";

    private static bool OptimizeMeshName = true;
    private static float ScaleFactor = 1.0f;

    [MenuItem("LayaAir3D/Export Tool", false, 1)]
    static void initLayaExport()
    {
        LayaAir3D layaWindow = (LayaAir3D)EditorWindow.GetWindow(typeof(LayaAir3D));
        WWW w = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/layabox.png");
        GUIContent titleContent = new GUIContent(" LayaAir3D", w.texture);
        layaWindow.titleContent = titleContent;
        layaWindow.Show(true);
        readConfiguration(true);
    }

    
    [MenuItem("LayaAir3D/Shortcuts/Switch to LayaAir3D Shader", false)]
    static void initLayaShader()
    {
        LayaExport.DataManager.SwitchToLayaShader();
    }

    [MenuItem("LayaAir3D/Help/Demo", false)]
    static void initLayaDemo()
    {
        Application.OpenURL("https://layaair.ldc.layabox.com/demo");
    }

    [MenuItem("LayaAir3D/Help/Study")]
    static void initLayaStudy()
    {
        Application.OpenURL("https://ldc.layabox.com/doc");
    }

    [MenuItem("LayaAir3D/Help/Answsers")]
    static void initLayaAsk()
    {
        Application.OpenURL("https://ask.layabox.com");
    }

    [MenuItem("LayaAir3D/Help/About LayaAir")]
    static void initLayaAir()
    {
        Application.OpenURL("https://www.layabox.com");
    }

   
    void OnGUI()
    {

        //是否完成了纹理压缩
        EditorGUI.BeginDisabledGroup(HTTPClient.LoadingTexture!=0);

        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(10));
        FirstlevelMenu = GUILayout.Toolbar(FirstlevelMenu, new string[] { "Scene", "Sprite3D" }, GUILayout.Height(30));
        configEnum = (ConfigEnum)EditorGUILayout.EnumPopup("", configEnum, GUILayout.Width(60));
        

        GUILayout.EndHorizontal();

        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);

        GUILayout.Box("", GUILayout.Height(3), GUILayout.ExpandWidth(true));

        //---------------------------------------MeshSetting------------------------------------------
        MeshSetting = GUILayout.Toggle(MeshSetting, " Mesh Setting");

        if (MeshSetting)
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(15));
            IgnoreVerticesUV = GUILayout.Toggle(IgnoreVerticesUV, " Ignore Vertices UV");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(15));
            IgnoreVerticesColor = GUILayout.Toggle(IgnoreVerticesColor, " Ignore Vertices Color");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(15));
            IgnoreVerticesNormal = GUILayout.Toggle(IgnoreVerticesNormal, " Ignore Vertices Normal");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(15));
            IgnoreVerticesTangent = GUILayout.Toggle(IgnoreVerticesTangent, " Ignore Vertices Tangent");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        //---------------------------------------MeshSetting------------------------------------------

        GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));


        //---------------------------------------TerrainSetting------------------------------------------

        TerrainSetting = GUILayout.Toggle(TerrainSetting, " Terrain Setting");
        if (TerrainSetting)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(15));
            ConvertTerrain = GUILayout.Toggle(ConvertTerrain, " Convert Terrain To Mesh");
            GUILayout.EndHorizontal();

            if (ConvertTerrain)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(30));
                terrainToMeshResolution = (TerrainToMeshResolution)EditorGUILayout.EnumPopup(" Resolution", terrainToMeshResolution);
                GUILayout.EndHorizontal();
            }
        }
        //---------------------------------------TerrainSetting------------------------------------------

        GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
        //---------------------------------------GameObjectSetting------------------------------------------
        GameObjectSetting = GUILayout.Toggle(GameObjectSetting, " GameObject Setting");
        if (GameObjectSetting)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(15));
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        
            IgnoreNotActiveGameObject = GUILayout.Toggle(IgnoreNotActiveGameObject, " Ignore Not Active Game Objects");
           
            if (FirstlevelMenu == 1)
            {
                BatchMade = GUILayout.Toggle(BatchMade, " Batch Make The First Level Game Objects");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        //---------------------------------------GameObjectSetting------------------------------------------
        //---------------------------------------CompressTexture---------------------------------------
       
        //miner
        GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
        CompressTexture = GUILayout.Toggle(CompressTexture, " Assets platform");
        if (CompressTexture)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(15));
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            
            EditorGUI.BeginDisabledGroup(!HTTPClient.vip);
            Android = GUILayout.Toggle(Android, "Android(Need Year VIP)");
            Ios = GUILayout.Toggle(Ios, "Ios（Need Year VIP）");
            if (!HTTPClient.vip)
            {
                Android = false;
                Ios = false;
            }
            EditorGUI.EndDisabledGroup();

            Conventional = GUILayout.Toggle(Conventional, " Conventional");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
        //---------------------------------------OtherSetting------------------------------------------
        OtherSetting = GUILayout.Toggle(OtherSetting, " Other Setting");
        if (OtherSetting)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(15));
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            //CoverOriginalFile = GUILayout.Toggle(CoverOriginalFile, " Cover Original Export Files");
            GUILayout.BeginHorizontal();
            CustomizeDirectory = GUILayout.Toggle(CustomizeDirectory, " Customize Export Root Directory Name",GUILayout.Width(250));
            if (CustomizeDirectory)
                CustomizeDirectoryName = GUILayout.TextField(CustomizeDirectoryName);
            GUILayout.EndHorizontal();
            AutoSave = GUILayout.Toggle(AutoSave, " Automatically Save The Configuration");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
       

        //---------------------------------------OtherSetting------------------------------------------

        GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Save Path", GUILayout.Width(65));
        SAVEPATH = GUILayout.TextField(SAVEPATH, GUILayout.Height(21));
        if (GUILayout.Button("Browse", GUILayout.MaxWidth(100), GUILayout.Height(22)))
        {
            SAVEPATH = EditorUtility.SaveFolderPanel("LayaUnityPlugin", "Assets", "");
        }
        GUILayout.EndHorizontal();

        GUILayout.Box("", GUILayout.Height(3), GUILayout.ExpandWidth(true));

        GUILayout.BeginHorizontal();
        GUILayout.Space(2f);
        if (GUILayout.Button("Clear Config", GUILayout.Height(22)))
        {
            clearConfiguration();
        }

        if (GUILayout.Button("Revert Config", GUILayout.Height(22)))
        {
            readConfiguration(false);
        }

        if (GUILayout.Button("Save Config", GUILayout.Height(22)))
        {
            saveConfiguration();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(2f);

        GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));

        GUILayout.Space(2f);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("LayaAir Run", GUILayout.Height(26)))
        {

            LayaExport.DataManager.Type = 0;
            exportResource(true, 0,"/Conventional");
                runLayaDemo();
            
           
        }
        if (GUILayout.Button("LayaAir Export", GUILayout.Height(26)))
        {
            LayaExport.DataManager.Type = FirstlevelMenu;
            if (Ios)
            {
                exportResource(false, 1,"/IOS");    
               
            }
            if (Android)
            {
                exportResource(false, 2,"/Android");
               
            }
            if (Conventional)
            {
                exportResource(false, 0,"/Conventional");
                
            }
        }

     
        GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		OnExtendGUI ();

		GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        EditorGUI.EndDisabledGroup();
        
    }

    private void Update()
    {
        version = LayaExport.DataManager.VERSION;

        if (configEnum == ConfigEnum.config1)
            curConfigIndex = 1;
        else if(configEnum == ConfigEnum.config2)
            curConfigIndex = 2;
        else if (configEnum == ConfigEnum.config3)
            curConfigIndex = 3;
        else if (configEnum == ConfigEnum.config4)
            curConfigIndex = 4;
        else if (configEnum == ConfigEnum.config5)
            curConfigIndex = 5;

        if(lastConfigIndex != curConfigIndex)
        {
            readConfiguration(false);
            lastConfigIndex = curConfigIndex;
        }
    }

    private static void exportResource(bool isDebug, int Platformindex, string lastname)
    {
        Debug.Log(" -- LayaAir3D UnityPlugin " + version + " -- ");
        //Debug.Log(Application.unityVersion);

        if (SAVEPATH != null && SAVEPATH.Length != 0 || isDebug)
        {
            if (AutoSave)
            {
                saveConfiguration();
            }

           

            LayaExport.DataManager.IgnoreUV = IgnoreVerticesUV;
            LayaExport.DataManager.IgnoreNormal = IgnoreVerticesNormal;
            LayaExport.DataManager.IgnoreTangent = IgnoreVerticesTangent;
            LayaExport.DataManager.IgnoreColor = IgnoreVerticesColor;

            LayaExport.DataManager.ConvertNonPNGAndJPG = ConvertNonPNGAndJPG;
            LayaExport.DataManager.ConvertOriginPNG = ConvertOriginPNG;
            LayaExport.DataManager.ConvertOriginJPG = ConvertOriginJPG;

            if (targetTextureTypeEnum == TargetTextureTypeEnum.jpg)
            {
                LayaExport.DataManager.ConvertToPNG = false;
                LayaExport.DataManager.ConvertToJPG = true;
            }
            else
            {
                LayaExport.DataManager.ConvertToPNG = true;
                LayaExport.DataManager.ConvertToJPG = false;
            }

            LayaExport.DataManager.ConvertQuality = ConvertQuality;

            LayaExport.DataManager.ConvertTerrainToMesh = ConvertTerrain;
            LayaExport.DataManager.TerrainToMeshResolution = (int)terrainToMeshResolution;


            LayaExport.DataManager.IgnoreNotActiveGameObject = IgnoreNotActiveGameObject;
           
            LayaExport.DataManager.BatchMade = BatchMade;

            //LayaExport.DataManager.CoverOriginalFile = CoverOriginalFile;
            LayaExport.DataManager.CustomizeDirectory = CustomizeDirectory;
            LayaExport.DataManager.CustomizeDirectoryName = CustomizeDirectoryName;

            LayaExport.DataManager.OptimizeMeshName = OptimizeMeshName;
            LayaExport.DataManager.ScaleFactor = ScaleFactor;

            //miner
            LayaExport.DataManager.Android = Android;
            LayaExport.DataManager.Ios = Ios;
            LayaExport.DataManager.Conventional = Conventional;
            if (isDebug)
            {
                LayaExport.DataManager.ConvertNonPNGAndJPG = true;
                LayaExport.DataManager.ConvertToPNG = true;
                LayaExport.DataManager.ConvertToJPG = false;

                LayaExport.DataManager.SAVEPATH = Application.dataPath + "/StreamingAssets/LayaDemo/res";
                LayaExport.DataManager.BatchMade = false;
            }
            else
            {
                LayaExport.DataManager.SAVEPATH = SAVEPATH;
            }
			Debug.Log (LayaExport.DataManager.Platformindex);
            LayaExport.DataManager.Platformindex = Platformindex;
		 
			Debug.Log (lastname);
            LayaExport.DataManager.getData(lastname);
            LayaExport.DataManager.textureInfo.Clear();
        }
        else
        {
            Debug.LogWarning("LayaUnityPlugin : Please check exporting path !");
        }
    }

    private static void clearConfiguration()
    {
        FirstlevelMenu = 0;

        MeshSetting = false;
        IgnoreVerticesUV = false;
        IgnoreVerticesNormal = false;
        IgnoreVerticesTangent = false;
        IgnoreVerticesColor = false;

        TextureSetting = false;
        ConvertTexture = false;
        ConvertNonPNGAndJPG = false;
        ConvertOriginPNG = false;
        ConvertOriginJPG = false;
        targetTextureTypeEnum = TargetTextureTypeEnum.jpg;
        ConvertQuality = 50.0f;

        TerrainSetting = false;
        ConvertTerrain = false;
        terrainToMeshResolution = TerrainToMeshResolution.Medium;

        GameObjectSetting = false;
 
        IgnoreNotActiveGameObject = false;
      
        BatchMade = false;

      

        OtherSetting = false;
        //CoverOriginalFile = true;
        CustomizeDirectory = false;
        CustomizeDirectoryName = "";
        AutoSave = true;

        SAVEPATH = "";

        ScrollPosition.y = 0.0f;
        //miner
        Ios = false;
        Android = false;
        Conventional = true;
    }

    private static void readConfiguration(bool readConfig)
    {

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load("Assets/LayaAir3D/LayaTool/Configuration.xml");
        XmlNode xn = xmlDoc.SelectSingleNode("LayaExportSetting");
        XmlNodeList xnList = xn.ChildNodes;

        if (readConfig)
        {
            switch (int.Parse(xnList[0].InnerText))
            {
                case 1:
                    configEnum = ConfigEnum.config1;
                    break;
                case 2:
                    configEnum = ConfigEnum.config2;
                    break;
                case 3:
                    configEnum = ConfigEnum.config3;
                    break;
                case 4:
                    configEnum = ConfigEnum.config4;
                    break;
                case 5:
                    configEnum = ConfigEnum.config5;
                    break;
                default:
                    break;
            }
            lastConfigIndex = curConfigIndex = int.Parse(xnList[0].InnerText);
        }

        FirstlevelMenu = int.Parse(xnList[curConfigIndex].SelectSingleNode("FirstlevelMenu").InnerText);

        MeshSetting = bool.Parse(xnList[curConfigIndex].SelectSingleNode("MeshSetting").InnerText);
        IgnoreVerticesUV = bool.Parse(xnList[curConfigIndex].SelectSingleNode("IgnoreVerticesUV").InnerText);
        IgnoreVerticesNormal = bool.Parse(xnList[curConfigIndex].SelectSingleNode("IgnoreVerticesNormal").InnerText);
        IgnoreVerticesTangent = bool.Parse(xnList[curConfigIndex].SelectSingleNode("IgnoreVerticesTangent").InnerText);
        IgnoreVerticesColor = bool.Parse(xnList[curConfigIndex].SelectSingleNode("IgnoreVerticesColor").InnerText);

        TextureSetting = bool.Parse(xnList[curConfigIndex].SelectSingleNode("TextureSetting").InnerText);
        ConvertTexture = bool.Parse(xnList[curConfigIndex].SelectSingleNode("ConvertTexture").InnerText);
        ConvertNonPNGAndJPG = bool.Parse(xnList[curConfigIndex].SelectSingleNode("ConvertNonPNGAndJPG").InnerText);
        ConvertOriginPNG = bool.Parse(xnList[curConfigIndex].SelectSingleNode("ConvertOriginPNG").InnerText);
        ConvertOriginJPG = bool.Parse(xnList[curConfigIndex].SelectSingleNode("ConvertOriginJPG").InnerText);
        targetTextureTypeEnum = int.Parse(xnList[curConfigIndex].SelectSingleNode("ConvertToType").InnerText) == 0 ? TargetTextureTypeEnum.jpg : TargetTextureTypeEnum.png;
        ConvertQuality = float.Parse(xnList[curConfigIndex].SelectSingleNode("ConvertQuality").InnerText);

        TerrainSetting = bool.Parse(xnList[curConfigIndex].SelectSingleNode("TerrainSetting").InnerText);
        ConvertTerrain = bool.Parse(xnList[curConfigIndex].SelectSingleNode("ConvertTerrain").InnerText);
        switch (int.Parse(xnList[curConfigIndex].SelectSingleNode("TerrainToMeshResolution").InnerText))
        {
            case 2:
                terrainToMeshResolution = TerrainToMeshResolution.VeryHeight;
                break;
            case 4:
                terrainToMeshResolution = TerrainToMeshResolution.Height;
                break;
            case 8:
                terrainToMeshResolution = TerrainToMeshResolution.Medium;
                break;
            case 16:
                terrainToMeshResolution = TerrainToMeshResolution.Low;
                break;
            case 32:
                terrainToMeshResolution = TerrainToMeshResolution.VeryLow;
                break;
            default:
                terrainToMeshResolution = TerrainToMeshResolution.Medium;
                break;
        }

        GameObjectSetting = bool.Parse(xnList[curConfigIndex].SelectSingleNode("GameObjectSetting").InnerText);
       
        IgnoreNotActiveGameObject = bool.Parse(xnList[curConfigIndex].SelectSingleNode("IgnoreNotActiveGameObject").InnerText);
       
        BatchMade = bool.Parse(xnList[curConfigIndex].SelectSingleNode("BatchMade").InnerText);

        OtherSetting = bool.Parse(xnList[curConfigIndex].SelectSingleNode("OtherSetting").InnerText);
       // CoverOriginalFile = bool.Parse(xnList[curConfigIndex].SelectSingleNode("CoverOriginalFile").InnerText);
        CustomizeDirectory = bool.Parse(xnList[curConfigIndex].SelectSingleNode("CustomizeDirectory").InnerText);
        CustomizeDirectoryName = xnList[curConfigIndex].SelectSingleNode("CustomizeDirectoryName").InnerText;
        AutoSave = bool.Parse(xnList[curConfigIndex].SelectSingleNode("AutoSave").InnerText);


        Ios = bool.Parse(xnList[curConfigIndex].SelectSingleNode("Ios").InnerText);
        Android = bool.Parse(xnList[curConfigIndex].SelectSingleNode("Android").InnerText);
        Conventional = bool.Parse(xnList[curConfigIndex].SelectSingleNode("Conventional").InnerText);

        SAVEPATH = xnList[curConfigIndex].SelectSingleNode("SavePath").InnerText;

        ScrollPosition.y = float.Parse(xnList[curConfigIndex].SelectSingleNode("ScrollPositionY").InnerText);

    }

    private static void saveConfiguration()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load("Assets/LayaAir3D/LayaTool/Configuration.xml");
        XmlNode xn = xmlDoc.SelectSingleNode("LayaExportSetting");
        XmlNodeList xnList = xn.ChildNodes;

        xnList[0].InnerText = curConfigIndex.ToString();

        xnList[curConfigIndex].SelectSingleNode("FirstlevelMenu").InnerText = FirstlevelMenu.ToString();

        xnList[curConfigIndex].SelectSingleNode("MeshSetting").InnerText = MeshSetting.ToString();

        xnList[curConfigIndex].SelectSingleNode("IgnoreVerticesUV").InnerText = IgnoreVerticesUV.ToString();
        xnList[curConfigIndex].SelectSingleNode("IgnoreVerticesNormal").InnerText = IgnoreVerticesNormal.ToString();
        xnList[curConfigIndex].SelectSingleNode("IgnoreVerticesTangent").InnerText = IgnoreVerticesTangent.ToString();
        xnList[curConfigIndex].SelectSingleNode("IgnoreVerticesColor").InnerText = IgnoreVerticesColor.ToString();

        xnList[curConfigIndex].SelectSingleNode("TextureSetting").InnerText = TextureSetting.ToString();
        xnList[curConfigIndex].SelectSingleNode("ConvertTexture").InnerText = ConvertTexture.ToString();
        xnList[curConfigIndex].SelectSingleNode("ConvertNonPNGAndJPG").InnerText = ConvertNonPNGAndJPG.ToString();
        xnList[curConfigIndex].SelectSingleNode("ConvertOriginPNG").InnerText = ConvertOriginPNG.ToString();
        xnList[curConfigIndex].SelectSingleNode("ConvertOriginJPG").InnerText = ConvertOriginJPG.ToString();
        xnList[curConfigIndex].SelectSingleNode("ConvertToType").InnerText = (targetTextureTypeEnum == TargetTextureTypeEnum.jpg ? 0 : 1).ToString();
        xnList[curConfigIndex].SelectSingleNode("ConvertQuality").InnerText = ConvertQuality.ToString();

        xnList[curConfigIndex].SelectSingleNode("TerrainSetting").InnerText = TerrainSetting.ToString();
        xnList[curConfigIndex].SelectSingleNode("ConvertTerrain").InnerText = ConvertTerrain.ToString();
        if (terrainToMeshResolution == TerrainToMeshResolution.VeryHeight)
        {
            xnList[curConfigIndex].SelectSingleNode("TerrainToMeshResolution").InnerText = 2.ToString();
        }
        else if (terrainToMeshResolution == TerrainToMeshResolution.Height)
        {
            xnList[curConfigIndex].SelectSingleNode("TerrainToMeshResolution").InnerText = 4.ToString();
        }
        else if (terrainToMeshResolution == TerrainToMeshResolution.Medium)
        {
            xnList[curConfigIndex].SelectSingleNode("TerrainToMeshResolution").InnerText = 8.ToString();
        }
        else if (terrainToMeshResolution == TerrainToMeshResolution.Low)
        {
            xnList[curConfigIndex].SelectSingleNode("TerrainToMeshResolution").InnerText = 16.ToString();
        }
        else if (terrainToMeshResolution == TerrainToMeshResolution.VeryLow)
        {
            xnList[curConfigIndex].SelectSingleNode("TerrainToMeshResolution").InnerText = 32.ToString();
        }

        xnList[curConfigIndex].SelectSingleNode("GameObjectSetting").InnerText = GameObjectSetting.ToString();
        
        xnList[curConfigIndex].SelectSingleNode("IgnoreNotActiveGameObject").InnerText = IgnoreNotActiveGameObject.ToString();
       
        xnList[curConfigIndex].SelectSingleNode("BatchMade").InnerText = BatchMade.ToString();

        xnList[curConfigIndex].SelectSingleNode("OtherSetting").InnerText = OtherSetting.ToString();
        //xnList[curConfigIndex].SelectSingleNode("CoverOriginalFile").InnerText = CoverOriginalFile.ToString();
        xnList[curConfigIndex].SelectSingleNode("CustomizeDirectory").InnerText = CustomizeDirectory.ToString();
        xnList[curConfigIndex].SelectSingleNode("CustomizeDirectoryName").InnerText = CustomizeDirectoryName;
        xnList[curConfigIndex].SelectSingleNode("AutoSave").InnerText = AutoSave.ToString();

        xnList[curConfigIndex].SelectSingleNode("Ios").InnerText = Ios.ToString();
        xnList[curConfigIndex].SelectSingleNode("Android").InnerText = Android.ToString();
        xnList[curConfigIndex].SelectSingleNode("Conventional").InnerText = Conventional.ToString();


        xnList[curConfigIndex].SelectSingleNode("SavePath").InnerText = SAVEPATH;

        xnList[curConfigIndex].SelectSingleNode("ScrollPositionY").InnerText = ScrollPosition.y.ToString();

       
        xmlDoc.Save("Assets/LayaAir3D/LayaTool/Configuration.xml");
    }

    private static void runLayaDemo()
    {
#if UNITY_STANDALONE_OSX
            Application.OpenURL(Application.dataPath + "/StreamingAssets/startServer_mac.sh"); 
#else
		Application.OpenURL(Application.dataPath + "/StreamingAssets/startServer_win.bat");
#endif

 

        Application.OpenURL("http://127.0.0.1:9999");
    }
}
