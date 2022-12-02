using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RenderWebcamTexture : MonoBehaviour
{
    public RenderTexture renderTexture;
    public Material material;

    public int width;
    public int height;
    public int fps;

    public int deviceIndex = 0; // can't be changed at runtime
    [Readonly] public List<string> availableDevices;

    private WebCamTexture _tex;

    void Start ()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        // for debugging purposes, prints available devices to the console
        for(int i = 0; i < devices.Length; i++)
        {
            print("Webcam available: " + devices[i].name);
        }

        // Renderer rend = this.GetComponentInChildren<Renderer>();

        availableDevices = devices.Select(_ => _.name).ToList();

        _tex = new WebCamTexture(devices[deviceIndex].name, width, height, fps);
        
        if (material) {
            material.mainTexture = _tex;
        }

        _tex.Play();
    }

    void Update() {
        // I guess inefficient but it's convenient to have it in a renderTexture
        if (renderTexture) {
            Graphics.Blit(_tex, renderTexture);
        }
    }
}