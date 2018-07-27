using System;
using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : EventDispatcher
{
    Dictionary<string, ModuleBase> m_Modules = new Dictionary<string,ModuleBase>();

    static ModuleManager s_instance;

    public static ModuleManager Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = new ModuleManager();
            return s_instance;
        }
    }

    public void RegisterModule(ModuleBase module)
    {
        if (m_Modules.ContainsKey(module.ModuleType))
        {
            Debug.LogError(string.Format("Module (%s) duplicated!", module.ModuleType));
            return;
        }

        m_Modules.Add(module.ModuleType, module);
    }

    public bool Init()
    {
        RegisterModule(new UIModule());

        Dictionary<string, ModuleBase>.Enumerator it = m_Modules.GetEnumerator();
        while (it.MoveNext())
        { 
            ModuleBase module = it.Current.Value;
            if (!module.Init())
            {
                Debug.LogError(string.Format("ModuleManager::init, Module ({0}) initialized failed!", module.ModuleType));
                return false;
            }
        }

        // 所有模块初始化完毕
        DispatchEvent(EventModuleInited.EVENTTYPE);
        return true;
    }

    public void Update()
    {
        Dictionary<string, ModuleBase>.Enumerator it = m_Modules.GetEnumerator();
        while (it.MoveNext())
        {
            ModuleBase module = it.Current.Value;
            module.Update();
        }
    }

    public ModuleBase GetModule(string name)
    {
        return m_Modules.ContainsKey(name) ? m_Modules[name] : null;
    }
};