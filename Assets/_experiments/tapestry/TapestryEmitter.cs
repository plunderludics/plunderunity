using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityHawk;

namespace Tapestry
{
    public class TapestryEmitter : MonoBehaviour {
        [SerializeField] bool m_UseSampleFile;

        [FormerlySerializedAs("m_Sample")]
        [FormerlySerializedAs("m_InitialSample")]
        [Tooltip("the sample this emitter starts at")]
        [HideIf(nameof(m_UseSampleFile))]
        [SerializeField] string m_InitialSampleName;

        [ShowIf(nameof(m_UseSampleFile))]
        [Tooltip("the savestate this emitter starts at")]
        [SerializeField] Savestate m_InitialSample;

        // the unique id for this emitter
        [SerializeField, Readonly] string m_Id;

        // the name of the current sample
        public string SampleName { get; private set; }

        public Savestate Sample { get; private set; }

        Guid m_Guid;

        /// -- queries --
        // the current sample for this emitter

        public Guid Id => m_Guid;

        /// -- commands --
        public void Save(string sampleName)
        {
            SampleName = sampleName;
        }

        public void Save(Savestate sample)
        {
            Sample = sample;
        }

        /// -- lifecycle --
        private void OnValidate() {
            this.name = m_UseSampleFile ? m_InitialSampleName : Sample.Name;

            var text = GetComponentInChildren<TMPro.TMP_Text>();
            if (text != null)
            {
                text.text = SampleName;
            }

            if (string.IsNullOrEmpty(m_Id))
            {
                m_Guid = Guid.NewGuid();
                m_Id = m_Guid.ToString();
            }
        }

        private void Awake()
        {
            m_Guid = Guid.Parse(m_Id);
            Sample = m_InitialSample;
        }
    }

}