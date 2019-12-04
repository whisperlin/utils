using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animation", "Blend Animation", CutsceneItemGenre.ActorItem)]
    public class BlendAnimationEvent : CinemaActorEvent
    {
        public string Animation = string.Empty;
        public float TargetWeight = 1f;
        public float FadeLength = 0.3f;

        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                Animation animation = actor.GetComponent<Animation>();
                if (!animation)
                {
                    return;
                }

                animation.Blend(Animation, TargetWeight, FadeLength);
            }
        }

        public override void Reverse(GameObject actor)
        {
        }
    }
}