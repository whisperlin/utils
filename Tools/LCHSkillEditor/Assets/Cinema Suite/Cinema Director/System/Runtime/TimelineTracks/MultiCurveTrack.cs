// Cinema Suite 2014

using UnityEngine;
namespace CinemaDirector
{
    [TimelineTrackAttribute("Curve Track", TimelineTrackGenre.MultiActorTrack, CutsceneItemGenre.MultiActorCurveClipItem)]
    public class MultiCurveTrack : TimelineTrack, IActorTrack
    {

        public override void Initialize()
        {
            for (int i = 0; i < this.TimelineItems.Length; i++)
            {
                CinemaMultiActorCurveClip clipCurve = this.TimelineItems[i] as CinemaMultiActorCurveClip;
                clipCurve.Initialize();
            }
        }

        public override void UpdateTrack(float time, float deltaTime)
        {
            base.elapsedTime = time;
            for (int i = 0; i < this.TimelineItems.Length; i++)
            {
                CinemaMultiActorCurveClip clipCurve = this.TimelineItems[i] as CinemaMultiActorCurveClip;
                clipCurve.SampleTime(time);
            }
        }

        public override void Stop()
        {
            for (int i = 0; i < this.TimelineItems.Length; i++)
            {
                CinemaMultiActorCurveClip clipCurve = this.TimelineItems[i] as CinemaMultiActorCurveClip;
                clipCurve.Revert();
            }
        }

        public override TimelineItem[] TimelineItems
        {
            get
            {
                return GetComponentsInChildren<CinemaMultiActorCurveClip>();
            }
        }

        public Transform Actor
        {
            get
            {
                ActorTrackGroup component = base.transform.parent.GetComponent<ActorTrackGroup>();
                if (component == null)
                {
                    return null;
                }
                return component.Actor;
            }
        }
    }
}