using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set Look At Weight", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class SetLookAtWeightAnimatorEvent : CinemaActorEvent
    {
        public float Weight;
        public float BodyWeight = 0f;
        public float HeadWeight = 1f;
        public float EyesWeight = 0f;
        public float ClampWeight = 0.5f;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetLookAtWeight(Weight, BodyWeight, HeadWeight, EyesWeight, ClampWeight);
        }
    }
}