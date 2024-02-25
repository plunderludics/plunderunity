using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class BasicMover : MonoBehaviour
{
    [Header("tuning")]
    [SerializeField] FloatReference m_Speed;

    [Header("refs")]
    [SerializeField] Vector2Reference m_Input;

    void Update() {
        // move walker
        var velocity =
            (m_Input.Value.y * Vector3.forward +
            m_Input.Value.x * Vector3.right)
            * m_Speed;

        transform.position += velocity * Time.deltaTime;
    }
}