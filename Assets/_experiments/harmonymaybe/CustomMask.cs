using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plunderludics.Mut.HarmonyMaybe {

/// the mask shape for the player eye
public class CustomMask: MaskableGraphic {
    // -- refs --
    List<Rect> m_Rects = new();

    UIVertex[] quad = new UIVertex[4];

    bool newRects = false;

    // -- lifecycle --
    // -- MaskableGraphic --
    protected override void OnPopulateMesh(VertexHelper vh) {
        base.OnPopulateMesh(vh);

        // get rect
        foreach (var rect in m_Rects) {
            quad[0] = Point(rect.xMin, rect.yMax);
            quad[1] = Point(rect.xMin, rect.yMin);
            quad[2] = Point(rect.xMax, rect.yMin);
            quad[3] = Point(rect.xMax, rect.yMax);
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
    static UIVertex Point(float x, float y) {
        var vert = UIVertex.simpleVert;
        vert.position = new Vector3(x, y, 0f);
        vert.color = Color.green;
        return vert;
    }

    public void DrawRect(Rect rect) {
        m_Rects.Add(rect);
        newRects = true;
    }

    public void AddRandomRect() {
        // get rect
        var rect = rectTransform.rect;
        var w0 = rect.width;
        var w2 = w0 / 2f;
        var h0 = rect.height;
        var h2 = h0 / 2f;

        // get random values
        var w = Random.Range(0f, w0);
        var h = Random.Range(0f, h0);
        var x = Random.Range(-w2, w2 - w);
        var y = Random.Range(-h2, h2 - h);

        // x = 0;
        // y = 0;
        // w = 1920;
        // h = 1080;

        Debug.Log($"ADDING RECT {x} {y} {w} {h}");
        DrawRect(new Rect(x, y, w, h));
    }

    public void Clear() {
        m_Rects.Clear();
        newRects = true;
    }
}

}