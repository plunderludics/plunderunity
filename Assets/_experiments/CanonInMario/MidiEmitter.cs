using System;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;

namespace CanonInMario {
public class MidiEmitter: MonoBehaviour {
    [Header("config")]
    [SerializeField] string m_FilePath;
    [SerializeField] bool m_Loop;

    MidiFile m_MidiFile;
    IOutputDevice m_OutputDevice;
    Playback m_Playback;

    void Awake() {
        var ext = "";
        if (!m_FilePath.Contains(".mid")) {
            ext = ".mid";
        }
        m_MidiFile = MidiFile.Read(Path.Combine(Application.streamingAssetsPath, m_FilePath + ext));

        m_OutputDevice = new OutputDevice();
        m_MidiFile.Play(m_OutputDevice);
        m_Playback = m_MidiFile.GetPlayback(m_OutputDevice);
    }

    void Start() {
        m_Playback.Started += (_, __) => {
            // Debug.Log("AAAAAA");
        };

        m_Playback.EventPlayed += (_, e) => {
            // Debug.Log($"{e}");
        };

        m_Playback.Finished += (_, e) => {
        };

        m_Playback.Start();
        m_Playback.Loop = m_Loop;
    }

    void OnEnable() {
        m_Playback.Start();
    }

    void OnDisable() {
        m_Playback.Stop();
    }

    void OnDestroy() {
        m_Playback.Stop();
        m_Playback.Dispose();
        m_OutputDevice.Dispose();
    }

    class OutputDevice: IOutputDevice {
        public void Dispose() {
        }

        public void PrepareForEventsSending() {
            // Debug.Log("output device: preparing for sending");
        }

        public void SendEvent(MidiEvent midiEvent) {
            // Debug.Log($"send event: {midiEvent} {midiEvent.EventType}");
        }

        public event EventHandler<MidiEventSentEventArgs> EventSent;
    }

    // -- queries --
    public event EventHandler <MidiEventPlayedEventArgs> OnEventPlayed {
        add => m_Playback.EventPlayed += value;
        remove => m_Playback.EventPlayed -= value;
    }

    public event EventHandler OnPlaybackStarted {
        add => m_Playback.Started += value;
        remove => m_Playback.Started -= value;
    }

    public event EventHandler OnPlaybackFinished {
        add => m_Playback.Finished += value;
        remove => m_Playback.Finished -= value;
    }

}

}