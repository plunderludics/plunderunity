using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

[RequireComponent(typeof(Image))]
public class Fade : MonoBehaviour {
    [Header("config")]
    [SerializeField] FloatReference m_MaxSpeed;
    [SerializeField] BoolReference m_Invert;
    
    [Header("refs")]
    [SerializeField] FloatVariable m_Fade;

    Image m_Image;

    void Awake() {
        m_Image = GetComponent<Image>();
    }
    
    // Start is called before the first frame update
    void Update() {
        var c = m_Image.color;
        
        var target = Mathf.Clamp01(m_Fade.Value);
        if (m_Invert.Value) {
            target = 1 - target;
        }
        
        c.a = m_MaxSpeed.Value > 0 
            ? Mathf.MoveTowards(c.a, target, m_MaxSpeed.Value * Time.deltaTime) 
            : target;

        m_Image.color = c;
    }
}
