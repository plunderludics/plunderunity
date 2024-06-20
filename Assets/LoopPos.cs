using System.Collections;
using System.Collections.Generic;
using Soil;
using UnityEngine;

public class LoopPos : MonoBehaviour {
    public FloatRange Range;

    public float Speed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {
        var size = Range.Length;
        var pos = transform.position;
        pos.x += Speed * Time.deltaTime;

        while (pos.x > Range.Max) {
            pos.x -= size;
        }

        while (pos.x <= Range.Min) {
            pos.x += size;
        }

        transform.position = pos;
    }
}