using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Audio Source", "Play One Shot", CutsceneItemGenre.ActorItem)]
    public class PlayOneShotAudioEvent : CinemaActorEvent
    {
        public AudioClip Clip;
        public float VolumeScale = 1f;

        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                AudioSource audio = actor.GetComponent<AudioSource>();
                if (!audio)
                {
                    return;
                }

                audio.PlayOneShot(Clip, VolumeScale);
            }
        }

    }

}