using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageEditorWindow : EditorWindow
{
    StageEditor targetObject = new StageEditor();

    void OnEnable()
    {
        targetObject.CreateNew();
    }

    void OnGUI()
    {
        EditorGUIUtility.labelWidth = 80f;
        EditorGUILayout.Space();

        GUILayout.Label("Stage Information");
        GUILayout.Label(string.Format("Filepath: {0}", targetObject.StageFile));
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create New"))
        {
            OnBtnCreateNew();
        }

        if (GUILayout.Button("Load"))
        {
            OnBtnLoad();
        }

        if (GUILayout.Button("Save"))
        {
            OnBtnSave();
        }

        GUILayout.EndHorizontal();

        int mapSize = EditorGUILayout.IntSlider("Map Size:", targetObject.StageMapSize, 10, 100);
        if (mapSize != targetObject.StageMapSize)
        {
            targetObject.StageMapSize = mapSize;
        }

        EditorGUILayout.Space();

        DrawElementTypes();
    }

    void OnSceneGUI()
    {
        UnityEngine.Event evt = UnityEngine.Event.current;

        if (targetObject.CurrentSelType != StageEditor.ElementType.None)
        {
            if (evt.type == EventType.layout)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));


        }
    }

    void OnBtnCreateNew()
    {
        targetObject.CreateNew();
    }

    void OnBtnLoad()
    { 
    }

    void OnBtnSave()
    { 
    }

    void DrawElementTypes()
    {
        GUILayout.Label("Stage Elements", GUILayout.Width(250f));

        int nRowCount = StageEditor.ELEMENT_TYPES.Length / 2;
        if (StageEditor.ELEMENT_TYPES.Length %2 != 0)
            nRowCount++;
        for (int i = 0; i < nRowCount; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 2; j++)
            {
                int n = i * 2 + j;
                bool isActive = (int)targetObject.CurrentSelType == n;
                if (GUILayout.Toggle(isActive, StageEditor.ELEMENT_TYPES[n], j == 0 ? "ButtonLeft" : "ButtonRight") != isActive)
                    targetObject.CurrentSelType = (StageEditor.ElementType)n;
            }
            GUILayout.EndHorizontal();
        }
    }
}
