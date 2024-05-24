using UnityEngine;
using UnityHawk;

// TODO: port to new unityhawk
public class SnowboardingMenu : MonoBehaviour
{
    [SerializeField] Emulator m_Emulator;

    [SerializeField] float m_SwapDelay = 0.5f;

    [SerializeField] int Character;
    [SerializeField] public int Board;

    public bool[] loadedCharacters = new bool[5];

    // Start is called before the first frame update
    void Start()
    {
        // m_Emulator.LoadSample("track1", "teneighty-characterselect");
    }

    public void OnReceiveCharacter(int newCharacter) {
        if (Character == newCharacter) return;
        if (loadedCharacters[newCharacter]) {
            // m_Emulator.SaveState("track2", Character+1);
            // this.DoAfterTime(m_SwapDelay, () => m_Emulator.LoadState("track2", newCharacter+1));

        } else {
            // m_Emulator.SaveState("track2", Character);
            // m_Emulator.LoadSample("track2", "teneighty-beginning-character"+(newCharacter+1));
            loadedCharacters[newCharacter] = true;
        }
        Character = newCharacter;
    }
}
