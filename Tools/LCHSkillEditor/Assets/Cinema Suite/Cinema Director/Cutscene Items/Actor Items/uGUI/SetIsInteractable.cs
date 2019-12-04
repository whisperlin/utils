using UnityEngine.UI;
using UnityEngine;
using CinemaDirector.Helpers;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace CinemaDirector
{
    [CutsceneItemAttribute("uGUI", "Interactable", CutsceneItemGenre.ActorItem)]
    public class SetIsInteractable : CinemaActorEvent, IRevertable
    {
        
        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
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
            Selectable UIcomponent = actor.GetComponent<Selectable>();
            if (UIcomponent != null)
                UIcomponent.interactable = !UIcomponent.interactable;

            #if UNITY_EDITOR
            EditorUtility.SetDirty(actor.GetComponent<Selectable>());
            #endif
        }

        public override void Reverse(GameObject actor)
        {
            Selectable UIcomponent = actor.GetComponent<Selectable>();

            if (UIcomponent != null && runtimeRevertMode == RevertMode.Revert && Application.isPlaying)
                UIcomponent.interactable = !UIcomponent.interactable;

            if (UIcomponent != null && editorRevertMode == RevertMode.Revert && Application.isEditor && !Application.isPlaying)
                UIcomponent.interactable = !UIcomponent.interactable;
            #if UNITY_EDITOR
            EditorUtility.SetDirty(actor.GetComponent<Selectable>());
            #endif
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
                    Selectable UIcomponent = go.GetComponent<Selectable>();
                    if (UIcomponent != null)
                    {
                        reverts.Add(new RevertInfo(this, UIcomponent, "interactable", UIcomponent.interactable));
                    }
                }
            }
            return reverts.ToArray();
        }
    }
}