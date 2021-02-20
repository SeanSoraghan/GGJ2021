using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PathRenderer : MonoBehaviour
{
    public float LineDrawingSpeedUnitsPerSecond = 1.0f;
    public float PointReachedThreshold = 0.1f;
    public float LineThickness = 0.1f;
    public List<Vector3> Positions = new List<Vector3>();
    public PathTile OwnerTile;

    List<Vector3> renderPositions = new List<Vector3>();
    Vector3 currentSegmentDirection = Vector3.zero;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    bool drawingLine = false;
    bool erasingLine = false;

    string textureName = "White";
    public string TextureName
    {
        get
        {
            return textureName;
        }
        set
        {
            textureName = value;
            UpdateTexture();
        }
    }

    void UpdateTexture()
    {
        Material m = (Material)Resources.Load(TextureName, typeof(Material));
        if (m != null)
            meshRenderer.material = m;
    }

    void Awake()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        UpdateTexture();
        meshFilter.mesh = new Mesh();
    }

    public void RenderPathImmediate()
    {
        LinePositionsToMeshVerts(Positions);
    }

    public void ErasePathImmediate()
    {
        renderPositions.Clear();
        LinePositionsToMeshVerts(renderPositions);
    }

    public void BeginDrawingLine()
    {
        if (Positions.Count > 1)
        {
            renderPositions.Clear();
            renderPositions.Add(Positions[0]);
            currentSegmentDirection = Positions[renderPositions.Count] - renderPositions[renderPositions.Count - 1];
            currentSegmentDirection.Normalize();
            renderPositions.Add(renderPositions[renderPositions.Count - 1]);
            drawingLine = true;
            StartCoroutine(LineDraw());
        }
    }

    public void BeginErasingLine()
    {
        if (renderPositions.Count > 1)
        {
            currentSegmentDirection = renderPositions[renderPositions.Count - 2] - renderPositions[renderPositions.Count - 1];
            currentSegmentDirection.Normalize();
            erasingLine = true;
            StartCoroutine(LineErase());
        }
    }

    void LinePositionsToMeshVerts(List<Vector3> linePositions)
    {
        List<Vector3> meshVerts = new List<Vector3>();
        List<int> meshTris = new List<int>();
        Mesh lineMesh = meshFilter.mesh;
        lineMesh.Clear();
        if (linePositions.Count > 1)
        {
            for (int p = 0; p < linePositions.Count - 1; ++p)
            {
                Vector3 start = linePositions[p];
                Vector3 end = linePositions[p + 1];
                float dx = end.x - start.x;
                float dy = end.y - start.y;
                Vector3 normal = new Vector3(dy, -dx, 0.0f).normalized;
                Vector3 a = start + normal * (LineThickness / 2);
                Vector3 b = start + normal * (LineThickness / -2);
                Vector3 c = end + normal * (LineThickness / -2);
                Vector3 d = end + normal * (LineThickness / 2);
                meshVerts.Add(a);
                meshVerts.Add(b);
                meshVerts.Add(c);
                meshVerts.Add(d);
                int indexStart = p * 4;
                // abc
                meshTris.Add(indexStart);
                meshTris.Add(indexStart + 1);
                meshTris.Add(indexStart + 2);
                // acd
                meshTris.Add(indexStart);
                meshTris.Add(indexStart + 2);
                meshTris.Add(indexStart + 3);
            }
            lineMesh.vertices = meshVerts.ToArray();
            lineMesh.triangles = meshTris.ToArray();
        }
    }

    void FinishedDrawingSegment()
    {
        if (renderPositions.Count < Positions.Count)
        {
            currentSegmentDirection = Positions[renderPositions.Count] - renderPositions[renderPositions.Count - 1];
            currentSegmentDirection.Normalize();
            renderPositions.Add(renderPositions[renderPositions.Count - 1]);
        }
        else
        {
            renderPositions = Positions;
            drawingLine = false;
            OwnerTile?.PathDrawingComplete();
        }
    }

    void FinishedErasingSegment()
    {
        if (renderPositions.Count > 0)
            renderPositions.RemoveAt(renderPositions.Count - 1);

        if (renderPositions.Count > 1)
        {
            currentSegmentDirection = renderPositions[renderPositions.Count - 2] - renderPositions[renderPositions.Count - 1];
            currentSegmentDirection.Normalize();
        }
        else
        {
            renderPositions.Clear();
            erasingLine = false;
            OwnerTile?.PathErasingComplete();
        }
    }

    IEnumerator LineDraw()
    {
        while (drawingLine && OwnerTile.State == PathTile.TileState.Drawing)
        {
            renderPositions[renderPositions.Count - 1] = renderPositions[renderPositions.Count - 1] + currentSegmentDirection * Time.deltaTime * LineDrawingSpeedUnitsPerSecond;
            float renderMagnitude = (renderPositions[renderPositions.Count - 1] - renderPositions[renderPositions.Count - 2]).magnitude;
            float targetMagnitude = (Positions[renderPositions.Count - 1] - Positions[renderPositions.Count - 2]).magnitude;
            if (renderMagnitude > targetMagnitude || Mathf.Abs(targetMagnitude - renderMagnitude) < PointReachedThreshold)
            {
                renderPositions[renderPositions.Count - 1] = Positions[renderPositions.Count - 1];
                FinishedDrawingSegment();
            }
            LinePositionsToMeshVerts(renderPositions);
            yield return null;
        }
        yield break;
    }

    IEnumerator LineErase()
    {
        while (erasingLine && OwnerTile.State == PathTile.TileState.Erasing)
        {
            renderPositions[renderPositions.Count - 1] = renderPositions[renderPositions.Count - 1] + currentSegmentDirection * Time.deltaTime * LineDrawingSpeedUnitsPerSecond;
            float renderMagnitude = (renderPositions[renderPositions.Count - 1] - renderPositions[renderPositions.Count - 2]).magnitude;
            float targetMagnitude = 0.0f;
            Vector2 currentUnitVec = (renderPositions[renderPositions.Count - 2] - renderPositions[renderPositions.Count - 1]).normalized;
            if (Mathf.Approximately(Vector2.Dot(currentSegmentDirection, currentUnitVec), -1.0f)  || Mathf.Abs(targetMagnitude - renderMagnitude) < PointReachedThreshold)
            {
                renderPositions[renderPositions.Count - 1] = Positions[renderPositions.Count - 1];
                FinishedErasingSegment();
            }
            LinePositionsToMeshVerts(renderPositions);
            yield return null;
        }
        yield break;
    }
}
