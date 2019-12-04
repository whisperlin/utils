// Cinema Suite
using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
namespace CinemaDirector
{
    /// <summary>
    /// Event for loading a new level
    /// </summary>
    [CutsceneItem("Utility", "Load Level", CutsceneItemGenre.GlobalItem)]
    public class LoadLevelEvent : CinemaGlobalEvent
    {
        public enum LoadLevelArgument
        {
            ByIndex,
            ByName,
        }

        public enum LoadLevelType
        {
            Standard,
            Additive,
            Async,
            AdditiveAsync,
        }

        public LoadLevelArgument Argument = LoadLevelArgument.ByIndex;
        public LoadLevelType Type = LoadLevelType.Standard;

        // The index of the level to be loaded.
        public int Level = 0;

        // The name of the level to be loaded.
        public string LevelName;

        /// <summary>
        /// Trigger the level load. Only in Runtime.
        /// </summary>
        public override void Trigger()
        {
            if (Application.isPlaying)
            {
                if (Argument == LoadLevelArgument.ByIndex)
                {
                    if (Type == LoadLevelType.Standard)
                    {
#if UNITY_5_3_OR_NEWER
                        SceneManager.LoadScene(Level);
#else
                        Application.LoadLevel(Level);
#endif
                    }
                    else if (Type == LoadLevelType.Additive)
                    {
#if UNITY_5_3_OR_NEWER
                        SceneManager.LoadScene(Level);
#else
                        Application.LoadLevelAdditive(Level);
#endif
                    }
                    else if (Type == LoadLevelType.Async)
                    {
#if UNITY_5_3_OR_NEWER
                        SceneManager.LoadSceneAsync(Level);
#else
                        Application.LoadLevelAsync(Level);
#endif
                    }
                    else if (Type == LoadLevelType.AdditiveAsync)
                    {
#if UNITY_5_3_OR_NEWER
                        SceneManager.LoadSceneAsync(Level);
#else
                        Application.LoadLevelAdditiveAsync(Level);
#endif
                    }
                }
                else if (Argument == LoadLevelArgument.ByName)
                {
                    if (Type == LoadLevelType.Standard)
                    {
#if UNITY_5_3_OR_NEWER
                        SceneManager.LoadScene(LevelName);
#else
                        Application.LoadLevel(LevelName);
#endif
                    }
                    else if (Type == LoadLevelType.Additive)
                    {
#if UNITY_5_3_OR_NEWER
                        SceneManager.LoadScene(LevelName);
#else
                        Application.LoadLevelAdditive(LevelName);
#endif
                    }
                    else if (Type == LoadLevelType.Async)
                    {
#if UNITY_5_3_OR_NEWER
                        SceneManager.LoadSceneAsync(LevelName);
#else
                        Application.LoadLevelAsync(LevelName);
#endif
                    }
                    else if (Type == LoadLevelType.AdditiveAsync)
                    {
#if UNITY_5_3_OR_NEWER
                        SceneManager.LoadSceneAsync(LevelName);
#else
                        Application.LoadLevelAdditiveAsync(LevelName);
#endif
                    }
                }
            }
        }
    }
}