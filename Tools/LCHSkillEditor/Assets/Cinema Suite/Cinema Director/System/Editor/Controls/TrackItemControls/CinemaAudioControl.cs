using UnityEngine;
using System.Collections;
using UnityEditor;
using CinemaDirector;

[CutsceneItemControlAttribute(typeof(CinemaAudio))]
public class CinemaAudioControl : ActionFixedItemControl
{
    private string audioItemName = string.Empty;
    private Texture2D texture = null;

    public CinemaAudioControl()
    {
        base.AlterFixedAction += CinemaAudioControl_AlterFixedAction;
    }

    public override void Initialize(TimelineItemWrapper wrapper, TimelineTrackWrapper track)
    {
        base.Initialize(wrapper, track);
        actionIcon = EditorGUIUtility.Load("Cinema Suite/Cinema Director/Director_AudioIcon.png") as Texture;
    }

    void CinemaAudioControl_AlterFixedAction(object sender, ActionFixedItemEventArgs e)
    {
        CinemaAudio audioItem = e.actionItem as CinemaAudio;
        if (audioItem == null) return;

        if (e.duration <= 0)
        {
            deleteItem(audioItem);
        }
        else
        {
            Undo.RecordObject(e.actionItem, string.Format("Change {0}", audioItem.name));
            audioItem.Firetime = e.firetime;
            audioItem.Duration = e.duration;
            audioItem.InTime = e.inTime;
            audioItem.OutTime = e.outTime;
        }
    }

    protected override void showContextMenu(Behaviour behaviour)
    {
        GenericMenu createMenu = new GenericMenu();
        if (TrackControl.isExpanded)
        {
            createMenu.AddDisabledItem(new GUIContent("Rename"));
        }
        else
        {
            createMenu.AddItem(new GUIContent("Rename"), false, renameItem, behaviour);
        }
        createMenu.AddItem(new GUIContent("Copy"), false, copyItem, behaviour);
        createMenu.AddItem(new GUIContent("Delete"), false, deleteItem, behaviour);
        createMenu.ShowAsContext();
    }

    public override void Draw(DirectorControlState state)
    {
        CinemaAudio audioItem = Wrapper.Behaviour as CinemaAudio;
        if (audioItem == null) return;
        AudioSource audioSource = audioItem.GetComponent<AudioSource>();

        if (!TrackControl.isExpanded)
        {
            if (Selection.Contains(audioItem.gameObject))
            {
                GUI.Box(controlPosition, string.Empty, TimelineTrackControl.styles.AudioTrackItemSelectedStyle);
            }
            else
            {
                GUI.Box(controlPosition, string.Empty, TimelineTrackControl.styles.AudioTrackItemStyle);
            }
            Color temp = GUI.color;
            GUI.color = new Color(1f, 0.65f, 0f);
            Rect icon = controlPosition;
            icon.x += 4;
            icon.width = 16;
            icon.height = 16;
            GUI.Box(icon, actionIcon, GUIStyle.none);
            GUI.color = temp;

            Rect labelPosition = controlPosition;
            labelPosition.x = icon.xMax;
            labelPosition.width -= (icon.width + 4);


            DrawRenameLabel(audioItem.name, labelPosition);
        }
        else
        {
            if (Selection.Contains(audioItem.gameObject))
            {
                GUI.Box(controlPosition, string.Empty, TimelineTrackControl.styles.TrackItemSelectedStyle);
            }
            else
            {
                GUI.Box(controlPosition, string.Empty, TimelineTrackControl.styles.TrackItemStyle);
            }
            if (audioSource != null && audioSource.clip != null)
            {
                GUILayout.BeginArea(controlPosition);
                if (texture == null || audioItemName != audioSource.clip.name)
                {
                    audioItemName = audioSource.clip.name;
                    texture = AssetPreview.GetAssetPreview(audioSource.clip);
                }

                float inTimeOffset = (audioItem.InTime) * state.Scale.x;
                float outTimeOffset = (audioItem.ItemLength - audioItem.OutTime) * state.Scale.x;
                Rect texturePosition = new Rect(-inTimeOffset + 2, 0, controlPosition.width - 4 + inTimeOffset + outTimeOffset, controlPosition.height);

                if (texture != null)
                {
                    GUI.DrawTexture(texturePosition, texture, ScaleMode.StretchToFill);
                }
                GUILayout.EndArea();
            }
        }
    }
}
