using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameTile : MonoBehaviour
{
    public int SideLength = 5;
    public float FrameThickness = 0.1f;
    PathRenderer framePathRenderer;

    private void Awake()
    {
        framePathRenderer = gameObject.AddComponent<PathRenderer>();
        framePathRenderer.LineThickness = FrameThickness;
        framePathRenderer.TextureName = "UI";
    }

    public void Show()
    {
        framePathRenderer.Positions.Clear();
        framePathRenderer.Positions.Add(new Vector3(-SideLength * 0.5f + FrameThickness * 0.5f, -SideLength * 0.5f + FrameThickness * 0.5f, 0.0f));
        framePathRenderer.Positions.Add(new Vector3(-SideLength * 0.5f + FrameThickness * 0.5f, SideLength * 0.5f - FrameThickness * 0.5f, 0.0f));
        framePathRenderer.Positions.Add(new Vector3(SideLength * 0.5f - FrameThickness * 0.5f, SideLength * 0.5f - FrameThickness * 0.5f, 0.0f));
        framePathRenderer.Positions.Add(new Vector3(SideLength * 0.5f - FrameThickness * 0.5f, -SideLength * 0.5f + FrameThickness * 0.5f, 0.0f));
        framePathRenderer.Positions.Add(new Vector3(-SideLength * 0.5f + FrameThickness * 0.5f, -SideLength * 0.5f + FrameThickness * 0.5f, 0.0f));
        framePathRenderer.RenderPathImmediate();
    }

    public void Hide()
    {
        framePathRenderer.Positions.Clear();
        framePathRenderer.RenderPathImmediate();
    }
}
