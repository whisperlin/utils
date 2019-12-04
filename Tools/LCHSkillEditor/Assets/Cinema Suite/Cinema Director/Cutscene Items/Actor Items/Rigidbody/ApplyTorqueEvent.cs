using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An Event for applying torque to a rigidbody.
    /// Can only be triggered in Runtime.
    /// </summary>
    [CutsceneItemAttribute("Physics", "Apply Torque", CutsceneItemGenre.ActorItem)]
    public class ApplyTorqueEvent : CinemaActorEvent
    {
        // The Torque to be applied
        public Vector3 Torque = Vector3.forward;

        // the ForceMode
        public ForceMode ForceMode = ForceMode.Impulse;

        /// <summary>
        /// Trigger this event and apple torque to the RigidBody component of the Actor
        /// </summary>
        /// <param name="actor">The Actor with a RigidBody to apply to torque to.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                Rigidbody affectedObjectRigidBody = actor.GetComponent<Rigidbody>();
                if (affectedObjectRigidBody != null)
                {
                    affectedObjectRigidBody.AddTorque(Torque, ForceMode);
                }
            }
        }
    }
}