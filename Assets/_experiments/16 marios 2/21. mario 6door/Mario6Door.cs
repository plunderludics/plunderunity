using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Mario6Door: MonoBehaviour {
    enum DoorState {
        None,
        Entering,
        Exiting
    }
    // -- tuning --
    [Header("tuning")]
    public float m_CutDistance;

    // -- config --
    [Header("config")]
    public MarioState Active;
    public MarioState Other;

    DoorState m_DoorState = DoorState.None;
    float maxCutDistance = -100f;
    Vector3 prevPos;

    void Awake() {
        Other.Emulator.OnRunning += () => Other.Emulator.Pause();
    }

    void Update() {
        var IsOpeningDoor = (MarioState.Action)Active.Curr.phase
            is MarioState.Action.Opening_a_regular_door
            or MarioState.Action.Opening_a_star_door
            or MarioState.Action.Opening_a_warp_door
            or MarioState.Action.Opening_a_door_with_star_power
            or MarioState.Action.Opening_a_locked_door_with_a_key;

        m_DoorState = (m_DoorState, IsOpeningDoor) switch {
            (DoorState.None, true) => DoorState.Entering,
            (DoorState.Exiting, false) => DoorState.None,
            _ => m_DoorState
        };

        var cutDistance = Vector3.Distance(Active.Curr.cam, prevPos);
        if (cutDistance > maxCutDistance) {
            maxCutDistance = cutDistance;
            print($"biggest cut so far: {maxCutDistance}");
        }

        if (m_DoorState == DoorState.Entering && cutDistance > m_CutDistance) {
            Swap();
            m_DoorState = DoorState.Exiting;
        }

        prevPos = Active.Curr.cam;
    }

    void Swap() {
        Active.Emulator.Pause();
        Other.Emulator.Unpause();

        var temp = Active;
        Active = Other;
        Other = temp;
    }
}