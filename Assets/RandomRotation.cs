using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class RandomRotation : MonoBehaviour {
    public float Scale;
    // Start is called before the first frame update
    void Update() {
        var rb = GetComponent<Rigidbody>();
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        rb.AddTorque(new Vector3(0, x, y) * Scale, ForceMode.Acceleration);

    }
}
