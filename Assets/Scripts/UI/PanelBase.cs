using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/**
 * UI面板基类
 */
class PanelBase
{
    GameObject m_panel;


    public bool CreatePanel(string resName)
    {
        Resources.LoadAsync(resName);
        return true;
    }

    public void DestroyPanel()
    {
        if (m_panel != null)
        {
            OnDestroy();
            UnityEngine.Object.Destroy(m_panel);
            m_panel = null;
        }
    }

    protected void OnCreate()
    { 
    }

    protected void OnDestroy()
    { 
    }

    protected void OnShow(bool bShow)
    {
    }

    public void Show(bool bShow)
    {
        if (m_panel != null)
            m_panel.SetActive(bShow);

        OnShow(bShow);
    }
}
