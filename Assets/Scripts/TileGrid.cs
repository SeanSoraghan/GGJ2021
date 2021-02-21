using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public int GridWidth = 4;
    public int GridHeight = 3;
    public int TileSideLength = 5;
    public GameObject pathTilePrefab;
    public GameObject GridPointPrefab;
    public float LineDrawingSpeedUnitsPerSecond = 5.0f;
    public float PointReachedThreshold = 0.1f;
    public LevelController levelController;
    public int VisualGridCoarse = 2;

    int numErasedTiles = 0;
    int noteIndex = 0;

    List<List<GameObject>> tiles = new List<List<GameObject>>();
    Vector2Int _currentTileXY = Vector2Int.zero;
    Vector2Int currentTileXY
    {
        get { return _currentTileXY; }
        set 
        {
            _currentTileXY = value;
        } 
    }
    List<Vector2Int> emptyTilePositions = new List<Vector2Int>();

    public void EraseGrid()
    {
        SFXManager.Instance?.PlayClip(SFXManager.SFXType.Erase);
        numErasedTiles = 0;
        for (int tileX = 0; tileX < GridWidth; ++tileX)
        {
            for (int tileY = 0; tileY < GridHeight; ++tileY)
            {
                tiles[tileX][tileY].GetComponent<PathTile>().BeginErasing();
            }
        }
    }

    public void ClearGrid()
    {
        noteIndex = 0;
        emptyTilePositions.Clear();
        for (int tileX = 0; tileX < GridWidth; ++tileX)
        {
            for (int tileY = 0; tileY < GridHeight; ++tileY)
            {
                tiles[tileX][tileY].GetComponent<PathTile>().ClearTile();
                emptyTilePositions.Add(new Vector2Int(tileX, tileY));
            }
        }
    }
    public void GenerateGrid()
    {
        ClearGrid();
        ChooseEmptyTile();
    }

    public Vector2 GetTileWorldCentre(Vector2Int tileXY)
    {
        return new Vector2(tileXY.x * (TileSideLength - 1) + (TileSideLength - 1) * 0.5f
                          , tileXY.y * (TileSideLength - 1) + (TileSideLength - 1) * 0.5f);
    }

    int LoopRoundMod(int x, int m)
    {
        return (x % m + m) % m;
    }
    public void TileCompleted()
    {
        int indexToRemove = emptyTilePositions.FindIndex(xy => xy == currentTileXY);
        if (indexToRemove == -1)
        {
            Debug.LogError("Empty tile position not found!");
        }
        emptyTilePositions.RemoveAt(indexToRemove);
        Vector2Int exitPosition = tiles[currentTileXY.x][currentTileXY.y].GetComponent<PathGenerator>().ExitPoint;
        // Need to deal with exiting directly on a corner - choose horizontal or vertical neighbour
        Vector2Int nextTileEntryPoint = exitPosition;
        if (exitPosition.x == 0)
        {
            currentTileXY = new Vector2Int(LoopRoundMod(currentTileXY.x - 1, GridWidth), currentTileXY.y);
            nextTileEntryPoint.x = TileSideLength - 1;
        }
        else if (exitPosition.x == TileSideLength - 1)
        {
            currentTileXY = new Vector2Int(LoopRoundMod(currentTileXY.x + 1, GridWidth), currentTileXY.y);
            nextTileEntryPoint.x = 0;
        }
        if (exitPosition.y == 0)
        {
            currentTileXY = new Vector2Int(currentTileXY.x, LoopRoundMod(currentTileXY.y - 1, GridHeight));
            nextTileEntryPoint.y = TileSideLength - 1;
        }
        else if (exitPosition.y == TileSideLength - 1)
        {
            currentTileXY = new Vector2Int(currentTileXY.x, LoopRoundMod(currentTileXY.y + 1, GridHeight));
            nextTileEntryPoint.y = 0;
        }
        if (tiles[currentTileXY.x][currentTileXY.y].GetComponent<PathTile>().State == PathTile.TileState.Empty)
        {
            tiles[currentTileXY.x][currentTileXY.y].GetComponent<PathGenerator>().EntryPoint = nextTileEntryPoint;
            tiles[currentTileXY.x][currentTileXY.y].GetComponent<PathTile>().GeneratePath();
            tiles[currentTileXY.x][currentTileXY.y].GetComponent<PathTile>().BeginDrawing();
            MusicManager.Instance?.TriggerNote(noteIndex, MusicManager.NoteLength.Long);
            MusicManager.Instance?.TriggerNote(noteIndex, MusicManager.NoteLength.Medium, true);
            noteIndex = LoopRoundMod(noteIndex + 1, MusicManager.numNotes);
        }
        else
        {
            ChooseEmptyTile();
        }
    }

    public void TileEraseCompleted()
    {
        ++numErasedTiles;
        if (numErasedTiles >= GridWidth * GridHeight)
        {
            levelController.AllTilesErased();
        }
    }

    void Start()
    {
        for (int gridX = 0; gridX < GridWidth; ++gridX)
        {
            tiles.Add(new List<GameObject>());
            int tileStartX = gridX * (TileSideLength - 1);
            for (int gridY = 0; gridY < GridHeight; ++gridY)
            {
                int tileStartY = gridY * (TileSideLength - 1);
                tiles[gridX].Add(Instantiate(pathTilePrefab, new Vector3(tileStartX, tileStartY, 0), Quaternion.identity));
                tiles[gridX][gridY].GetComponent<PathTile>().OwnerGrid = this;
                tiles[gridX][gridY].GetComponent<PathGenerator>().SideLength = TileSideLength;
                tiles[gridX][gridY].GetComponent<PathRenderer>().LineDrawingSpeedUnitsPerSecond = LineDrawingSpeedUnitsPerSecond;
                tiles[gridX][gridY].GetComponent<PathRenderer>().PointReachedThreshold = PointReachedThreshold;
                tiles[gridX][gridY].GetComponent<PathRenderer>().TextureName = "Squiggle";
                emptyTilePositions.Add(new Vector2Int(gridX, gridY));
                for (int gridMarkerX = 0; gridMarkerX < VisualGridCoarse; ++gridMarkerX)
                {
                    for (int gridMarkerY = 0; gridMarkerY < VisualGridCoarse; ++gridMarkerY)
                    {
                        float gridMarkerWorldX = tileStartX + ((TileSideLength - 1) / (float)VisualGridCoarse) * gridMarkerX;
                        float gridMarkerWorldY = tileStartY + ((TileSideLength - 1) / (float)VisualGridCoarse) * gridMarkerY;
                        Instantiate(GridPointPrefab, new Vector3(gridMarkerWorldX, gridMarkerWorldY, 1.0f), Quaternion.identity);
                    }
                }
            }
            // Last row of grid points
            for (int gridMarkerX = 0; gridMarkerX < VisualGridCoarse; ++gridMarkerX)
            {
                float gridMarkerWorldX = tileStartX + ((TileSideLength - 1) / (float)VisualGridCoarse) * gridMarkerX;
                int gridPointY = GridHeight * (TileSideLength - 1);
                Instantiate(GridPointPrefab, new Vector3(gridMarkerWorldX, gridPointY, 1.0f), Quaternion.identity);
            }
        }
        // Last column of grid points
        for (int gridY = 0; gridY < GridHeight; ++gridY)
        {
            int tileStartY = gridY * (TileSideLength - 1);
            for (int gridMarkerY = 0; gridMarkerY < VisualGridCoarse; ++gridMarkerY)
            {
                int gridPointX = GridWidth * (TileSideLength - 1);
                float gridMarkerWorldY = tileStartY + ((TileSideLength - 1) / (float)VisualGridCoarse) * gridMarkerY;
                Instantiate(GridPointPrefab, new Vector3(gridPointX, gridMarkerWorldY, 1.0f), Quaternion.identity);
            }
        }
        // Top right grid point
        Instantiate(GridPointPrefab, new Vector3(GridWidth * (TileSideLength - 1), GridHeight * (TileSideLength - 1), 1.0f), Quaternion.identity);
    }

    void ChooseEmptyTile()
    {
        // Set to -1, -1 for note triggering (start new sequence).
        currentTileXY = new Vector2Int(-1, -1);
        if (emptyTilePositions.Count > 0)
        {
            currentTileXY = emptyTilePositions[Random.Range(0, emptyTilePositions.Count)];
            Vector2Int randomEntryPoint = Vector2Int.zero;
            tiles[currentTileXY.x][currentTileXY.y].GetComponent<PathGenerator>().EntryPoint = randomEntryPoint;
            tiles[currentTileXY.x][currentTileXY.y].GetComponent<PathTile>().GeneratePath();
            tiles[currentTileXY.x][currentTileXY.y].GetComponent<PathTile>().BeginDrawing();
            MusicManager.Instance?.TriggerNote(noteIndex, MusicManager.NoteLength.Long);
            MusicManager.Instance?.TriggerNote(noteIndex, MusicManager.NoteLength.Medium, true);
            noteIndex = LoopRoundMod(noteIndex + 1, MusicManager.numNotes);
        }
        else
        {
            GridComplete();
        }
    }

    void GridComplete()
    {
        levelController?.GridSetupComplete();
    }
}
