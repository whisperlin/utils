using CinemaDirector;
using System;
using System.Timers;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Timeline window that displays and allows edits to cutscenes
/// </summary>
public class DirectorWindow : EditorWindow
{
    private DirectorControl directorControl;
    private Cutscene cutscene;
    private int cutsceneInstanceID;
    private CutsceneWrapper cutsceneWrapper;
    private DateTime previousTime;
    private bool isSnappingEnabled = false;
    private Cutscene[] cachedCutscenes;
    private static Timer timer;

    private bool betaFeaturesEnabled = false;

    #region Language
    private const string URL = "http://www.cinema-suite.com";
    private const string PREVIEW_MODE = "Preview Mode";
    private const string CREATE = "Create";
    private GUIContent new_cutscene = new GUIContent("New Cutscene...");
    private GUIContent default_cutscene = new GUIContent("Default Cutscene");
    private GUIContent empty_cutscene = new GUIContent("Empty Cutscene");
    private GUIContent new_cutscene_trigger = new GUIContent("New Cutscene Trigger");

    const string TITLE = "Director";
    const string MENU_ITEM = "Window/Cinema Suite/Cinema Director/Director %#d";
    #endregion

    #region UI
    private Texture settingsImage = null;
    private Texture rescaleImage = null;
    private Texture zoomInImage = null;
    private Texture zoomOutImage = null;
    private Texture snapImage = null;
    private Texture rollingEditImage = null;
    private Texture rippleEditImage = null;
    private Texture pickerImage = null;
    private Texture refreshImage = null;
    private Texture titleImage = null;
    private Texture cropImage = null;
    private Texture scaleImage = null;

    private const float TOOLBAR_HEIGHT = 17f;
    private const string PRO_SKIN = "Director_LightSkin";
    private const string FREE_SKIN = "Director_DarkSkin";

    private const string SETTINGS_ICON = "Director_SettingsIcon";
    private const string HORIZONTAL_RESCALE_ICON = "Director_HorizontalRescaleIcon";
    private const string PICKER_ICON = "Director_PickerIcon";
    private const string REFRESH_ICON = "Director_RefreshIcon";
    private const string MAGNET_ICON = "Director_Magnet";
    private const string ZOOMIN_ICON = "Director_ZoomInIcon";
    private const string ZOOMOUT_ICON = "Director_ZoomOutIcon";
    private const string TITLE_ICON = "Director_Icon";

    private const float FRAME_LIMITER = 1 / 60f;
    private double accumulatedTime = 0f;
    int popupSelection = 0;
    #endregion

    /// <summary>
    /// Sets the window title and minimum pane size
    /// </summary>
    public void Awake()
    {

#if UNITY_5 && !UNITY_5_0  || UNITY_2017_1_OR_NEWER
        base.titleContent = new GUIContent(TITLE, titleImage);
#else
        base.title = TITLE;
#endif

        this.minSize = new Vector2(400f, 250f);
        loadTextures();

        previousTime = DateTime.Now;
        accumulatedTime = 0;
    }

    /// <summary>
    /// Update the current cutscene
    /// </summary>
    private void DirectorUpdate()
    {
        // Restrict the Repaint rate
        double delta = (DateTime.Now - previousTime).TotalSeconds;
        previousTime = System.DateTime.Now;

        if (delta > 0)
        {
            accumulatedTime += delta;
        }
        if (accumulatedTime >= FRAME_LIMITER)
        {
            base.Repaint();
            accumulatedTime -= FRAME_LIMITER;
        }

        if (delta == 0.0)
            return;

        if (cutscene != null)
        {
            if (!Application.isPlaying && cutscene.State == Cutscene.CutsceneState.PreviewPlaying)
            {
                cutscene.UpdateCutscene((float)delta);
            }
        }
    }

