using CinemaDirector;
using CinemaDirector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
    To create a custom timeline item, you must create a class that derives from one of:
        
        CinemaGlobalAction  -   An item that spans a period of time (an Action), and affects the global scene.
        CinemaGlobalEvent   -   An item that triggers at a specific point in time (an Event), and affects the global scene.
        CinemaActorAction   -   An item that spans a period of time (an Action), and affects 1 or more actors in the scene.
        CinemaActorEvent    -   An item that triggers at a specific point in time (an Event), and affects 1 or more actors in the scene.

    Your class will also need a CutsceneItemAttribute where you can set what category and label your item can be found under when adding
    an item to your timeline, as well as 1 or more genres that dictates which types of timeline tracks will allow the use of your item.

    Lastly, if your cutscene item manipulates any data in your scene, you will want your class to implement the IRevertable interface,
    which will help restore your scene to its original state whenever you enter or exit play mode. The required method implementations will
    be covered below.

    This simple example cutscene item will shrink an actor until it is gone over the duration of the item at a constant rate. (The same
    effect can be achieved using a curve track)
*/

[CutsceneItem("Examples", "Example Shrink", CutsceneItemGenre.ActorItem)]
class ExampleShrinkAction : CinemaActorAction, IRevertable
{
    // Include any fields needed by your cutscene item.
    Vector3 scaleStart = Vector3.one;
    Vector3 scaleEnd = Vector3.zero;

    /*
        IRevertable Implementation

        If your item manipulates any data in your scene, you need a way to revert any changes made during the cutscene, or else your cutscene
        item will permanently change your scene! There are a few different modes for reverting:

            Revert      -   Reverts the object to its initial state.
            Finalize    -   Sets the object to its final state.

        You can set different modes for both in the editor and during runtime. The IRevertable interface will also require you to implement
        a get and set for both the editor revert mode and runtime revert mode.
    */

    // Options for reverting in editor.
    [SerializeField]
    private RevertMode editorRevertMode = RevertMode.Revert;

    // Options for reverting during runtime.
    [SerializeField]
    private RevertMode runtimeRevertMode = RevertMode.Revert;

    public RevertMode EditorRevertMode
    {
        get { return editorRevertMode; }
        set { editorRevertMode = value; }
    }

    public RevertMode RuntimeRevertMode
    {
        get { return runtimeRevertMode; }
        set { runtimeRevertMode = value; }
    }

    /*
        IRevertable Implementation - CacheState

        Here is where you can cache any data that will be manipulated during your cutscene item. 
        
        When creating an Actor item, you may be manipulating multiple actors if your item is used in a MultiActor Track Group, 
        so be sure to use the GetActors() method to retrieve all actors in the group so that you can cache the data for each
        actor. To cache the data you will want to create a new RevertInfo object for each value that will be modified, and create
        an array containing all these objects that can be returned.        
    */

    public RevertInfo[] CacheState()
    {
        List<Transform> actors = new List<Transform>(GetActors());
        List<RevertInfo> reverts = new List<RevertInfo>();
        for (int i = 0; i < actors.Count; i++)
        {
            Transform go = actors[i];
            if (go != null)
            {
                Transform t = go.GetComponent<Transform>();
                if (t != null)
                {
                    reverts.Add(new RevertInfo(this, t, "localScale", t.localScale));
                }
            }
        }

        return reverts.ToArray();
    }

    /*
        Cutscene Item Methods

        Every cutscene item will require you to override the Trigger() callback method. If you are creating an Action item that spans a
        period of time, you will need to override the End() callback method as well. The Trigger() method is called when the cutscene time
        passes the beginning of an action or the point of an event whie moving forward through the cutscene. If the cutscene is played in
        reverse, or you are scrubbing through the cutscene backwards, it will not be called. Instead, ReverseTrigger() will be called (or
        Reverse() for events). The same is true for End(), where passing the end going backwards calls ReverseEnd().

        There are other optional callback methods you may override for more control. UpdateTime() is called during every update that the
        item is playing. SetTime() is called when the time is manually moved or set in the cutscene. These methods are for actions only.
        Pause(), Resume(), and Stop() are not needed in this example, but are called when the cutscene playback is paused, resumed, or 
        stopped. An example of when these methods are useful is during audio playback.
    */

    /// <summary>
    /// Called when the running time of the cutscene hits the firetime of the action,
    /// saves the initial scale of the actor.
    /// </summary>
    /// <param name="Actor">The actor to target for this action.</param>
    public override void Trigger(GameObject Actor)
    {
        Debug.Log("Trigger");
        if (Actor != null)
        {
            scaleStart = Actor.transform.localScale;
        }
    }

    /// <summary>
    /// Called when the running time of the cutscene exceeds the duration of the action,
    /// forces the object's scale to the final scale (zero).
    /// </summary>
    /// <param name="Actor">The actor to target for this action.</param>
    public override void End(GameObject Actor)
    {
        Debug.Log("End");
        if (Actor != null)
        {
            Actor.transform.localScale = scaleEnd;
        }
    }

    /// <summary>
    /// Called at each update when the action is to be played,
    /// calculate how far into the item we are, and set the 
    /// appropriate scale.
    /// </summary>
    /// <param name="Actor">The actor to target for this action.</param>
    /// <param name="time">The new running time.</param>
    /// <param name="deltaTime">The deltaTime since the last update call.</param>
    public override void UpdateTime(GameObject Actor, float time, float deltaTime)
    {
        float transition = time / Duration;
        LerpScale(Actor, scaleStart, scaleEnd, transition);
    }

    /// <summary>
    /// Called when the cutscene time is set/skipped manually,
    /// in this example we simply need to call UpdateTime().
    /// </summary>
    /// <param name="Actor">The actor to target for this action.</param>
    /// <param name="time">The new running time.</param>
    /// <param name="deltaTime">The deltaTime since the last update call.</param>
    public override void SetTime(GameObject Actor, float time, float deltaTime)
    {
        if (Actor != null)
        {
            if (time >= 0 && time <= Duration)
            {
                UpdateTime(Actor, time, deltaTime);
            }
        }
    }

    /// <summary>
    /// Reverse trigger. Called when scrubbing backwards past the start of the action.
    /// Set the scale back to the initial scale.
    /// </summary>
    /// <param name="Actor">The actor to target for this action.</param>
    public override void ReverseTrigger(GameObject Actor)
    {
        Debug.Log("ReverseTrigger");
        if (Actor != null)
        {
            Actor.transform.localScale = scaleStart;
        }
    }

    /// <summary>
    /// Reverse End. Called when scrubbing backwards past the end of the action.
    /// In this example, simply call End() to force the scale to the final scale (zero).
    /// </summary>
    /// <param name="Actor">The actor to target for this action.</param>
    public override void ReverseEnd(GameObject Actor)
    {
        Debug.Log("ReverseEnd");
        End(Actor);
    }

    //Include any other methods you need for your custom item.
    private void LerpScale (GameObject Actor, Vector3 scaleFrom, Vector3 scaleTo, float transition)
    {
        if (Actor != null)
        {
            Actor.transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, transition);
        }
    }
}

