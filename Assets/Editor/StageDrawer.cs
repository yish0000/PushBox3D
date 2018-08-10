using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageDrawer
{
    StageData _data;
    StageData.StageElement _paintType;
    UnityEngine.Object[] _resources = new UnityEngine.Object[(int)StageData.StageElement.Num];    // For stage brush.
    Vector2 _brushCenter;
    GameObject _curBrushShape;

    public StageData.StageElement PaintType
    {
        get { return _paintType; }
        set
        {
            if (_paintType != value)
            {
                _paintType = value;
                UpdatePaintType();
            }
        }
    }
    public GameObject gameObject { get; private set; }

    public StageDrawer(StageData data)
    {
        _data = data;
        InitMesh("__stageDrawer");
    }

    void InitMesh(string name)
    {
        if (gameObject != null)
            GameObject.DestroyImmediate(gameObject);
        
        gameObject = GameObject.Find(name);
        if (gameObject == null)
            gameObject = new GameObject(name);
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        for (int i = 0; i < (int)StageData.StageElement.Num; i++)
        {
            _resources[i] = StageUtil.GetElementAsset((StageData.StageElement)i);
        }
    }

    void UpdatePaintType()
    {
        if (_curBrushShape)
            GameObject.DestroyImmediate(_curBrushShape);

        UnityEngine.Object asset = _resources[(int)_paintType];
        if (asset != null)
        {
            GameObject obj = UnityEngine.Object.Instantiate(asset) as GameObject;
            obj.transform.SetParent(gameObject.transform);
            obj.transform.position = StageUtil.GetElementWorldPos(_paintType, _brushCenter.x, _brushCenter.y);

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
                renderer.sharedMaterial.SetColor("Main Color", new Color(1f, 1f, 1f, 0.5f));
            }

            _curBrushShape = obj;
        }
    }

    public void SetBrushCenter(int x, int z)
    {
        Vector3 vRealPos = StageUtil.GetElementWorldPos(_paintType, x, z);
        if (_curBrushShape != null)
            _curBrushShape.transform.position = vRealPos;
    }

    public void PaintAt(int x, int z)
    { 
    }
}
