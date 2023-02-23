using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;

[ExecuteInEditMode]
public class BlitWindowTexture : MonoBehaviour {
    public UwcWindowTexture windowCapture;
    public RenderTexture targetTexture;
    public Material blitMaterial;
    public int pass = 0;
    public bool clear = true;

    void Update() {
        if (!windowCapture) {
            Debug.LogWarning(this.name + ": BlitWindowTexture: no windowCapture");
            return;
        }
        if (windowCapture.window == null) {
            Debug.LogWarning(this.name + ": BlitWindowTexture: no windowCapture.window");
            return;
        }
        if (windowCapture.window.texture == null) {
            Debug.LogWarning(this.name + ": BlitWindowTexture: no windowCapture.window.texture");
            return;
        }
        if (!targetTexture) {
            Debug.LogWarning("BlitWindowTexture: no targetTexture, can't Blit");
            return;
        }
        
        // Clear texture before drawing new frame
        RenderTexture.active = targetTexture;
        if (clear) GL.Clear(false, true, Color.clear);
        RenderTexture.active = null;
        
        // Draw new frame
        if (blitMaterial) {
            Graphics.Blit(windowCapture.window.texture, targetTexture, blitMaterial, pass);
        } else {
            Graphics.Blit(windowCapture.window.texture, targetTexture);
        }
    }
}