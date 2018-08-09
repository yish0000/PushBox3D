using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageDrawer
{
    public GameObject gameObject { get; private set; }
    StageEditor.ElementType _paintType;

    public StageEditor.ElementType PaintType
    {
        get { return _paintType; }
        set
        {
            _paintType = value;
        }
    }
}
