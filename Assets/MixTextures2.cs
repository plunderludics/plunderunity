using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixTextures2 : MonoBehaviour
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
    public float MinInput = -4.95f;
    public float MaxInput = 4.95f;
    public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
    private List<float> mixList;
    private int index;
    public void SetTex1mix(float value) => SetTexMix(1, value);
    public void SetTex2mix(float value) => SetTexMix(2, value);
    public void SetTex3mix(float value) => SetTexMix(3, value);
    public void SetTex4mix(float value) => SetTexMix(4, value);
    public void SetTex5mix(float value) => SetTexMix(5, value);
    public void SetTex6mix(float value) => SetTexMix(6, value);
    public void SetTex7mix(float value) => SetTexMix(7, value);
    public void SetTex8mix(float value) => SetTexMix(8, value);

    public void SetTexMix(int index, float value) {
        switch (index) {
            case 1:
                tex1mix = Curve.Evaluate(Mathf.InverseLerp(MinInput, MaxInput, value));
                break;
            case 2:
                tex2mix = Curve.Evaluate(Mathf.InverseLerp(MinInput, MaxInput, value));
                break;
            case 3:
                tex3mix = Curve.Evaluate(Mathf.InverseLerp(MinInput, MaxInput, value));
                break;
            case 4:
                tex4mix = Curve.Evaluate(Mathf.InverseLerp(MinInput, MaxInput, value));
                break;

            case 5:
                tex5mix = Curve.Evaluate(Mathf.InverseLerp(MinInput, MaxInput, value));
                break;

            case 6:
                tex6mix = Curve.Evaluate(Mathf.InverseLerp(MinInput, MaxInput, value));
                break;

            case 7:
                tex7mix = Curve.Evaluate(Mathf.InverseLerp(MinInput, MaxInput, value));
                break;

            case 8:
                tex8mix = Curve.Evaluate(Mathf.InverseLerp(MinInput, MaxInput, value));
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
        index = 0;
        mat.SetFloat("_tex1mix", tex1mix);
        mat.SetFloat("_tex2mix", tex2mix);
        mat.SetFloat("_tex3mix", tex3mix);
        mat.SetFloat("_tex4mix", tex4mix);
        mat.SetFloat("_tex5mix", tex5mix);
        mat.SetFloat("_tex6mix", tex6mix);
        mat.SetFloat("_tex7mix", tex7mix);
        mat.SetFloat("_tex8mix", tex8mix);
    }

    // Update is called once per frame
    void Update()
    {
        
        mat.SetFloat("_tex1mix", tex1mix);
        mat.SetFloat("_tex2mix", tex2mix);
        mat.SetFloat("_tex3mix", tex3mix);
        mat.SetFloat("_tex4mix", tex4mix);
        mat.SetFloat("_tex5mix", tex5mix);
        mat.SetFloat("_tex6mix", tex6mix);
        mat.SetFloat("_tex7mix", tex7mix);
        mat.SetFloat("_tex8mix", tex8mix);

    }
}
