using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plunderludics.Lib;
using UnityHawk;

public class PerformanceApp : MonoBehaviour {
    [SerializeField] List<Track> m_Tracks;
    [SerializeField] Savestate m_DefaultState;
    [SerializeField] Rom m_DefaultRom;

    bool m_ResetStarted;
    
    void Update() {
        var targetId = -1;

        if (Input.GetKeyDown(KeyCode.R)) {
            m_ResetStarted = true;
        }
        
        if (Input.GetKey(KeyCode.R)) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                targetId = 0;
                m_ResetStarted = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                targetId = 1;
                m_ResetStarted = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                targetId = 2;
                m_ResetStarted = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                targetId = 3;
                m_ResetStarted = false;
            }
        }
        
        if (targetId == -1 && m_ResetStarted && Input.GetKeyUp(KeyCode.R)) {
            targetId = -2;
            m_ResetStarted = false;
        }

        if (targetId == -1) {
            return;
        }

        for (int i = 0; i < m_Tracks.Count; i++) {
            if (targetId == -2 || targetId == i) {
                if (Input.GetKey(KeyCode.LeftControl)) {
                    m_Tracks[i].LoadSample(m_DefaultState, m_DefaultRom);
                } else {
                    m_Tracks[i].ReloadState();
                }
            }
        }
    }
}
