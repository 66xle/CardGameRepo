using System.Collections.Generic;
using System.Linq;

public class EventTracker
{
    private List<Event> events = new List<Event>();
    private int index = 0;
    private EventTracker nextEvent;

    private const string LINKEDEVENT = "Linked Event";

    public EventTracker(Event evt)
    {
        AddEvent(evt);
    }

    void AddEvent(Event evt)
    {
        if (evt.type == LINKEDEVENT)
        {
            // Grab start node guid to find event data
            string startNodeGUID = evt.DialogueNodeData.First(evt => evt.isStartNode == true).Guid;

            AddConnectedEvent(evt, startNodeGUID);

        }
        else
        {
            events.Add(evt);
        }
        
        if (evt.nextEvent != null)
            nextEvent = new EventTracker(evt.nextEvent);
    }

    void AddConnectedEvent(Event evt, string targetGUID)
    {
        // Grab start node guid to find event data
        DialogueNodeData nodeData = evt.DialogueNodeData.First(evt => evt.Guid == targetGUID);

        ChildEventData eventData = evt.listChildData.First(evt => evt.guid == nodeData.Guid);

        Event newEvent = new Event();
        newEvent.DialogueNodeData = eventData.dialogueNodeData;
        newEvent.type = LINKEDEVENT;

        events.Add(newEvent);

        if (nodeData.Connections.Count > 0)
        {
            AddConnectedEvent(evt, nodeData.Connections[0].TargetNodeGuid);
        }
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
