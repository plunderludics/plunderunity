using System.Collections;
using System.Collections.Generic;
using Plunderludics.Tools;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Plunderludics.Mut.HarmonyMaybe {

partial class Composer: MonoBehaviour {
    public VoidEvent AllLoaded;
    public Resonator[] Resonators;

    readonly HashSet<int> _ActiveNotes = new();
    IEnumerator WaitForEmulators() {
        var numLoaded = 0;
        var total = Resonators.Length;

        foreach (var resonator in Resonators) {
            resonator.Emulator.OnRunning = () => {
                numLoaded++;
                Debug.Log($"{name} running! total = {numLoaded}");
            };
        }

        while (numLoaded < total) {
            yield return null;
        }

        AllLoaded.Raise();
    }

    void Start() {
        StartCoroutine(WaitForEmulators());
    }
}
}