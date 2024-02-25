using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class TankMover : MonoBehaviour
{
    [Header("tuning")]
    [SerializeField] FloatReference m_Speed;
    [SerializeField] FloatReference m_TurnSpeed;

    [Header("tuning")]
    [SerializeField] Vector2Reference m_Input;

    Vector2 Input => m_Input.Value;

    void Update() {
        // move walker
        var t = transform;
        var velocity =
            Input.y *
            t.forward *
            m_Speed;

        var angularSpeed =
            Input.x *
            m_TurnSpeed;

        // transform.Rotate
        var pos = t.position;
        transform.RotateAround(pos, t.up, angularSpeed * Time.deltaTime);
        pos += velocity * Time.deltaTime;
        t.position = pos;
    }
}