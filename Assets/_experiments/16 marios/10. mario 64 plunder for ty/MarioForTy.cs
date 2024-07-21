using Soil;
using UnityEngine;
using UnityHawk;

public class MarioForTy : MonoBehaviour {
    [Header("config")]
    [SerializeField] FloatRange DownDoorPosY;
    [SerializeField] IntRange EnterHoleAddress;
    [SerializeField] IntRange EnterDoorAddress;
    [SerializeField] Savestate _LeavingDoor;
    [SerializeField] Savestate _LeavingDownDoor;
    [SerializeField] Savestate _LeavingHole;

    [Header("refs")]
    [SerializeField] Emulator _Emulator;
    [SerializeField] MarioState _State;

    // Update is called once per frame
    void Update() {
        var curr = _State.Curr;
        var prev = _State.Prev;
        if (curr.action == prev.action) {
            return;
        }

        if (EnterDoorAddress.Contains(prev.action)) {
            if (DownDoorPosY.Contains(prev.posY)) {
                _Emulator.LoadState(_LeavingDownDoor);
            } else {
                _Emulator.LoadState(_LeavingDoor);
            }
        }

        if (EnterHoleAddress.Contains(prev.action)) {
            _Emulator.LoadState(_LeavingHole);
        }
    }
}