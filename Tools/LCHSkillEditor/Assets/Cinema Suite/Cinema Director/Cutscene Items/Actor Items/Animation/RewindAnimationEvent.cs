using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animation", "Rewind Animation", CutsceneItemGenre.ActorItem)]
    public class RewindAnimationEvent : CinemaActorEvent
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

                animation.Rewind(Animation);
            }
        }

        public override void Reverse(GameObject actor)
        {
        }
    }
}