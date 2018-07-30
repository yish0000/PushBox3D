using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/**
 * UI面板基类
 */
public class UIPanelBase
{
    enum PanelStatus
    {
        IDLE,
        ASYNC_LOADING,
        READY,
    };

    string m_ResPath;
    GameObject m_panel;
    bool m_bShow;
    bool m_bSyncLoad;
    PanelStatus m_Status;

    public string ResPath
    {
        get { return m_ResPath; }
        set { m_ResPath = value; }
    }

    public UIPanelBase()
    {
        m_Status = PanelStatus.IDLE;
        m_bShow = true;
        m_bSyncLoad = false;
    }

    public bool CreatePanel()
    {
        if (m_panel)
            return true;

        Action<UnityEngine.Object> onLoaded = (UnityEngine.Object asset) =>
        {
            if (asset == null)
                return;

            if (m_Status != PanelStatus.ASYNC_LOADING)
                return;

            m_panel = UnityEngine.Object.Instantiate(asset) as GameObject;
            if (m_panel == null)
            {
                Debug.LogWarning(string.Format("CreatePanel, the asset({0}) cannot be instantiated as a GameObject!!", m_ResPath));
                return;
            }

            m_Status = PanelStatus.READY;
            m_panel.transform.SetParent(GetUIRoot().transform);
            m_panel.transform.localPosition = Vector3.zero;
            m_panel.transform.localRotation = Quaternion.identity;
            m_panel.transform.localScale = Vector3.one;
            m_panel.SetActive(m_bShow);

            OnCreate();

            if (m_bShow)
                OnShow(m_bShow);
        };

        if (m_bSyncLoad)
        {
            UnityEngine.Object asset = GameUtils.LoadResource(m_ResPath);
            if (asset == null)
                return false;

            onLoaded(asset);
            return true;
        }
        else
        {
            GameUtils.LoadResourceAsync(m_ResPath, onLoaded);
            m_Status = PanelStatus.ASYNC_LOADING;
            return true;
        }
    }

    GameObject GetUIRoot()
    {
        return GameObject.Find("Canvas");
    }

    public void DestroyPanel()
    {
        if (m_panel != null)
        {
            OnDestroy();
            UnityEngine.Object.Destroy(m_panel);
            m_panel = null;
        }
        else
        { 
            // If we are in the loading status.
            if (m_Status == PanelStatus.ASYNC_LOADING)
                m_Status = PanelStatus.IDLE;
        }
    }

    protected virtual void OnCreate()
    { 
    }

    protected virtual void OnDestroy()
    { 
    }

    protected virtual void OnShow(bool bShow)
    {
    }

    public void Show(bool bShow)
    {
        if (m_bShow != bShow)
        {
            if (m_panel != null)
                m_panel.SetActive(bShow);

            OnShow(bShow);
            m_bShow = bShow;
        }
    }

    protected void RegisterButtonEvent(string buttonPath, UnityAction cb)
    {
        GameObject btnObj = m_panel.transform.Find("Widget/Btn_Test").gameObject;

        Button.ButtonClickedEvent evt = new Button.ButtonClickedEvent();
        evt.AddListener(cb);
        Button btn = btnObj.GetComponent<Button>();
        btn.onClick = evt;
    }
}
