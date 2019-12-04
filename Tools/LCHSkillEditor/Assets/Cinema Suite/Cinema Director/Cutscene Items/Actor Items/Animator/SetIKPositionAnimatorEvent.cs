using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set IK Position", CutsceneItemGenre.ActorItem)]
    public class SetIKPositionAnimatorEvent : CinemaActorEvent
    {
        public AvatarIKGoal Goal;
        public Vector3 GoalPosition;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetIKPosition(Goal, GoalPosition);
        }
    }
}