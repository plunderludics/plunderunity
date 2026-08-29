using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Plunderludics.Tools {

public class BeatLoop : MonoBehaviour {
    [Header("config")]
    public FloatReference Tempo;

    [Header("refs - state")]
    public IntReference TotalSteps;
    public BoolReference Paused;
    public IntVariable CurrStep;

    [Header("refs events")]
    public VoidEvent OnBeat;

    [Header("debug")]
    [Readonly]
    public bool OffBeat;

    public float currElapsed;

    float secondsPerBeat => 60f / Tempo;

    // -- lifecycle --
    void Update() {
        var delta = Time.deltaTime;
        currElapsed += delta;
        while (currElapsed > secondsPerBeat) {
            OnBeat.Raise();
            if (!Paused.Value) {
                CurrStep.Value = (CurrStep.Value + 1) % TotalSteps.Value;
            }

            currElapsed -= secondsPerBeat;
            OffBeat = !OffBeat;
        }
    }
}

}