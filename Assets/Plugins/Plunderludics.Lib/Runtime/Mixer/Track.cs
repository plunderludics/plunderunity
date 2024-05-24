using System;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using UnityHawk;

namespace Plunderludics.Lib
{
    
// TODO: have texture reference
[RequireComponent(typeof(Emulator))]
public class Track : MonoBehaviour
{
    [Header("config")] [SerializeField] string m_Sample;

    [Range(0, 100)] 
    [OnValueChanged(nameof(SetVolume))] 
    [SerializeField] float m_Volume;

    [ShowNativeProperty]
    public bool IsPaused {
        get => m_IsPaused;
        set {
            if (m_IsPaused == value) return;
            if (value) {
                m_Emulator.Pause();
            } else { 
                m_Emulator.Unpause();
            }
            m_IsPaused = value;
        }
    }

    [Header("debug")] 
    [SerializeField, ReadOnly] bool m_IsLoaded;
    [SerializeField, ReadOnly] bool m_LoadedSample;
    [SerializeField, ReadOnly] bool m_SavedSample;

    Emulator m_Emulator;

    bool m_IsPaused;

    // -- lifecycle --
    void Awake() {
        m_Emulator = GetComponent<Emulator>();
    }

    void Start() {
        // m_Emulator.RegisterMethod("OnLoad", Lua_OnLoad);
    }

    void Update() {
        if (m_IsLoaded && !string.IsNullOrEmpty(m_Sample) && !m_LoadedSample) {
            LoadSample(m_Sample);
        }
    }

    public void TogglePause() {
        m_IsPaused = !m_IsPaused;
        OnPauseChanged();
    }

    public void OnPauseChanged() {
         
    }

    public void SetVolume(float volume) {
        m_Volume = volume;
        m_Emulator.SetVolume(m_Volume);
    }

    // -- lua --
    string Lua_OnLoad(string _) {
        m_IsLoaded = true;
        return "";
    }

    // -- commands --
    public void LoadSample(string sampleName) {
        var path = Path.Combine(Application.streamingAssetsPath, "samples", sampleName, "save.State");
        Debug.Log($"[track] {name} loading sample : {sampleName} @ {path}");
        m_Emulator.LoadSample(path);
        m_Sample = sampleName;
        m_LoadedSample = true;
    }

    public void SaveSample(string sampleName) {
        Debug.Log($"[track] {name} saveSample sample : {sampleName}");
        m_Emulator.SaveState(sampleName);
    }

    // -- queries --
    public string Sample {
        get => m_Sample;
        set => m_Sample = value;
    }

    public bool IsLoaded {
        get => m_IsLoaded;
    }

    public bool IsRunning {
        get => m_Emulator.IsRunning;
    }
}
}