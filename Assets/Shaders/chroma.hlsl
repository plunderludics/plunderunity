#include "shared.hlsl"

void chroma_float(float4 col, float3 key, float threshold, out float4 Out)
{
    float3 col_hsl = RGBtoHSL(col.rgb);
    float3 key_hsl = RGBtoHSL(col.rgb);

    // TODO separate threshold for h/s/l
    float dist = hslDistance(col_hsl, key_hsl);

    float m = (dist > threhold) ? 0. : 1.; // TODO make this a smooth thing with smoothness param

    Out = col;
    Out.a *= d*m;
}