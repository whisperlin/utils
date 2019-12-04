using CinemaDirector;
using UnityEngine;

[CutsceneTrackGroupAttribute(typeof(CharacterTrackGroup))]
public class CharacterTrackGroupControl : ActorTrackGroupControl
{
    public override void Initialize()
    {
        base.Initialize();
        LabelPrefix = styles.CharacterGroupIcon.normal.background;
    }
}