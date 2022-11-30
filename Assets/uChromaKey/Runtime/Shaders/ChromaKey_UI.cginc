#ifndef CHROMA_KEY_UNLIT_CGINC
#define CHROMA_KEY_UNLIT_CGINC

#include "UnityCG.cginc"
#include "UnityUI.cginc"
#include "./ChromaKey.cginc"

#pragma multi_compile_local _ UNITY_UI_CLIP_RECT
#pragma multi_compile_local _ UNITY_UI_ALPHACLIP

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 worldPosition : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};

float _ChromaKeyEnabled;
float4 _Color;
sampler2D _MainTex;
fixed4 _TextureSampleAdd;
float4 _ClipRect;
float4 _MainTex_ST;

v2f vert(appdata v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.worldPosition = v.vertex;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    float4 col = (tex2D(_MainTex, i.uv) + _TextureSampleAdd) * _Color;

    if (_ChromaKeyEnabled > 0.) {
        #ifdef CHROMA_KEY_ALPHA
        ChromaKeyApplyAlpha(col);
        #else
        ChromaKeyApplyCutout(col);
        #endif
    }

    #ifdef UNITY_UI_CLIP_RECT
    col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
    #endif

    #ifdef UNITY_UI_ALPHACLIP
    clip (col.a - 0.001);
    #endif

    return col;
}

#endif