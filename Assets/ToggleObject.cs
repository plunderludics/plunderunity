using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OscJack;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject parent;
    List<GameObject> m_Objects;

    private void Awake() {
        m_Objects = parent.GetComponentsInChildren<Transform>()
            .Select(t=>t.gameObject)
            .ToList();
    }

    // Start is called before the first frame update
    public void Show(int index) {
        if(index < 0) return;
        if(index >= m_Objects.Count) return;
        m_Objects[index].SetActive(true);
    }

    public void Hide(int index) {
        if(index < 0) return;
        if(index >= m_Objects.Count) return;
        m_Objects[index].SetActive(false);
    }

    public void Solo(int index)
    {
        index = index % m_Objects.Count;
        for(int i = 0 ; i < m_Objects.Count; i++) {
            m_Objects[i].SetActive(index == i);
        }
    }
}
