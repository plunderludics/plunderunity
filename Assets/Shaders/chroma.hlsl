#include "shared.hlsl"


void chroma_float(float4 col, float3 key, float3 hsl_thresholds, float smooth, bool invert, out float4 Out)
{
    float3 col_hsl = RGBtoHSL(col.rgb);
    float3 key_hsl = RGBtoHSL(key.rgb);

    hsl_thresholds *= 0.01; // to make it easier to slide the param
    smooth *= 0.01; //

    float3 delta = hsl_delta(col_hsl, key_hsl);
    float m;
    if (smooth <= 0) {
        bool p = all(step(0.0, hsl_thresholds - delta));
        m = p ? 0. : 1.;
    } else {
        m = saturate(delta/smooth - hsl_thresholds);
        if (all(1.-m <= 0.)) {
            m = 0.;
        }
    }

    if (invert) m = 1.-m; 
    Out = col;
    Out.a *= m;
}

