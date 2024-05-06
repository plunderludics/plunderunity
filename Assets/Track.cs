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

    Emulator m_Emulator;

    // -- lifecycle --
    void Awake() {
        m_Emulator = GetComponent<Emulator>();
    }

    void Start()
    {
        m_Emulator.RegisterMethod("OnLoad", Lua_OnLoad);
        m_Emulator.RegisterMethod("LoadSample", Lua_LoadSample);
        m_Emulator.RegisterMethod("OnLoadedSample", Lua_OnLoadedSample);
        m_Emulator.RegisterMethod("Pause", Pause);
        m_Emulator.RegisterMethod("SetVolume", SetVolume);

        if (string.IsNullOrEmpty(m_Sample)) return;
        LoadSample(m_Sample);
    }

    string Lua_OnLoad(string arg) {
        m_IsLoaded = true;
        return "";
    }

    string SetVolume(string arg) {
        return $"{Volume}";
    }

    // -- lua --
    string Lua_OnLoadedSample(string arg) {
        Debug.Log($"[track] {name} loaded sample : {arg}");
        m_LoadedSample = true;
        return "";
    }

    string Lua_LoadSample(string arg) {
        return m_LoadedSample ? k_NoSample : m_Sample;
    }

    string Pause(string arg) {
        return IsPaused ? "true" : "false";
    }

    // -- commands --
    public void LoadSample(string sampleName) {
        Debug.Log($"[track] {name} loading sample : {sampleName}");
        m_LoadedSample = false;
        m_Sample = sampleName;
    }

    // -- queries --
    public string Sample {
        get => m_Sample;
        set => m_Sample = value;
    }

    public bool IsLoaded {
        get => m_IsLoaded;
    }
}