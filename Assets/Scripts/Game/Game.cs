using System;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public bool Init()
    {
        if (!ModuleManager.Instance.Init())
        {
            Debug.LogError("Failed to initialize the modules!");
            return false;
        }

        return true;
    }

    public void Update()
    {
        // Update all the modules.
        ModuleManager.Instance.Update();

        // Process all the events.
        EventProcessQueue.Instance.Update();
    }
}
