using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTile : MonoBehaviour
{
    public TileGrid OwnerGrid;

    PathRenderer pathRenderer;
    PathGenerator pathGenerator;

    public enum TileState
    {
        Empty = 0,
        Drawing,
        Complete
    }

    TileState _tileState = TileState.Empty;
    public TileState State
    {
        get
        {
            return _tileState;
        }
    }

    public void ClearTile()
    {
        pathGenerator.ClearPath();
        pathRenderer.Positions.Clear();
        pathRenderer.Positions = pathGenerator.PathPositions;
        pathRenderer.RenderPathImmediate();
        _tileState = TileState.Empty;
    }

    public void GeneratePath()
    {
        pathGenerator.GeneratePath();
        pathRenderer.Positions.Clear();
        pathRenderer.Positions = pathGenerator.PathPositions;
    }

    public void BeginDrawing()
    {
        _tileState = TileState.Drawing;
        pathRenderer.BeginDrawingLine();
    }

    public void PathDrawingComplete()
    {
        _tileState = TileState.Complete;
        OwnerGrid?.TileCompleted();
    }

    void Awake()
    {
        pathRenderer = gameObject.AddComponent<PathRenderer>();
        pathGenerator = gameObject.AddComponent<PathGenerator>();
        pathRenderer.OwnerTile = this;
    }
}
