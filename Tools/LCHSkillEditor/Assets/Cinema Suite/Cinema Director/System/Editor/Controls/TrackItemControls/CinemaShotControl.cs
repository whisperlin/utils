using UnityEngine;
using System.Collections;
using UnityEditor;
using CinemaDirector;

[CutsceneItemControlAttribute(typeof(CinemaShot))]
public class CinemaShotControl : CinemaActionControl
{
    #region Language
    private const string MODIFY_CAMERA = "Set Camera/{0}";
    #endregion


    public override void Initialize(TimelineItemWrapper wrapper, TimelineTrackWrapper track)
    {
        base.Initialize(wrapper, track);
        actionIcon = EditorGUIUtility.Load("Cinema Suite/Cinema Director/Director_ShotIcon.png") as Texture;
    }

    public override void Draw(DirectorControlState state)
    {
        CinemaShot shot = Wrapper.Behaviour as CinemaShot;
        if (shot == null) return;

        if (Selection.Contains(shot.gameObject))
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.ShotTrackItemSelectedStyle);
        }
        else
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.ShotTrackItemStyle);
        }
        

        // Draw Icon
        Color temp = GUI.color;
        GUI.color = (shot.shotCamera != null) ? new Color(0.19f, 0.76f, 0.84f) : Color.red;
        Rect icon = controlPosition;
        icon.x += 4;
        icon.width = 16;
        icon.height = 16;
        //GUI.DrawTexture(icon, shotIcon, ScaleMode.ScaleToFit, true, 0);
        GUI.Box(icon, actionIcon, GUIStyle.none);
        GUI.color = temp;

        Rect labelPosition = controlPosition;
        labelPosition.x = icon.xMax;
        labelPosition.width -= (icon.width + 4);

        if (TrackControl.isExpanded)
        {
            labelPosition.height = TimelineTrackControl.ROW_HEIGHT;

            if (shot.shotCamera != null)
            {
                Rect extraInfo = labelPosition;
                extraInfo.y += TimelineTrackControl.ROW_HEIGHT;
                GUI.Label(extraInfo, string.Format("Camera: {0}", shot.shotCamera.name));
            }
        }
        DrawRenameLabel(shot.name, labelPosition);
    }

    protected override void showContextMenu(Behaviour behaviour)
    {
        CinemaShot shot = behaviour as CinemaShot;
        if (shot == null) return;

        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        
        GenericMenu createMenu = new GenericMenu();
        createMenu.AddItem(new GUIContent("Rename"), false, renameItem, behaviour);
        createMenu.AddItem(new GUIContent("Copy"), false, copyItem, behaviour);
        createMenu.AddItem(new GUIContent("Delete"), false, deleteItem, shot);
        createMenu.AddSeparator(string.Empty);
        createMenu.AddItem(new GUIContent("Focus"), false, focusShot, shot);
        for (int i = 0; i < cameras.Length; i++)
        {
            ContextSetCamera arg = new ContextSetCamera();
            arg.shot = shot;
            arg.camera = cameras[i];
            createMenu.AddItem(new GUIContent(string.Format(MODIFY_CAMERA, cameras[i].gameObject.name)), false, setCamera, arg);
        }
        
        createMenu.ShowAsContext();
    }

    private void focusShot(object userData)
    {
        CinemaShot shot = userData as CinemaShot;
        if (shot.shotCamera != null)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.AlignViewToObject(shot.shotCamera.transform);
            }
            else
            {
                Debug.Log("Focus is not supported in this version of Unity.");
            }
        }
    }

    private void setCamera(object userData)
    {
        ContextSetCamera arg = userData as ContextSetCamera;
        if (arg != null)
        {
            Undo.RecordObject(arg.shot, "Set Camera");
            arg.shot.shotCamera = arg.camera;
        }
    }

    private class ContextSetCamera
    {
        public Camera camera;
        public CinemaShot shot;
    }
}
