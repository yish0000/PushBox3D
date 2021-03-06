﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class StageUtil
{
    public static StageData.StageElement[] ELEMENT_TYPE_ENUMS = new StageData.StageElement[] {
        StageData.StageElement.None, StageData.StageElement.Floor, StageData.StageElement.Wall,
        StageData.StageElement.BornPoint, StageData.StageElement.Box, StageData.StageElement.Target,
    };

    static string[] element_asset_path = new string[] {
        "",
        "Models/prefabs/Floor",
        "Models/prefabs/Wall",
        "",
        "Models/prefabs/largeCrate",
        ""
    };

    public static int GetPaintTypeIndex(StageData.StageElement ele)
    {
        for (int i = 0; i < ELEMENT_TYPE_ENUMS.Length; i++)
        {
            if (ELEMENT_TYPE_ENUMS[i] == ele)
                return i;
        }

        return -1;
    }

    public static UnityEngine.Object GetElementAsset(StageData.StageElement ele)
    {
        string path = element_asset_path[GetPaintTypeIndex(ele)];
        if (path == null || path.Length == 0)
            return null;
        return GameUtils.LoadResource(path);
    }

    static Vector3[] element_asset_pos_offset = new Vector3[] {
        Vector3.zero,
        new Vector3(0.5f, 0f, 0.5f),
        new Vector3(0.5f, 1f, 0.5f),
        new Vector3(0f, 0f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(0f, 0f, 0f),
    };

    public static Vector3 GetElementOffset(StageData.StageElement ele)
    {
        int index = GetPaintTypeIndex(ele);
        if (index >= 0 && index < element_asset_pos_offset.Length)
            return element_asset_pos_offset[index];
        return Vector3.zero;
    }

    public static Vector3 GetElementWorldPos(StageData.StageElement ele, float x, float z)
    {
        Vector3 vNew = new Vector3(x, 0, z);
        Vector3 vOffset = GetElementOffset(ele);
        return vNew + vOffset;
    }
}