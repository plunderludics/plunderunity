using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Plunderludics.Lib;
using UnityHawk;

public class SampleDropdown : MonoBehaviour {
    [SerializeField] TMP_Dropdown m_Dropdown;
    [SerializeField] List<Savestate> m_List;
    [SerializeField] List<Track> m_Tracks;
    
    [SerializeField] Rom m_Rom;
    
    Dictionary<string, Savestate> m_Map = new();

    void OnValidate() {
        if (!m_Dropdown) {
            m_Dropdown = GetComponent<TMP_Dropdown>();
        }

        if (m_List == null || m_List.Count == 0) {
            return;
        }

        if (m_Map != null) {
            m_Map.Clear();
        } 

        var listC = m_List.Count;
        m_Dropdown.ClearOptions();
        
        for (int i = 0; i < listC; i++) {
            var listE = m_List[i];
            var name = listE.name.Replace("sm64-", "")
                        .Replace("100%_", "")
                        .Replace("_", " ");
            m_Dropdown.options.Add(new TMP_Dropdown.OptionData(name));
            m_Map.Add(name, listE);
        }
    }
    
    // Update is called once per frame
    void Update() {
        var targetId = -1;
        if (Input.GetKey(KeyCode.R)) {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            targetId = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            targetId = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            targetId = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            targetId = 3;
        }

        if (targetId >= 0) {
            var key = m_Dropdown.options[m_Dropdown.value].text;
            var sample = m_Map[key];
            m_Tracks[targetId].LoadSample(sample, m_Rom);
        }
    }
}
