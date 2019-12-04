using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Cross Fade Animator", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class CrossFadeAnimatorEvent : CinemaActorEvent
    {
        public string AnimationStateName = string.Empty;
        public float TransitionDuration = 1f;
        public int Layer = -1;
        float NormalizedTime = float.NegativeInfinity;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.CrossFade(AnimationStateName, TransitionDuration, Layer, NormalizedTime);
        }
    }
}