using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Utility to allow me to use RectTransform handles to set the CropMin and CropMax params of the crop shader
[ExecuteInEditMode]
public class Cropper : MonoBehaviour
{
    public Renderer targetRenderer;
    private Transform _targetTransform;
    private Material _targetMaterial;
    private RectTransform _rectTransform; // Scale of the transform must be 1

    public float transformScale = 10f;

    void Start() {
        _rectTransform = GetComponent<RectTransform>();
        _targetTransform = targetRenderer.GetComponent<Transform>();
        _targetMaterial = targetRenderer.sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetRenderer) return;
        _targetMaterial = targetRenderer.sharedMaterial;
        if (!_targetMaterial) return;
        // Get the corners of this rect transform, relative to the target material's transform.

        // Remap [-5, 5] -> [0, 1]
        // (not sure exactly why the width is 10)
        Vector2 localXY = new Vector2(_rectTransform.localPosition.x, _rectTransform.localPosition.z)*-1f;
        // localXY is the center of the rect
        Vector2 cropMax = ((localXY + _rectTransform.sizeDelta/2f) + 0.5f*transformScale*Vector2.one)/transformScale;
        Vector2 cropMin = ((localXY - _rectTransform.sizeDelta/2f) + 0.5f*transformScale*Vector2.one)/transformScale;

        // Send those params to the shader
        _targetMaterial.SetVector("_CropMin", cropMin);
        _targetMaterial.SetVector("_CropMax", cropMax);
    }
}
