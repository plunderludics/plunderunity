using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityHawk;

public class MarioDeathTransition : MonoBehaviour
{
    [Header("Refs")]
    public VoidEvent OnDeath;
    public VoidEvent OnRespawn;
    public BoolReference IsAlive;
    
    [Header("Refs")]
    public Emulator Emulator;

    public Action delayed;
    
    void Start() {
        Emulator.RegisterMethod("OnDeath", OnDeathCallback);
        Emulator.RegisterMethod("OnRespawn", OnRespawnCallback);
    }

    void Update() {
        delayed?.Invoke();
        delayed = null;
    }

    string OnRespawnCallback(string arg) {
        delayed += OnRespawn.Raise;
        delayed += () => IsAlive.Value = true;
        return "";
    }

    string OnDeathCallback(string arg) {
        delayed += OnDeath.Raise;
        delayed += () => IsAlive.Value = false;
        return "";
    }
}
