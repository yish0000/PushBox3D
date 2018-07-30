using System;
using System.Collections.Generic;

public class EventSwitchGameState : Event
{
    public const string EVENTTYPE = "event_switch_game_state";

    public Game.GameState oldState;
    public Game.GameState newState;

    public EventSwitchGameState(Game.GameState o, Game.GameState n)
    {
        Type = EVENTTYPE;
        oldState = o;
        newState = n;
    }
};