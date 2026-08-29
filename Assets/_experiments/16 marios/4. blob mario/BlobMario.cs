using System.Collections;
using System.Collections.Generic;
using Plunderludics.Lib;
using UnityEngine;
using UnityHawk;

public class BlobMario : MonoBehaviour {
    const float k_2pi = 2f * Mathf.PI;

    [Header("config")]
    public float Frequency;
    public float Offset;

    [Header("refs")]
    public Track[] Blobs;

    public TrackMixer Mixer;

    // Update is called once per frame
    void Update() {
        var pow = 1;
        for (var i = 0; i < Blobs.Length; i++) {
            var blob = Blobs[i];
            var offset = Offset * k_2pi;
            var value = Mathf.Cos(k_2pi * pow * (Time.time * Frequency + offset));
            pow *= 2;

            value = (1f + value) / 2f;

            Mixer.SetTrackMix(i, value);

            var input = blob.GetComponent<InputProvider>();
            input.AddInputEvent(new InputEvent() {
                name = "A",
                value = 1
            });

            // input.AddAxisInputEvent("XAxis")
            // input.AddAxisInputEvent("XAxis")
        }

    }
}