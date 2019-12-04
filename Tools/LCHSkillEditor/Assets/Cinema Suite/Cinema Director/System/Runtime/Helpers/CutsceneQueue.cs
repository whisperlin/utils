using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Plays through a list of given cutscenes one by one.
    /// </summary>
    public class CutsceneQueue : MonoBehaviour
    {
        public List<Cutscene> Cutscenes;
        private int index = 0;

        /// <summary>
        /// Play the first cutscene and waits for it to finish
        /// </summary>
        void Start()
        {
            if (Cutscenes != null && Cutscenes.Count > 0)
            {
                Cutscenes[index].CutsceneFinished += CutsceneQueue_CutsceneFinished;
                Cutscenes[index].Play();
            }
        }

        /// <summary>
        /// On cutscene finish, play the next cutscene.
        /// </summary>
        void CutsceneQueue_CutsceneFinished(object sender, CutsceneEventArgs e)
        {
            Cutscenes[index].CutsceneFinished -= CutsceneQueue_CutsceneFinished;
            if (Cutscenes != null && index + 1 < Cutscenes.Count)
            {
                index++;
                Cutscenes[index].Play();
                Cutscenes[index].CutsceneFinished += CutsceneQueue_CutsceneFinished;
            }
        }
    }
}