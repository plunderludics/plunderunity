using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;
using NaughtyAttributes;

[ExecuteInEditMode]
public class BizhawkSample : MonoBehaviour
{
    [Button("Update")]
    // Configure everything according to the current settings
    private void OnResetClicked() { Reset(); }

    [Header("Renderer")]
    [SerializeField] Renderer targetRenderer;

    [Header("WindowCapture")]
    [SerializeField] bool useCustomWindowCapture;
    [HideIf("useCustomWindowCapture"), SerializeField] string windowTitleSubstring = "";
    [EnableIf("useCustomWindowCapture"), SerializeField] UwcWindowTexture windowCapture;

    [Header("RenderTexture")]
    [SerializeField] bool useCustomRenderTexture;
    private static readonly Vector2Int defaultRenderTextureSize = new Vector2Int(256, 256);
    [EnableIf("useCustomRenderTexture"), SerializeField] RenderTexture renderTexture;
    RenderTexture _createdRenderTexture;

    [Header("Material")]
    [SerializeField] bool useCustomMaterial;
    [EnableIf("useCustomMaterial"), SerializeField] Material targetMaterial;
    private static readonly string defaultShaderName = "Shader Graphs/SimpleCrop";
    Material _createdMaterial;

    void OnEnable() {
        Reset();
    }

    void Reset()
    {
        Debug.Log(this.name + ": " + "OnEnable");

        if (targetRenderer == null) {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null) {
                Debug.LogError("BizhawkSample has no configured targetRenderer and no attached Renderer component");
            }
        }

        if (_createdRenderTexture) {
            _createdRenderTexture.Release(); // Free memory used by previous created rendertexture
        }
        if (!useCustomRenderTexture) {
            Debug.Log("Creating RenderTexture");
            renderTexture = new RenderTexture(defaultRenderTextureSize.x, defaultRenderTextureSize.y, 32);
            renderTexture.name = this.name;
            _createdRenderTexture = renderTexture;
        }

        if (useCustomWindowCapture) {
            UwcWindowTexture attachedWindowCapture = gameObject.GetComponent<UwcWindowTexture>();
            if (attachedWindowCapture && attachedWindowCapture != windowCapture) {
                DestroyImmediate(attachedWindowCapture);
            }
        } else {
            windowCapture = gameObject.GetComponent<UwcWindowTexture>();
            if (!windowCapture) windowCapture = gameObject.AddComponent<UwcWindowTexture>();
            windowCapture.partialWindowTitle = windowTitleSubstring;

            windowCapture.type = WindowTextureType.Window;
            windowCapture.searchTiming = WindowSearchTiming.Always;
            windowCapture.targetTexture = renderTexture;
            windowCapture.createChildWindows = false;
            windowCapture.updateTitle = false;
            windowCapture.drawCursor = false;
            windowCapture.scaleControlType = WindowTextureScaleControlType.Manual;
            windowCapture.captureRequestTiming = WindowTextureCaptureTiming.EveryFrame;
            windowCapture.capturePriority = CapturePriority.High;
            windowCapture.captureFrameRate = -1;
        }

        if (!targetMaterial) {
            targetMaterial = new Material(Shader.Find(defaultShaderName));
            targetMaterial.name = this.name;
            // _createdMaterial = targetMaterial;
        }

        // Set renderer to use material
        targetRenderer.material = targetMaterial;

        // Set material to use renderTexture
        targetMaterial.mainTexture = renderTexture;
    }

    void OnDisable()
    {
        // TODO: should probably clean up materials and rendertextures here
    }
}
