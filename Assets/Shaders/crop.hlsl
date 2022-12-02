void crop_float(float4 col, float4 uv0, float2 min, float2 max, bool flipV, out float4 Out)
{
    Out = col;
    if (flipV) uv0.y = 1. - uv0.y;
    if (!(all(uv0.xy >= min.xy) && all(uv0.xy <= max.xy))) {
        Out.a = 0;
    }
}

