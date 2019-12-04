using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set IK Rotation Weight", CutsceneItemGenre.ActorItem)]
    public class SetIKRotationWeightAnimatorEvent : CinemaActorEvent
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

            animator.SetIKRotationWeight(Goal, Value);
        }
    }
}