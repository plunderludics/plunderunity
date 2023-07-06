using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;

[ExecuteInEditMode]
public class Blitter : MonoBehaviour {
    // public UwcWindowTexture windowCapture;
    public Func<Texture> textureGetter;
    public RenderTexture targetTexture;
    public Material blitMaterial;
    public int pass = 0;
    public bool clear = true;

    void Update() {
        if (textureGetter == null) {
            // Debug.LogWarning("Blitter: no textureGetter, can't Blit");
            return;
        }
        if (!targetTexture) {
            // Debug.LogWarning("Blitter: no targetTexture, can't Blit");
            return;
        }
        
        // Clear texture before drawing new frame
        RenderTexture.active = targetTexture;
        if (clear) GL.Clear(false, true, Color.clear);
        RenderTexture.active = null;
        
        Texture sourceTexture = textureGetter();
        // Draw new frame
        if (blitMaterial) {
            Graphics.Blit(sourceTexture, targetTexture, blitMaterial, pass);
        } else {
            Graphics.Blit(sourceTexture, targetTexture);
        }
    }
}