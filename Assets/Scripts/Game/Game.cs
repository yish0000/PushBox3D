using System;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public enum GameState
    {
        NONE,
        STARTMENU,
        SELECTSTAGE,
        INGAME,
    };

    GameState m_gameState = GameState.NONE;

    public bool Init()
    {
        if (!ModuleManager.Instance.Init())
        {
            Debug.LogError("Failed to initialize the modules!");
            return false;
        }

        // Firstly, we show start menu panel.
        ChangeGameState(GameState.STARTMENU);

        return true;
    }

    public void Update()
    {
        // Update all the modules.
        ModuleManager.Instance.Update();

        // Process all the events.
        EventProcessQueue.Instance.Update();
    }

    // Change the current game state.
    public void ChangeGameState(GameState gs)
    {
        GameState oldState = m_gameState;
        m_gameState = gs;

        ModuleManager.Instance.DispatchEvent(new EventSwitchGameState(oldState, gs));
    }
}
