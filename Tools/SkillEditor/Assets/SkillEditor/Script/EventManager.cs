using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void EventDelegate(params object[] args);
public class EventManager  {

     
    static Dictionary<int, EventDelegate> events = new Dictionary<int, EventDelegate>();

    public static void AddEvent(int id, EventDelegate e)
    {
        EventDelegate e0;
        if (events.TryGetValue(id, out e0))
        {
            e0 += e;
        }
        else
        {
            events[id] = e;
        } 
    }
    public static void CallEvent(int id, params object[] args  )
    {
        EventDelegate e0;
        if (events.TryGetValue(id, out e0))
        {
            e0(args);
        }
    }
    public static void RemoveEvent(int id, EventDelegate e)
    {
        EventDelegate e0;
        if (events.TryGetValue(id, out e0))
        {
            e0 -= e;
        }
    }
}
