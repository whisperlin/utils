using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animation", "Stop Animation", CutsceneItemGenre.ActorItem)]
    public class StopAnimationEvent : CinemaActorEvent
    {
        public string Animation = string.Empty;

        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                Animation animation = actor.GetComponent<Animation>();
                if (!animation)
                {
                    return;
                }

                animation.Stop(Animation);
            }
        }

        public override void Reverse(GameObject actor)
        {
        }
    }
}