using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Enable the Actor related to this event.
    /// </summary>
    [CutsceneItemAttribute("Particle System", "Play", CutsceneItemGenre.ActorItem)]
    public class PlayParticleSystemEvent : CinemaActorEvent
    {
        /// <summary>
        /// Trigger the particle system to play.
        /// </summary>
        /// <param name="actor">The actor to be triggered.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                ParticleSystem ps = actor.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                }
            }
        }

        /// <summary>
        /// Reverse this event and stop the particle system.
        /// </summary>
        /// <param name="actor">The actor to reverse this event on.</param>
        public override void Reverse(GameObject actor)
        {
            if (actor != null)
            {
                ParticleSystem ps = actor.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                }
            }
        }
    }
}