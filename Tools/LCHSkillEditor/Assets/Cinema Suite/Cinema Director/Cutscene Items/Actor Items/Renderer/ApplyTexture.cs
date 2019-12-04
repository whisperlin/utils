using UnityEngine;
using System.Collections;
using CinemaDirector.Helpers;
using System;
using System.Collections.Generic;

namespace CinemaDirector
{
    /// <summary>
    /// An Event for applying a texture to an actor.
    /// </summary>
    [CutsceneItemAttribute("Renderer", "Apply Texture", CutsceneItemGenre.ActorItem)]
    public class ApplyTexture : CinemaActorEvent, IRevertable
    {
        public Texture texture;
        private Texture initialTexture;

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        /// <summary>
        /// Trigger this event and apply the texture.
        /// </summary>
        /// <param name="actor">The actor to apply the texture to.</param>
        public override void Trigger(GameObject actor)
        {
            Renderer r = actor.GetComponent<Renderer>();
            if (r != null && texture != null)
            {
                initialTexture = r.sharedMaterial.mainTexture;
                r.sharedMaterial.mainTexture = texture;
            }
        }

        /// <summary>
        /// Reverse trigger this event and revert to the initial texture.
        /// </summary>
        /// <param name="actor">The actor to apply the texture to.</param>
        public override void Reverse(GameObject actor)
        {
            Renderer r = actor.GetComponent<Renderer>();
            if (r != null && texture != null)
            {
                r.sharedMaterial.mainTexture = initialTexture;
            }
        }

        public RevertInfo[] CacheState()
        {
            List<Transform> actors = new List<Transform>(GetActors());
            List<RevertInfo> reverts = new List<RevertInfo>();
            for (int i = 0; i < actors.Count; i++)
            {
                Renderer r = actors[i].GetComponent<Renderer>();
                if (r != null)
                {
                    reverts.Add(new RevertInfo(this, r.sharedMaterial, "mainTexture", r.sharedMaterial.mainTexture));
                }
            }
            return reverts.ToArray();
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
    }
}
