struct float3pair
{
  float3 a;
  float3 b;
};

static const int maxPaletteSize = 1023; // this is unity's limit for passing array params to shaders

static const float Epsilon = 1e-10;

float hueDistance(float h1, float h2) {
    float diff = abs((h1 - h2));
    return min(abs((1.0 - diff)), diff);
}

float3 HUEtoRGB(in float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R,G,B));
}

float3 RGBtoHCV(float3 RGB)
{
    // Based on work by Sam Hocevar and Emil Persson
    float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
    float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
    float C = Q.x - min(Q.w, Q.y);
    float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
    return float3(H, C, Q.x);
}

float3 RGBtoHSL(float3 RGB)
{
    float3 HCV = RGBtoHCV(RGB);
    float L = HCV.z - HCV.y * 0.5;
    float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
    return float3(HCV.x, S, L);
}

float3 HSLtoRGB(in float3 HSL)
{
    float3 RGB = HUEtoRGB(HSL.x);
    float C = (1 - abs(2 * HSL.z - 1)) * HSL.y;
    return (RGB - 0.5) * C + HSL.z;
}

float3 hslToLin(float3 hsl) {
    return float3(hsl.y*hsl.z*cos(hsl.x), hsl.y*hsl.z*cos(hsl.x), hsl.z);
}

float hslDistance(float3 hsl1, float3 hsl2) {
    return distance(hslToLin(hsl1), hslToLin(hsl2));
}

float3pair closestColorsHue(float hue, int paletteSize, float3 palette[maxPaletteSize]) {
    float3pair ret;
    float3 closest = float3(-2, 0, 0);
    float3 secondClosest = float3(-2, 0, 0);
    float3 temp;
    for (int i = 0; i < paletteSize; ++i) {
        temp = RGBtoHSL(palette[i]);
        float tempDistance = hueDistance(temp.x, hue);
        if (tempDistance < hueDistance(closest.x, hue)) {
            secondClosest = closest;
            closest = temp;
        } else {
            if (tempDistance < hueDistance(secondClosest.x, hue)) {
                secondClosest = temp;
            }
        }
    }
    ret.a = closest;
    ret.b = secondClosest;
    return ret;
}

float3pair closestColorsHsl(float3 hsl, int paletteSize, float3 palette[maxPaletteSize]) {
    float3pair ret;
    float3 closest = float3(-200, -200, -200);
    float3 secondClosest = float3(-200, -200, -200);
    float3 temp;
    for (int i = 0; i < paletteSize; ++i) {
        temp = RGBtoHSL(palette[i]);
        float tempDistance = hslDistance(temp, hsl);
        if (tempDistance < hslDistance(closest, hsl)) {
            secondClosest = closest;
            closest = temp;
        } else {
            if (tempDistance < hslDistance(secondClosest, hsl)) {
                secondClosest = temp;
            }
        }
    }
    ret.a = closest;
    ret.b = secondClosest;
    return ret;
}

float3pair closestColorsRGB(float3 col, int paletteSize, float3 palette[maxPaletteSize]) {
    float3pair ret;
    float3 closest = float3(-200, -200, -200);
    float3 secondClosest = float3(-200, -200, -200);
    float3 temp;
    for (int i = 0; i < paletteSize; ++i) {
        temp = palette[i];
        float tempDistance = distance(temp, col);
        if (tempDistance < distance(closest, col)) {
            secondClosest = closest;
            closest = temp;
        } else {
            if (tempDistance < distance(secondClosest, col)) {
                secondClosest = temp;
            }
        }
    }
    ret.a = closest;
    ret.b = secondClosest;
    return ret;
}

float quantize(float t, int n) {
    return floor((0.5 + t * n)) / n;
}

float3 quantizeHueLightness(float3 color, int paletteSize, float3 palette[maxPaletteSize], int lightnessSteps /*, float2 coord */) {
    float3 hsl = RGBtoHSL(color);

    float3 cs[2] = closestColorsHue(hsl.x, paletteSize, palette);
    float3 c1 = cs[0];
    float3 c2 = cs[1];
    float hueDiff = hueDistance(hsl.x, c1.x) / hueDistance(c2.x, c1.x);

    float l1 = quantize(max((hsl.z - 0.125), 0.0), lightnessSteps);
    float l2 = quantize(min((hsl.z + 0.124), 1.0), lightnessSteps);
    float lightnessDiff = (hsl.z - l1) / (l2 - l1);

    // float d = indexValue(coord);
    float3 resultColor = c1; //(hueDiff < d) ? c1 : c2;
    resultColor.z = l1; //(lightnessDiff < d) ? l1 : l2;
    return HSLtoRGB(resultColor);
}
