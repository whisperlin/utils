// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Track for containing only mecanim Animator related content.
    /// </summary>
    [TimelineTrackAttribute("Mecanim Track", TimelineTrackGenre.CharacterTrack, CutsceneItemGenre.MecanimItem)]
    public class MecanimTrack : ActorItemTrack
    { }
}