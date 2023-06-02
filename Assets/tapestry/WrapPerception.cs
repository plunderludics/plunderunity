using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapPerception : MonoBehaviour
{
    public static bool didthing = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (didthing) return;
        didthing = true;

        var p = transform.position;
        var d = FindObjectOfType<TapestryBlender>().WrapLength;
        // linear
        Instantiate(this, p + d * Vector3.forward, Quaternion.identity);
        Instantiate(this, p - d * Vector3.forward, Quaternion.identity);
        Instantiate(this, p + d * Vector3.right, Quaternion.identity);
        Instantiate(this, p - d * Vector3.right, Quaternion.identity);

        // diagonal
        Instantiate(this, p + d * Vector3.forward + d * Vector3.right, Quaternion.identity);
        Instantiate(this, p - d * Vector3.forward + d * Vector3.right, Quaternion.identity);
        Instantiate(this, p + d * Vector3.forward - d * Vector3.right, Quaternion.identity);
        Instantiate(this, p - d * Vector3.forward - d * Vector3.right, Quaternion.identity);

    }
}
