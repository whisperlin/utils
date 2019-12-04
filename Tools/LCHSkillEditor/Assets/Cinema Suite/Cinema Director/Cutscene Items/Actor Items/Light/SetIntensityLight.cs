using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Set the intensity of a light component of a given actor.
    /// </summary>
    [CutsceneItemAttribute("Light", "Set Intensity", CutsceneItemGenre.ActorItem)]
    public class SetIntensityLight : CinemaActorEvent, IRevertable
    {
        // The new intensity to be set.
        public float Intensity;

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        private float previousIntensity;

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
                    Light light = go.GetComponent<Light>();
                    if (light != null)
                    {
                        reverts.Add(new RevertInfo(this, light, "intensity", light.intensity));
                    }
                }
            }

            return reverts.ToArray();
        }

        /// <summary>
        /// Initialize and save the original intensity state.
        /// </summary>
        /// <param name="actor">The actor to initialize the light intensity with.</param>
        public override void Initialize(GameObject actor)
        {
            Light light = actor.GetComponent<Light>();
            if (light != null)
            {
                previousIntensity = light.intensity;
            }
        }

        /// <summary>
        /// Trigger this event and change the intensity of a given actor's light component.
        /// </summary>
        /// <param name="actor">The actor with the light component to be altered.</param>
        public override void Trigger(GameObject actor)
        {
            Light light = actor.GetComponent<Light>();
            if (light != null)
            {
                previousIntensity = light.intensity;
                light.intensity = Intensity;
            }
        }

        /// <summary>
        /// Reverse setting the light intensity.
        /// </summary>
        /// <param name="actor">The actor to reverse the light setting on.</param>
        public override void Reverse(GameObject actor)
        {
            Light light = actor.GetComponent<Light>();
            if (light != null)
            {
                light.intensity = previousIntensity;
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