// Cinema Suite
using CinemaDirector.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    public enum EVENT
    {
        END,
    }
    public delegate void EventFunction(EVENT type,object o);
    /// <summary>
    /// The Cutscene is the main Behaviour of Cinema Director.
    /// </summary>
    [ExecuteInEditMode, Serializable]
    public class Cutscene : MonoBehaviour, IOptimizable
    {
        #region Fields
        [SerializeField]
        private float duration = 30f; // Duration of cutscene in seconds.
        
        [SerializeField]
        private float playbackSpeed = 1f; // Multiplier for playback speed.

        [SerializeField]
        private bool isSkippable = true;

        [SerializeField]
        private bool isLooping = false;

        [SerializeField]
        private bool canOptimize = true;


        [SerializeField]
        public bool resetPositionOnEnd = true;

        [NonSerialized]
        private float runningTime = 0f; // Running time of the cutscene in seconds.

        [NonSerialized]
        private CutsceneState state = CutsceneState.Inactive;


        [NonSerialized]
        public object CutsonData;
        [NonSerialized]
        public EventFunction eventHandle = null;

        // Keeps track of the previous time an update was made.
        //private float previousTime = 0;

        // Has the Cutscene been optimized yet.
        private bool hasBeenOptimized = false;

        // Has the Cutscene been initialized yet.
        private bool hasBeenInitialized = false;

        // The cache of Track Groups that this Cutscene contains.
        private TrackGroup[] trackGroupCache;

        // A list of all the Tracks and Items revert info, to revert states on Cutscene entering and exiting play mode.
        private List<RevertInfo> revertCache = new List<RevertInfo>();
        #endregion

        // Event fired when Cutscene starts to play or is resumed.
        public event CutsceneHandler CutsceneStarted;

        // Event fired when Cutscene's runtime reaches it's duration.
        public event CutsceneHandler CutsceneFinished;

        // Event fired when Cutscene has been paused.
        public event CutsceneHandler CutscenePaused;

        /// <summary>
        /// Optimizes the Cutscene by preparing all tracks and timeline items into a cache.
        /// Call this on scene load in most cases. (Avoid calling in edit mode).
        /// </summary>
        public void Optimize()
        {
            if (canOptimize)
            {
                trackGroupCache = GetTrackGroups();
                hasBeenOptimized = true;
            }
            TrackGroup[] trackGroups = GetTrackGroups();
            for (int i = 0; i < trackGroups.Length; i++)
            {
                trackGroups[i].Optimize();
            }
        }

        /// <summary>
        /// Plays/Resumes the cutscene from inactive/paused states using a Coroutine.
        /// </summary>
        public void Play()
        {
            if (state == CutsceneState.Inactive)
            {
                StartCoroutine(freshPlay());
            }
            else if (state == CutsceneState.Paused)
            {
                state = CutsceneState.Playing;
                StartCoroutine(updateCoroutine());
            }

            if (CutsceneStarted != null)
            {
                CutsceneStarted(this, new CutsceneEventArgs());
            }
        }
        
        private IEnumerator freshPlay()
        {
            yield return StartCoroutine(PreparePlay());

            // Wait one frame.
            yield return null;

            // Beging playing
            state = CutsceneState.Playing;
            StartCoroutine(updateCoroutine());
        }

        /// <summary>
        /// Pause the playback of this cutscene.
        /// </summary>
        public void Pause()
        {
            if (state == CutsceneState.Playing)
            {
                StopCoroutine("updateCoroutine");
            }
            if (state == CutsceneState.PreviewPlaying || state == CutsceneState.Playing || state == CutsceneState.Scrubbing)
            {
                TrackGroup[] trackGroups = GetTrackGroups();
                for (int i = 0; i < trackGroups.Length; i++)
                {
                    trackGroups[i].Pause();
                }
            }
            state = CutsceneState.Paused;

            if (CutscenePaused != null)
            {
                CutscenePaused(this, new CutsceneEventArgs());
            }
        }

        /// <summary>
        /// Skip the cutscene to the end and stop it
        /// </summary>
        public void Skip()
        {
            if (isSkippable)
            {
                SetRunningTime(this.Duration);
                Stop();
            }
        }

        /// <summary>
        /// Stops the cutscene.
        /// </summary>
        public void Stop()
        {
           
            
            this.runningTime = 0f;


            if (!resetPositionOnEnd)
            {
                TrackGroup[] trackGroups = GetTrackGroups();
                for (int i = 0; i < trackGroups.Length; i++)
                {
                    trackGroups[i].Stop();
                }

                if (state != CutsceneState.Inactive)
                    revert();
            }

            

            if (state == CutsceneState.Playing)
            {
                StopCoroutine("updateCoroutine");
                if (state == CutsceneState.Playing && isLooping)
                {
                    state = CutsceneState.Inactive;
                    Play();
                }
                else
                {
                    state = CutsceneState.Inactive;
                }
            }
            else
            {
                state = CutsceneState.Inactive;
            }

            if (state == CutsceneState.Inactive)
            {
                if (CutsceneFinished != null)
                {
                    CutsceneFinished(this, new CutsceneEventArgs());
                }
            }
        }

        /// <summary>
        /// Updates the cutscene by the amount of time passed.
        /// </summary>
        /// <param name="deltaTime">The delta in time between the last update call and this one.</param>
        public void UpdateCutscene(float deltaTime)
        {
            this.RunningTime += (deltaTime * playbackSpeed);

            TrackGroup[] trackGroups = GetTrackGroups();
            for (int i = 0; i < trackGroups.Length; i++)
            {
                trackGroups[i].UpdateTrackGroup(RunningTime, deltaTime * playbackSpeed);
            }
            if (state != CutsceneState.Scrubbing)
            {
                if (runningTime >= duration || runningTime < 0f)
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Preview play readies the cutscene to be played in edit mode. Never use for runtime.
        /// This is necessary for playing the cutscene in edit mode.
        /// </summary>
        public void PreviewPlay()
        {
            if (state == CutsceneState.Inactive)
            {
                EnterPreviewMode();
            }
            else if (state == CutsceneState.Paused)
            {
                resume();
            }

            if (Application.isPlaying)
            {
                state = CutsceneState.Playing;
            }
            else
            {
                state = CutsceneState.PreviewPlaying;
            }
        }

        /// <summary>
        /// Play the cutscene from it's given running time to a new time
        /// </summary>
        /// <param name="newTime">The new time to make up for</param>
        public void ScrubToTime(float newTime)
        {
            float deltaTime = Mathf.Clamp(newTime, 0, Duration) - this.RunningTime;

            state = CutsceneState.Scrubbing;
            if (deltaTime != 0)
            {
                if (deltaTime > (1 / 30f))
                {
                    float prevTime = RunningTime;
                    List<float> milestones = getMilestones(RunningTime + deltaTime);
                    for (int i = 0; i < milestones.Count; i++)
                    {
                        float delta = milestones[i] - prevTime;
                        UpdateCutscene(delta);
                        prevTime = milestones[i];
                    }
                }
                else
                {
                    UpdateCutscene(deltaTime);
                }
            }
            else
            {
                Pause();
            }
        }

        /// <summary>
        /// Set the cutscene to the state of a given new running time and do not proceed to play the cutscene
        /// </summary>
        /// <param name="time">The new running time to be set.</param>
        public void SetRunningTime(float time)
        {
            List<float> milestones = getMilestones(time);
            for (int i = 0; i < milestones.Count; i++)
            {
                for (int j = 0; j < this.TrackGroups.Length; j++)
                {
                    this.TrackGroups[j].SetRunningTime(milestones[i]);
                }
            }

            this.RunningTime = time;
        }

        /// <summary>
        /// Set the cutscene into an active state.
        /// </summary>
        public void EnterPreviewMode()
        {
            EnterPreviewMode(runningTime);
        }

        /// <summary>
        /// Set the cutscene into an active state at the specified running time.
        /// </summary>
        /// <param name="time">The new running time to be set.</param>
        public void EnterPreviewMode(float time)
        {
            if (state == CutsceneState.Inactive)
            {
                initialize();
                bake();
                SetRunningTime(time);
                state = CutsceneState.Paused;
            }
        }

        /// <summary>
        /// Set the cutscene into an inactive state.
        /// </summary>
        public void ExitPreviewMode()
        {
            Stop();
        }

        /// <summary>
        /// Cutscene has been destroyed. Revert everything if necessary.
        /// </summary>
        protected void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                Stop();
            }
        }

        /// <summary>
        /// Exit and Re-enter preview mode at the current running time.
        /// Important for Mecanim Previewing.
        /// </summary>
        public void Refresh()
        {
            if (state != CutsceneState.Inactive)
            {
                float tempTime = runningTime;
                Stop();
                EnterPreviewMode();
                SetRunningTime(tempTime);
            }
        }

        /// <summary>
        /// Bake all Track Groups who are Bakeable.
        /// This is necessary for things like mecanim previewing.
        /// </summary>
        private void bake()
        {
            if (Application.isEditor)
            {
                for (int i = 0; i < this.TrackGroups.Length; i++)
                {
                    if (this.TrackGroups[i] is IBakeable)
                    {
                        (this.TrackGroups[i] as IBakeable).Bake();
                    }
                }
            }
        }

        /// <summary>
        /// The duration of this cutscene in seconds.
        /// </summary>
        public float Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
                if (this.duration <= 0f)
                {
                    this.duration = 0.1f;
                }
            }
        }

        /// <summary>
        /// Returns true if this cutscene is currently playing.
        /// </summary>
        public CutsceneState State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// The current running time of this cutscene in seconds. Values are restricted between 0 and duration.
        /// </summary>
        public float RunningTime
        {
            get
            {
                return this.runningTime;
            }
            set
            {
                runningTime = Mathf.Clamp(value, 0, duration);
            }
        }

        /// <summary>
        /// Get all Track Groups in this Cutscene. Will return cache if possible.
        /// </summary>
        /// <returns></returns>
        public TrackGroup[] GetTrackGroups()
        {
            // Return the cache if possible
            if (hasBeenOptimized)
            {
                return trackGroupCache;
            }

            return TrackGroups;
        }

        /// <summary>
        /// Get all track groups in this cutscene.
        /// </summary>
        public TrackGroup[] TrackGroups
        {
            get
            {
                return base.GetComponentsInChildren<TrackGroup>();
            }
        }

        /// <summary>
        /// Get the director group of this cutscene.
        /// </summary>
        public DirectorGroup DirectorGroup
        {
            get
            {
                return base.GetComponentInChildren<DirectorGroup>();
            }
        }

        /// <summary>
        /// Cutscene state is used to determine if the cutscene is currently Playing, Previewing (In edit mode), paused, scrubbing or inactive.
        /// </summary>
        public enum CutsceneState
        {
            Inactive,
            Playing,
            PreviewPlaying,
            Scrubbing,
            Paused
        }

        /// <summary>
        /// Enable this if the Cutscene does not have TrackGroups added/removed during running.
        /// </summary>
        public bool CanOptimize
        {
            get { return canOptimize; }
            set { canOptimize = value; }
        }

        /// <summary>
        /// True if Cutscene can be skipped.
        /// </summary>
        public bool IsSkippable
        {
            get { return isSkippable; }
            set { isSkippable = value; }
        }

        /// <summary>
        /// Will the Cutscene loop upon completion.
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
            set { isLooping = value; }
        }

        /// <summary>
        /// An important call to make before sampling the cutscene, to initialize all track groups 
        /// and save states of all actors/game objects.
        /// </summary>
        private void initialize()
        {
            saveRevertData();
            
            // Initialize each track group.
            for (int i = 0; i < this.TrackGroups.Length; i++)
            {
                this.TrackGroups[i].Initialize();
            }
            hasBeenInitialized = true;
        }

        /// <summary>
        /// Cache all the revert data.
        /// </summary>
        private void saveRevertData()
        {
            revertCache.Clear();
            // Build the cache of revert info.
            MonoBehaviour[] mbArray = this.GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < mbArray.Length; i++)
            {
                IRevertable revertable = mbArray[i] as IRevertable;
                if (revertable != null)
                {
                    RevertInfo[] ri = revertable.CacheState();
                    if(ri == null || ri.Length < 1)
                    {
                        Debug.Log(string.Format("Cinema Director tried to cache the state of {0}, but failed.", mbArray[i].name));
                    }
                    else
                    {
                        revertCache.AddRange(ri);
                    }
                }
            }
        }

        /// <summary>
        /// Revert all data that has been manipulated by the Cutscene.
        /// </summary>
        private void revert()
        {
            for (int i = 0; i < revertCache.Count; i++)
            {
                RevertInfo revertable = revertCache[i];
                if (revertable != null)
                {
                    if ((revertable.EditorRevert == RevertMode.Revert && !Application.isPlaying) ||
                        (revertable.RuntimeRevert == RevertMode.Revert && Application.isPlaying))
                    {
                        revertable.Revert();
                    }
                }
            }
        }

        /// <summary>
        /// Get the milestones between the current running time and the given time.
        /// </summary>
        /// <param name="time">The time to progress towards</param>
        /// <returns>A list of times that state changes are made in the Cutscene.</returns>
        private List<float> getMilestones(float time)
        {
            // Create a list of ordered milestone times.
            List<float> milestoneTimes = new List<float>();
            milestoneTimes.Add(time);
            for (int i = 0; i < this.TrackGroups.Length; i++)
            {
                List<float> times = this.TrackGroups[i].GetMilestones(RunningTime, time);
                for (int j = 0; j < times.Count; j++)
                {
                    if (!milestoneTimes.Contains(times[j]))
                        milestoneTimes.Add(times[j]);
                }
            }

            milestoneTimes.Sort();
            if (time < RunningTime)
            {
                milestoneTimes.Reverse();
            }

            return milestoneTimes; 
        }

        private IEnumerator PreparePlay()
        {
            if (!hasBeenOptimized)
            {
                Optimize();
            }
            if (!hasBeenInitialized)
            {
                initialize();
            }
            yield return null;
        }

        /// <summary>
        /// Couroutine to call UpdateCutscene while the cutscene is in the playing state.
        /// </summary>
        /// <returns></returns>
        private IEnumerator updateCoroutine()
        {
            //bool firstFrame = true;
            while (state == CutsceneState.Playing)
            {
                //if (firstFrame)
                //{
                //    UpdateCutscene(0);
                //    firstFrame = false;
                //}
                //else
                {
                    UpdateCutscene(Time.deltaTime);
                }
                yield return null;
            }

            if (eventHandle == null)
            {
                Debug.LogError("anim end not call back");
            }
            else
            {
                eventHandle(EVENT.END, CutsonData);
            }
        }

        /// <summary>
        /// Prepare all track groups for resuming from pause.
        /// </summary>
        private void resume()
        {
            for (int i = 0; i < this.TrackGroups.Length; i++)
            {
                this.TrackGroups[i].Resume();
            }
        }

        /// <summary>
        /// Will exit preview mode, cache all actor data, and re-enter the cutscene.
        /// Call when adding new items or curves to the cutscene.
        /// </summary>
        public void recache()
        {
            if (state != CutsceneState.Inactive)
            {
                float runningTime = RunningTime;
                ExitPreviewMode();
                EnterPreviewMode();
                ScrubToTime(runningTime);
            }
        }
    }

    // Delegate for handling Cutscene Events
    public delegate void CutsceneHandler(object sender, CutsceneEventArgs e);

    /// <summary>
    /// Cutscene event arguments. Blank for now.
    /// </summary>
    public class CutsceneEventArgs : EventArgs
    {
        public CutsceneEventArgs()
        {
        }
    }

}
