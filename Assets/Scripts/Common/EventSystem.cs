using System;
using System.Collections.Generic;

/**
 base class of all the events.
 */
public class Event
{
    protected string m_Type;
    protected EventDispatcher m_Dispatcher;

    public string Type
    {
        get { return m_Type; }
        set { m_Type = value; }
    }

    public EventDispatcher Source
    {
        get { return m_Dispatcher; }
        set { m_Dispatcher = value; }
    }
}

/**
 Event dispatcher.
 */
public class EventDispatcher
{
    public delegate void EventListener(EventDispatcher sender, Event evt);

    //Dictionary<string, PriorityQueue<> >

    public void DispatchEvent(Event evt)
    {
        EventProcessQueue.Instance.AddEvent(this, evt);
    }

    public void DispatchEvent(string eventName)
    {
        Event evt = new Event();
        evt.Source = this;
        evt.Type = eventName;
        EventProcessQueue.Instance.AddEvent(this, evt);
    }

    public void OnEvent(Event evt)
    { 
    }
}

/**
 Event queue.
 */
public class EventProcessQueue
{
    struct EventNode
    {
        public EventDispatcher dispatcher;
        public Event evt;
    }

    Queue<EventNode> m_Queue = new Queue<EventNode>();

    static EventProcessQueue s_instance;

    public static EventProcessQueue Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = new EventProcessQueue();
            return s_instance;
        }
    }

    public void AddEvent(EventDispatcher dispatcher, Event evt)
    {
        EventNode node;
        node.dispatcher = dispatcher;
        node.evt = evt;
        m_Queue.Enqueue(node);
    }

    public void Update()
    {
        if (m_Queue.Count == 0)
            return;

        while (m_Queue.Count > 0)
        {
            EventNode node = m_Queue.Dequeue();
            node.dispatcher.OnEvent(node.evt);
        }
    }
}