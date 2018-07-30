using System;
using System.Collections.Generic;
using UnityEngine;

public class UIModule : ModuleBase
{
    Dictionary<string, UIPanelBase> m_panelMap = new Dictionary<string, UIPanelBase>();

    public UIModule()
    {
        m_Type = "UI";
    }

    public override bool Init()
    {
        if (!base.Init())
            return false;

        InitPanels();
        InitEventHandlers();
        return true;
    }

    void InitPanels()
    {
        RegisterPanel("MenuStart", "UI/Panel_MenuStart", new UIMenuStart());
    }

    void InitEventHandlers()
    {
        AddEventListener(EventSwitchGameState.EVENTTYPE, (Event evt) => { OnEventSwitchGameState(evt); });
    }

    void OnEventSwitchGameState(Event evt)
    {
        EventSwitchGameState state = evt as EventSwitchGameState;
        if (state.newState == Game.GameState.STARTMENU)
        {
            GetPanelByName("MenuStart").CreatePanel();
        }
    }

    void RegisterPanel(string name, string path, UIPanelBase panel)
    {
        if (m_panelMap.ContainsKey(name))
        {
            Debug.LogWarning(string.Format("RegisterPanel, Panel name ({0}) duplicated!", name));
            return;
        }

        panel.ResPath = path;
        m_panelMap.Add(name, panel);
    }

    public UIPanelBase GetPanelByName(string name)
    {
        return m_panelMap.ContainsKey(name) ? m_panelMap[name] : null;
    }
}
