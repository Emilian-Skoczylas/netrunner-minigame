using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineConnector : Graphic
{
    [SerializeField] private RectTransform from;
    [SerializeField] private RectTransform to;

    [Range(0f, 1f)] public float progress = 0.5f;

    [SerializeField] private float thickness = 8f;
    [SerializeField] private Color progressColor = Color.blue;
    [SerializeField] private Color backgroundColor = Color.white;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private bool reverseProgress = false;

    private bool isInteractive = false;

    public void SetOffset(Vector2 offset)
    {
        _offset = offset;
        SetAllDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (from == null || to == null) return;

        Vector2 start = WorldToLocalCanvasPoint(from) + _offset;
        Vector2 end = WorldToLocalCanvasPoint(to) + _offset;

        float t = Mathf.Clamp01(progress);
        Vector2 dir = (end - start).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x) * (thickness / 2f);

        Color bgColor = isInteractive
            ? backgroundColor
            : new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0.25f);

        if (reverseProgress)
        {
            // progress idzie od END → START
            Vector2 mid = Vector2.Lerp(end, start, t);
            AddQuad(vh, end, mid, perp, progressColor);
            AddQuad(vh, mid, start, perp, bgColor);
        }
        else
        {
            // progress idzie od START → END
            Vector2 mid = Vector2.Lerp(start, end, t);
            AddQuad(vh, start, mid, perp, progressColor);
            AddQuad(vh, mid, end, perp, bgColor);
        }
    }

    private void AddQuad(VertexHelper vh, Vector2 a, Vector2 b, Vector2 perp, Color color)
    {
        int startIndex = vh.currentVertCount;

        vh.AddVert(a - perp, color, Vector2.zero);
        vh.AddVert(a + perp, color, Vector2.zero);
        vh.AddVert(b + perp, color, Vector2.zero);
        vh.AddVert(b - perp, color, Vector2.zero);

        vh.AddTriangle(startIndex + 0, startIndex + 1, startIndex + 2);
        vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex + 0);
    }

    private Vector2 WorldToLocalCanvasPoint(RectTransform rt)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            RectTransformUtility.WorldToScreenPoint(null, rt.position),
            null,
            out Vector2 local
        );
        return local;
    }

    public void SetTargets(RectTransform fromTarget, RectTransform toTarget, bool interactive)
    {
        from = fromTarget;
        to = toTarget;
        isInteractive = interactive;
        SetAllDirty();
    }

    public void SetProgress(float value)
    {
        progress = Mathf.Clamp01(value);
        SetAllDirty();
    }
    public void SetReverseProgress(bool reverse)
    {
        reverseProgress = reverse;
        progressColor = Color.red;
        SetAllDirty();
    }
}
