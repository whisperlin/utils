using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CutsceneTrackGroupAttribute(typeof(MultiActorTrackGroup))]
public class MultiActorTrackGroupControl : GenericTrackGroupControl
{
    public override void Initialize()
    {
        base.Initialize();
        LabelPrefix = styles.MultiActorGroupIcon.normal.background;
    }
}

