using System;
using MutCommon;
using Soil;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ManyMarios {

public class JumpScare : MonoBehaviour {
    [SerializeField] FloatRange m_Duration;
    [SerializeField] int m_InitialSkips;
    [SerializeField] IntRange m_SkipRange;
    [SerializeField] Image m_Image;
    [SerializeField] AudioClip m_Clip;
    [SerializeField] AudioSource m_Source;
    [SerializeField] MarioState m_MarioState;

    bool m_IsScaring = false;
    int m_SkipsLeft = 1;

    void Awake() {
        m_SkipsLeft = m_InitialSkips;
        m_IsScaring = false;
    }

    public void Update() {
        var prev = m_MarioState.Prev;
        var curr = m_MarioState.Curr;
        if(curr.phase != prev.phase) {
            if ((MarioState.Action)curr.phase
                is MarioState.Action.Jump
                or MarioState.Action.Second_jump
                or MarioState.Action.Triple_jump
                or MarioState.Action.Side_somersault
                or MarioState.Action.Longjump
                ) {
                TryScare();
            }

        }
    }

    void TryScare() {
        Debug.Log($"trying to scare: skips left : {m_SkipsLeft}");
        if (m_IsScaring) {
            return;
        }

        if (m_SkipsLeft > 0) {
            m_SkipsLeft--;
            return;
        }

        m_IsScaring = true;
        m_Source.Play();
        m_Image.gameObject.SetActive(true);
        m_SkipsLeft = Random.Range(m_SkipRange.Min, m_SkipRange.Max);
        var duration = Random.Range(m_Duration.Min, m_Duration.Max);
        this.DoAfterTime(duration, () => {
            m_Image.gameObject.SetActive(false);
            m_Source.Stop();
            m_IsScaring = false;
        });

    }
}
}