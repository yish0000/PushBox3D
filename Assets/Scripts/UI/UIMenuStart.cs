using System;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuStart : UIPanelBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RegisterButtonEvent("Widget/Btn_Test", () => { OnBtnTest(); });
    }

    void OnBtnTest()
    {
        Debug.Log("OnBtnTest");
    }
}
