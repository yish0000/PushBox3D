using System;
using System.Collections.Generic;

public class EventGameStart : Event
{
    public const string EVENTTYPE = "event_game_start";

    public EventGameStart()
    {
        Type = EVENTTYPE;
    }
};