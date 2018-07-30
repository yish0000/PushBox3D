using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils
{
    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return EntryPoint.Instance.StartCoroutine(routine);
    }

    public static void LoadResourceAsync(string resPath, Action<UnityEngine.Object> cb)
    {
        StartCoroutine(LoadResourceAsyncCoroutine(resPath, cb));
    }

    static IEnumerator LoadResourceAsyncCoroutine(string resPath, Action<UnityEngine.Object> cb)
    {
        ResourceRequest rq = Resources.LoadAsync(resPath);
        if (rq == null)
        {
            Debug.LogWarning(string.Format("Failed to load resource with name '{0}'", resPath));

            if (cb != null)
                cb(null);
            yield break;
        }

        while (!rq.isDone)
            yield return null;

        UnityEngine.Object asset = rq.asset;
        if (asset == null)
            Debug.LogWarning(string.Format("Failed to load resource with name '{0}'", resPath));

        if (cb != null)
            cb(asset);
    }

    public static UnityEngine.Object LoadResource(string resPath)
    {
        UnityEngine.Object asset = Resources.Load(resPath);
        if (asset == null)
            Debug.LogWarning(string.Format("Failed to load resource with name '{0}'", resPath));
        return asset;
    }
}
