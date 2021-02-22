using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGrid : MonoBehaviour
{
	public int GridWidth = 0;
    public int GridHeight = 0;
	public GameObject GridPointPrefab;
	List<List<GameObject>> gridPoints = new List<List<GameObject>>();

    void Start()
    {
        for (int gridX = 0; gridX < GridWidth; ++gridX)
        {
            gridPoints.Add(new List<GameObject>());
            for (int gridY = 0; gridY < GridHeight; ++gridY)
            {
                gridPoints[gridX].Add(Instantiate(GridPointPrefab, new Vector3(gridX, gridY, 1.0f), Quaternion.identity));
            }
        }
    }

    public void PulsePoint(int x, int y, float pulseScaleProportion)
    {
        gridPoints[x][y].GetComponent<GridPointAnimator>().Pulse(pulseScaleProportion);
    }
}
