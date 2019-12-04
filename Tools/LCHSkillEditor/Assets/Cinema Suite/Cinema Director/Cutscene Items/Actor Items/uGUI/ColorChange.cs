using CinemaDirector.Helpers;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using UnityEngine;

namespace CinemaDirector
{
    [CutsceneItemAttribute("uGUI", "Change Color", CutsceneItemGenre.ActorItem)]
    public class ColorChange : CinemaActorAction, IRevertable
    {
        [SerializeField]
        Color colorValue = Color.white;

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        Color initialColor;

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
                    Graphic clr = go.GetComponent<Graphic>();
                    if (clr != null)
                    {
                        reverts.Add(new RevertInfo(this, clr, "color", clr.color));
                    }
                }
            }
            return reverts.ToArray();
        }


        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                Graphic UIcomponent = actor.GetComponent<Graphic>();

                if (UIcomponent != null)
                {
                    initialColor = UIcomponent.color;
                }
            }
        }
        
        public override void SetTime(GameObject actor, float time, float deltaTime)
        {
            if (actor != null)
            {
                if (time > 0 && time <= Duration)
                {
                    UpdateTime(actor, time, deltaTime);
                }
            }
        }

        public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
        {
            if (actor != null)
            {
                float transition = runningTime / Duration;

                Graphic UIcomponent = actor.GetComponent<Graphic>();

                if (UIcomponent != null)
                {
                    Color lerpedColor = Color.Lerp(initialColor, colorValue, transition);
                    UIcomponent.color = lerpedColor;

                    #if UNITY_EDITOR
                        EditorUtility.SetDirty(actor.GetComponent<Graphic>());
                    #endif
                }
            }      
        }

        public override void End(GameObject actor)
        {
            if (actor != null)
            {
                Graphic UIcomponent = actor.GetComponent<Graphic>();

                if (UIcomponent != null)
                {
                    UIcomponent.color = colorValue;

                    #if UNITY_EDITOR
                        EditorUtility.SetDirty(actor.GetComponent<Graphic>());
                    #endif
                }
            }
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