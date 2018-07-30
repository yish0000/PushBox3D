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

public delegate void EventListener(Event evt);

/**
 Event dispatcher.
 */
public class EventDispatcher
{
    class PrioritySorter : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y - x;
        }
    }

    Dictionary<string, SortedDictionary<int, List<EventListener>>> m_EventMap = new Dictionary<string,SortedDictionary<int,List<EventListener>>>();

    public void AddEventListener(string eventType, EventListener listener, int priority = 0)
    {
        SortedDictionary<int, List<EventListener>> sortedDic = null;
        if (!m_EventMap.ContainsKey(eventType))
        {
            sortedDic = new SortedDictionary<int, List<EventListener>>(new PrioritySorter());
            m_EventMap.Add(eventType, sortedDic);
        }
        else
        {
            sortedDic = m_EventMap[eventType];
        }

        List<EventListener> queue = null;
        if (!sortedDic.ContainsKey(priority))
        {
            queue = new List<EventListener>();
            sortedDic.Add(priority, queue);
        }
        else
        { 
            queue = sortedDic[priority];
        }

        queue.Add(listener);
    }

    public void RemoveEventListener(string eventType, EventListener listener)
    {
        if (!m_EventMap.ContainsKey(eventType))
            return;

        SortedDictionary<int, List<EventListener>> sortedDic = m_EventMap[eventType];
        SortedDictionary<int, List<EventListener>>.Enumerator it = sortedDic.GetEnumerator();
        while (it.MoveNext())
        {
            List<EventListener> list = it.Current.Value;
            list.Remove(listener);
        }
    }

    public void RemoveAllForEvent(string eventType)
    {
        m_EventMap.Remove(eventType);
    }

    public void RemoveAllEvents()
    {
        m_EventMap.Clear();
    }

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
        if (!m_EventMap.ContainsKey(evt.Type))
            return;

        SortedDictionary<int, List<EventListener>> sortedDic = m_EventMap[evt.Type];
        SortedDictionary<int, List<EventListener>>.Enumerator it = sortedDic.GetEnumerator();
        while (it.MoveNext())
        {
            List<EventListener> list = it.Current.Value;
            List<EventListener>.Enumerator it2 = list.GetEnumerator();
            while (it2.MoveNext())
            {
                it2.Current(evt);
            }
        }
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
        while (m_Queue.Count > 0)
        {
            EventNode node = m_Queue.Dequeue();
            node.dispatcher.OnEvent(node.evt);
        }
    }
}