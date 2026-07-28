using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using MutCommon;
using UnityEngine;
using UnityHawk;

public class ConvertSaveState: MonoBehaviour {
    [SerializeField]
    List<ConversionMap> m_ToConvert;

    [SerializeField]
    string m_OutputPath;

    [SerializeField]
    bool m_Reconvert = true;

    [SerializeField]
    Emulator m_Emulator;

    [SerializeField]
    float m_Delay = 2f;

    bool m_IsLoaded = false;
    bool m_IsDone = true;
    int m_LastFrame = 0;

    int m_CurrRom = 0;
    int m_CurrSave = 0;

    [System.Serializable]
    class ConversionMap {
        public Rom Rom;
        public Savestate[] ToConvert;
    }

    // Update is called once per frame
    void Update() {
        if (!m_Emulator.IsRunning) {
            return;
        }

        if (m_IsDone) {
            var convert = m_ToConvert[m_CurrRom];
            m_IsLoaded = false;
            m_IsDone = false;

            m_Emulator.LoadRom(convert.Rom);
            m_Emulator.LoadState(convert.ToConvert[m_CurrSave]);
        }

        var currFrame = m_Emulator.CurrentFrame;
        if (!m_IsLoaded && m_LastFrame != currFrame && currFrame > 0) {
            m_IsLoaded = true;
            OnLoadedSavestate();
        }

        m_LastFrame = m_Emulator.CurrentFrame;
    }

    void OnGUI() {
        GUILayout.Label($"Frame: {m_Emulator.CurrentFrame}", new GUIStyle() {
            fontSize = 64,
        });
    }

    void OnLoadedSavestate() {
        var currConvert = m_ToConvert[m_CurrRom];
        var romHash = currConvert.Rom.Hash;
        var currSave = currConvert.ToConvert[m_CurrSave];
        m_Emulator.SaveState($"{Application.dataPath}/{m_OutputPath}/{currSave.name}");
        Debug.Log($"CONVERTING {currSave.name} => {currConvert.Rom.name}");

        while (true) {
            m_CurrSave++;

            if (m_CurrSave >= currConvert.ToConvert.Length) {
                m_CurrSave = 0;
                m_CurrRom++;
                if (m_CurrRom >= m_ToConvert.Count) {
                    m_Emulator.Pause();
                    Debug.Log("DONE CONVERTING");
                    return;
                }

                break;
            }

            var nextConvert = currConvert.ToConvert[m_CurrSave];
            if (nextConvert.RomInfo.Hash != romHash) {
                break;
            }

            Debug.Log($"skipping {nextConvert.RomInfo.Name}, already defined");
        }

        this.DoAfterTime(m_Delay, () => m_IsDone = true);
    }
}