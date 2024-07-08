using System.Collections;
using System.Collections.Generic;
using Soil;
using UnityEngine;

public class MoveWithYPosition : MonoBehaviour {
    public MarioState State;

    public FloatRange HeightRange;
    public FloatRange ScreenPosRange;
    public float Speed;

    bool started = false;

    // Update is called once per frame
    void Update() {
        if (State.Curr.posY != 0) {
            started = true;
        }

        if(!started) return;

        var pos = transform.localPosition;
        var y = State.Curr.posY;
        if (y > HeightRange.Max) {
            HeightRange.Max = y;
        } else if (y <= HeightRange.Min) {
            HeightRange.Min = y;
        }

        pos.y = Mathf.MoveTowards(pos.y, ScreenPosRange.Lerp(HeightRange.InverseLerp(State.Curr.posY)), Speed * Time.deltaTime);

        transform.localPosition = pos;
    }
}