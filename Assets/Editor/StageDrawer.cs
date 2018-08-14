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
    Dictionary<int, List<GameObject>> _nodeGeometry = new Dictionary<int,List<GameObject>>();

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
        gameObject = GameObject.Find(name);
        if (gameObject != null)
            GameObject.DestroyImmediate(gameObject);
        gameObject = new GameObject(name);
        //gameObject.hideFlags = HideFlags.HideAndDontSave;

        for (int i = 0; i < (int)StageData.StageElement.Num; i++)
        {
            _resources[i] = StageUtil.GetElementAsset(StageUtil.ELEMENT_TYPE_ENUMS[i]);
        }
    }

    void UpdatePaintType()
    {
        if (_curBrushShape)
            GameObject.DestroyImmediate(_curBrushShape);

        UnityEngine.Object asset = _resources[StageUtil.GetPaintTypeIndex(_paintType)];
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
        _brushCenter.x = (float)x;
        _brushCenter.y = (float)z;

        Vector3 vRealPos = StageUtil.GetElementWorldPos(_paintType, x, z);
        if (_curBrushShape != null)
            _curBrushShape.transform.position = vRealPos;
    }

    public void PaintAt(int x, int z)
    {
        bool bChanged = false;
        if (PaintType == StageData.StageElement.None)
        {
            _data.Clear(x, z);
            bChanged = true;
        }
        else if (PaintType == StageData.StageElement.Floor || PaintType == StageData.StageElement.Wall)
            bChanged = _data.SetGround(x, z, PaintType);
        else if (PaintType == StageData.StageElement.Box || PaintType == StageData.StageElement.BornPoint)
            bChanged = _data.SetEntity(x, z, PaintType);
        else if (PaintType == StageData.StageElement.Target)
            bChanged = _data.SetExtProperty(x, z, PaintType);

        if (bChanged)
            PaintStageGeometry(x, z);
    }

    void PaintStageGeometry(int x, int z)
    {
        List<GameObject> geometry;
        int key = _data.GetKey(x, z);
        if (_nodeGeometry.ContainsKey(key))
        {
            geometry = _nodeGeometry[key];
            List<GameObject>.Enumerator it = geometry.GetEnumerator();
            while (it.MoveNext())
            {
                GameObject.DestroyImmediate(it.Current);
            }

            geometry.Clear();
        }
        else
        {
            geometry = new List<GameObject>();
            _nodeGeometry.Add(key, geometry);
        }

        if (_data.IsValidPos(x, z))
        {
            CreateStageElement(_data.GetGround(x, z), x, z, geometry);
            CreateStageElement(_data.GetEntity(x, z), x, z, geometry);
            CreateStageElement(_data.GetExtProperty(x, z), x, z, geometry);
        }
    }

    void CreateStageElement(StageData.StageElement ele, int x, int z, List<GameObject> list)
    {
        if (ele == StageData.StageElement.None) return;
        GameObject stageRoot = GameObject.Find("World/Stage");
        if (stageRoot == null) return;
        if (_resources[StageUtil.GetPaintTypeIndex(ele)] == null) return;
        GameObject newObj = GameObject.Instantiate(_resources[StageUtil.GetPaintTypeIndex(ele)]) as GameObject;
        newObj.transform.position = StageUtil.GetElementWorldPos(ele, x, z);
        newObj.transform.SetParent(stageRoot.transform);
        list.Add(newObj);
    }

    void ClearStageGeometry()
    {
        Dictionary<int, List<GameObject>>.Enumerator it = _nodeGeometry.GetEnumerator();
        while (it.MoveNext())
        {
            List<GameObject> curNode = it.Current.Value;
            List<GameObject>.Enumerator it2 = curNode.GetEnumerator();
            while (it2.MoveNext())
            {
                GameObject.DestroyImmediate(it2.Current);
            }
        }

        _nodeGeometry.Clear();
    }

    public void RepaintStageWorld()
    {
        ClearStageGeometry();

        for (int x = 0; x < _data.XCount; x++)
        {
            for (int y = 0; y < _data.YCount; y++)
            {
                PaintStageGeometry(x, y);
            }
        }
    }
}
