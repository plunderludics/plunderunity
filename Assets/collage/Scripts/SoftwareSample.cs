using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;
using NaughtyAttributes;

[ExecuteInEditMode]
public class SoftwareSample : MonoBehaviour
{

    [Header("Renderer")]
    [SerializeField] Renderer targetRenderer;

    [Header("WindowCapture")]
    [SerializeField] bool useCustomWindowCapture;
    [HideIf("useCustomWindowCapture"), SerializeField] string windowTitleSubstring = "";
    [EnableIf("useCustomWindowCapture"), SerializeField] UwcWindowTexture windowCapture;
    
    [Header("Blitter")]
    [Readonly, SerializeField] BlitWindowTexture blitter;

    [Header("RenderTexture")]
    [SerializeField] bool useCustomRenderTexture;
    [HideIf("useCustomRenderTexture")]
    [SerializeField] bool setRenderTextureSizeFromUwcWindow = true;
    [HideIf("useCustomRenderTexture"),
     DisableIf("setRenderTextureSizeFromUwcWindow"),
     SerializeField] Vector2Int renderTextureSize = new Vector2Int(350, 480);
    [EnableIf("useCustomRenderTexture"), SerializeField] RenderTexture renderTexture;
    RenderTexture _createdRenderTexture;

    [Header("RenderMaterial")]
    [SerializeField] bool useCustomRenderMaterial;
    [EnableIf("useCustomRenderMaterial"), SerializeField] Material renderMaterial;
    private static readonly string defaultShaderName = "Shader Graphs/SimpleCrop";
    Material _createdMaterial;

    [Header("BlitMaterial")]
    [SerializeField] Material blitMaterial;

    [Header("Debug")]
    [SerializeField, Readonly] bool _waitingForWindowCapture = false;

    void OnEnable() {
        Reset();
    }

    [Button("Reset")]
    // Resets everything (material, etc) and applies settings
    public void Reset() {
        if (!useCustomRenderTexture) renderTexture = null;
        if (!useCustomRenderMaterial) renderMaterial = null;
        if (!useCustomWindowCapture) windowCapture = null;
        Apply();
    }

    [Button("Apply")]
    // Applies config
    public void Apply()
    {
        // Debug.Log(this.name + ": " + "Apply");

        // Set up UwcWindowCapture
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
            windowCapture.createChildWindows = false;
            windowCapture.updateTitle = false;
            windowCapture.drawCursor = false;
            windowCapture.scaleControlType = WindowTextureScaleControlType.Manual;
            windowCapture.captureRequestTiming = WindowTextureCaptureTiming.EveryFrame;
            windowCapture.capturePriority = CapturePriority.High;
            windowCapture.captureFrameRate = -1;
        }

        // Do the rest of the setup after the windowCapture is locked in
        _waitingForWindowCapture = true;
    }

    private void PostWindowCaptureSetup() {
        // Set up RenderTexture
        if (_createdRenderTexture) {
            _createdRenderTexture.Release(); // Free memory used by previous created rendertexture
        }
        
        if (!useCustomRenderTexture) {
            Debug.Log("Creating RenderTexture");
            if (setRenderTextureSizeFromUwcWindow) {
                renderTextureSize = new Vector2Int(windowCapture.window.width, windowCapture.window.height); // [Should this be rawWidth/rawHeight?]
            }
    
            renderTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 32);
            renderTexture.name = this.name;
            _createdRenderTexture = renderTexture;
        }

        if (targetRenderer == null) {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null) {
                Debug.LogError("BizhawkSample has no configured targetRenderer and no attached Renderer component");
            }
        }

        // Set up BlitWindowTexture
        if (!blitter) {
            blitter = GetComponent<BlitWindowTexture>();
            if (!blitter) {
                blitter = gameObject.AddComponent<BlitWindowTexture>();
            }
        }
        blitter.targetTexture = renderTexture;
        blitter.windowCapture = windowCapture;
        blitter.blitMaterial = blitMaterial;

        // Set up renderMaterial
        if (!renderMaterial) {
            renderMaterial = new Material(Shader.Find(defaultShaderName));
            renderMaterial.name = this.name+"-Surface";
            renderMaterial.SetFloat("_FlipVertical", 1f); // Set to true by default (has to be default false in shader for annoying reasons)
            // _createdMaterial = targetMaterial;
        }

        // Set renderer to use material
        targetRenderer.material = renderMaterial;

        // Set material to use renderTexture
        renderMaterial.mainTexture = renderTexture;
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
            Reset();
        }

        if (_waitingForWindowCapture && windowCapture != null && windowCapture.window != null && windowCapture.window.texture != null) {
            PostWindowCaptureSetup();
            _waitingForWindowCapture = false;
        }
    }

    void OnDisable()
    {
        // TODO: should probably clean up materials and rendertextures here
    }
}
