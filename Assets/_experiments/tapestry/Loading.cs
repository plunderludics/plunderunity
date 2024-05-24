using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

public class Loading : MonoBehaviour {
    [Header("config")]
    [SerializeField] string m_Template = "loading... {0:00}%";
    [SerializeField] IntReference m_Target;

    [Header("events")]
    [SerializeField] UnityEvent m_OnDone;

    [Header("refs")]
    [SerializeField] IntVariable m_Progress;
    [SerializeField] TMP_Text m_Text;


    void OnValidate() {
        OnValueChanged();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Progress.Changed.Register(_ => {
            OnValueChanged();
            if (m_Progress.Value >= m_Target.Value) {
                m_OnDone?.Invoke();
            }
        });
        OnValueChanged();
    }

    void OnValueChanged() {
        m_Text.text = string.Format(m_Template, 100 * Mathf.Clamp01(m_Progress.Value/(float)m_Target.Value));
    }

}