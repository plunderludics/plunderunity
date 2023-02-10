using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SetPivot : MonoBehaviour
{
    public RectTransform target;
    private RectTransform _rectTransform;

    public Vector2 point;

    // Start is called before the first frame update
    void OnEnable()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target) {
            point = _rectTransform.InverseTransformPoint(target.position);
        }
    }
}
