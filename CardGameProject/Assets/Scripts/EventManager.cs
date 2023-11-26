using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] List<Event> basicEventList;
    

    private List<Event> eventQueue;

    private void Start()
    {
        eventQueue = basicEventList;
    }

    public Event GetNextEvent()
    {
        Event nextEvent = eventQueue[0];
        eventQueue.RemoveAt(0);

        return nextEvent;
    }

    public void LoadEvent(Event eventObj, EventDisplay eventDisplay)
    {
        eventDisplay.Display(eventObj);
        eventDisplay.gameObject.SetActive(true);

    }
}
