using System;
using NaughtyAttributes;
using Plunderludics.Lib;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityHawk;

namespace Tapestry
{
    public class TapestryEmitter : MonoBehaviour {
        [SerializeField] bool m_UseSampleFile;

        [Tooltip("the sample this emitter starts at")]
        [HideIf(nameof(m_UseSampleFile))]
        [SerializeField] string m_SampleName;

        [ShowIf(nameof(m_UseSampleFile))]
        [Tooltip("the savestate this emitter starts at")]
        [SerializeField] Savestate m_Sample;

        // the unique id for this emitter
        [SerializeField] string m_Id;

        // the name of the current sample
        public string SampleName => m_UseSampleFile ? (m_Sample?.Name ?? "null") : m_SampleName;

        public Savestate Sample => m_Sample;

        /// -- queries --
        // the current sample for this emitter

        public string Id => m_Id;
        public bool UseSampleFile => m_UseSampleFile;

        void OnValidate() {
            if (string.IsNullOrEmpty(m_Id)) {
                m_Id = GUID.Generate().ToString();
            }
        }

        /// -- commands --
        public void Save(string sampleName) {
            m_SampleName = sampleName;
        }

        public void Save(Savestate sample)
        {
            m_Sample = sample;
        }

        public void LoadSample(Track track) {
            if (m_UseSampleFile) {
                track.LoadSample(m_Sample);
            } else {
                track.LoadSample(SampleName);
            }
        }
    }
}