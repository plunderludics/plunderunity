using System;
using System.Collections;
using System.Linq;
using Soil;
using UnityEngine;
using UnityEngine.Serialization;
using UnityHawk;

public class CuttingMario : MonoBehaviour {
    [Serializable]
    public class Sample {
        public Savestate Savestate;
        public bool IsPersistent;
    }

    [Serializable]
    public class Data {
        public string SavePath = "./Test/{0}-{1}";
        public Emulator Emulator;
        public GameObject Image;
        public Sample[] Samples;
        public FloatRange DurationRange;
        public bool PauseOnStop;

        int _Curr = 0;

        public void Play() {
            Emulator.Unpause();
            Image.SetActive(true);
        }

        public void Stop() {
            Image.SetActive(false);

            var currSample = Samples[_Curr];
            if (currSample.IsPersistent) {
                currSample.Savestate = Emulator.SaveState(string.Format(SavePath, currSample.Savestate.Name, _Curr));
            }

            // preload the next sample
            var next = UnityEngine.Random.Range(0, Samples.Length);
            if (next != _Curr) {
                var nextSample = Samples[next];
                Emulator.LoadState(nextSample.Savestate);
            }

            Emulator.Pause();

            _Curr = next;
        }
    }

    public FloatRange CutInterval;
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

        // var input = Face.Emulator.inputProvider;
        // input.AddInputEvent(new InputEvent() {
        //     keyName = "A",
        //     isPressed = true
        // });
    }

    IEnumerator Sequence() {
        var i = 0;
        while (true) {
            // Face then Look then face again
            var curr = i == 0 ? Look : Face;
            var next = i == 0 ? Face : Look;

            curr.Play();

            yield return new WaitForSeconds(CutInterval.Random());

            next.Stop();

            var wait = curr.DurationRange.Random();
            Debug.Log($"showing {curr.Image.name} for {wait} seconds");
            yield return new WaitForSeconds(wait);
            i = (i + 1) % 2;
        }
    }
}