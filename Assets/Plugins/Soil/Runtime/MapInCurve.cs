using System;
using UnityEngine;

namespace Soil {

/// a normalized curve with a source range
[Serializable]
public struct MapInCurve {
    // -- fields --
    [Tooltip("the curve")]
    public AnimationCurve Curve;

    [Tooltip("the source range")]
    public FloatRange Src;

    // -- queries --
    /// evaluate the value along the curve
    public float Evaluate(float input) {
        var k = Src.InverseLerp(input);

        if (Curve != null && Curve.length != 0) {
            k = Curve.Evaluate(k);
        }

        return k;
    }

    // -- debug --
    public override string ToString() {
        return $"<MapInCurve src={Src}>";
    }
}

}