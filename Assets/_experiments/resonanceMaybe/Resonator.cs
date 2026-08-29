using UnityEngine;
using UnityHawk;
using Plunderludics.Tools;

namespace Plunderludics.Mut.HarmonyMaybe {

[RequireComponent(typeof(Emulator))]
public class Resonator: MonoBehaviour {
    Emulator m_Emulator;
    CustomMask m_Mask;
    AudioSource m_Audio;

    public Emulator Emulator => m_Emulator;
    public CustomMask Mask => m_Mask;
    public InputProvider Input => Emulator.inputProvider;
    public AudioSource Audio => m_Audio;

    void Awake() {
        m_Emulator = GetComponent<Emulator>();
        m_Mask = GetComponentInChildren<CustomMask>(true);
        m_Audio = GetComponent<AudioSource>();
    }
}

}