using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class StageEditorWindow : EditorWindow
{
    StageEditor targetObject;

    void Awake()
    {
        GameObject world = GameObject.Find("World");
        if (world == null)
            return;

        GameObject stageRoot = GameObject.Find("World/Stage");
        if (stageRoot != null)
            GameObject.DestroyImmediate(stageRoot);

        stageRoot = new GameObject("Stage");
        stageRoot.transform.SetParent(world.transform);
        stageRoot.transform.localPosition = Vector3.zero;

        targetObject = new StageEditor();
        targetObject.Init();

        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDestroy()
    {
        GameObject stageRoot = GameObject.Find("World/Stage");
        if (stageRoot != null)
            GameObject.DestroyImmediate(stageRoot);

        GameObject drawer = GameObject.Find("__stageDrawer");
        if (drawer != null)
            GameObject.DestroyImmediate(drawer);

        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (targetObject.CurrentSelType != StageEditor.StagePaintType.Select)
        {
            UnityEngine.Event evt = UnityEngine.Event.current;

            if (evt.type == EventType.layout)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

            if (targetObject.Drawer != null)
            {
                int x, z;
                bool isHit = targetObject.GetCurrentGrid(evt, out x, out z);
                if (isHit)
                {
                    targetObject.Drawer.SetBrushCenter(x, z);
                    if (evt.isMouse && evt.button == 0 && (evt.type == EventType.mouseDown || evt.type == EventType.mouseDrag))
                        targetObject.Drawer.PaintAt(x, z);

                    SceneView.RepaintAll();
                }
            }
        }
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

    void OnBtnCreateNew()
    {
        if (targetObject != null)
            targetObject.CreateNew();
    }

    void OnBtnLoad()
    {
        string loadPath = EditorUtility.OpenFilePanel("Open Stage file", "StreamingAssets", "");
        if (loadPath.Length > 0)
        {
            if (targetObject != null)
            {
                if (loadPath.EndsWith(".xml"))
                    targetObject.LoadXML(loadPath);
                else
                    targetObject.Load(loadPath);
            }
        }
    }

    void OnBtnSave()
    {
        string savePath = targetObject.StageFile;
        if (savePath.Length == 0)
            savePath = EditorUtility.SaveFilePanel("Save Stage File", "StreamingAssets", "", "");
        if (targetObject != null)
        {
            if (savePath.EndsWith(".xml"))
                targetObject.SaveXML(savePath);
            else
                targetObject.Save(savePath);
        }
    }

    public static string[] ELEMENT_TYPES = new string[] { "None", "Floor", "Wall", "BornPoint", "Box", "Target" };

    void DrawElementTypes()
    {
        GUILayout.Label("Stage Elements", GUILayout.Width(250f));

        GUILayout.BeginHorizontal();
        bool bIsSelect = targetObject.CurrentSelType == StageEditor.StagePaintType.Select;
        if (GUILayout.Toggle(bIsSelect, "Select", "Button") != bIsSelect)
            targetObject.CurrentSelType = StageEditor.StagePaintType.Select;
        GUILayout.EndHorizontal();

        int nRowCount = ELEMENT_TYPES.Length / 2;
        if (ELEMENT_TYPES.Length % 2 != 0)
            nRowCount++;
        for (int i = 0; i < nRowCount; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 2; j++)
            {
                int n = i * 2 + j;
                bool isActive = targetObject.CurrentSelType == (StageEditor.StagePaintType)n;
                if (GUILayout.Toggle(isActive, ELEMENT_TYPES[n], j == 0 ? "ButtonLeft" : "ButtonRight") != isActive)
                    targetObject.CurrentSelType = (StageEditor.StagePaintType)n;
            }
            GUILayout.EndHorizontal();
        }
    }
}
