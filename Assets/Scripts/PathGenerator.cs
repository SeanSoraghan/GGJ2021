using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PathGenerator : MonoBehaviour
{
    public float LengthMultiplier = 1.0f;
    public int SideLength = 5;
    public int Inset = 1;
    public Vector2Int EntryPoint = Vector2Int.zero;
    public Vector2Int ExitPoint = Vector2Int.zero;
    public int MinNumPathPoints = 5;
    public List<Vector3> PathPositions
    {
        get
        {
            List<Vector3> positions = new List<Vector3>();
            if (pathPoints.Count > 0)
            {
                Vector3 currentPoint = new Vector3(EntryPoint.x, EntryPoint.y, 0.0f) * LengthMultiplier;
                positions.Add(new Vector3(EntryPoint.x, EntryPoint.y, 0.0f) * LengthMultiplier);
                for (int i = 0; i < pathPoints.Count; ++i)
                {
                    positions.Add(new Vector3(pathPoints[i].x, pathPoints[i].y, 0.0f) * LengthMultiplier);
                }
                positions.Add(new Vector3(ExitPoint.x, ExitPoint.y, 0.0f) * LengthMultiplier);
            }
            return positions;
        }
    }
    List<Vector2Int> pathPoints = new List<Vector2Int>();
    List<Vector2Int> deadEndPoints = new List<Vector2Int>();

    public void ClearPath()
    {
        pathPoints.Clear();
        deadEndPoints.Clear();
    }
    public void GeneratePath()
    {
        ClearPath();
        ChooseNextPoint();
        while (pathPoints.Count < MinNumPathPoints || !PointIsOnInnerPerimeter(pathPoints[pathPoints.Count - 1]))
        {
            ChooseNextPoint();
        }
        ChooseExitPoint();
    }

    List<Vector2Int> GetPossibleNextPositions(Vector2Int currentPosition)
    {
        List<Vector2Int> possiblePoints = new List<Vector2Int>();
        for (int i = 0; i < 8; ++i)
        {
            int x = i == 3 || i == 7 ? 0 : i < 3 ? -1 : 1;
            int y = i == 1 || i == 5 ? 0 : i > 1 && i < 5 ? 1 : -1;
            Vector2Int positionCandidate = currentPosition + new Vector2Int(x, y);
            if (!pathPoints.Contains(positionCandidate) && !deadEndPoints.Contains(positionCandidate) && PointOnOrWithinInnerPerimeter(positionCandidate))
            {
                possiblePoints.Add(positionCandidate);
            }
        }
        return possiblePoints;
    }

    List<Vector2Int> GetPossibleExitPositions(Vector2Int penultimatePosition)
    {
        List<Vector2Int> possiblePoints = new List<Vector2Int>();
        for (int i = 0; i < 8; ++i)
        {
            int x = i == 3 || i == 7 ? 0 : i < 3 ? -1 : 1;
            int y = i == 1 || i == 5 ? 0 : i > 1 && i < 5 ? 1 : -1;
            Vector2Int positionCandidate = penultimatePosition + new Vector2Int(x, y);
            if (PointIsOnOuterPerimeter(positionCandidate))
            {
                possiblePoints.Add(positionCandidate);
            }
        }
        return possiblePoints;
    }

    void ChooseNextPoint()
    {
        Vector2Int currentPoint = pathPoints.Count > 0 ? pathPoints[pathPoints.Count - 1] : EntryPoint;
        List<Vector2Int> possibleNextPositions = GetPossibleNextPositions(currentPoint);
        if (possibleNextPositions.Count == 0)
        {
            Assert.IsTrue(pathPoints.Count > 0);
            deadEndPoints.Add(currentPoint);
            pathPoints.RemoveAt(pathPoints.Count - 1);
        }
        else
        {
            Vector2Int nextPoint = possibleNextPositions[Random.Range(0, possibleNextPositions.Count)];
            pathPoints.Add(nextPoint);
        }
    }

    void ChooseExitPoint() 
    {
        Assert.IsTrue(pathPoints.Count > 0);
        Vector2Int currentPoint = pathPoints[pathPoints.Count - 1];
        Assert.IsTrue(PointIsOnInnerPerimeter(currentPoint));
        List<Vector2Int> possibleExitPositions = GetPossibleExitPositions(currentPoint);
        Assert.IsTrue(possibleExitPositions.Count > 0);
        ExitPoint = possibleExitPositions[Random.Range(0, possibleExitPositions.Count)];
    }

    bool PointIsOnInnerPerimeter(Vector2Int p)
    {
        return p.x == Inset || p.y == Inset || p.x == (SideLength - 1) - Inset || p.y == (SideLength - 1) - Inset;
    }

    bool PointOnOrWithinInnerPerimeter(Vector2Int p)
    {
        return p.x >= Inset && p.y >= Inset && p.x <= (SideLength - 1) - Inset && p.y <= (SideLength - 1) - Inset;
    }

    bool PointIsOnOuterPerimeter(Vector2Int p)
    {
        return p.x == 0 || p.y == 0 || p.x == SideLength - 1 || p.y == SideLength - 1;
    }
}
