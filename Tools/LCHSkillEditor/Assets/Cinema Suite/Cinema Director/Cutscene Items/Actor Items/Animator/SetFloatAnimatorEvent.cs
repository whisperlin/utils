using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set Float", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class SetFloatAnimatorEvent : CinemaActorEvent
    {
        public string FloatName;
        public float Value = 0f;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetFloat(FloatName, Value);
        }
    }
}