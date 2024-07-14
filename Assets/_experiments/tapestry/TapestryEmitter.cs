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
        [Tooltip("the savestate this emitter starts at")]
        [SerializeField] Savestate m_Sample;

        [Tooltip("the sample this emitter starts at")]
        [SerializeField] string m_SampleName;

        // the unique id for this emitter
        [SerializeField] string m_Id;

        /// -- queries --
        // the name of the current sample
        public string SampleName => m_Sample.Name;

        // the current sample for this emitter
        public Savestate Sample => m_Sample;

        // an id for this emitter (for wrapping)
        public string Id => m_Id;

        void OnValidate() {
            if (string.IsNullOrEmpty(m_Id)) {
                m_Id = UnityEngine.Random.value.ToString();
            }
        }

        /// -- commands --
        public void Save(string sampleName) {
            m_SampleName = sampleName;
        }

        public void Save(Savestate sample) {
            m_Sample = sample;
        }

        public void LoadSample(Track track) {
            track.LoadSample(m_Sample);
        }
    }
}