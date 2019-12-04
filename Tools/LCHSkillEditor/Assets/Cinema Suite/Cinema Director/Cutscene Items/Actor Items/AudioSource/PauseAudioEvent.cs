using UnityEngine;
using System.Collections;
namespace CinemaDirector
{
    [CutsceneItemAttribute("Audio Source", "Pause Audio", CutsceneItemGenre.ActorItem)]
    public class PauseAudioEvent : CinemaActorEvent
    {
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                AudioSource audio = actor.GetComponent<AudioSource>();
                if (!audio)
                {
                    return;
                }

                audio.Pause();
            }
        }

    }
}