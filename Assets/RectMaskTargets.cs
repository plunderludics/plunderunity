using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectMask2D))]
[ExecuteAlways]
public class RectMaskTargets : MonoBehaviour
{
    public List<MaskableGraphic> targets;
    // Start is called before the first frame update
    void OnEnable()
    {
        Refresh();
    }

    void Refresh() {        
        RectMask2D _rectMask = GetComponent<RectMask2D>();
        // Flip the rectMask on/off to clear the clippable list
        _rectMask.enabled = false;
        _rectMask.enabled = true;
        // Add targets
        foreach (var target in targets) {
            _rectMask.AddClippable(target);
        }
    }
}
