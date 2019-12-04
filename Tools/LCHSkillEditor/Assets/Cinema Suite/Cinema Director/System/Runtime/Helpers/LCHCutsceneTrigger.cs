using System;
using System.Collections;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A sample behaviour for triggering Cutscenes.
    /// </summary>
    public class LCHCutsceneTrigger : MonoBehaviour
    {
        public StartMethod StartMethod;
        public Cutscene Cutscene;
        public GameObject TriggerObject;
        public string SkipButtonName = "Jump";
        public string TriggerButtonName = "Fire1";
        public float Delay = 0f;
        private bool hasTriggered = false;

        /// <summary>
        /// When the trigger is loaded, optimize the Cutscene.
        /// </summary>
        void Awake()
        {
            if (Cutscene != null)
            {
                Cutscene.Optimize();

                Cutscene.eventHandle = onEnd;
            }
        }

        private void onEnd(EVENT type, object o)
        {
            LCharacterAction.pauseActions = false;
            LCharacterFellowCamera.notUpdateCamera = false;
            LCharacterFellowCamera.moveCameraBack = 3f; 
        }


        // When the scene starts trigger the Cutscene if necessary.

        void OnEnable() //OnEnable()
        {
               
            if (StartMethod == StartMethod.OnStart && Cutscene != null)
            {
                hasTriggered = true;
                StartCoroutine(PlayCutscene());
            }
        }

        private IEnumerator PlayCutscene()
        {
            yield return new WaitForSeconds(Delay);

            this.Cutscene.Play();
            LCharacterAction.pauseActions = true;
            LCharacterFellowCamera.notUpdateCamera = true;
        }

        void Update()
        {
            if (SkipButtonName != null && SkipButtonName != string.Empty)
            {
                // Check if the user wants to skip.
                if (Input.GetButtonDown(SkipButtonName))
                {
                    if (Cutscene != null && Cutscene.State == CinemaDirector.Cutscene.CutsceneState.Playing)
                    {
                        Cutscene.Skip();
                    }
                }
            }
        }


        /// <summary>
        /// If Cutscene is setup to play on trigger, watch for the trigger event.
        /// </summary>
        /// <param name="other">The other collider.</param>
        void OnTriggerEnter(Collider other)
        {
            if (StartMethod == StartMethod.OnTrigger && !hasTriggered && other.gameObject == TriggerObject)
            {
                hasTriggered = true;
                Cutscene.Play();
                LCharacterAction.pauseActions = true;
                LCharacterFellowCamera.notUpdateCamera = true;
            }
        }

        /// <summary>
        /// If Cutscene is setup to play on trigger, watch for the trigger event.
        /// </summary>
        /// <param name="other">The other collider.</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            if (StartMethod == StartMethod.OnTrigger && !hasTriggered && other.gameObject == TriggerObject)
            {
                hasTriggered = true;
                Cutscene.Play();
                LCharacterAction.pauseActions = true;
                LCharacterFellowCamera.notUpdateCamera = true;
            }
        }


        /// <summary>
        /// If Cutscene is setup to play on button down and on trigger, watch for the trigger event.
        /// </summary>
        /// <param name="other">The other collider.</param>
        void OnTriggerStay(Collider other)
        {
            if (StartMethod == StartMethod.OnTriggerStayAndButtonDown && !hasTriggered && other.gameObject == TriggerObject && Input.GetButtonDown(TriggerButtonName))
            {
                hasTriggered = true;
                Cutscene.Play();
                LCharacterAction.pauseActions = true;
                LCharacterFellowCamera.notUpdateCamera = true;
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (StartMethod == StartMethod.OnTriggerStayAndButtonDown && !hasTriggered && other.gameObject == TriggerObject && Input.GetButtonDown(TriggerButtonName))
            {
                hasTriggered = true;
                Cutscene.Play();
                LCharacterAction.pauseActions = true;
                LCharacterFellowCamera.notUpdateCamera = true;
            }
        }
    }

     
}