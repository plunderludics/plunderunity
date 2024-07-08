using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class Axis2dInput : MonoBehaviour {
    [SerializeField] Vector2Variable m_Input;
    [SerializeField] float m_Rotation;
    [SerializeField] Vector2 m_Scale = Vector2.one;

    public Vector2 Value {
        get => m_Input.Value;
    }

    // Update is called once per frame
    void Update() {
        var x = m_Scale.x * UnityEngine.Input.GetAxis("Horizontal");
        var y = m_Scale.y * UnityEngine.Input.GetAxis("Vertical");
        var a = m_Rotation * Mathf.Deg2Rad;

        var input = m_Input.Value;
        input.x = x * Mathf.Cos(a) - y * Mathf.Sin(a);
        input.y = x * Mathf.Sin(a) + y * Mathf.Cos(a);

        m_Input.Value = input;
    }
}