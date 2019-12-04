using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An Event for applying force to a rigidbody.
    /// Can only be triggered in Runtime.
    /// </summary>
    [CutsceneItemAttribute("Physics", "Apply Force", CutsceneItemGenre.ActorItem)]
    public class ApplyForceEvent : CinemaActorEvent
    {
        // The amount of Force.
        public Vector3 Force = Vector3.forward;

        // The Force Mode of the add force action.
        public ForceMode ForceMode = ForceMode.Impulse;

        /// <summary>
        /// Trigger this event and Apply force to the RigidBody component of the Actor
        /// </summary>
        /// <param name="actor">The Actor of the RigidBody</param>
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                Rigidbody affectedObjectRigidBody = actor.GetComponent<Rigidbody>();

                if (affectedObjectRigidBody != null)
                {
                    affectedObjectRigidBody.AddForce(Force, ForceMode);
                }
            }
        }
    }
}