    /// <summary>
    /// Spools the events and loads the timeline control
    /// </summary>
    protected void OnEnable()
    {
        EditorApplication.update = (EditorApplication.CallbackFunction)System.Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.DirectorUpdate));
        EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)System.Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.PlaymodeStateChanged)); //TODO: Depricated in 2017.2


        GUISkin skin = ScriptableObject.CreateInstance<GUISkin>();
        skin = (EditorGUIUtility.isProSkin) ? EditorGUIUtility.Load("Cinema Suite/Cinema Director/" + PRO_SKIN + ".guiskin") as GUISkin : EditorGUIUtility.Load("Cinema Suite/Cinema Director/" + FREE_SKIN + ".guiskin") as GUISkin;
        loadTextures();

#if UNITY_5 && !UNITY_5_0 || UNITY_2017_1_OR_NEWER
        base.titleContent = new GUIContent(TITLE, titleImage);
#else
        base.title = TITLE;
#endif

        directorControl = new DirectorControl();
        directorControl.OnLoad(skin);

        directorControl.PlayCutscene += directorControl_PlayCutscene;
        directorControl.PauseCutscene += directorControl_PauseCutscene;
        directorControl.StopCutscene += directorControl_StopCutscene;
        directorControl.ScrubCutscene += directorControl_ScrubCutscene;
        directorControl.SetCutsceneTime += directorControl_SetCutsceneTime;
        directorControl.EnterPreviewMode += directorControl_EnterPreviewMode;
        directorControl.ExitPreviewMode += directorControl_ExitPreviewMode;
        directorControl.DragPerformed += directorControl_DragPerformed;
        isSnappingEnabled = directorControl.IsSnappingEnabled;

        directorControl.RepaintRequest += directorControl_RepaintRequest;


        previousTime = DateTime.Now;
        accumulatedTime = 0;

        int instanceId = -1;
        if (EditorPrefs.HasKey("DirectorControl.CutsceneID"))
        {
            instanceId = EditorPrefs.GetInt("DirectorControl.CutsceneID");
        }

        if (instanceId >= 0)
        {
            Cutscene[] cutscenes = GameObject.FindObjectsOfType<Cutscene>();
            for (int i = 0; i < cutscenes.Length; i++)
            {
                if (cutscenes[i].GetInstanceID() == instanceId)
                {
                    cutscene = cutscenes[i];
                }
            }
        }

        LoadSettings();
    }

    /// <summary>
    /// Load all user settings and preferences from EditorPrefs.
    /// </summary>
    public void LoadSettings()
    {
        if (EditorPrefs.HasKey("DirectorControl.EnableBetaFeatures"))
        {
            betaFeaturesEnabled = EditorPrefs.GetBool("DirectorControl.EnableBetaFeatures");
        }

        if (EditorPrefs.HasKey("DirectorControl.DefaultTangentMode"))
        {
            //directorControl.DefaultTangentMode = EditorPrefs.GetInt("DirectorControl.DefaultTangentMode");
        }
    }

    void directorControl_RepaintRequest(object sender, CinemaDirectorArgs e)
    {
        base.Repaint();
    }

#region EventHandlers

    void directorControl_DragPerformed(object sender, CinemaDirectorDragArgs e)
    {
        Cutscene c = e.cutscene as Cutscene;
        if (c != null)
        {
            if(e.references != null)
            {
                if(e.references.Length == 1)
                {
                    GameObject gameObject = e.references[0] as GameObject;
                    if(gameObject != null)
                    {
                        ActorTrackGroup atg = CutsceneItemFactory.CreateTrackGroup(c, typeof(ActorTrackGroup), string.Format("{0} Track Group", gameObject.name)) as ActorTrackGroup;
                        atg.Actor = gameObject.GetComponent<Transform>();

                        Undo.RegisterCreatedObjectUndo(atg.gameObject, string.Format("Created {0}", atg.gameObject.name));
                    }
                }
            }
        }
    }

    

    void directorControl_ExitPreviewMode(object sender, CinemaDirectorArgs e)
    {
        Cutscene c = e.cutscene as Cutscene;
        if (c != null)
        {
            c.ExitPreviewMode();
        }
    }

    void directorControl_EnterPreviewMode(object sender, CinemaDirectorArgs e)
    {
        Cutscene c = e.cutscene as Cutscene;
        if (c != null)
        {
            c.EnterPreviewMode();
        }
    }

    void directorControl_SetCutsceneTime(object sender, CinemaDirectorArgs e)
    {
        Cutscene c = e.cutscene as Cutscene;
        if (c != null)
        {
            c.SetRunningTime(e.timeArg);
            cutsceneWrapper.RunningTime = e.timeArg;
        }
    }

    void directorControl_ScrubCutscene(object sender, CinemaDirectorArgs e)
    {
        Cutscene c = e.cutscene as Cutscene;
        if (c != null)
        {
            c.ScrubToTime(e.timeArg);
        }
    }

    void directorControl_StopCutscene(object sender, CinemaDirectorArgs e)
    {
        Cutscene c = e.cutscene as Cutscene;
        if (c != null)
        {
            c.Stop();
        }
    }

    void directorControl_PauseCutscene(object sender, CinemaDirectorArgs e)
    {
        Cutscene c = e.cutscene as Cutscene;
        if (c != null)
        {
            c.Pause();
        }
    }

    void directorControl_PlayCutscene(object sender, CinemaDirectorArgs e)
    {
        Cutscene c = e.cutscene as Cutscene;
        if (c != null)
        {
            if (Application.isPlaying)
            {
                c.Play();
            }
            else
            {
                c.PreviewPlay();
                previousTime = System.DateTime.Now;
            }
        }
    }

