using System;
using TwitchLib.Api.Helix.Models.Common;
using UnityEngine;
using UnityEngine.UI;
using UnityHawk;
using Random = UnityEngine.Random;

namespace Plunderludics.Mut.HarmonyMaybe {
    [Serializable]
    class OscillateInput: NoteBehaviour {
        [Header("Input")]
        [SerializeField] Resonator[] Targets;
        public float frequency;
        public string Positive;
        public string Negative;

        [SerializeField] bool isOctave;

        public override void Init() {
            Targets = FindObjectsByType<Resonator>(FindObjectsSortMode.None);
        }

        public override void Step(float delta) {
            base.Step(delta);
        }

        public override void OnNoteOn() {
            base.OnNoteOn();
        }

        public override void OnNoteHold(float delta) {
            var v = 1; //Mathf.Sin(Mathf.PI * 2 * frequency * Time.time);
            foreach (var resonator in Targets) {
                var keyName = v > 0 ? (Positive, Negative) : (Negative, Positive);

                resonator.Input.AddInputEvent(new InputEvent() {
                    name = keyName.Item1,
                    value = v >= Random.value ? 1 : 0,
                });

                resonator.Input.AddInputEvent(new InputEvent() {
                    name = keyName.Item2,
                    value = 0
                });
            }
            base.OnNoteHold(delta);
        }

        public override void OnNoteOff() {
            foreach (var resonator in Targets) {
                resonator.Input.AddInputEvent(new InputEvent() {
                    name = Positive,
                    value = 0
                });

                resonator.Input.AddInputEvent(new InputEvent() {
                    name = Negative,
                    value = 0
                });
            }
        }
    }
}