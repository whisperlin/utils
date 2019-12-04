using CinemaDirector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CutsceneCreatorWindow : EditorWindow
{
    #region UI Fields
    private string txtCutsceneName = "Cutscene";
    private float txtDuration = 30;
    private DirectorHelper.TimeEnum timeEnum = DirectorHelper.TimeEnum.Seconds;
    private bool isLooping = false;
    private bool isSkippable = true;
    private Vector2 scrollPosition = new Vector2();
    private StartMethod StartMethod = StartMethod.None;

    private int directorTrackGroupsSelection = 1;
    private int actorTrackGroupsSelection = 0;
    private int multiActorTrackGroupsSelection = 0;
    private int characterTrackGroupsSelection = 0;

    private int shotTrackSelection = 1;
    private int audioTrackSelection = 2;
    private int globalItemTrackSelection = 0;

    private List<GUIContent> intValues1 = new List<GUIContent>();
    private List<GUIContent> intValues4 = new List<GUIContent>();
    private List<GUIContent> intValues10 = new List<GUIContent>();

    private Transform[] actors = new Transform[0];
    private Transform[] characters = new Transform[0];
    #endregion
    
    #region Language

    const string TITLE = "Creator";
    const string NAME_DUPLICATE = "Cutscene with that name already exists";
    
    GUIContent NameContentCutscene = new GUIContent("Name", "The name of the Cutscene to be created");
    GUIContent DurationContentCutscene = new GUIContent("Duration", "The duration of the Cutscene in seconds/minutes");
    GUIContent LoopingContentCutscene = new GUIContent("Looping", "Will the Cutscene loop.");
    GUIContent SkippableContentCutscene = new GUIContent("Skippable", "Can the Cutscene be skipped.");

    GUIContent AddDirectorGroupContent = new GUIContent("Director Group");
    GUIContent AddShotTracksContent = new GUIContent("Shot Tracks");
    GUIContent AddAudioTracksContent = new GUIContent("Audio Tracks");
    GUIContent AddGlobalTracksContent = new GUIContent("Global Tracks");

    #endregion


    /// <summary>
    /// Sets the window title and minimum pane size
    /// </summary>
    public void Awake()
    {

#if UNITY_5 && !UNITY_5_0  || UNITY_2017_1_OR_NEWER
        base.titleContent = new GUIContent(TITLE);
#else
        base.title = TITLE;
#endif

        this.minSize = new Vector2(250f, 150f);

        intValues1.Add(new GUIContent("0"));
        intValues1.Add(new GUIContent("1"));

        for (int i = 0; i <= 4; i++)
        {
            intValues4.Add(new GUIContent(i.ToString()));
        }

        for (int i = 0; i <= 10; i++)
        {
            intValues10.Add(new GUIContent(i.ToString()));
        }
    }

    [MenuItem("Window/Cinema Suite/Cinema Director/Create Cutscene", false, 10)]
    static void Init()
    {
        EditorWindow.GetWindow<CutsceneCreatorWindow>();
    }

    /// <summary>
    /// Draws the Director GUI
    /// </summary>
    protected void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        {
            txtCutsceneName = EditorGUILayout.TextField(NameContentCutscene, txtCutsceneName);

            EditorGUILayout.BeginHorizontal();
            txtDuration = EditorGUILayout.FloatField(DurationContentCutscene, txtDuration);
            timeEnum = (DirectorHelper.TimeEnum)EditorGUILayout.EnumPopup(timeEnum);
            EditorGUILayout.EndHorizontal();

            isLooping = EditorGUILayout.Toggle(LoopingContentCutscene, isLooping);
            isSkippable = EditorGUILayout.Toggle(SkippableContentCutscene, isSkippable);
            StartMethod = (StartMethod)EditorGUILayout.EnumPopup(new GUIContent("Start Method"), StartMethod);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Track Groups", EditorStyles.boldLabel);

            // Director Group
            directorTrackGroupsSelection = EditorGUILayout.Popup(AddDirectorGroupContent, directorTrackGroupsSelection, intValues1.ToArray());
            
            if(directorTrackGroupsSelection > 0)
            {
                EditorGUI.indentLevel++;

                // Shot Tracks
                shotTrackSelection = EditorGUILayout.Popup(AddShotTracksContent, shotTrackSelection, intValues1.ToArray());

                // Audio Tracks
                audioTrackSelection = EditorGUILayout.Popup(AddAudioTracksContent, audioTrackSelection, intValues4.ToArray());

                // Global Item Tracks
                globalItemTrackSelection = EditorGUILayout.Popup(AddGlobalTracksContent, globalItemTrackSelection, intValues10.ToArray());

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Actor Track Groups
            int actorCount = EditorGUILayout.Popup(new GUIContent("Actor Track Groups"), actorTrackGroupsSelection, intValues10.ToArray());

            if (actorCount != actorTrackGroupsSelection)
            {
                actorTrackGroupsSelection = actorCount;

                Transform[] tempActors = new Transform[actors.Length];
                Array.Copy(actors, tempActors, actors.Length);
                
                actors = new Transform[actorCount];
                int amount = Math.Min(actorCount, tempActors.Length);
                Array.Copy(tempActors, actors, amount);
            }

            EditorGUI.indentLevel++;
            for(int i = 1; i <= actorTrackGroupsSelection; i++)
            {
                actors[i - 1] = EditorGUILayout.ObjectField(new GUIContent(string.Format("Actor {0}", i)), actors[i - 1], typeof(Transform), true) as Transform;
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            // Multi Actor Track Groups
            multiActorTrackGroupsSelection = EditorGUILayout.Popup(new GUIContent("Multi-Actor Track Groups"), multiActorTrackGroupsSelection, intValues10.ToArray());
            {
                EditorGUI.indentLevel++;

                // Event Tracks

                // Curve Tracks

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Character Track Groups
            int characterTGCount = EditorGUILayout.Popup(new GUIContent("Character Track Groups"), characterTrackGroupsSelection, intValues10.ToArray());

            if (characterTGCount != characterTrackGroupsSelection)
            {
                characterTrackGroupsSelection = characterTGCount;

                Transform[] tempCharacters = new Transform[characters.Length];
                Array.Copy(characters, tempCharacters, characters.Length);

                characters = new Transform[characterTGCount];
                int amount = Math.Min(characterTGCount, tempCharacters.Length);
                Array.Copy(tempCharacters, characters, amount);
            }

            EditorGUI.indentLevel++;
            for (int i = 1; i <= characterTrackGroupsSelection; i++)
            {
                characters[i - 1] = EditorGUILayout.ObjectField(new GUIContent(string.Format("Character {0}", i)), characters[i - 1], typeof(Transform), true) as Transform;
            }
            EditorGUI.indentLevel--;

            
        }
        
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("I'm Feeling Lucky"))
            {
                List<Transform> interestingActors = UnitySceneEvaluator.GetHighestRankedGameObjects(10);

                actorTrackGroupsSelection = interestingActors.Count;
                actors = interestingActors.ToArray();
            }

            if (GUILayout.Button("Create Cutscene"))
            {
                CreateCutscene();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public Cutscene CreateCutscene()
    {
        string cutsceneName = DirectorHelper.getCutsceneItemName(txtCutsceneName, typeof(Cutscene));

        GameObject cutsceneGO = new GameObject(cutsceneName);
        Cutscene cutscene = cutsceneGO.AddComponent<Cutscene>();

        for (int i = 0; i < directorTrackGroupsSelection; i++)
        {
            DirectorGroup dg = CutsceneItemFactory.CreateDirectorGroup(cutscene);
            dg.Ordinal = 0;
            for (int j = 0; j < shotTrackSelection; j++)
            {
                CutsceneItemFactory.CreateShotTrack(dg);
            }
            for (int j = 0; j < audioTrackSelection; j++)
            {
                CutsceneItemFactory.CreateAudioTrack(dg);
            }
            for (int j = 0; j < globalItemTrackSelection; j++)
            {
                CutsceneItemFactory.CreateGlobalItemTrack(dg);
            }
        }

        for (int i = 0; i < actorTrackGroupsSelection; i++)
        {
            CutsceneItemFactory.CreateActorTrackGroup(cutscene, actors[i]);
        }

        for (int i = 0; i < multiActorTrackGroupsSelection; i++)
        {
            CutsceneItemFactory.CreateMultiActorTrackGroup(cutscene);
        }

        for (int i = 0; i < characterTrackGroupsSelection; i++)
        {
            CutsceneItemFactory.CreateCharacterTrackGroup(cutscene, characters[i]);
        }

        float duration = txtDuration;
        if (timeEnum == DirectorHelper.TimeEnum.Minutes) duration *= 60;
        cutscene.Duration = duration;

        cutscene.IsLooping = isLooping;

        cutscene.IsSkippable = isSkippable;

        int undoIndex = Undo.GetCurrentGroup();

        if (StartMethod != StartMethod.None)
        {
            CreateCutsceneTrigger(cutscene);
        }

        Undo.RegisterCreatedObjectUndo(cutsceneGO, string.Format("Created {0}", txtCutsceneName));
        Undo.CollapseUndoOperations(undoIndex);

        Selection.activeTransform = cutsceneGO.transform;

        return cutscene;
    }

    public Cutscene CreateEmptyCutscene()
    {
        directorTrackGroupsSelection = 0;
        actorTrackGroupsSelection = 0;
        multiActorTrackGroupsSelection = 0;
        characterTrackGroupsSelection = 0;
        return CreateCutscene();
    }

    public CutsceneTrigger CreateCutsceneTrigger(Cutscene cutscene)
    {
        GameObject cutsceneTriggerGO = new GameObject("Cutscene Trigger");
        CutsceneTrigger cutsceneTrigger = cutsceneTriggerGO.AddComponent<CutsceneTrigger>();
        if (StartMethod == StartMethod.OnTrigger)
        {
            cutsceneTriggerGO.AddComponent<BoxCollider>();
        }
        cutsceneTrigger.StartMethod = StartMethod;
        cutsceneTrigger.Cutscene = cutscene;
        Undo.RegisterCreatedObjectUndo(cutsceneTriggerGO, string.Format("Created {0}", txtCutsceneName));

        return cutsceneTrigger;
    }
}