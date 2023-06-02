using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class TankMover : MonoBehaviour
{
    [Header("tuning")]
    [SerializeField] FloatReference m_Speed;
    [SerializeField] FloatReference m_TurnSpeed;

    public Vector2 Input;

    void Update() {
        // move walker
        var velocity =
            Input.y *
            transform.forward *
            m_Speed;

        var angularSpeed =
            Input.x *
            m_TurnSpeed;

        // transform.Rotate
        transform.RotateAround(transform.position, transform.up, angularSpeed * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }
}
