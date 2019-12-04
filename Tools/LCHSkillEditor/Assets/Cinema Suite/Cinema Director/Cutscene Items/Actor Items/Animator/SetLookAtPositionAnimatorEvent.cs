using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set Look At Position", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class SetLookAtPositionAnimatorEvent : CinemaActorEvent
    {
        public Transform LookAtPosition;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetLookAtPosition(LookAtPosition.position);
        }
    }
}