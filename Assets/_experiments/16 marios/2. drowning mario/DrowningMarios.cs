using System;
using System.Collections;
using System.Collections.Generic;
using MutCommon;
using Plunderludics.Lib;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;
using UnityHawk;

public class DrowningMarios : MonoBehaviour {
    [Serializable]
    public struct DrownableMario {
        public Emulator Emulator;
        public VoidEvent OnDead;
        public VoidEvent OnRespawn;
    }
    
    public RawImage Front;
    public RawImage Back;

    public float StartDelay;

    public DrownableMario[] Drownables;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Drownables.Length; i++) {
            var i1 = i;
            var d = Drownables[i];
            
            d.Emulator.OnRunning += (() => {
                d.OnRespawn.Register(() => OnRespawn(i1));
                d.OnDead.Register(() => OnDead(i1));
                if (i1 != 0) {
                    d.Emulator.Pause();
                }
            });
        }
    }

    void OnDead(int i) {
        var nextI = (i + 1) % Drownables.Length;
        Debug.Log($"{i} dead, unpausing {nextI}");
            
        var emulator = Drownables[nextI].Emulator;
        // Front.texture = emulator.renderTexture;
        
        this.DoAfterTime(StartDelay, () => {
            emulator.Unpause();
            Debug.Log($"{nextI} unpaused");
        });
        
    }

    void OnRespawn(int i) {
        Debug.Log($"{i} respawned");
        var emulator = Drownables[i].Emulator;
        
        emulator.ReloadState();
        emulator.Pause();

        Back.texture = Front.texture;
        Front.texture = emulator.renderTexture;
    }
}
