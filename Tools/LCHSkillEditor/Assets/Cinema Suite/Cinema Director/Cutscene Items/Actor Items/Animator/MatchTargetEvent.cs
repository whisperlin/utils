using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Match Target",CutsceneItemGenre.MecanimItem)]
    public class MatchTargetEvent : CinemaActorEvent
    {
        public GameObject target;
        public AvatarTarget avatarTarget;
        public float startTime;
        public float targetTime;

        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 0);
            animator.MatchTarget(target.transform.position, target.transform.rotation, avatarTarget, weightMask, startTime, targetTime);
            
        }
    }
}