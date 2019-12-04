// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Track for containing only mecanim Animator related content.
    /// </summary>
    [TimelineTrackAttribute("Transform Track", TimelineTrackGenre.CharacterTrack, CutsceneItemGenre.TransformItem)]
    public class TransformTrack : ActorItemTrack
    { }
}