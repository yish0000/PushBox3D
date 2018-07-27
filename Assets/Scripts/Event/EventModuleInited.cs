using System;
using System.Collections.Generic;

public class EventModuleInited : Event
{
    public const string EVENTTYPE = "event_module_inited";

    public EventModuleInited()
    {
        Type = EVENTTYPE;
    }
};