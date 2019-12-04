using UnityEngine;
using System.Collections;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animation", "Play Animation", CutsceneItemGenre.ActorItem)]
    public class PlayAnimationEvent : CinemaActorAction
    {
        public AnimationClip animationClip = null;

        public WrapMode wrapMode;

        public void Update()
        {
            // Loop & PingPong clips can be of any length, other wrap modes can not be longer than clip, but can be shorter
            if (wrapMode != WrapMode.Loop && wrapMode != WrapMode.PingPong && animationClip)
            {
                if (base.Duration > animationClip.length)
                {
                    base.Duration = animationClip.length;
                }
            }                
        }

        public override void Trigger(GameObject Actor)
        {
            Animation animation = Actor.GetComponent<Animation>();
            if (!animationClip || !animation)
            {
                return;
            }

            animation.wrapMode = wrapMode;
            //animation.Play(animationClip.name);
        }

        public override void UpdateTime(GameObject Actor, float runningTime, float deltaTime)
        {
            Animation animation = Actor.GetComponent<Animation>();

            if (!animation || animationClip == null)
            {
                return;
            }

            if (animation[animationClip.name] == null)
            {
                animation.AddClip(animationClip, animationClip.name);
            }

            AnimationState state = animation[animationClip.name];

            if (!animation.IsPlaying(animationClip.name))
            {
                animation.wrapMode = wrapMode;
                animation.Play(animationClip.name);
            }

            state.time = runningTime;
            state.enabled = true;
            animation.Sample();
            state.enabled = false;
        }

        public override void End(GameObject Actor)
        {
            Animation animation = Actor.GetComponent<Animation>();
            if (animation)
                animation.Stop();
        }
    }
}