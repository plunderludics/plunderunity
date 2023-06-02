using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class BasicMover : MonoBehaviour
{
    [Header("tuning")]
    [SerializeField] FloatReference m_Speed;

    void Update() {
        // move walker
        var velocity =
            (Input.GetAxis("Vertical") * Vector3.forward +
            Input.GetAxis("Horizontal") * Vector3.right)
            * m_Speed;

        transform.position += velocity * Time.deltaTime;
    }
}
