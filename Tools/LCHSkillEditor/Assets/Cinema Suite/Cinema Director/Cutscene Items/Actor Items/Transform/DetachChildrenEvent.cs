using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Detaches all children in hierarchy from this Parent.
    /// </summary>
    [CutsceneItemAttribute("Transform", "Detach Children", CutsceneItemGenre.ActorItem)]
    public class DetachChildrenEvent : CinemaActorEvent
    {
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                actor.transform.DetachChildren();
            }
        }

        public override void Reverse(GameObject actor)
        {
        }
    }
}