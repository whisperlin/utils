using CinemaDirector.Helpers;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using UnityEngine;

namespace CinemaDirector
{
    [CutsceneItemAttribute("uGUI", "Text Remover", CutsceneItemGenre.ActorItem)]
    public class TextDegenerationEvent : CinemaActorAction, IRevertable
    {

        string textValue;

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        /// <summary>
        /// Cache the state of all actors related to this event.
        /// </summary>
        /// <returns></returns>
        public RevertInfo[] CacheState()
        {
            List<Transform> actors = new List<Transform>(GetActors());
            List<RevertInfo> reverts = new List<RevertInfo>();
            for (int i = 0; i < actors.Count; i++)
            {
                Transform go = actors[i];
                if (go != null)
                {
                    Text txt = go.GetComponentInChildren<Text>();
                    if (txt != null)
                    {
                        reverts.Add(new RevertInfo(this, txt, "text", txt.text));
                    }
                }
            }
            return reverts.ToArray();
        }

        public override void Trigger(GameObject actor)
        {
            textValue= actor.GetComponentInChildren<Text>().text;
        }

        public override void SetTime(GameObject actor, float time, float deltaTime)
        {
            if (actor != null)
                if (time > 0 && time <= Duration)
                    UpdateTime(actor, time, deltaTime);
        }

        public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
        {
            float transition = runningTime / Duration;
            int numericalValue;

            if (textValue!=null)
            {
                numericalValue = (int)Mathf.Round(Mathf.Lerp(textValue.Length,0, transition));

                actor.GetComponentInChildren<Text>().text = textValue.Substring(0, numericalValue);
                #if UNITY_EDITOR
                EditorUtility.SetDirty(actor.GetComponentInChildren<Text>());
                #endif
            }
        }

        public override void End(GameObject actor)
        {
            actor.GetComponentInChildren<Text>().text = "";
            #if UNITY_EDITOR
            EditorUtility.SetDirty(actor.GetComponentInChildren<Text>());
            #endif
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