using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Serialization;
using UnityEngine.UI;

// TODO: save all the doors into some json file
// TODO: find the rest of the doors
// TODO: save state for every door
// TODO: question which kind of door it is
// TODO: make work for star door
public class Mario6Door: MonoBehaviour {
    enum DoorState {
        None,
        Entering,
        Exiting
    }
    // -- tuning --
    [Header("tuning")]
    [SerializeField] float m_CutDistance;
    [SerializeField] float m_MinDoorDistance;
    [SerializeField] float m_MinDoorAngle;
    [SerializeField] float m_FadeTime;

    // -- config --
    [Header("config")]
    [SerializeField] MarioDoors m_Doors;
    [SerializeField] MarioState m_Mario;

    // [SerializeField] MarioState m_Other;

    [SerializeField] RawImage m_ActiveImage;

    // [FormerlySerializedAs("m_OtherImage")]

    [SerializeField] RawImage m_GridElementPrefab;
    [SerializeField] GridLayoutGroup m_Grid;

    RawImage m_FirstImage;
    RawImage m_PrevImage;

    DoorState m_DoorState = DoorState.None;
    MarioDoor m_CurrDoor = null;
    MarioDoor.Entrance m_CurrEntrance = null;

    Dictionary<MarioDoor.Entrance, RawImage> m_DoorToGridElement = new();

    float m_MaxCutDistance = -100f;
    Vector3 m_PrevPos;

    void Awake() {
        // m_Other.Emulator.OnRunning += () => m_Other.Emulator.Pause();

        m_ActiveImage = Instantiate(m_GridElementPrefab, m_Grid.transform);
        m_ActiveImage.transform.localScale = Vector3.one;
        m_Mario.Emulator.renderTexture = (RenderTexture)m_ActiveImage.texture;
    }

    void Update() {
        var IsOpeningDoor = (MarioState.Action)m_Mario.Curr.phase
            is MarioState.Action.Opening_a_regular_door
            or MarioState.Action.Opening_a_star_door
            or MarioState.Action.Opening_a_warp_door
            or MarioState.Action.Opening_a_door_with_star_power
            or MarioState.Action.Opening_a_locked_door_with_a_key;

        if (m_DoorState == DoorState.None && IsOpeningDoor) {
            m_DoorState = DoorState.Entering;
            OnEnterDoorStarted(m_Mario);
        } else if (m_DoorState == DoorState.Exiting && !IsOpeningDoor) {
            m_DoorState = DoorState.None;
            OnExitDoorComplete(m_Mario, m_CurrDoor);
            m_CurrDoor = null;
        }

        var cutDistance = Vector3.Distance(m_Mario.Curr.cam, m_PrevPos);
        if (cutDistance > m_MaxCutDistance) {
            m_MaxCutDistance = cutDistance;
            print($"biggest cut so far: {m_MaxCutDistance}");
        }

        if (m_DoorState == DoorState.Entering && cutDistance > m_CutDistance) {
            OnExitDoorStarted();
            m_DoorState = DoorState.Exiting;
        }

        m_PrevPos = m_Mario.Curr.cam;
    }

    void OnDestroy() {
        foreach (var image in m_DoorToGridElement.Values) {
            RenderTexture.ReleaseTemporary((RenderTexture)image.texture);
        }
    }

    void OnExitDoorComplete(MarioState state, MarioDoor door) {
        var exit = door.GetExit(m_CurrEntrance);
        if (exit == null || exit.Level == uint.MaxValue || exit.Level == 0) {
            Debug.Log($"[mario 6door] save door exit @ {state.Curr.level} - {state.Curr.pos}");
            var angle = state.Curr.angleMove - Mathf.PI;
            while (angle < -Mathf.PI) {
                angle += Mathf.PI * 2;
            }

            exit = new MarioDoor.Entrance() {
                Level = state.Curr.level,
                Position = state.Curr.pos,
                Angle = angle
            };

            // save exit
            door.Out = exit;
        }

        Debug.Log($"[mario 6door] exited at {exit.Position} {exit.Angle}");
        if (!m_DoorToGridElement.TryGetValue(exit, out var gridElement)) {
            m_DoorToGridElement.TryAdd(exit, m_PrevImage);
            m_PrevImage.name = m_CurrDoor.Name + " " + exit.Angle;
        }
    }

