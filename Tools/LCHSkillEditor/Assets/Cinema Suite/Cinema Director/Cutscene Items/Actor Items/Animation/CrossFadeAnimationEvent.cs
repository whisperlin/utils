using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animation", "Cross Fade Animation", CutsceneItemGenre.ActorItem)]
    public class CrossFadeAnimationEvent : CinemaActorEvent
    {
        public string Animation = string.Empty;
        public float TargetWeight = 1f;
        public PlayMode PlayMode = PlayMode.StopSameLayer;

        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                Animation animation = actor.GetComponent<Animation>();
                if (!animation)
                {
                    return;
                }

                animation.CrossFade(Animation, TargetWeight, PlayMode);
            }
        }

        public override void Reverse(GameObject actor)
        {
        }
    }
}