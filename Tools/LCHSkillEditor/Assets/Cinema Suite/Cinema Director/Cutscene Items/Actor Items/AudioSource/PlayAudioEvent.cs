using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Audio Source", "Play Audio", CutsceneItemGenre.ActorItem)]
    public class PlayAudioEvent : CinemaActorAction
    {
        public AudioClip audioClip = null;
        public bool loop = false;

        private bool wasPlaying = false;

        public void Update()
        {
            if (!loop && audioClip)
                Duration = audioClip.length;
            else
                Duration = -1;
        }

        public override void Trigger(GameObject Actor)
        {
            AudioSource audio = Actor.GetComponent<AudioSource>();
            if (!audio)
            {
                audio = Actor.AddComponent<AudioSource>();
                audio.playOnAwake = false;
            }

            if (audio.clip != audioClip)
                audio.clip = audioClip;

            audio.time = 0.0f;
            audio.loop = loop;
            audio.Play();
        }

        public override void UpdateTime(GameObject Actor, float runningTime, float deltaTime)
        {
            AudioSource audio = Actor.GetComponent<AudioSource>();
            if (!audio)
            {
                audio = Actor.AddComponent<AudioSource>();
                audio.playOnAwake = false;
            }

            if (audio.clip != audioClip)
                audio.clip = audioClip;

            if (audio.isPlaying)
                return;

            audio.time = deltaTime;


            audio.Play();

        }

        public override void Resume(GameObject Actor)
        {
            AudioSource audio = Actor.GetComponent<AudioSource>();
            if (!audio)
                return;

            audio.time = Cutscene.RunningTime - Firetime;

            if (wasPlaying)
                audio.Play();
        }

        public override void Pause(GameObject Actor)
        {
            AudioSource audio = Actor.GetComponent<AudioSource>();

            wasPlaying = false;
            if (audio && audio.isPlaying)
                wasPlaying = true;

            if (audio)
                audio.Pause();
        }

        public override void End(GameObject Actor)
        {
            AudioSource audio = Actor.GetComponent<AudioSource>();
            if (audio)
                audio.Stop();
        }

    }

}