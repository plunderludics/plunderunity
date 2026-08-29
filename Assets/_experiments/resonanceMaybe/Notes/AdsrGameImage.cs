using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityHawk;

namespace Plunderludics.Mut.HarmonyMaybe {
    [Serializable]
    class AdsrGameImage: NoteBehaviour {
        [SerializeField] Resonator Target;
        [SerializeField] RawImage TargetImage;
        [SerializeField] Savestate m_Savestate;

        [HideIf(nameof(IsRandom))]
        public Rect Rect = new Rect(0, 0, 1920, 1080);

        public bool IsRandom;

        [ShowIf(nameof(IsRandom))]
        public Rect MinRect;

        [ShowIf(nameof(IsRandom))]
        public Rect MaxRect;


        public override void Init() {
            base.Init();
            TargetImage = Target.gameObject.GetComponentInChildren<RawImage>(true);

            Target.Emulator.LoadState(m_Savestate);
            if (!IsRandom) {
                Target.Mask.DrawRect(Rect);
            }
        }

        public override void Step(float delta) {
            base.Step(delta);

            var c = TargetImage.color;
            c.a = Envelope;
            c.r = Envelope;
            TargetImage.color = c;

            Target.Audio.volume = Envelope;
        }

        public override void OnNoteOn() {
            base.OnNoteOn();
            Target.Mask.Clear();
            Target.Mask.DrawRandomRect(MinRect, MaxRect);
        }

        public override void OnNoteHold(float delta) {
            base.OnNoteHold(delta);
        }

        public override void OnNoteOff() {
            base.OnNoteOff();
        }
    }
}