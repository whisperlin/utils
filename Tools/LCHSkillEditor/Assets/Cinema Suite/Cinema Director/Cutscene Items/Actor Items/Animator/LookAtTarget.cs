using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Look At Target", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class LookAtTarget : CinemaActorAction
    {
        Animator animator;
        public Transform LookAtPosition;

        public float Weight;
        public float BodyWeight = 0f;
        public float HeadWeight = 1f;
        public float EyesWeight = 0f;
        public float ClampWeight = 0.5f;

        public override void Trigger(GameObject Actor)
        {
            animator = Actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }
        }

        public override void UpdateTime(GameObject Actor, float runningTime, float deltaTime)
        {
            Animator animator = Actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            //animator.SetLookAtPosition(LookAtPosition.position);
            //animator.SetLookAtWeight(Weight, BodyWeight, HeadWeight, EyesWeight, ClampWeight);
        }

        public override void End(GameObject Actor)
        {
            Animator animator = Actor.GetComponent<Animator>();
            if (animator)
            {
                //animator.SetLookAtWeight(0);
            }
        }

        void OnAnimatorIK()
        {
            animator.SetLookAtPosition(LookAtPosition.position);
            animator.SetLookAtWeight(Weight, BodyWeight, HeadWeight, EyesWeight, ClampWeight);
        }
    }
}