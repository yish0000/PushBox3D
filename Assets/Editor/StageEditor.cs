using UnityEngine;
using UnityEditor;
using System.Collections;

public class StageEditor
{
    public static string[] ELEMENT_TYPES = new string[] { "None", "Floor", "Wall", "BornPoint", "Box", "Target" };

    StageData.StageElement mCurrentSelType = StageData.StageElement.None;
    string mStageFile;
    int mStageMapSize;
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

    public StageEditor()
    {
        mStageMapSize = 20;
    }

    public StageData.StageElement CurrentSelType
    {
        get { return mCurrentSelType; }
        set {
            mCurrentSelType = value;
            mDrawer.PaintType = value;
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

    void CreateWorldPlane()
    {
        GameObject world = GameObject.Find("World");
        if (world == null)
            return;

        mStageRoot = GameObject.Find("World/Stage");
        if (mStageRoot != null)
            GameObject.DestroyImmediate(mStageRoot);
        mStageRoot = new GameObject("Stage");
        mStageRoot.transform.SetParent(world.transform);

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
        CreateWorldPlane();

        mData = new StageData();
        mDrawer = new StageDrawer(mData);
    }

    public bool LoadXML(string filename)
    {
        CreateWorldPlane();
        return true;
    }

    public void SaveXML(string filename)
    { 
    }

    public bool Load(string filename)
    {
        CreateWorldPlane();
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
            Vector3 localPos = mNewWorldPlane.transform.InverseTransformPoint(hitPoint);
            x = Mathf.FloorToInt(localPos.x);
            z = Mathf.FloorToInt(localPos.z);
            if (!IsInMapGrid(x, z))
                isHit = false;
        }

        return true;
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
