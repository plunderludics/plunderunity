using System;
using UnityEngine;

namespace Soil {

[Serializable]
public record EaseTimer {
    // -- constants --
    // the sentinel time for an inacitve timer
    const float k_Inactive = -1.0f;

    // -- cfg --
    [Tooltip("the timer duration")]
    [SerializeField] float m_Duration;

    [Tooltip("the timer curve")]
    [SerializeField] AnimationCurve m_Curve;

    // -- props --
    /// the time elapsed through the timer
    float m_Elapsed;

    /// the uncurved percent through the timer
    float m_RawPct;

    /// if the timer is running in reverse
    bool m_IsReversed;

    // -- lifetime --
    public EaseTimer(): this(0.0f) {}
    public EaseTimer(
        float duration,
        AnimationCurve curve = null
    ) {
        m_Elapsed = k_Inactive;
        m_Duration = duration;
        m_Curve = curve;
    }

    // -- commands --
    /// start the timer (optionally, at a particular raw percent)
    public void Start(float pct = 0.0f, bool isReversed = false) {
        m_Elapsed = pct * m_Duration;
        m_IsReversed = isReversed;
    }

    /// cancel the timer
    public void Cancel() {
        m_Elapsed = k_Inactive;
    }

    /// advance the timer based on current time
    public void Tick() {
        // if not active, abort
        if (!IsActive) {
            return;
        }

        // check progress
        // TODO: do unscaled time?
        // TODO: do negative time?
        m_Elapsed += Time.deltaTime;
        var k = m_Elapsed / m_Duration;

        // if complete, clamp and stop the timer
        if (k >= 1.0f) {
            k = 1.0f;
            m_Elapsed = k_Inactive;
        }

        // save current progress
        m_RawPct = m_IsReversed ? 1f - k : k;
    }

    /// try to complete this timer if it's active; calls #Tick
    public bool TryComplete() {
        if (!IsActive) {
            return false;
        }

        Tick();
        return IsComplete;
    }

    // -- queries --
    /// if the timer is active
    public bool IsActive {
        get => m_Elapsed != k_Inactive;
    }

    /// if the timer is running in reverse
    public bool IsReversed {
        get => m_IsReversed;
    }

    /// if the timer is complete
    public bool IsComplete {
        get => m_IsReversed ? m_RawPct == 0f : m_RawPct == 1f;
    }

    /// the curved progress
    public float Pct {
        get => PctFrom(m_RawPct);
    }

    /// curve an arbitrary progress pct
    public float PctFrom(float value) {
        if (m_Curve == null || m_Curve.length == 0) {
             return value;
        }

        return m_Curve.Evaluate(value);
    }

    /// the uncurved progress
    public float Raw {
        get => m_RawPct;
    }
}

}