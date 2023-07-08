using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;
using NaughtyAttributes;
using UnityHawk;

[ExecuteInEditMode]
public class SoftwareSample : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] Renderer targetRenderer;

    public enum Source {WindowCapture, UnityHawk}
    public Source source;

    [Header("WindowCapture")]
    [ShowIf("source", Source.WindowCapture)]
    [SerializeField] bool useCustomWindowCapture;
    [ShowIf("source", Source.WindowCapture)]
    [HideIf("useCustomWindowCapture"), SerializeField] string windowTitleSubstring = "";
    [ShowIf("source", Source.WindowCapture)]
    [EnableIf("useCustomWindowCapture"), SerializeField] UwcWindowTexture windowCapture;
    
    [Header("UnityHawk")]
    [ShowIf("source", Source.UnityHawk)]
    [SerializeField] UnityHawk.Emulator unityHawkEmulator;

    [Header("Blitter")]
    [Readonly, SerializeField] Blitter blitter;

    [Header("RenderTexture")]
    [SerializeField] bool useCustomRenderTexture;
    [HideIf("useCustomRenderTexture")]
    [SerializeField] bool setRenderTextureSizeFromSource = true;
    [HideIf("useCustomRenderTexture"),
     DisableIf("setRenderTextureSizeFromSource"),
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
    [SerializeField, Readonly] bool _waitingForSource = false;

    void OnEnable() {
        UnityHawk.UnityHawk.InitIfNeeded();
        BizHawk.Emulation.Cores.Waterbox.WaterboxHost.nDllCopies = 1; // temp hack TODO remove
        Reset();
    }

    [Button("Reset")]
    // Resets everything (material, etc) and applies settings
    public void Reset() {
        if (!useCustomRenderTexture) renderTexture = null;
        if (!useCustomRenderMaterial) renderMaterial = null;
        if (!useCustomWindowCapture) windowCapture = null;

        // Set up Blitter
        if (!blitter) {
            blitter = GetComponent<Blitter>();
            if (!blitter) {
                blitter = gameObject.AddComponent<Blitter>();
            }
        }

        Apply();
    }

    [Button("Apply")]
    // Applies config
    public void Apply()
    {
        // Debug.Log(this.name + ": " + "Apply");

        if (source == Source.WindowCapture) {
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
        } else if (source == Source.UnityHawk) {
            // Nothing really needed here, the UnityHawk.Emulator component should be configured separately
        }

        // Do the rest of the setup after the source is locked in
        // (this is only really needed for windowCapture since UnityHawk should start up synchronously, but whatever)
        _waitingForSource = true;
    }

    private void SetupAfterSourceAvailable() {
        // Set up RenderTexture
        if (_createdRenderTexture) {
            _createdRenderTexture.Release(); // Free memory used by previous created rendertexture
        }
        
        if (!useCustomRenderTexture) {
            Debug.Log("Creating RenderTexture");
            if (setRenderTextureSizeFromSource) {
                if (source == Source.WindowCapture) {
                    renderTextureSize = new Vector2Int(windowCapture.window.width, windowCapture.window.height); // [Should this be rawWidth/rawHeight?]
                } else if (source == Source.UnityHawk) {
                    renderTextureSize = new Vector2Int(unityHawkEmulator.Texture.width, unityHawkEmulator.Texture.height);
                }
            }
    
            renderTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 32);
            renderTexture.name = this.name;
            _createdRenderTexture = renderTexture;
        }

        if (targetRenderer == null) {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null) {
                Debug.LogError("SoftwareSample has no configured targetRenderer and no attached Renderer component");
            }
        }

        // [This bit sort of sucks but i don't have a better way rn]
        if (source == Source.WindowCapture) {
            blitter.textureGetter = () => windowCapture?.window?.texture;
        } else if (source == Source.UnityHawk) {
            blitter.textureGetter = () => unityHawkEmulator?.Texture;
        }

        blitter.targetTexture = renderTexture;
        blitter.blitMaterial = blitMaterial;

        // Set up renderMaterial
        if (!renderMaterial) {
            renderMaterial = new Material(Shader.Find(defaultShaderName));
            renderMaterial.name = this.name+"-Surface";
            if (source == Source.WindowCapture) {
                // For some reason the textures from windowCapture are flipped upside down
                renderMaterial.SetFloat("_FlipVertical", 1f);
            } else {
                renderMaterial.SetFloat("_FlipVertical", 0f);
            }
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

        if (_waitingForSource) {
            if (source == Source.WindowCapture && windowCapture != null && windowCapture.window != null && windowCapture.window.texture != null) {
                SetupAfterSourceAvailable();
                _waitingForSource = false;
            } else if (source == Source.UnityHawk && unityHawkEmulator != null && unityHawkEmulator.IsRunning) {
                SetupAfterSourceAvailable();
                _waitingForSource = false;
            }
        }
    }

    void OnDisable()
    {
        // TODO: should probably clean up materials and rendertextures here
    }
}
