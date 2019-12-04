using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set Bool", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class SetBoolAnimatorEvent : CinemaActorEvent
    {
        public string BoolName;
        public bool Value = false;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool(BoolName, Value);
            }
        }
    }
}