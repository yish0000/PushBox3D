using UnityEngine;
using UnityEditor;
using System.Collections;

public class StageEditor
{
    public enum StagePaintType
    {
        Select = -1,
        None,
        Floor,
        Wall,
        Box,
        BornPoint,
        Target,
    }
    
    StagePaintType mCurrentSelType = StagePaintType.Select;
    string mStageFile = "";
    int mStageMapSize = 20;
    GameObject mStageRoot;
    GameObject mNewWorldPlane;
    StageData mData;
    StageDrawer mDrawer;

    [MenuItem("Tools/StageEditor")]
    public static void OpenStageEditor()
    {
        StageEditorWindow window = (StageEditorWindow)EditorWindow.GetWindow<StageEditorWindow>();
        window.minSize = new Vector2(700, 450);
        window.titleContent = new GUIContent("PushBox Stage Editor");
        window.Show();
    }

    StageData.StageElement PaintType2ElementType(StagePaintType pt)
    {
        switch (pt)
        {
            case StagePaintType.None: return StageData.StageElement.None;
            case StagePaintType.Floor: return StageData.StageElement.Floor;
            case StagePaintType.Wall: return StageData.StageElement.Wall;
            case StagePaintType.Box: return StageData.StageElement.Box;
            case StagePaintType.BornPoint: return StageData.StageElement.BornPoint;
            case StagePaintType.Target: return StageData.StageElement.Target;
            default: return StageData.StageElement.None;
        }
    }

    public StagePaintType CurrentSelType
    {
        get { return mCurrentSelType; }
        set {
            mCurrentSelType = value;
            mDrawer.PaintType = PaintType2ElementType(value);
        }
    }

    public string StageFile
    { 
        get { return mStageFile; }
        set { mStageFile = value; }
    }

    public int StageMapSize
    {
        get { return mStageMapSize; }
        set
        {
            mStageMapSize = value;

            if (mNewWorldPlane != null)
            {
                mNewWorldPlane.transform.localScale = new Vector3(mStageMapSize * 0.1f, 1f, mStageMapSize * 0.1f);
                mNewWorldPlane.transform.localPosition = new Vector3(mStageMapSize * 0.5f, 0.001f, mStageMapSize * 0.5f);
            }
        }
    }

    public StageDrawer Drawer
    {
        get { return mDrawer; }
    }

    public void Init()
    {
        mData = new StageData(mStageMapSize, mStageMapSize);
        mDrawer = new StageDrawer(mData);

        CreateWorldPlane();
    }

    void CreateWorldPlane()
    {
        mStageRoot = GameObject.Find("World/Stage");
        if (mStageRoot == null)
            return;

        UnityEngine.Object asset = GameUtils.LoadResource("Models/prefabs/NewWorldPlane");
        if (asset != null)
            mNewWorldPlane = UnityEngine.Object.Instantiate(asset) as GameObject;
        if (mNewWorldPlane != null)
        {
            mNewWorldPlane.transform.SetParent(mStageRoot.transform);
            mNewWorldPlane.name = "NewWorldPlane";
            mNewWorldPlane.transform.localScale = new Vector3(mStageMapSize * 0.1f, 1f, mStageMapSize * 0.1f);
            mNewWorldPlane.transform.localPosition = new Vector3(mStageMapSize * 0.5f, 0.001f, mStageMapSize * 0.5f);
        }
    }

    public void CreateNew()
    {
        mStageFile = "";
        mData.ClearAll();
        mDrawer.RepaintStageWorld();
    }

    public bool LoadXML(string filename)
    {
        if (!mData.LoadXML(filename))
            return false;

        mStageFile = filename;
        mDrawer.RepaintStageWorld();
        return true;
    }

    public void SaveXML(string filename)
    {
        mData.SaveXML(filename);
        mStageFile = filename;
    }

    public bool Load(string filename)
    {
        return true;
    }

    public void Save(string filename)
    {
    }

    public bool GetCurrentGrid(UnityEngine.Event evt, out int x, out int z)
    {
        x = 0;
        z = 0;
        Vector3 hitPoint;
        bool isHit = MousePositionInWorld(evt, out hitPoint);
        if (isHit)
        {
            x = Mathf.FloorToInt(hitPoint.x);
            z = Mathf.FloorToInt(hitPoint.z);
            if (!IsInMapGrid(x, z))
                isHit = false;
        }

        return isHit;
    }

    bool IsInMapGrid(int x, int z)
    {
        if (mData == null) return false;
        if (x < 0 || x >= mStageMapSize) return false;
        if (z < 0 || z >= mStageMapSize) return false;
        return true;
    }

    bool MousePositionInWorld(UnityEngine.Event current, out Vector3 hitPoint)
    {
        Vector3 pos = new Vector3(current.mousePosition.x, current.mousePosition.y, 0);
        Ray ray = Camera.current.ScreenPointToRay(pos);
        ray = HandleUtility.GUIPointToWorldRay(pos);
        RaycastHit hit = new RaycastHit();

        bool isHit = Raycast(ray, out hit);
        hitPoint = hit.point;
        return isHit;
    }

    bool Raycast(Ray ray, out RaycastHit hit)
    {
        MeshCollider cld = mNewWorldPlane.GetComponent<MeshCollider>();
        if (cld)
        {
            if (cld.Raycast(ray, out hit, float.PositiveInfinity))
                return true;
        }

        hit = new RaycastHit();
        return false;
    }
}