    void OnEnterDoorStarted(MarioState state) {
        var curr = state.Curr;
        var pos = curr.pos;
        var angle = state.Curr.angleMove;
        var level = curr.level;

        // save door to door history
        var door = (MarioDoor)null;
        var sqrMinDist = m_MinDoorDistance * m_MinDoorDistance;
        var minAngle = m_MinDoorAngle;
        var entrance = (MarioDoor.Entrance)null;
        foreach (var other in m_Doors.All) {
            var otherEntrance = other.In;
            var sqrDist = Vector3.SqrMagnitude(otherEntrance.Position - pos);
            var deltaAngle = Mathf.Abs(Mathf.DeltaAngle(otherEntrance.Angle * Mathf.Rad2Deg, angle * Mathf.Rad2Deg));
            if (otherEntrance.Level == level && sqrDist < sqrMinDist && deltaAngle < minAngle)  {
                door = other;
                entrance = otherEntrance;
                minAngle = deltaAngle;
            }

            otherEntrance = other.Out;
            sqrDist = Vector3.SqrMagnitude(otherEntrance.Position - pos);
            deltaAngle = Mathf.Abs(Mathf.DeltaAngle(otherEntrance.Angle * Mathf.Rad2Deg, angle * Mathf.Rad2Deg));
            if (otherEntrance.Level == level && sqrDist < sqrMinDist && deltaAngle < minAngle) {
                door = other;
                entrance = otherEntrance;
                minAngle = deltaAngle;
            }
        }

        if (door == null) {
            Debug.Log($"[mario 6door] new door found @ {level} - {pos}");
            entrance = new MarioDoor.Entrance() {
                Level = level,
                Position = pos,
                Angle = angle
            };

            door = new MarioDoor() {
                In = entrance
            };

            m_Doors.All.Add(door);
        } else {
            Debug.Log($"[mario 6door] door already existed @ {level} - {entrance.Position}");
        }


        m_CurrDoor = door;
        Debug.Log($"[mario 6door] entered at {entrance.Position} {entrance.Angle}");
        m_CurrEntrance = entrance;
    }

    void OnExitDoorStarted() {
        // swap texture to door location
        if (!m_DoorToGridElement.TryGetValue(m_CurrEntrance, out var gridElement)) {
            // door doesn't exist yet
            gridElement = Instantiate(m_GridElementPrefab, m_Grid.transform);
            gridElement.transform.localScale = Vector3.one;
            gridElement.texture = RenderTexture.GetTemporary(m_Mario.Emulator.renderTexture.descriptor);
            gridElement.name = m_CurrDoor.Name + " " + m_CurrEntrance.Angle;
            m_DoorToGridElement.Add(m_CurrEntrance, gridElement);
        }

        m_PrevImage = m_ActiveImage;
        m_ActiveImage = gridElement;
        m_Mario.Emulator.renderTexture = (RenderTexture)m_ActiveImage.texture;

        m_PrevImage.color = Color.red;
        m_ActiveImage.color = Color.green;

        return;

        // write the currently active texture to the other texture
        // var activeTexture = m_Mario.Emulator.Texture;
        // Graphics.Blit(activeTexture, m_OtherTexture);

        // swap where the emulator is rendering to
        // var temp = m_ActiveImage;
        // m_ActiveImage = m_PrevImage;
        // m_PrevImage = temp;

        // m_ActiveImage.texture = activeTexture;
        // m_PrevImage.texture = m_OtherTexture;

        // Active.Emulator.Pause();
        // Other.Emulator.Unpause();
        //
        // var temp = Active;
        // Active = Other;
        // Other = temp;
    }
}