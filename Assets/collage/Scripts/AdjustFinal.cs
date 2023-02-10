using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustFinal : MonoBehaviour
{
    public float scaleAdjust = 0.95f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (Input.GetKeyDown(KeyCode.X)) {
                transform.Rotate(0, 90, 0);
            }
            if (Input.GetKeyDown(KeyCode.LeftBracket)) {
                transform.localScale *= scaleAdjust;
            }
            if (Input.GetKeyDown(KeyCode.RightBracket)) {
                transform.localScale /= scaleAdjust;
            }
        }
    }
}
