using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An action that has some firetime and duration.
    /// </summary>
    public abstract class TimelineAction : TimelineItem
    {
        [SerializeField]
        protected float duration = 0f;

        /// <summary>
        /// The duration of the action
        /// </summary>
        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        /// <summary>
        /// The end time of this action. (Firetime + Duration).
        /// </summary>
        public float EndTime
        {
            get
            {
                return firetime + duration;
            }
        }

        /// <summary>
        /// Set a default duration of 5 seconds for most actions.
        /// </summary>
        public override void SetDefaults()
        {
            duration = 5f;
        }
    }
}