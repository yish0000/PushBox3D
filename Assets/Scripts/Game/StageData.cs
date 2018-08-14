using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public const int VERSION = 1;

    public enum StageElement
    {
        None,

        Floor = 0x1,
        Wall = 0x2,

        BornPoint = 0x0100,
        Box = 0x0200,

        Target = 0x010000,

        Num = 6,
    };

    public class StageNode
    {
        public UInt32 mask;

        public StageNode()
        {
            mask = 0;
        }

        public StageElement ground
        {
            get { return (StageElement)(mask & 0xff); }
            set { mask &= 0xffffff00; mask |= (UInt32)value; }
        }

        public StageElement entity
        {
            get { return (StageElement)(mask & 0xff00); }
            set { mask &= 0xffff00ff; mask |= (UInt32)value; }
        }

        public StageElement ext_property
        {
            get { return (StageElement)(mask & 0xff0000); }
            set { mask &= 0xff00ffff; mask |= (UInt32)value; }
        }
    }

    int _max_xcount;
    int _max_ycount;
    Dictionary<int, StageNode> _data = new Dictionary<int, StageNode>();

    public int XCount
    {
        get { return _max_xcount; }
        set { _max_xcount = value; }
    }

    public int YCount
    {
        get { return _max_ycount; }
        set { _max_ycount = value; }
    }

    public StageData(int xcount, int ycount)
    {
        _max_xcount = xcount;
        _max_ycount = ycount;
    }

    public int GetKey(int x, int y)
    {
        if (x < 0 || x >= _max_xcount)
            return -1;
        if (y < 0 || y >= _max_ycount)
            return -1;
        return ((x & 0xffff) << 16) | (y & 0xffff);
    }

    public bool IsValidPos(int x, int y)
    {
        int key = GetKey(x, y);
        return _data.ContainsKey(key);
    }

    public StageElement GetGround(int x, int y)
    {
        int key = GetKey(x, y);
        if (_data.ContainsKey(key))
            return _data[key].ground;
        else
            return StageElement.None;
    }

    public bool SetGround(int x, int y, StageElement ele)
    {
        if (ele == StageElement.None)
            return false;

        if (ele != StageElement.Floor && ele != StageElement.Wall)
        {
            Debug.LogError(string.Format("unknown ground element({0},{1},{2})", x, y, (int)ele));
            return false;
        }

        int key = GetKey(x, y);
        if (key == -1)
        {
            Debug.LogError(string.Format("specified position is out of the map! ({0},{1})", x, y));
            return false;
        }

        if (!_data.ContainsKey(key))
            _data.Add(key, new StageNode());

        if (_data[key].ground != ele)
        {
            _data[key].ground = ele;
            return true;
        }

        return false;
    }

    public StageElement GetEntity(int x, int y)
    {
        int key = GetKey(x, y);
        if (_data.ContainsKey(key))
            return _data[key].entity;
        else
            return StageElement.None;
    }

    public bool SetEntity(int x, int y, StageElement ele)
    {
        if (ele == StageElement.None)
            return false;

        if (ele != StageElement.BornPoint && ele != StageElement.Box)
        {
            Debug.LogError(string.Format("unknown entity element({0},{1},{2})", x, y, (int)ele));
            return false;
        }

        int key = GetKey(x, y);
        if (key == -1)
        {
            Debug.LogError(string.Format("specified position is out of the map! ({0},{1})", x, y));
            return false;
        }

        if (!_data.ContainsKey(key))
            _data.Add(key, new StageNode());

        if (_data[key].entity != ele)
        {
            _data[key].entity = ele;
            return true;
        }

        return false;
    }

    public StageElement GetExtProperty(int x, int y)
    {
        int key = GetKey(x, y);
        if (_data.ContainsKey(key))
            return _data[key].ext_property;
        else
            return StageElement.None;
    }

    public bool SetExtProperty(int x, int y, StageElement ele)
    {
        if (ele == StageElement.None)
            return false;

        if (ele != StageElement.Target)
        {
            Debug.LogError(string.Format("unknown ExtProperty element({0},{1},{2})", x, y, (int)ele));
            return false;
        }

        int key = GetKey(x, y);
        if (key == -1)
        {
            Debug.LogError(string.Format("specified position is out of the map! ({0},{1})", x, y));
            return false;
        }

        if (!_data.ContainsKey(key))
            _data.Add(key, new StageNode());

        if (_data[key].ext_property != ele)
        {
            _data[key].ext_property = ele;
            return true;
        }

        return false;
    }

    public void Clear(int x, int y)
    {
        int key = GetKey(x, y);
        if (key == -1)
        {
            Debug.LogError(string.Format("specified position is out of the map! ({0}, {1})", x, y));
            return;
        }

        _data.Remove(key);
    }

    public void ClearAll()
    {
        _data.Clear();
    }

    public bool LoadXML(string filename)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlElement root = doc.DocumentElement;
            _max_xcount = Convert.ToInt32(root.GetAttribute("max_xcount"));
            _max_ycount = Convert.ToInt32(root.GetAttribute("max_ycount"));

            foreach (XmlElement node in root.ChildNodes)
            {
                int x = Convert.ToInt32(node.GetAttribute("x"));
                int y = Convert.ToInt32(node.GetAttribute("y"));
                StageElement ground = (StageElement)Convert.ToInt32(node.GetAttribute("ground"));
                StageElement entity = (StageElement)Convert.ToInt32(node.GetAttribute("entity"));
                StageElement ext_property = (StageElement)Convert.ToInt32(node.GetAttribute("ext_property"));
                SetGround(x, y, ground);
                SetEntity(x, y, entity);
                SetExtProperty(x, y, ext_property);
            }
        }
        catch (XmlException e)
        {
            Debug.LogError(e);
            return false;
        }

        return true;
    }

    public void SaveXML(string filename)
    {
        if (!CheckDataValid())
            return;

        XmlDocument doc = new XmlDocument();
        XmlElement root = doc.CreateElement("stage");
        root.SetAttribute("version", VERSION.ToString());
        root.SetAttribute("max_xcount", _max_xcount.ToString());
        root.SetAttribute("max_ycount", _max_ycount.ToString());
        doc.AppendChild(root);

        Dictionary<int, StageNode>.Enumerator it = _data.GetEnumerator();
        while (it.MoveNext())
        {
            int x = (it.Current.Key >> 16);
            int y = it.Current.Key & 0xffff;

            XmlElement node = doc.CreateElement("node");
            node.SetAttribute("x", x.ToString());
            node.SetAttribute("y", y.ToString());
            node.SetAttribute("ground", ((int)it.Current.Value.ground).ToString());
            node.SetAttribute("entity", ((int)it.Current.Value.entity).ToString());
            node.SetAttribute("ext_property", ((int)it.Current.Value.ext_property).ToString());
            root.AppendChild(node);
        }

        doc.Save(filename);
    }

    public bool Load(string filename)
    {
        try
        {
            FileStream file = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(file);

            int version = br.ReadInt32();
            if (version != VERSION)
            {
                Debug.LogError(string.Format("StageData.Load, invalid stage data version({0}, {1} expected)!", version, VERSION));
                return false;
            }

            _max_xcount = br.ReadInt32();
            _max_ycount = br.ReadInt32();

            int len = br.ReadInt32();
            for (int i = 0; i < len; i++)
            {
                int key = br.ReadInt32();
                uint mask = br.ReadUInt32();

                StageNode node = new StageNode();
                node.mask = mask;
                _data.Add(key, node);
            }

            br.Close();
            file.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e);
            return false;
        }

        return true;
    }

    public void Save(string filename)
    {
        if (!CheckDataValid())
            return;

        try
        {
            FileStream file = new FileStream(filename, FileMode.CreateNew);
            BinaryWriter bw = new BinaryWriter(file);
            bw.Write(VERSION);
            bw.Write(_max_xcount);
            bw.Write(_max_ycount);

            bw.Write(_data.Count);
            Dictionary<int, StageNode>.Enumerator it = _data.GetEnumerator();
            while (it.MoveNext())
            {
                bw.Write(it.Current.Key);
                bw.Write(it.Current.Value.mask);
            }

            bw.Close();
            file.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e);
        }
    }

    bool CheckDataValid()
    {
        int boxCount = 0;
        int targetCount = 0;
        int bornPointCount = 0;
        Dictionary<int, StageNode>.Enumerator it = _data.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.Value.entity == StageElement.Box)
                boxCount++;
            if (it.Current.Value.ext_property == StageElement.Target)
                targetCount++;
            if (it.Current.Value.entity == StageElement.BornPoint)
                bornPointCount++;
        }

        if (bornPointCount != 1)
        {
            Debug.LogError("StageData.CheckDataValid, stage must have one born point!");
            return false;
        }

        if (boxCount <= 0)
        {
            Debug.LogError("StageData.CheckDataValid, stage must have one box at least!");
            return false;
        }

        if (boxCount != targetCount)
        {
            Debug.LogError("StageData.CheckDataValid, box count must equal target count!");
            return false;
        }

        return true;
    }
}
