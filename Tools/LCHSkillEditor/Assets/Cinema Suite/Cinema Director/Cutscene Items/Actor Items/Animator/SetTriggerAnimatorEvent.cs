using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set Trigger", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class SetTriggerAnimatorEvent : CinemaActorEvent
    {
        public string Name;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetTrigger(Name);
        }
    }
}