using System.Collections;
using System.Collections.Generic;
using OscJack;
using UnityEngine;
using MutCommon;

public class SnowboardingMenu : MonoBehaviour
{
    [SerializeField] SampleLoader loader;
    [SerializeField] OscConnection m_Connection;

    [SerializeField] float m_SwapDelay = 0.5f;

    [SerializeField] int Character;
    [SerializeField] public int Board;

    public bool[] loadedCharacters = new bool[5];

    // Start is called before the first frame update
    void Start()
    {
        loader.LoadSample("track1", "teneighty-characterselect");
    }

    public void OnReceiveCharacter(int newCharacter) {
        if (Character == newCharacter) return;
        if (loadedCharacters[newCharacter]) {
            loader.SaveState("track2", Character+1);
            this.DoAfterTime(m_SwapDelay, () => loader.LoadState("track2", newCharacter+1));

        } else {
            loader.SaveState("track2", Character);
            loader.LoadSample("track2", "teneighty-beginning-character"+(newCharacter+1));
            loadedCharacters[newCharacter] = true;
        }
        Character = newCharacter;
    }
}
