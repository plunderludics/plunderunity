using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace uWindowCapture {

[ExecuteAlways]
public class UwcWindowToRenderTexture : MonoBehaviour
{
    bool shouldUpdateWindow_ = true;
    bool shouldUpdateWindow 
    {
        get
        {
            return shouldUpdateWindow_;
        }
        set
        {
            if (value && searchTiming == WindowSearchTiming.Manual) return;
            shouldUpdateWindow_ = value;
        }
    }

    [SerializeField]
    WindowSearchTiming searchTiming_ = WindowSearchTiming.OnlyWhenParameterChanged;
    public WindowSearchTiming searchTiming
    {
        get
        {
            return searchTiming_;
        }
        set
        {
            searchTiming_ = value;
            if (searchTiming_ == WindowSearchTiming.Manual) {
                shouldUpdateWindow = false;
            } else {
                shouldUpdateWindow = true;
            }
        }
    }

    [SerializeField]
    WindowTextureType type_ = WindowTextureType.Window;
    public WindowTextureType type
    {
        get
        {
            return type_;
        }
        set
        {
            shouldUpdateWindow = true;
            type_ = value;
        }
    }

    [SerializeField]
    bool altTabWindow_ = false;
    public bool altTabWindow
    {
        get 
        { 
            return altTabWindow_; 
        }
        set
        {
            shouldUpdateWindow = true;
            altTabWindow_ = value;
        }
    }

    [SerializeField]
    bool createChildWindows_ = true;
    public bool createChildWindows
    {
        get 
        { 
            return createChildWindows_;
        }
        set
        {
            createChildWindows_ = value;

            var manager = GetComponent<UwcWindowTextureChildrenManager>();
            if (createChildWindows_) {
                if (!manager) {
                    gameObject.AddComponent<UwcWindowTextureChildrenManager>();
                }
            } else {
                if (manager) {
                    Destroy(manager);
                }
            }
        }
    }

    public GameObject childWindowPrefab;
    public float childWindowZDistance = 0.02f;

    [SerializeField]
    string partialWindowTitle_;
    public string partialWindowTitle 
    {
        get 
        {
            return partialWindowTitle_;
        }
        set 
        {
            shouldUpdateWindow = true;
            partialWindowTitle_ = value;
        }
    }

    [SerializeField]
    int desktopIndex_ = 0;
    public int desktopIndex
    {
        get
        {
            return desktopIndex_;
        }
        set
        {
            shouldUpdateWindow = true;
            desktopIndex_ = (UwcManager.desktopCount > 0) ?
                Mathf.Clamp(value, 0, UwcManager.desktopCount - 1) : 0;
        }
    }

    public CaptureMode captureMode = CaptureMode.Auto;
    public CapturePriority capturePriority = CapturePriority.Auto;
    public WindowTextureCaptureTiming captureRequestTiming = WindowTextureCaptureTiming.OnlyWhenVisible;
    public int captureFrameRate = 30;
    public bool drawCursor = true;
    public bool updateTitle = true;
    public bool searchAnotherWindowWhenInvalid = false;

    public WindowTextureScaleControlType scaleControlType = WindowTextureScaleControlType.BaseScale;
    public float scalePer1000Pixel = 1f;
    public bool updateScaleForcely = false;

    UwcWindow window_;
    public UwcWindow window 
    { 
        get 
        {
            return window_;
        }
        set 
        {
            if (window_ == value) {
                return;
            }

            if (window_ != null) {
                window_.onCaptured.RemoveListener(OnCaptured);
            }

            var old = window_;
            window_ = value;
            onWindowChanged_.Invoke(window_, old);

            if (window_ != null) {
                shouldUpdateWindow = false;
                window_.onCaptured.AddListener(OnCaptured);
                window_.RequestCapture(CapturePriority.High);
            }
        }
    }

    public UwcWindowTextureManager manager { get; set; }
    public UwcWindowTexture parent { get; set; }

    UwcWindowChangeEvent onWindowChanged_ = new UwcWindowChangeEvent();
    public UwcWindowChangeEvent onWindowChanged
    {
        get { return onWindowChanged_; }
    }

    float basePixel
    {
        get { return 1000f / scalePer1000Pixel; }
    }

    public bool isValid
    {
        get
        {
            return window != null && window.isValid;
        }
    }

    public RenderTexture targetTexture;

    float captureTimer_ = 0f;
    bool isCaptureRequested_ = false;
    bool hasBeenCaptured_ = false;

    void Awake()
    {
        //list_.Add(this);
    }

    void OnDestroy()
    {
        //list_.Remove(this);
    }

    void Update()
    {
        UpdateSearchTiming();
        UpdateTargetWindow();

        if (!isValid) {
            return;
        }

        UpdateTexture();
        UpdateRenderer();
        UpdateScale();
        UpdateTitle();
        UpdateCaptureTimer();
        UpdateRequestCapture();

        UpdateBasicComponents();
    }

    void OnWillRenderObject()
    {
        if (!isCaptureRequested_) return;

        if (captureRequestTiming == WindowTextureCaptureTiming.OnlyWhenVisible) {
            RequestCapture();
        }
    }

    void UpdateTexture()
    {
        if (!isValid) return;

        window.cursorDraw = drawCursor;

        Graphics.Blit(window.texture, targetTexture);
    }

    void UpdateRenderer()
    {
        if (hasBeenCaptured_) {
            //if (renderer_) renderer_.enabled = !window.isIconic && window.isVisible;
        }
    }

    void UpdateScale()
    {
        if (!isValid || (!updateScaleForcely && window.isChild)) return;

        var scale = transform.localScale;

        switch (scaleControlType) {
            case WindowTextureScaleControlType.BaseScale: {
                targetTexture.width = window.width;
                targetTexture.height = window.height;
                break;
            }
            case WindowTextureScaleControlType.FixedWidth: {
                targetTexture.height = (int)(transform.localScale.x * window.height / window.width);
                break;
            }
            case WindowTextureScaleControlType.FixedHeight: {
                targetTexture.width = (int)(transform.localScale.y * window.width / window.height);
                break;
            }
            case WindowTextureScaleControlType.Manual: {
                break;
            }
        }

        if (float.IsNaN(scale.x)) scale.x = 0f;
        if (float.IsNaN(scale.y)) scale.y = 0f;

        transform.localScale = scale;
    }

    void UpdateTitle()
    {
        if (updateTitle && isValid) {
            window.RequestUpdateTitle();
        }
    }

    void UpdateCaptureTimer()
    {
        if (captureFrameRate < 0) {
            captureTimer_ = 0f;
            isCaptureRequested_ = true;
        } else { 
            captureTimer_ += Time.deltaTime;

            float T = 1f / captureFrameRate;
            if (captureTimer_ < T) return;

            while (captureTimer_  > T) {
                captureTimer_ -= T;
            }
        }

        isCaptureRequested_ = true;
    }

    void UpdateRequestCapture()
    {
        if (!isCaptureRequested_) return;

        if (captureRequestTiming == WindowTextureCaptureTiming.EveryFrame) {
            RequestCapture();
        }
    }

    void UpdateSearchTiming()
    {
        if (searchTiming == WindowSearchTiming.Always) {
            shouldUpdateWindow = true;
        }
    }

    void UpdateTargetWindow()
    {
        if (!shouldUpdateWindow) return;
        // Debug.Log("UpdateTargetWindow");

        switch (type)
        {
            case WindowTextureType.Window:
                window = UwcManager.Find(partialWindowTitle, altTabWindow);
                break;
            case WindowTextureType.Desktop:
                window = UwcManager.FindDesktop(desktopIndex);
                break;
            case WindowTextureType.Child:
                break;
        }
        // Debug.Log(window.title);
    }

    void UpdateBasicComponents()
    {
        //if (renderer_) renderer_.enabled = isValid;
        //if (collider_) collider_.enabled = isValid;
    }

    void OnCaptured()
    {
        hasBeenCaptured_ = true;
    }

    public void RequestCapture()
    {
        if (!isValid) return;

        isCaptureRequested_ = false;
        window.captureMode = captureMode;

        var priority = capturePriority;
        if (priority == CapturePriority.Auto) {
            priority = CapturePriority.Low;
            if (window == UwcManager.cursorWindow) {
                priority = CapturePriority.High;
            } else if (window.zOrder < UwcSetting.MiddlePriorityMaxZ) {
                priority = CapturePriority.Middle;
            }
        }

        window.RequestCapture(priority);
    }

    public void RequestWindowUpdate()
    {
        shouldUpdateWindow = true;
    } 
}

}