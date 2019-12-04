using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Play Mecanim Animation", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class PlayAnimatorEvent : CinemaActorEvent
    {
        public string StateName;
        public int Layer = -1;
        float Normalizedtime = float.NegativeInfinity;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.Play(StateName, Layer, Normalizedtime);
        }
    }
}