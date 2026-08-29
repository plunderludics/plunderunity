using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Mario Doors", menuName = "mutplunders/Mario Doors", order = 0)]
public class MarioDoors : ScriptableObject {
    public List<MarioDoor> All;

    [Button]
    void Clear() {
        All = new List<MarioDoor>();
    }
}