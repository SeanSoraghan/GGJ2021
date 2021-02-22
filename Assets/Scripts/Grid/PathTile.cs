using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTile : MonoBehaviour
{
    public TileGrid OwnerGrid;

    PathRenderer pathRenderer;
    PathGenerator pathGenerator;

    public int GetSideLength() { return pathGenerator.SideLength; }
    public enum TileState
    {
        Empty = 0,
        Drawing,
        Complete,
        Erasing
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
        if (OwnerGrid.levelController.drawImmediate)
        {
            pathRenderer.RenderPathImmediate();
            PathDrawingComplete();
        }
        else 
        { 
            pathRenderer.BeginDrawingLine(); 
        }
    }

    public void BeginErasing()
    {
        _tileState = TileState.Erasing;
        if (OwnerGrid.levelController.drawImmediate)
        {
            pathRenderer.ErasePathImmediate();
            PathErasingComplete();
        }
        else
        {
            pathRenderer.BeginErasingLine();
        }
    }

    public void PathDrawingComplete()
    {
        _tileState = TileState.Complete;
        OwnerGrid?.TileCompleted();
    }

    public void PathErasingComplete()
    {
        _tileState = TileState.Empty;
        OwnerGrid?.TileEraseCompleted();
    }

    void Awake()
    {
        pathRenderer = gameObject.AddComponent<PathRenderer>();
        pathGenerator = gameObject.AddComponent<PathGenerator>();
        pathRenderer.OwnerTile = this;
    }
}
