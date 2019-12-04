using CinemaDirector.Helpers;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using UnityEngine;

namespace CinemaDirector
{
    [CutsceneItemAttribute("uGUI", "Change Color Selectable", CutsceneItemGenre.ActorItem)]
    public class ColorChangeSelectable : CinemaActorAction, IRevertable
    {
        enum ColorBlockChoices {normalColor,highlightedColor,pressedColor, disabledColor};
        [SerializeField]
        ColorBlockChoices colorField;

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
                    Selectable clr = go.GetComponent<Selectable>();
                    if (clr != null)
                    {
                        reverts.Add(new RevertInfo(this, clr, "colors", clr.colors));
                    }
                }
            }
            return reverts.ToArray();
        }


        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                Selectable UIcomponent = actor.GetComponent<Selectable>();

                if (UIcomponent != null)
                {
                    switch ((int)colorField)
                    {
                        case 0:
                            initialColor = UIcomponent.colors.normalColor;
                            break;
                        case 1:
                            initialColor = UIcomponent.colors.highlightedColor;
                            break;
                        case 2:
                            initialColor = UIcomponent.colors.pressedColor;
                            break;
                        case 3:
                            initialColor = UIcomponent.colors.disabledColor;
                            break;
                    }
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

                Selectable UIcomponent = actor.GetComponent<Selectable>();

                if (UIcomponent != null)
                {
                    ColorBlock tempValue = UIcomponent.colors;
                    Color lerpedColor = Color.Lerp(initialColor, colorValue, transition);

                    switch ((int)colorField)
                    {
                        case 0:
                            tempValue.normalColor = lerpedColor;
                            break;
                        case 1:
                            tempValue.highlightedColor = lerpedColor;
                            break;
                        case 2:
                            tempValue.pressedColor = lerpedColor;
                            break;
                        case 3:
                            tempValue.disabledColor = lerpedColor;
                            break;
                    }

                    UIcomponent.colors = tempValue;

                    #if UNITY_EDITOR
                        EditorUtility.SetDirty(actor.GetComponent<Selectable>());
                    #endif
                }
            }         
        }

        public override void End(GameObject actor)
        {
            if (actor != null)
            {
                Selectable UIcomponent = actor.GetComponent<Selectable>();

                if (UIcomponent != null)
                {
                    ColorBlock tempValue = UIcomponent.colors;

                    switch ((int)colorField)
                    {
                        case 0:
                            tempValue.normalColor = colorValue;
                            break;
                        case 1:
                            tempValue.highlightedColor = colorValue;
                            break;
                        case 2:
                            tempValue.pressedColor = colorValue;
                            break;
                        case 3:
                            tempValue.disabledColor = colorValue;
                            break;
                    }

                    if (UIcomponent != null)
                        UIcomponent.colors = tempValue;
                    #if UNITY_EDITOR
                        EditorUtility.SetDirty(actor.GetComponent<Selectable>());
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