using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] List<Event> singleEventList;
    [SerializeField] List<Event> linkedEventList;

    public List<EventTracker> events = new List<EventTracker>();
    

    private List<EventTracker> eventQueue;

    private void Awake()
    {
        SetupEvents();
        eventQueue = events;
    }

    void SetupEvents()
    {
        singleEventList.ForEach(evt => events.Add(new EventTracker(evt)));
        linkedEventList.ForEach(evt => events.Add(new EventTracker(evt)));
    }

    public EventTracker GetEventFromQueue()
    {
        EventTracker nextEvent = eventQueue[0];
        eventQueue.RemoveAt(0);

        return nextEvent;
    }

    public void InsertIntoQueue(EventTracker eventTracker)
    {
        eventQueue.Insert(0, eventTracker);
    }
}
