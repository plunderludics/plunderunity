using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plunderludics.Tools {

/// the mask shape for the player eye
[RequireComponent(typeof(Mask))]
[RequireComponent(typeof(CanvasRenderer))]
public class CustomMask: MaskableGraphic {
    public static readonly Rect EmptyRect = new(0f, 0f, 0f, 0f);
    public static readonly Rect FullRect = new(0f, 0f, 1920f, 1080f);

    // -- refs --
    List<MaskRect> m_Rects = new();

    struct MaskRect {
        public readonly Rect Rect;
        public readonly bool IsErase;

        public MaskRect(Rect rect, bool isErase = false) {
            Rect = rect;
            IsErase = isErase;
        }
    }

    UIVertex[] quad = new UIVertex[4];

    bool newRects = false;

    // -- lifecycle --
    // -- MaskableGraphic --
    protected override void OnPopulateMesh(VertexHelper vh) {
        base.OnPopulateMesh(vh);

        // get rect
        foreach (var maskRect in m_Rects) {
            var rect = maskRect.Rect;

            var w = rect.xMax - rect.xMin;
            var h = rect.yMax - rect.yMin;
            var w2 = w / 2f;
            var h2 = h / 2f;
            quad[0] = Point(rect.xMin - w2, rect.yMax - h2, maskRect.IsErase);
            quad[1] = Point(rect.xMin - w2, rect.yMin - h2, maskRect.IsErase);
            quad[2] = Point(rect.xMax - w2, rect.yMin - h2, maskRect.IsErase);
            quad[3] = Point(rect.xMax - w2, rect.yMax - h2, maskRect.IsErase);

            vh.AddUIVertexQuad(quad);
        }
    }

    void Update() {
        if (newRects) {
            SetVerticesDirty();
        }

        newRects = false;
    }

    // -- queries --
    /// create a vert w/ the point
    static UIVertex Point(float x, float y, bool erase = false) {
        var vert = UIVertex.simpleVert;
        vert.position = new Vector3(x, y, 0f);
        var color = Color.green;
        color.a = erase ? 0f : 1f;
        vert.color = color;
        return vert;
    }

    public void DrawRect(Rect rect) {
        m_Rects.Add(new MaskRect(rect));
        newRects = true;
    }

    public void EraseRect(Rect rect) {
        m_Rects.Add(new MaskRect(rect, isErase: true));
        newRects = true;
    }


    public void DrawRandomRect(Rect? min = null, Rect? max = null) {
        min = min ?? EmptyRect;
        max = max ?? FullRect;

        // get rect
        var rect = rectTransform.rect;
        var w0 = rect.width;
        var w2 = w0 / 2f;
        var h0 = rect.height;
        var h2 = h0 / 2f;

        // get random values
        var w = Random.Range(min.Value.width, max.Value.width);
        var h = Random.Range(min.Value.height, max.Value.height);
        var x = Random.Range(min.Value.xMin, w);
        var y = Random.Range(min.Value.yMin, h);

        DrawRect(new Rect(x, y, w, h));
    }

    public void Clear() {
        m_Rects.Clear();
        newRects = true;
    }
}

}