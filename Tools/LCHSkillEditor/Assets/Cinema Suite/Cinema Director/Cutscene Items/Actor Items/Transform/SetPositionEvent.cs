using UnityEngine;
using CinemaDirector.Helpers;
using System;
using System.Collections.Generic;

namespace CinemaDirector
{
    /// <summary>
    /// Detaches all children in hierarchy from this Parent.
    /// </summary>
    [CutsceneItemAttribute("Transform", "Set Position", CutsceneItemGenre.ActorItem)]
    public class SetPositionEvent : CinemaActorEvent, IRevertable
    {
        public Vector3 Position;
        public Vector3 InitialPosition;

        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;



        public RevertMode EditorRevertMode
        {
            get { return editorRevertMode; }
            set { editorRevertMode = value; }
        }

        public RevertMode RuntimeRevertMode
        {
            get { return runtimeRevertMode; }
            set { runtimeRevertMode = value; }
        }

        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                InitialPosition = actor.transform.position;
                actor.transform.position = Position;
            }
        }

        public override void Reverse(GameObject actor)
        {
            if (actor != null)
            {
                actor.transform.position = InitialPosition;
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
                    reverts.Add(new RevertInfo(this, go.gameObject.transform, "position", go.gameObject.transform.position));
                }
            }
            return reverts.ToArray();
        }
    }
}