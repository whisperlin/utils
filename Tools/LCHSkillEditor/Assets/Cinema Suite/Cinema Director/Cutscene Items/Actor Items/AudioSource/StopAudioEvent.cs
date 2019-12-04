using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Audio Source", "Stop Audio", CutsceneItemGenre.ActorItem)]
    public class StopAudioEvent : CinemaActorEvent
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

                audio.Stop();
            }
        }
    }
}