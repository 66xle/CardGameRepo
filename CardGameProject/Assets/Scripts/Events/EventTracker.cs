using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EventTracker
{
    private List<Event> events = new List<Event>();
    private int index;

    public EventTracker(Event evt)
    {
        AddEvent(evt);
    }

    void AddEvent(Event evt)
    {
        events.Add(evt);

        if (evt.nextEvent != null)
            AddEvent(evt.nextEvent);
    }

    public void EventCompleted()
    {
        index++;
    }

    public bool IsEventFinished()
    { 
        if (index == events.Count)
        {
            return true;
        }

        return false;
    }

    public Event GetEvent()
    {
        return events[index];
    }
}
