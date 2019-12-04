// Cinema Suite
using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An action that updates the transform of the Actor to smoothly look at a target game object.
    /// </summary>
    [CutsceneItem("Transform", "Rotate to Follow", CutsceneItemGenre.ActorItem, CutsceneItemGenre.TransformItem)]
    public class RotateFollow : CinemaActorAction, IRevertable
    {
        [SerializeField]
        [Tooltip("The target that the Actor should look at.")]
        GameObject LookAtTarget;

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        private Quaternion initialRotation;


        /// <summary>
        /// Cache the state of all actors related to this event.
        /// </summary>
        /// <returns>Info to revert rotation</returns>
        public RevertInfo[] CacheState()
        {
            List<Transform> actors = new List<Transform>(GetActors());
            List<RevertInfo> reverts = new List<RevertInfo>();
            for (int i = 0; i < actors.Count; i++)
            {
                Transform go = actors[i];
                if (go != null)
                {
                    Transform t = go.GetComponent<Transform>();
                    if (t != null)
                    {
                        reverts.Add(new RevertInfo(this, t, "localRotation", t.localRotation));
                    }
                }
            }

            return reverts.ToArray();
        }

        /// <summary>
        /// Trigger this action and have the actor look at the target.
        /// </summary>
        /// <param name="actor">The actor to update the transform of.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor == null || LookAtTarget == null) return;
            initialRotation = actor.transform.rotation;
            updateRotation(actor, 0f);
        }

        /// <summary>
        /// Continue to update the transform to look at the target.
        /// </summary>
        /// <param name="actor">The actor being updated.</param>
        /// <param name="runningTime">The running time of the cutscene.</param>
        /// <param name="deltaTime">The deltaTime since last call.</param>
        public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
        {
            if (actor == null || LookAtTarget == null) return;
            updateRotation(actor, runningTime);
        }

        /// <summary>
        /// End the action.
        /// </summary>
        /// <param name="actor">The actor of this action.</param>
        public override void End(GameObject actor)
        {
            if (actor == null || LookAtTarget == null) return;
            updateRotation(actor, Duration);
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Editor.
        /// </summary>
        public RevertMode EditorRevertMode
        {
            get { return editorRevertMode; }
            set { editorRevertMode = value; }
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Runtime.
        /// </summary>
        public RevertMode RuntimeRevertMode
        {
            get { return runtimeRevertMode; }
            set { runtimeRevertMode = value; }
        }

        private void updateRotation(GameObject actor, float runningTime)
        {
            // Get the target rotation for the current call
            Quaternion targetRotation = Quaternion.LookRotation(LookAtTarget.transform.position - actor.transform.position);

            float t = runningTime / Duration;

            actor.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
        }
    }
}