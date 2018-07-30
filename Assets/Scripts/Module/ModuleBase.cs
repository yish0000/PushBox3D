using System;
using System.Collections.Generic;
using UnityEngine;

public class ModuleBase
{
    protected string m_Type;

    public string ModuleType
    {
        get { return m_Type; }
    }

    public virtual bool Init()
    {
        AddEventListener(EventModuleInited.EVENTTYPE, (Event evt) =>
        {
            OnEventModuleInited();
        });

        return true;
    }

    public virtual void Update()
    {
    }

    public void AddEventListener(string eventType, EventListener listener, int priority = 0)
    {
        ModuleManager.Instance.AddEventListener(eventType, listener, priority);
    }

    public void DispatchEvent(string eventName)
    {
        ModuleManager.Instance.DispatchEvent(eventName);
    }

    public void DispatchEvent(Event evt)
    {
        ModuleManager.Instance.DispatchEvent(evt);
    }

    public virtual void OnEventModuleInited()
    {
        Debug.Log(string.Format("Module ({0}) initialized.", m_Type));
    }
}
