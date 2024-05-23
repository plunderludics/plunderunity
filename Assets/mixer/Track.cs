using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityHawk;

[RequireComponent(typeof(Emulator))]
// TODO: have texture reference
public class Track : MonoBehaviour {
    const string k_NoSample = "none";

    [SerializeField] string m_Sample;

    [Range(0, 100)]
    public float Volume;
    public bool IsPaused;

    [SerializeField] bool m_IsLoaded;
    
    [SerializeField] bool m_LoadedSample;
    [SerializeField] bool m_SavedSample;
    
    Emulator m_Emulator;

    // -- lifecycle --
    void Awake() {
        m_Emulator = GetComponent<Emulator>();
    }

    void Start()
    {
        m_Emulator.RegisterMethod("OnLoad", Lua_OnLoad);
        m_Emulator.RegisterMethod("Pause", Lua_Pause);
        m_Emulator.RegisterMethod("SetVolume", Lua_SetVolume);
    }

    private void Update()
    {
        if (m_IsLoaded && !string.IsNullOrEmpty(m_Sample) && !m_LoadedSample)
        {
            LoadSample(m_Sample);
        }
    }

    // -- lua --
    string Lua_OnLoad(string _) {
        m_IsLoaded = true;
        return "";
    }

    string Lua_SetVolume(string _) {
        return $"{Volume}";
    }

    string Lua_LoadSample(string _) {
        return m_LoadedSample ? k_NoSample : m_Sample;
    }
    
    string Lua_OnLoadedSample(string arg) {
        // Debug.Log($"[track] {name} loaded sample : {arg}");
        m_LoadedSample = true;
        return "";
    }

    string Lua_Pause(string arg) {
        return IsPaused ? "true" : "false";
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