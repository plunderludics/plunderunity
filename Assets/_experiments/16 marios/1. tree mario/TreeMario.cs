using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityHawk;

public class TreeMario : MonoBehaviour {

    [Header("config")]
    public BoolReference IsOnTree;
    public FloatReference MaxTimeOutOfTree;
    public FloatReference GraceTimer;
    
    [Header("refs")]
    public FloatReference TimeOutOfTree;
    public FloatReference OutOfTreeElapsed;
    
    public Emulator Emulator;

    bool m_Wait = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Emulator.RegisterMethod("IsOnTree", IsOnTreeCallback);
    }

    void Update() {
        if (!Emulator.IsRunning) return;
        
        if (IsOnTree.Value) {
            TimeOutOfTree.Value = 0.0f;
            if (m_Wait) {
                m_Wait = false;
            }
        } else {
            TimeOutOfTree.Value += Time.deltaTime;
        }

        if (!m_Wait && TimeOutOfTree.Value > MaxTimeOutOfTree.Value + GraceTimer.Value) {
            Emulator.ReloadState();
            m_Wait = true;
        }
        
        OutOfTreeElapsed.Value = TimeOutOfTree.Value / MaxTimeOutOfTree.Value;
    }

    string IsOnTreeCallback(string val) {
        IsOnTree.Value = val == "true";
        return "";
    }

}
