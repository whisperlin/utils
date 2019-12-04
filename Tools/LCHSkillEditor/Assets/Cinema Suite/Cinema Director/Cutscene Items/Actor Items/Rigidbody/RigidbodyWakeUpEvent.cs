using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Event to wake up the rigidbody of a given actor.
    /// </summary>
    [CutsceneItemAttribute("Physics", "Wake Up", CutsceneItemGenre.ActorItem)]
    public class RigidbodyWakeUpEvent : CinemaActorEvent
    {
        /// <summary>
        /// Trigger this event and wake up the rigidbody component of the given actor.
        /// </summary>
        /// <param name="actor">The actor to wake up.</param>
        public override void Trigger(GameObject actor)
        {
            Rigidbody rb = actor.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.WakeUp();
            }
        }

        /// <summary>
        /// Trigger this event and wake up the rigidbody component of the given actor.
        /// </summary>
        /// <param name="actor">The actor to wake up.</param>
        public override void Reverse(GameObject actor)
        {
            Rigidbody rb = actor.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.Sleep();
            }
        }
    }
}