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

    public void SetTex1mix(float value) => tex1mix = Mathf.InverseLerp(-4.95f, 4.95f, value);
    public void SetTex2mix(float value) => tex2mix = Mathf.InverseLerp(-4.95f, 4.95f, value);
    public void SetTex3mix(float value) => tex3mix = Mathf.InverseLerp(-4.95f, 4.95f, value);
    public void SetTex4mix(float value) => tex4mix = Mathf.InverseLerp(-4.95f, 4.95f, value);
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
