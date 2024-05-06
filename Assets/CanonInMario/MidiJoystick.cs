using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;
using UnityHawk;
using Object = System.Object;

namespace CanonInMario {
public class MidiJoystick: MonoBehaviour {
    [SerializeField] int m_Channel;
    [SerializeField] Emulator m_Emulator;
    [SerializeField] MidiEmitter m_Emitter;
    [SerializeField] float m_MinScale;
    [SerializeField] float m_MaxScale;

    [SerializeField] float m_OuterRadius;
    [SerializeField] float m_AngularSpeed;
    [SerializeField] float m_ScaleSpeed;
    public Transform target;

    float m_CurrAngle;
    float m_DestAngle;
    float m_CurrScale;
    float m_DestScale;

    void Start() {
        m_Emitter.OnEventPlayed += OnEventPlayed;
        m_Emulator.RegisterMethod("SetAnalog", SetAnalog);
    }

    string SetAnalog(string _) {
        var radius = Mathf.InverseLerp(m_MinScale, m_MaxScale, m_DestScale);
        var x = radius * Mathf.Cos(m_DestAngle);
        var y = radius * Mathf.Sin(m_DestAngle);
        return $"{x},{y}";
    }

    void Update() {
        m_CurrAngle = Mathf.MoveTowardsAngle(m_CurrAngle, m_DestAngle, m_AngularSpeed * Time.deltaTime);
        var pos = m_OuterRadius * new Vector3(Mathf.Cos(m_CurrAngle), 0, Mathf.Sin(m_CurrAngle));
        m_CurrScale = Mathf.MoveTowards(m_CurrScale, m_DestScale, m_ScaleSpeed * Time.deltaTime);

        target.position = pos;
        target.localScale = m_CurrScale * Vector3.one;
    }

    void OnEventPlayed(Object sender, MidiEventPlayedEventArgs e) {
        var read = e.Event.ToString();
        switch (e.Event.EventType) {
            case MidiEventType.NoteOn:
            case MidiEventType.NoteOff:
                var note = e.Event as NoteEvent;
                if (note == null || note.Channel != m_Channel) return;

                var noteInOctave = note.NoteNumber % 12;
                var angle = noteInOctave * 2f * Mathf.PI / 12f;
                var octave = note.GetNoteOctave();
                // var type = note.EventType;

                m_DestScale = octave;
                m_DestAngle = angle;
                break;
            default:
                break;
        }

    }
}
}