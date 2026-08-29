using System;
using UnityEngine;


[Serializable]
public class MarioDoor {
    public Entrance In = null;
    public Entrance Out = null;
    public string Name;

    public MarioDoorType Type;

    [Serializable]
    public enum MarioDoorType {
        LightWood,
        LightWoodWithStar,
        DarkWood,
        Large,
        LargeWithKey,
        LargeWithStar,
        Scary,
    }

    [Serializable]
    public class Entrance {
        public uint Level = UInt32.MaxValue;
        public Vector3 Position;
        public float Angle;
    }

    public Entrance GetExit(Entrance entrance) {
        return entrance == In ? Out : In;
    }
}