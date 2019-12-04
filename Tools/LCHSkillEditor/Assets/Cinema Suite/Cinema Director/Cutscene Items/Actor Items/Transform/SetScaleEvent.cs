using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Detaches all children in hierarchy from this Parent.
    /// </summary>
    [CutsceneItemAttribute("Transform", "Set Scale", CutsceneItemGenre.ActorItem)]
    public class SetScaleEvent : CinemaActorEvent, IRevertable
    {

        public Vector3 Scale = Vector3.one;
        private Vector3 InitialScale;

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                InitialScale = actor.transform.localScale;
                actor.transform.localScale = Scale;
            }
        }

        public override void Reverse(GameObject actor)
        {
            if (actor != null)
            {
                actor.transform.localScale = InitialScale;
            }
        }

        public RevertInfo[] CacheState()
        {
            List<Transform> actors = new List<Transform>(GetActors());
            List<RevertInfo> reverts = new List<RevertInfo>();
            for (int i = 0; i < actors.Count; i++)
            {
                Transform go = actors[i];
                if (go != null)
                {
                    reverts.Add(new RevertInfo(this, go.gameObject.transform, "localScale", go.gameObject.transform.localScale));
                }
            }

            return reverts.ToArray();
        }

        /// <summary>
        /// Option for choosing when this curve clip will Revert to initial state in Editor.
        /// </summary>
        public RevertMode EditorRevertMode
        {
            get { return editorRevertMode; }
            set { editorRevertMode = value; }
        }

        /// <summary>
        /// Option for choosing when this curve clip will Revert to initial state in Runtime.
        /// </summary>
        public RevertMode RuntimeRevertMode
        {
            get { return runtimeRevertMode; }
            set { runtimeRevertMode = value; }
        }
    }
}