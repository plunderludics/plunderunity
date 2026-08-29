using System;
using System.Collections;
using System.Collections.Generic;
using Minis;
using Plunderludics.Tools;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class HarmonyMaybe : MonoBehaviour {
    [SerializeField] BoolVariable m_IsGameOver;
    [SerializeField] VoidEvent m_DoQuit;
    [SerializeField] NoteSequenceRecorder m_Sequence;

    [SerializeField] IntEvent m_NoteOn;
    [SerializeField] IntEvent m_NoteOff;

    Dictionary<KeyCode, int> KeyboardNotes = new() {
        // first octave
        // { KeyCode.Alpha1,       48},
        // { KeyCode.Alpha2,       49},
        // { KeyCode.Alpha3,       50},
        // { KeyCode.Alpha4,       51},
        // { KeyCode.Alpha5,       52},
        // { KeyCode.Alpha6,       53},
        // { KeyCode.Alpha7,       54},
        // { KeyCode.Alpha8,       55},
        // { KeyCode.Alpha9,       56},
        // { KeyCode.Alpha0,       57},
        // { KeyCode.Minus,        58},
        // { KeyCode.Plus,         59},

        { KeyCode.Alpha1,       1},
        { KeyCode.Alpha2,       2},
        { KeyCode.Alpha3,       3},
        { KeyCode.UpArrow,       4},
        { KeyCode.DownArrow,       5},
        { KeyCode.LeftArrow,       6},
        { KeyCode.RightArrow,       7},
        { KeyCode.A,       8},
        { KeyCode.Z,       8},
        { KeyCode.X,       9},
        { KeyCode.S,       9},
        { KeyCode.Alpha4,       0},

        // // second octave
        // { KeyCode.Q,            60},
        // { KeyCode.W,            61},
        // { KeyCode.E,            62},
        // { KeyCode.R,            63},
        // { KeyCode.T,            65},
        // { KeyCode.Y,            66},
        // { KeyCode.U,            67},
        // { KeyCode.I,            68},
        // { KeyCode.O,            69},
        // { KeyCode.P,            70},
        // { KeyCode.LeftBracket,  71},
        // { KeyCode.RightBracket, 72},
    };

    HashSet<int> notesPlayed = new();
    [SerializeField] VoidEvent m_AllLoaded;

    bool loaded = false;

    void Start()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        m_AllLoaded.Register(() => {
            Debug.Log("All emulators loaded");
            loaded = true;
        });
    }

    // https://github.com/keijiro/Minis/blob/master/Assets/Script/NoteCallback.cs
    void OnDeviceChange(InputDevice device, InputDeviceChange change) {
        if (change != InputDeviceChange.Added) return;

        if (device is not MidiDevice midiDevice) return;

        Debug.Log($"new device connected");

        midiDevice.onWillNoteOn += OnBeforeNoteOn;
        midiDevice.onWillNoteOff += OnBeforeNoteOff;
    }

    void OnBeforeNoteOn(MidiNoteControl note, float velocity) {
        Debug.Log($"Note On #{note.noteNumber} ({note.shortDisplayName}) vel:{velocity:0.00} ch:{(note.device as Minis.MidiDevice)?.channel} dev:'{note.device.description.product}'");
        var num = note.noteNumber;
        NoteOn(num);
    }

    void NoteOn(int num) {
        if (!loaded) return;
        if (m_IsGameOver.Value) {
            m_DoQuit.Raise();
            return;
        };

        // if (!m_Sequence.AddNote(num)) {
        //     if (Random.value > 0.95) {
        //         m_IsGameOver.Value = true;
        //         return;
        //     }
        // }

        m_NoteOn.Raise(num);
    }

    void OnBeforeNoteOff(MidiNoteControl note) {
        Debug.Log($"Note Off #{note.noteNumber} ({note.shortDisplayName}) vel:{note.velocity:0.00} ch:{(note.device as Minis.MidiDevice)?.channel} dev:'{note.device.description.product}'");
        var num = note.noteNumber;
        NoteUp(num);
    }

    void NoteUp(int num) {
        if (!loaded) return;
        if (m_IsGameOver.Value) {
            m_DoQuit.Raise();
            return;
        }

        notesPlayed.Add(num);

        m_NoteOff.Raise(num);
    }

    void Update() {
        if (!loaded) return;
        foreach (var (key, num) in KeyboardNotes) {
            if (Input.GetKeyDown(key)) {
                NoteOn(num);
                continue;
            }

            if (Input.GetKeyUp(key)) {
                NoteUp(num);
            }
        }
    }
}