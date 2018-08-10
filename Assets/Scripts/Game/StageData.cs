using System;
using System.Collections.Generic;

[System.Serializable]
public class StageData
{
    public const int VERSION = 1;

    public enum StageElement
    {
        None,
        Floor,
        Wall,
        BornPoint,
        Box,
        Target,

        Num,
    };

    [System.Serializable]
    public class StageNode
    {
        int _x, _z;

        public int X {
            get { return _x; }
            set {
                _x = value;
                UpdatePosition();
            }
        }

        public int Z {
            get { return _z; }
            set {
                _z = value;
                UpdatePosition();
            }
        }


        void UpdatePosition()
        { 
        }
    }
}
