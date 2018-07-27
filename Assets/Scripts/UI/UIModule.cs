using System;
using System.Collections.Generic;
using UnityEngine;

public class UIModule : ModuleBase
{
    Dictionary<string, PanelBase> m_panelMap;

    public UIModule()
    {
        m_Type = "UI";
    }

    public override bool Init()
    {
        if (!base.Init())
            return false;

        return true;
    }
}
