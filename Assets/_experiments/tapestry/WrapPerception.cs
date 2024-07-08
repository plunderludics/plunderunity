using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tapestry
{

public class WrapPerception : MonoBehaviour {
    [SerializeField] bool m_Horizontal = true;
    [SerializeField] bool m_Vertical = true;
    public static bool didthing = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (didthing) return;
        didthing = true;

        var p = transform.position;
        var d = FindObjectOfType<TapestryBlender>().WrapLength;
        // linear
        if (m_Vertical) {
            Instantiate(this, p + d * Vector3.forward, Quaternion.identity).name = "u";
            Instantiate(this, p - d * Vector3.forward, Quaternion.identity).name = "d";
        }

        if (m_Horizontal) {
            Instantiate(this, p + d * Vector3.right, Quaternion.identity).name = "r";
            Instantiate(this, p - d * Vector3.right, Quaternion.identity).name = "l";
        }

        // diagonal
        if (m_Vertical && m_Horizontal) {
            Instantiate(this, p + d * Vector3.forward + d * Vector3.right, Quaternion.identity).name = "ur";
            Instantiate(this, p - d * Vector3.forward + d * Vector3.right, Quaternion.identity).name = "dr";
            Instantiate(this, p + d * Vector3.forward - d * Vector3.right, Quaternion.identity).name = "ul";
            Instantiate(this, p - d * Vector3.forward - d * Vector3.right, Quaternion.identity).name = "dl";
        }
    }
}

}