using System;
using NaughtyAttributes;
using Soil;
using TwitchLib.Api.Helix;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Plunderludics.Mut.HarmonyMaybe {
    [Serializable]
    class NoteBehaviour: MonoBehaviour {
        [SerializeField] int Note;

        [SerializeField] protected AdsrCurve Adsr;

        [Header("refs-atoms")]
        [SerializeField] IntEvent _NoteOn;
        [SerializeField] IntEvent _NoteOff;
        [SerializeField] VoidEvent _AllLoaded;

        float elapsed = AdsrCurve.NotReleased;
        float releasedAt = AdsrCurve.NotReleased;
        bool pressed = false;

        void Awake() {
            pressed = false;
            elapsed = -1;
            releasedAt = AdsrCurve.NotReleased;
            _AllLoaded.Register(OnAllLoaded);
        }

        void OnAllLoaded() {
            Init();

            pressed = false;
            elapsed = -1;
            releasedAt = AdsrCurve.NotReleased;

            _NoteOn.Register(i => {
                if (i != Note) return;
                OnNoteOn();
            });

            _NoteOff.Register(i => {
                if (i != Note) return;
                OnNoteOff();
            });
        }

        protected virtual void Update() {
            var delta = Time.deltaTime;
            if (pressed) {
                OnNoteHold(delta);
            }

            Step(delta);
        }

        public virtual void Init() { }

        public virtual void Step(float delta) {
            if (elapsed >= 0) {
                elapsed += delta;
            }
        }

        public virtual void OnNoteOn() {
            elapsed = 0;
            releasedAt = AdsrCurve.NotReleased;
            pressed = true;
        }

        public virtual void OnNoteHold(float delta) { }

        public virtual void OnNoteOff() {
            releasedAt = elapsed;
        }

        void ResetNote() {
            elapsed = AdsrCurve.NotReleased;
        }

        [ShowNativeProperty]
        protected float Envelope => elapsed < 0 ? 0.0f : Adsr.Evaluate(elapsed, releasedAt);
    }
}