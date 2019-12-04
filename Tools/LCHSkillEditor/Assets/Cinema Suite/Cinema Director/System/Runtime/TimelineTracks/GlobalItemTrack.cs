using UnityEngine;

namespace CinemaDirector
{
    [TimelineTrackAttribute("Global Track", TimelineTrackGenre.GlobalTrack, CutsceneItemGenre.GlobalItem)]
    public class GlobalItemTrack : TimelineTrack
    {
        public CinemaGlobalEvent[] Events
        {
            get
            {
                return base.GetComponentsInChildren<CinemaGlobalEvent>();
            }
        }

        public CinemaGlobalAction[] Actions
        {
            get
            {
                return base.GetComponentsInChildren<CinemaGlobalAction>();
            }
        }

        public override TimelineItem[] TimelineItems
        {
            get
            {
                CinemaGlobalEvent[] events = Events;
                CinemaGlobalAction[] actions = Actions;

                TimelineItem[] items = new TimelineItem[events.Length + actions.Length];
                for (int i = 0; i < events.Length; i++)
                {
                    items[i] = events[i];
                }

                for (int i = 0; i < actions.Length; i++)
                {
                    items[i + events.Length] = actions[i];
                }

                return items;
            }
        }
    }
}