using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityHawk;
using Random = UnityEngine.Random;

public class LoadRandomSavestate : MonoBehaviour {
    [SerializeField]
    Savestate[] m_States;

    [SerializeField]
    Emulator m_Emulator;

    void OnValidate() {
        m_Emulator = GetComponent<Emulator>();
        if (!m_Emulator) {
            m_Emulator = GetComponentInParent<Emulator>();
        }
        if (!m_Emulator) {
            m_Emulator = GetComponentInChildren<Emulator>();
        }
    }

    public void Load() {
        if (!m_Emulator) {
            Debug.LogError($"LoadRandomSample {gameObject.name} has no emulator", gameObject);
        }

        var i = Random.Range(0, m_States.Length);
        m_Emulator.LoadState(m_States[i]);
    }
}