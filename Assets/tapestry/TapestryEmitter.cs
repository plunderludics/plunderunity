using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapestryEmitter : MonoBehaviour
{
    [SerializeField] string m_Sample;
    [SerializeField] Texture m_TestTexture;

    public string Sample => m_Sample;
    public Texture TestTexture => m_TestTexture;

    private void OnValidate() {
        this.name = Sample;
        var text = GetComponentInChildren<TMPro.TMP_Text>();
        if (text != null) {
            text.text = Sample;
        }
    }

}
