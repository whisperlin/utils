using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set IK Position Weight", CutsceneItemGenre.ActorItem)]
    public class SetIKPositionWeightAnimatorEvent : CinemaActorEvent
    {
        public AvatarIKGoal Goal;
        public float Value;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetIKPositionWeight(Goal, Value);
        }
    }
}