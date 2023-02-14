using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixTextures : MonoBehaviour
{
    public Material mat;

    public float tex1mix;
    public float tex2mix;
    public float tex3mix;
    public float tex4mix;

    [Header("remapping input")]
    public float MinInput = -4.95f;
    public float MaxInput = 4.95f;
    public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

    public void SetTex1mix(float value) => SetTexMix(1, value);
    public void SetTex2mix(float value) => SetTexMix(2, value);
    public void SetTex3mix(float value) => SetTexMix(3, value);
    public void SetTex4mix(float value) => SetTexMix(4, value);

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
            default:
                throw new System.Exception($"index {index} does not exist");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_tex1mix", tex1mix);
        mat.SetFloat("_tex2mix", tex2mix);
        mat.SetFloat("_tex3mix", tex3mix);
        mat.SetFloat("_tex4mix", tex4mix);
    }
}
