using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Soil;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using UnityHawk;

public class KuleshovMario : MonoBehaviour {
    [Serializable]
    public class Data {
        public Emulator Emulator;
        public GameObject Image;
        public Savestate[] Samples;
        public FloatRange DurationRange;

        int _Curr = -1;

        public void Play() {
            Image.SetActive(true);
        }

        public void Stop() {
            Image.SetActive(false);
            var next = UnityEngine.Random.Range(0, Samples.Length);
            if (next == _Curr) {
                // idk, made sense
                next = (next + 1) % Samples.Length;
            }

            var nextSavestate = Samples[next];
            Emulator.LoadState(nextSavestate);

            _Curr = next;
        }
    }

    public Data Face;
    public Data Look;


    bool _Started = false;

    void Start() {
        Look.Stop();
        Face.Stop();
    }

    void Update() {
        if (!Face.Emulator.IsRunning || !Look.Emulator.IsRunning) return;

        if (!_Started) {
            _Started = true;
            StartCoroutine(Sequence());
        }

        var input = Face.Emulator.inputProvider;
        input.AddInputEvent(new InputEvent() {
            keyName = "A",
            isPressed = true
        });
    }

    IEnumerator Sequence() {
        var i = 0;
        while (true) {
            // Face then Look then face again
            var curr = i == 0 ? Look : Face;
            var next = i == 0 ? Face : Look;

            curr.Play();
            next.Stop();

            var wait = curr.DurationRange.Random();
            Debug.Log($"showing {curr.Image.name} for {wait} seconds");
            yield return new WaitForSeconds(wait);
            i = (i + 1) % 2;
        }
    }
}