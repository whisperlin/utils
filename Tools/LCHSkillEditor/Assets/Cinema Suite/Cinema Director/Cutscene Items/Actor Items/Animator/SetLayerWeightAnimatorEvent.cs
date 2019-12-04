using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Set Layer Weight", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class SetLayerWeightAnimatorEvent : CinemaActorEvent
    {
        public int LayerIndex;
        public float Weight = 0f;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.SetLayerWeight(LayerIndex, Weight);
        }
    }
}