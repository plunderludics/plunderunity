using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uWindowCapture;

public class EmulatorWindow : MonoBehaviour {
    UwcWindowTexture windowTexturePrefab;
    HashSet<int> windowIds;
    string partialTitle = "BizHawk";

    [System.Serializable]
    class WindowToId {
      public string PartialTitle;
      public string Id;
    }

    // WindowToId[] nameremaps = new WindowToId[] {    };
    List<UwcWindowTexture> windowTexturePool = new List<UwcWindowTexture>() ;

    void Start() {
      UwcManager.onWindowAdded.AddListener((window) => {
        if(!window.title.ToLower().Contains(partialTitle)) return;
        windowIds.Add(window.id);
        var newWindow = Instantiate(windowTexturePrefab, this.transform);
        windowTexturePool.Add(newWindow);
      });
    }
}