using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set Target", CutsceneItemGenre.ActorItem)]
    public class SetTargetAnimatorEvent : CinemaActorEvent
    {
        public AvatarTarget TargetIndex;
        public float TargetNormalizedTime;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetTarget(TargetIndex, TargetNormalizedTime);
        }
    }
}