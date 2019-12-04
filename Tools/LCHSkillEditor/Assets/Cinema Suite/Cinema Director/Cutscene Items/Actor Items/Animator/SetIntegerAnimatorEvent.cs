using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set Integer", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class SetIntegerAnimatorEvent : CinemaActorEvent
    {
        public string IntName;
        public int Value = 0;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetInteger(IntName, Value);
        }
    }
}