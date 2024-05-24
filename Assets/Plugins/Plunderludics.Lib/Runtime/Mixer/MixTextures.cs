using System.Collections.Generic;
using Soil;
using UnityEngine;

public class MixTextures : MonoBehaviour
{
    public Material mat;

    public float tex1mix;
    public float tex2mix;
    public float tex3mix;
    public float tex4mix;
    public float tex5mix;
    public float tex6mix;
    public float tex7mix;
    public float tex8mix;

    [Header("remapping input")]
    public MapInCurve Curve;

    List<float> mixList;

    static readonly int s_TEX1Mix = Shader.PropertyToID("_tex1mix");
    static readonly int s_TEX2Mix = Shader.PropertyToID("_tex2mix");
    static readonly int s_TEX3Mix = Shader.PropertyToID("_tex3mix");
    static readonly int s_TEX4Mix = Shader.PropertyToID("_tex4mix");
    static readonly int s_TEX5Mix = Shader.PropertyToID("_tex5mix");
    static readonly int s_TEX6Mix = Shader.PropertyToID("_tex6mix");
    static readonly int s_TEX7Mix = Shader.PropertyToID("_tex7mix");
    static readonly int s_TEX8Mix = Shader.PropertyToID("_tex8mix");

    public void SetTex1mix(float value) => SetTexMix(1, value);
    public void SetTex2mix(float value) => SetTexMix(2, value);
    public void SetTex3mix(float value) => SetTexMix(3, value);
    public void SetTex4mix(float value) => SetTexMix(4, value);
    public void SetTex5mix(float value) => SetTexMix(5, value);
    public void SetTex6mix(float value) => SetTexMix(6, value);
    public void SetTex7mix(float value) => SetTexMix(7, value);
    public void SetTex8mix(float value) => SetTexMix(8, value);

    public void SetTrackMix(int index, float value) => SetTexMix(index, value);


    void SetTexMix(int index, float value) {
        switch (index+1) {
            case 1:
                tex1mix = value;
                break;
            case 2:
                tex2mix = value;
                break;
            case 3:
                tex3mix = value;
                break;
            case 4:
                tex4mix = value;
                break;
            case 5:
                tex5mix = value;
                break;
            case 6:
                tex6mix = value;
                break;
            case 7:
                tex7mix = value;
                break;
            case 8:
                tex8mix = value;
                break;
            default:
                throw new System.Exception($"index {index} does not exist");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mixList = new List<float>();
        mixList.Add(tex1mix);
        mixList.Add(tex2mix);
        mixList.Add(tex3mix);
        mixList.Add(tex4mix);
        mixList.Add(tex5mix);
        mixList.Add(tex6mix);
        mixList.Add(tex7mix);
        mixList.Add(tex8mix);
        SetMaterialProperties();
    }

    // Update is called once per frame
    void Update()
    {
        SetMaterialProperties();
    }

    void SetMaterialProperties() {
        mat.SetFloat(s_TEX1Mix, Curve.Evaluate(tex1mix));
        mat.SetFloat(s_TEX2Mix, Curve.Evaluate(tex2mix));
        mat.SetFloat(s_TEX3Mix, Curve.Evaluate(tex3mix));
        mat.SetFloat(s_TEX4Mix, Curve.Evaluate(tex4mix));
        mat.SetFloat(s_TEX5Mix, Curve.Evaluate(tex5mix));
        mat.SetFloat(s_TEX6Mix, Curve.Evaluate(tex6mix));
        mat.SetFloat(s_TEX7Mix, Curve.Evaluate(tex7mix));
        mat.SetFloat(s_TEX8Mix, Curve.Evaluate(tex8mix));
    }
}