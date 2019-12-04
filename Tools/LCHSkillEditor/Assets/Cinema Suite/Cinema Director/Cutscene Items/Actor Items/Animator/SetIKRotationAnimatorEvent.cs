using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set IK Rotation", CutsceneItemGenre.ActorItem)]
    public class SetIKRotationAnimatorEvent : CinemaActorEvent
    {
        public AvatarIKGoal Goal;
        public Quaternion GoalRotation;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetIKRotation(Goal, GoalRotation);
        }
    }
}