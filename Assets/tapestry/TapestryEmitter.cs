using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapestryEmitter : MonoBehaviour
{
    [SerializeField] string m_Name;
    [SerializeField] Texture m_TestTexture;

    public string Name => m_Name;
    public Texture TestTexture => m_TestTexture;
}
