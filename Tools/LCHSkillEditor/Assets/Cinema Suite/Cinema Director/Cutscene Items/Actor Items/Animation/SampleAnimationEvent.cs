using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animation", "Sample Animation", CutsceneItemGenre.ActorItem)]
    public class SampleAnimationEvent : CinemaActorEvent
    {
        public string Animation = string.Empty;
        public float Time = 0f;

        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                Animation animation = actor.GetComponent<Animation>();
                if (!animation)
                {
                    return;
                }

                animation[Animation].time = Time;
                animation[Animation].enabled = true;
                animation.Sample();
                animation[Animation].enabled = false;
            }
        }

        public override void Reverse(GameObject actor)
        {
        }
    }
}