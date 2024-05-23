using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tapestry
{

    public class TapestryEmitter : MonoBehaviour
    {
        [FormerlySerializedAs("m_Sample")] 
        [Tooltip("the sample this emitter starts at")] 
        [SerializeField] string m_InitialSample;
        
        // the unique id for this emitter
        [SerializeField, Readonly] string m_Id;

        // the name of the current sample
        string m_Sample;
        
        Guid m_Guid; 

        /// -- queries -- 
        // the current sample for this emitter
        public string Sample => m_Sample;

        public Guid Id => m_Guid;
        
        /// -- commands -- 
        public void Save(string sample)
        {
            m_Sample = sample;
        }

        /// -- lifecycle -- 
        private void OnValidate()
        {
            this.name = m_InitialSample;
            var text = GetComponentInChildren<TMPro.TMP_Text>();
            if (text != null)
            {
                text.text = Sample;
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
            m_Sample = m_InitialSample;
        }
    }

}
