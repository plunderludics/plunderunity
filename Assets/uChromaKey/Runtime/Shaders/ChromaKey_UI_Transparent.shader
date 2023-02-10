Shader "ChromaKey/UI/Transparent"
{

Properties
{
    [Header(Material)]
    [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)
    _StencilComp ("Stencil Comparison", Float) = 8
    _Stencil ("Stencil ID", Float) = 0
    _StencilOp ("Stencil Operation", Float) = 0
    _StencilWriteMask ("Stencil Write Mask", Float) = 255
    _StencilReadMask ("Stencil Read Mask", Float) = 255

    _ColorMask ("Color Mask", Float) = 15

    [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

    [Header(Chroma Key)]
    [Toggle] _ChromaKeyEnabled("Enabled", Float) = 1.0
    [Toggle] _ChromaKeyInvert("Invert", Float) = 0.0
    _ChromaKeyColor("Color", Color) = (0.0, 0.0, 1.0, 0.0)
    _ChromaKeyHueRange("Hue Range", Range(0, 1)) = 0.1
    _ChromaKeySaturationRange("Saturation Range", Range(0, 1)) = 0.5
    _ChromaKeyBrightnessRange("Brightness Range", Range(0, 1)) = 0.5
}

SubShader
{
    Tags
    {
        "Queue"="Transparent"
        "IgnoreProjector"="True"
        "RenderType"="Transparent"
        "PreviewType"="Plane"
        "CanUseSpriteAtlas"="True"
    }

    Stencil
    {
        Ref [_Stencil]
        Comp [_StencilComp]
        Pass [_StencilOp]
        ReadMask [_StencilReadMask]
        WriteMask [_StencilWriteMask]
    }

    Cull Off
    Lighting Off
    ZWrite Off
    ZTest [unity_GUIZTestMode]
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask [_ColorMask]

    Pass
    {
        Name "Default"
        CGPROGRAM
        #define CHROMA_KEY_ALPHA
        #include "./ChromaKey_UI.cginc"
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        ENDCG
    }
}

Fallback "Unlit/Texture"

}
