using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Event to set the rigidbody of a given actor into sleep mode.
    /// </summary>
    [CutsceneItemAttribute("Physics", "Sleep", CutsceneItemGenre.ActorItem)]
    public class RigidbodySleepEvent : CinemaActorEvent
    {
        /// <summary>
        /// Trigger this event and set the rigidbody of the actor to sleep.
        /// </summary>
        /// <param name="actor">The actor to put to sleep.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                Rigidbody rb = actor.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.Sleep();
                }
            }
        }

        /// <summary>
        /// Trigger this event and wake up the rigidbody component of the given actor.
        /// </summary>
        /// <param name="actor">The actor to wake up.</param>
        public override void Reverse(GameObject actor)
        {
            if (actor != null)
            {
                Rigidbody rb = actor.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.WakeUp();
                }
            }
        }
    }
}