#endregion

    /// <summary>
    /// Called when the object is destroyed
    /// </summary>
    protected void OnDestroy()
    {
    }

    /// <summary>
    /// Draws the GUI for the Timeline Window.
    /// </summary>
    protected void OnGUI()
    {
        Rect toolbarArea = new Rect(0, 0, base.position.width, TOOLBAR_HEIGHT);
        Rect controlArea = new Rect(0, TOOLBAR_HEIGHT, base.position.width, base.position.height - TOOLBAR_HEIGHT);

        updateToolbar(toolbarArea);

        cutsceneWrapper = DirectorHelper.UpdateWrapper(cutscene, cutsceneWrapper);
        directorControl.OnGUI(controlArea, cutsceneWrapper);
        DirectorHelper.ReflectChanges(cutscene, cutsceneWrapper);
    }

    /// <summary>
    /// Draw and update the toolbar for the director control
    /// </summary>
    /// <param name="toolbarArea">The area for the toolbar</param>
    private void updateToolbar(Rect toolbarArea)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        // If there are no cutscenes, then only give option to create a new cutscene.
        if (GUILayout.Button(CREATE, EditorStyles.toolbarDropDown, GUILayout.Width(60)))
        {
            GenericMenu createMenu = new GenericMenu();
            createMenu.AddItem(new_cutscene, false, openCutsceneCreatorWindow);
            createMenu.AddItem(default_cutscene, false, createDefaultCutscene);
            createMenu.AddItem(empty_cutscene, false, createEmptyCutscene);


            if (cutscene != null)
            {
                createMenu.AddSeparator(string.Empty);
                createMenu.AddItem(new_cutscene_trigger, false, createCutsceneTrigger);
                createMenu.AddSeparator(string.Empty);

                Type[] subTypes = DirectorHelper.GetAllSubTypes(typeof(TrackGroup));
                for (int i = 0; i < subTypes.Length; i++)
                {
                    TrackGroupContextData userData = getContextDataFromType(subTypes[i]);
                    string text = string.Format(userData.Label);
                    createMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(AddTrackGroup), userData);
                }
            }

            createMenu.DropDown(new Rect(5, TOOLBAR_HEIGHT, 0, 0));
        }

        // Cutscene selector
        cachedCutscenes = GameObject.FindObjectsOfType<Cutscene>();
        if (cachedCutscenes != null && cachedCutscenes.Length > 0)
        {
            // Get cutscene names
            GUIContent[] cutsceneNames = new GUIContent[cachedCutscenes.Length];
            for (int i = 0; i < cachedCutscenes.Length; i++)
            {
                cutsceneNames[i] = new GUIContent(cachedCutscenes[i].name);
            }
            
            // Sort alphabetically
            Array.Sort(cutsceneNames, delegate(GUIContent content1, GUIContent content2)
            {
                return string.Compare(content1.text, content2.text);
            });

            int count = 1;
            // Resolve duplicate names
            for (int i = 0; i < cutsceneNames.Length - 1; i++)
            {
                int next = i + 1;
                while (next < cutsceneNames.Length && string.Compare(cutsceneNames[i].text, cutsceneNames[next].text) == 0)
                {
                    cutsceneNames[next].text = string.Format("{0} (duplicate {1})", cutsceneNames[next].text, count++);
                    next++;
                }
                count = 1;
            }

            Array.Sort(cachedCutscenes, delegate(Cutscene c1, Cutscene c2)
            {
                return string.Compare(c1.name, c2.name);
            });

            // Find the currently selected cutscene.
            for (int i = 0; i < cachedCutscenes.Length; i++)
            {
                if (cutscene != null && cutscene.GetInstanceID() == cachedCutscenes[i].GetInstanceID())
                {
                    popupSelection = i;
                }
            }

            // Show the popup
            int tempPopup = EditorGUILayout.Popup(popupSelection, cutsceneNames, EditorStyles.toolbarPopup);
            if (cutscene == null || tempPopup != popupSelection || cutsceneInstanceID != cachedCutscenes[Math.Min(tempPopup, cachedCutscenes.Length - 1)].GetInstanceID())
            {
                popupSelection = tempPopup;
                popupSelection = Math.Min(popupSelection, cachedCutscenes.Length - 1);
                FocusCutscene(cachedCutscenes[popupSelection]);
            }
        }
        if (cutscene != null)
        {
            if (GUILayout.Button(pickerImage, EditorStyles.toolbarButton, GUILayout.Width(24)))
            {
                Selection.activeObject = cutscene;
            }
            if (GUILayout.Button(refreshImage, EditorStyles.toolbarButton, GUILayout.Width(24)))
            {
                cutscene.Refresh();
            }

            if (Event.current.control && Event.current.keyCode == KeyCode.Space)
            {
                cutscene.Refresh();
                Event.current.Use();
            }
        }
        GUILayout.FlexibleSpace();

        if (betaFeaturesEnabled)
        {
            Texture resizeTexture = cropImage;
            if (directorControl.ResizeOption == DirectorEditor.ResizeOption.Scale)
            {
                resizeTexture = scaleImage;
            }
            Rect resizeRect = GUILayoutUtility.GetRect(new GUIContent(resizeTexture), EditorStyles.toolbarDropDown, GUILayout.Width(32));
            if (GUI.Button(resizeRect, new GUIContent(resizeTexture, "Resize Option"), EditorStyles.toolbarDropDown))
            {
                GenericMenu resizeMenu = new GenericMenu();

                string[] names = Enum.GetNames(typeof(DirectorEditor.ResizeOption));

                for (int i = 0; i < names.Length; i++)
                {
                    resizeMenu.AddItem(new GUIContent(names[i]), directorControl.ResizeOption == (DirectorEditor.ResizeOption)i, chooseResizeOption, i);
                }

                resizeMenu.DropDown(new Rect(resizeRect.x, TOOLBAR_HEIGHT, 0, 0));
            }
        }

        bool tempSnapping = GUILayout.Toggle(isSnappingEnabled, snapImage, EditorStyles.toolbarButton, GUILayout.Width(24));
        if (tempSnapping != isSnappingEnabled)
        {
            isSnappingEnabled = tempSnapping;
            directorControl.IsSnappingEnabled = isSnappingEnabled;
        }

        //GUILayout.Button(rollingEditImage, EditorStyles.toolbarButton, GUILayout.Width(24));
        GUILayout.Space(10f);

        if (GUILayout.Button(rescaleImage, EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            directorControl.Rescale();
        }
        if (GUILayout.Button(new GUIContent(zoomInImage, "Zoom In"), EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            directorControl.ZoomIn();
        }
        if (GUILayout.Button(zoomOutImage, EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            directorControl.ZoomOut();
        }
        GUILayout.Space(10f);

        Color temp = GUI.color;
        GUI.color = directorControl.InPreviewMode ? Color.red : temp;
        directorControl.InPreviewMode = GUILayout.Toggle(directorControl.InPreviewMode, PREVIEW_MODE, EditorStyles.toolbarButton, GUILayout.Width(150));
        GUI.color = temp;
        GUILayout.Space(10);

        if (GUILayout.Button(settingsImage, EditorStyles.toolbarButton, GUILayout.Width(30)))
        {
            EditorWindow.GetWindow<DirectorSettingsWindow>();
        }
        // Check if the Welcome Window exists and if so, show an icon for it.
        var helpWindowType = Type.GetType("CinemaSuite.CinemaSuiteWelcome");
        if (helpWindowType != null)
        {
            if (GUILayout.Button(new GUIContent("?", "Help"), EditorStyles.toolbarButton))
            {
                EditorWindow.GetWindow(helpWindowType);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void chooseResizeOption(object userData)
    {
        int selection = (int) userData;

        directorControl.ResizeOption = (DirectorEditor.ResizeOption)selection;
    }

    public void FocusCutscene(Cutscene cutscene)
    {
        if (this.cutscene != null)
        {
            this.cutscene.ExitPreviewMode();
        }
        directorControl.InPreviewMode = false;

        EditorPrefs.SetInt("DirectorControl.CutsceneID", cutscene.GetInstanceID());
        cutsceneInstanceID = cutscene.GetInstanceID();
        this.cutscene = cutscene;
    }

    /// <summary>
    /// Handle the playmode state changed event.
    /// </summary>
    public void PlaymodeStateChanged()
    {
        directorControl.InPreviewMode = false;
    }

    /// <summary>
    /// Save user preferences
    /// </summary>
    public void OnDisable()
    {
        directorControl.OnDisable();
        if (Application.isEditor && cutscene != null)
        {
            cutscene.ExitPreviewMode();
        }

        if (cutscene != null)
        {
            EditorPrefs.SetInt("DirectorControl.CutsceneID", cutscene.GetInstanceID());
        }
    }

    /// <summary>
    /// Load textures from Resources folder.
    /// </summary>
    private void loadTextures()
    {
        string dir = "Cinema Suite/Cinema Director/";
        string suffix = EditorGUIUtility.isProSkin ? "_Light" : "_Dark";
        string filetype_png = ".png";
        string missing = " is missing from Resources folder.";

        settingsImage = EditorGUIUtility.Load(dir + SETTINGS_ICON + suffix + filetype_png)  as Texture;
        if (settingsImage == null)
        {
            Debug.Log(SETTINGS_ICON + suffix + missing);
        }

        rescaleImage = EditorGUIUtility.Load(dir + HORIZONTAL_RESCALE_ICON + suffix + filetype_png) as Texture;
        if (rescaleImage == null)
        {
            Debug.Log(HORIZONTAL_RESCALE_ICON + suffix + missing);
        }

        zoomInImage = EditorGUIUtility.Load(dir + ZOOMIN_ICON + suffix + filetype_png) as Texture;
        if (zoomInImage == null)
        {
            Debug.Log(ZOOMIN_ICON + suffix + missing);
        }

        zoomOutImage = EditorGUIUtility.Load(dir + ZOOMOUT_ICON + suffix + filetype_png) as Texture;
        if (zoomOutImage == null)
        {
            Debug.Log(ZOOMOUT_ICON + suffix + missing);
        }

        snapImage = EditorGUIUtility.Load(dir + MAGNET_ICON + suffix + filetype_png) as Texture;
        if (snapImage == null)
        {
            Debug.Log(MAGNET_ICON + suffix + missing);
        }

        rollingEditImage = EditorGUIUtility.Load(dir + "Director_RollingIcon" + filetype_png) as Texture;
        if (rollingEditImage == null)
        {
            Debug.Log("Rolling edit icon missing from Resources folder.");
        }
        
        rippleEditImage = EditorGUIUtility.Load(dir + "Director_RippleIcon" + filetype_png) as Texture;
        if (rippleEditImage == null)
        {
            Debug.Log("Ripple edit icon missing from Resources folder.");
        }

        pickerImage = EditorGUIUtility.Load(dir + PICKER_ICON + suffix + filetype_png) as Texture;
        if (pickerImage == null)
        {
            Debug.Log(PICKER_ICON + suffix + missing);
        }

        refreshImage = EditorGUIUtility.Load(dir + REFRESH_ICON + suffix + filetype_png) as Texture;
        if (refreshImage == null)
        {
            Debug.Log(REFRESH_ICON+suffix+missing);
        }

        titleImage = EditorGUIUtility.Load(dir + TITLE_ICON + suffix + filetype_png) as Texture;
        if (titleImage == null)
        {
            Debug.Log(TITLE_ICON + suffix + missing);
        }

        cropImage = EditorGUIUtility.Load(dir + "Director_Resize_Crop" + suffix + filetype_png) as Texture;
        if (cropImage == null)
        {
            Debug.Log("Director_Resize_Crop" + suffix + missing);
        }

        scaleImage = EditorGUIUtility.Load(dir + "Director_Resize_Scale" + suffix + filetype_png) as Texture;
        if (scaleImage == null)
        {
            Debug.Log("Director_Resize_Crop" + suffix + missing);
        }
    }

    /// <summary>
    /// Add a new track group to the current cutscene.
    /// </summary>
    /// <param name="userData">TrackGroupContextData containing track group label and type</param>
    private void AddTrackGroup(object userData)
    {
        TrackGroupContextData data = userData as TrackGroupContextData;
        if (data != null)
        {
            GameObject item = CutsceneItemFactory.CreateTrackGroup(cutscene, data.Type, data.Label).gameObject;
            Undo.RegisterCreatedObjectUndo(item, string.Format("Create {0}", item.name));
        }
    }

    private TrackGroupContextData getContextDataFromType(Type type)
    {
        string label = string.Empty;
        object[] attrs = type.GetCustomAttributes(typeof(TrackGroupAttribute), true);
        for (int i = 0; i < attrs.Length; i++)
        {
            TrackGroupAttribute attribute = attrs[i] as TrackGroupAttribute;
            if (attribute != null)
            {
                label = attribute.Label;
                break;
            }
        }
        TrackGroupContextData userData = new TrackGroupContextData { Type = type, Label = label };
        return userData;
    }

    /// <summary>
    /// Show the TimelineWindow.
    /// </summary>
    [MenuItem(MENU_ITEM, false, 10)]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DirectorWindow));

        // Check if we should show the welcome window
        bool showWelcome = true;
        if (EditorPrefs.HasKey("CinemaSuite.WelcomeWindow.ShowOnStartup"))
        {
            showWelcome = EditorPrefs.GetBool("CinemaSuite.WelcomeWindow.ShowOnStartup");
        }

        if (showWelcome)
        {
            // Check if the Welcome Window exists and if so, show it.
            var helpWindowType = Type.GetType("CinemaSuite.CinemaSuiteWelcome");
            if (helpWindowType != null)
            {
                EditorWindow.GetWindow(helpWindowType);
            }
        }
    }

    /// <summary>
    /// Opens the Cutscene creator window.
    /// </summary>
    internal void openCutsceneCreatorWindow()
    {
        EditorWindow.GetWindow<CutsceneCreatorWindow>();
    }

    internal void createDefaultCutscene()
    {
        CutsceneCreatorWindow ccw = CreateInstance<CutsceneCreatorWindow>();
        ccw.CreateCutscene();
    }

    internal void createEmptyCutscene()
    {
        CutsceneCreatorWindow ccw = CreateInstance<CutsceneCreatorWindow>();
        ccw.CreateEmptyCutscene();
    }

    internal void createCutsceneTrigger()
    {
        CutsceneCreatorWindow ccw = CreateInstance<CutsceneCreatorWindow>();
        Cutscene cs = cutscene != null ? cutscene : null; // explicitly set to null if cutscene is missing
        CutsceneTrigger ct = ccw.CreateCutsceneTrigger(cs);

        Selection.activeGameObject = ct.gameObject;
    }

    public bool BetaFeaturesEnabled
    {
        get 
        { 
            return betaFeaturesEnabled; 
        }
        set 
        { 
            betaFeaturesEnabled = value; 
        }
    }

    private class TrackGroupContextData
    {
        public Type Type;
        public string Label;
    }
}

