using System;
using NaughtyAttributes;
using Plunderludics.Lib;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;
using UnityHawk;
using Color = Soil.Color;

namespace Tapestry
{
    public class TapestryEmitter : MonoBehaviour {
        [Tooltip("the savestate this emitter starts at")]
        [SerializeField] Savestate m_Sample;

        [Tooltip("the rom related to the sample")]
        [SerializeField] Rom m_Rom;

        // the unique id for this emitter
        [SerializeField] string m_Id;

        [Tooltip("canvas item")]
        [SerializeField] Canvas m_Canvas;

        string m_SamplePath;

        /// -- queries --
        // the name of the current sample
        public string SampleName => m_Sample.name;

        // the current sample for this emitter
        public Savestate Sample => m_Sample;

        // an id for this emitter (for wrapping)
        public string Id => m_Id;

        void OnValidate() {
            if (string.IsNullOrEmpty(m_Id)) {
                // m_Id = Guid.NewGuid().ToString();
                Debug.Log($"new guid {m_Id}");
            }

            if (m_Sample) {
                name = SampleName;
                SetText();
            }
        }

        void Awake() {
            if (!m_Sample) {
                return;
            }

            SetText();
        }

        /// -- commands --
        public void Save(string samplePath) {
            m_SamplePath = samplePath;
        }

        public void SetText() {
            var text = m_Canvas.GetComponentInChildren<TMPro.TMP_Text>();
            if (!text) {
                return;
            }

            text.text = name.Replace("-", "\n");
            text.color = name switch {
                string n when n.Contains("water") => Color.Aqua,
                string n when n.Contains("bay") => Color.Aqua,
                string n when n.Contains("swim") => Color.Aqua,
                string n when n.Contains("rinkwatch") => Color.Red,
                _ => Color.White,
            };
        }

        // TODO
        public void Save(Savestate sample) {
            m_Sample = sample;
        }

        public void LoadSample(Track track) {
            track.LoadSample(m_Sample, m_Rom);
        }
    }
}