using UnityEngine;
using UnityEditor;
using System.Collections;

public class StageEditor
{
    public enum ElementType
    {
        None,
        Floor,
        Wall,
        BornPoint,
        Box,
        Target,
    };

    public static string[] ELEMENT_TYPES = new string[] { "None", "Floor", "Wall", "BornPoint", "Box", "Target" };

    ElementType mCurrentSelType = ElementType.None;
    string mStageFile;
    int mStageMapSize;
    GameObject mNewWorldPlane;

    public StageEditor()
    {
        mStageMapSize = 20;
    }

    public ElementType CurrentSelType
    {
        get { return mCurrentSelType; }
        set { mCurrentSelType = value; }
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
                mNewWorldPlane.transform.localScale = new Vector3(mStageMapSize * 0.1f, 1f, mStageMapSize * 0.1f);
        }
    }

    void CreateWorldPlane()
    {
        mNewWorldPlane = GameObject.Find("NewWorldPlane");
        if (mNewWorldPlane == null)
        {
            UnityEngine.Object asset = GameUtils.LoadResource("Models/prefabs/NewWorldPlane");
            if (asset != null)
                mNewWorldPlane = UnityEngine.Object.Instantiate(asset) as GameObject;
        }

        if (mNewWorldPlane != null)
        {
            GameObject world = GameObject.Find("World");
            if (world != null)
                mNewWorldPlane.transform.SetParent(world.transform);
            mNewWorldPlane.name = "NewWorldPlane";
            mNewWorldPlane.transform.localScale = new Vector3(mStageMapSize * 0.1f, 1f, mStageMapSize * 0.1f);
            mNewWorldPlane.transform.localPosition = new Vector3(0f, 0.001f, 0f);
        }
    }

    public void CreateNew()
    {
        CreateWorldPlane();
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

    [MenuItem("Tools/StageEditor")]
    public static void OpenStageEditor()
    {
        StageEditorWindow window = (StageEditorWindow)EditorWindow.GetWindow<StageEditorWindow>();
        window.minSize = new Vector2(700, 450);
        window.titleContent = new GUIContent("PushBox Stage Editor");
        window.Show();
    }
}
