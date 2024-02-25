using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class Axis2dInput : MonoBehaviour {
    [SerializeField] Vector2Variable m_Input;

    public Vector2 Input => m_Input.Value;

    // Update is called once per frame
    void Update() {
        var input = m_Input.Value;
        input.x = UnityEngine.Input.GetAxis("Horizontal");
        input.y = UnityEngine.Input.GetAxis("Vertical");
        m_Input.Value = input;
    }
}