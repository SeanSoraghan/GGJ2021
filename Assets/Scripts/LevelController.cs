using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public TileGrid Grid;
    public GameObject MainCameraObject;
    public GameObject TargetCamObject;
    public GameObject PlayerCameraObject;
    public float MainGridHeightProportion = 0.8f;

    void Start()
    {
        CreateLevel();
    }

    private void Awake()
    {
        if (Grid != null)
            Grid.levelController = this;
        LayoutCameras();
    }

    public void CreateLevel()
    {
        if (TargetCamObject != null)
        {
            Camera targetCamera = TargetCamObject.GetComponent<Camera>();
            targetCamera.enabled = false;
        }
        if (PlayerCameraObject != null)
        {
            Camera playerCamera = PlayerCameraObject.GetComponent<Camera>();
            playerCamera.enabled = false;
            FrameTile playerFrameTile = PlayerCameraObject.GetComponentInChildren<FrameTile>();
            if (playerFrameTile != null)
            {
                playerFrameTile.Hide();
            }
        }
        Grid.GenerateGrid();
    }

    void LayoutCameras()
    {
        float screenAspect = (float)Screen.height / (float)Screen.width;
        if (MainCameraObject != null)
        {
            MainCameraObject.transform.position = new Vector3(Grid.GridWidth * (Grid.TileSideLength - 1), Grid.GridHeight * (Grid.TileSideLength - 1), -2.0f) * 0.5f;
            Camera mainCamera = MainCameraObject.GetComponent<Camera>();
            mainCamera.orthographicSize = 6;
            mainCamera.rect = new Rect(0.0f, 0.2f, 1.0f, MainGridHeightProportion);
        }
        float panelCamWidth = (1.0f - MainGridHeightProportion) * screenAspect;
        if (TargetCamObject != null)
        {
            UIFrame targetCamFrame = TargetCamObject.GetComponent<UIFrame>();
            if (targetCamFrame != null)
            {
                targetCamFrame.panelFrameCentre = new Vector2(1.0f - panelCamWidth * 0.5f, MainGridHeightProportion + (1.0f - MainGridHeightProportion) * 0.5f);
                targetCamFrame.sideLengthProportion = 1.0f - MainGridHeightProportion;
            }
            Camera targetCamera = TargetCamObject.GetComponent<Camera>();
            targetCamera.rect = new Rect(1.0f - panelCamWidth, 0.0f, panelCamWidth, 1.0f - MainGridHeightProportion);
            targetCamera.enabled = false;
        }
        if (PlayerCameraObject != null)
        {
            UIFrame playerCamFrame = PlayerCameraObject.GetComponent<UIFrame>();
            if (playerCamFrame != null)
            {
                playerCamFrame.panelFrameCentre = new Vector2(panelCamWidth * 0.5f, MainGridHeightProportion + (1.0f - MainGridHeightProportion) * 0.5f);
                playerCamFrame.sideLengthProportion = 1.0f - MainGridHeightProportion;
            }
            Camera playerCamera = PlayerCameraObject.GetComponent<Camera>();
            playerCamera.rect = new Rect(0.0f, 0.0f, panelCamWidth, 1.0f - MainGridHeightProportion);
            playerCamera.enabled = false;
        }
    }

    public void GridSetupComplete()
    {
        if (PlayerCameraObject != null)
        {
            Vector2 topLeftTileWorldPos = Grid.GetTileWorldCentre(new Vector2Int(0, 0));
            PlayerCameraObject.transform.position = new Vector3(topLeftTileWorldPos.x, topLeftTileWorldPos.y, -1.0f);
            Camera playerCamera = PlayerCameraObject.GetComponent<Camera>();
            playerCamera.enabled = true;

            FrameTile playerFrameTile = PlayerCameraObject.GetComponentInChildren<FrameTile>();
            if (playerFrameTile != null)
            {
                playerFrameTile.Show();
            }
        }

        if (TargetCamObject != null)
        {
            Vector3 targetPos = ChooseTargetPosition();
            TargetCamObject.transform.position = targetPos;
            Camera targetCamera = TargetCamObject.GetComponent<Camera>();
            targetCamera.enabled = true;
        }
    }

    Vector3 ChooseTargetPosition()
    {
        Vector2Int targetTilePosition = new Vector2Int(Random.Range(0, Grid.GridWidth), Random.Range(0, Grid.GridHeight));
        Vector2 worldPos = Grid.GetTileWorldCentre(targetTilePosition);
        return new Vector3(worldPos.x, worldPos.y, -1.0f);
    }
}
