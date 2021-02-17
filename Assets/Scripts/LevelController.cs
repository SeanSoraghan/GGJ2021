using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public TileGrid Grid;
    public GameObject GridCameraObject;
    public GameObject TargetCamObject;
    public GameObject PlayerCameraObject;
    public GameObject CurrentFrameObject;
    public GameObject TargetFrameObject;
    public GameObject InstructionsObject;
    public float MainGridHeightProportion = 0.8f;

    public bool setupComplete = false;

    float[] rotationOptions = { -45.0f, 0.0f, 45.0f };
    int numRotationOptions = 3;

    void Start()
    {
        ClearLevel();
        CreateLevel();
    }

    private void Awake()
    {
        if (Grid != null)
            Grid.levelController = this;
        LayoutCameras();
    }

    void ShowCamFeed(GameObject camObj)
    {
        Vector3 pos = camObj.transform.position;
        camObj.transform.position = new Vector3(pos.x, pos.y, -1.0f);
    }

    void HideCamFeed(GameObject camObj)
    {
        Vector3 pos = camObj.transform.position;
        camObj.transform.position = new Vector3(pos.x, pos.y, 2.0f);
    }

    public void ClearLevel()
    {
        setupComplete = false;
        if (TargetCamObject != null)
        {
            Camera targetCamera = TargetCamObject.GetComponent<Camera>();
            HideCamFeed(TargetCamObject);
            UIFrame targetCamFrame = TargetFrameObject.GetComponent<UIFrame>();
            if (targetCamFrame != null)
            {
                targetCamFrame.display = false;
            }
        }
        if (PlayerCameraObject != null)
        {
            Camera playerCamera = PlayerCameraObject.GetComponent<Camera>();
            HideCamFeed(PlayerCameraObject);
            FrameTile playerFrameTile = PlayerCameraObject.GetComponentInChildren<FrameTile>();
            if (playerFrameTile != null)
            {
                playerFrameTile.Hide();
            }
            UIFrame playerCamFrame = CurrentFrameObject.GetComponent<UIFrame>();
            if (playerCamFrame != null)
            {
                playerCamFrame.display = false;
            }
        }
        if (InstructionsObject != null)
            InstructionsObject.GetComponent<Instructions>().display = false;
        Grid.ClearGrid();
    }

    public void CreateLevel()
    {
        Grid.GenerateGrid();
    }

    void LayoutCameras()
    {
        float screenAspect = (float)Screen.height / (float)Screen.width;
        if (GridCameraObject != null)
        {
            GridCameraObject.transform.position = new Vector3(Grid.GridWidth * (Grid.TileSideLength - 1), Grid.GridHeight * (Grid.TileSideLength - 1), -2.0f) * 0.5f;
            Camera gridCamera = GridCameraObject.GetComponent<Camera>();
            gridCamera.orthographicSize = 6;
            gridCamera.rect = new Rect(0.0f, 0.2f, 1.0f, MainGridHeightProportion);
        }
        float panelCamWidth = (1.0f - MainGridHeightProportion) * screenAspect;
        float textWidthProportion = 0.3f;
        if (TargetCamObject != null)
        {
            UIFrame targetCamFrame = TargetFrameObject.GetComponent<UIFrame>();
            if (targetCamFrame != null)
            {
                targetCamFrame.panelFrameCentre = new Vector2(1.0f - panelCamWidth * 0.5f, MainGridHeightProportion + (1.0f - MainGridHeightProportion) * 0.5f);
                targetCamFrame.sideLengthProportion = 1.0f - MainGridHeightProportion;
            }
            Camera targetCamera = TargetCamObject.GetComponent<Camera>();
            targetCamera.rect = new Rect(1.0f - panelCamWidth, 0.0f, panelCamWidth, 1.0f - MainGridHeightProportion);
            HideCamFeed(TargetCamObject);
        }
        if (PlayerCameraObject != null)
        {
            UIFrame playerCamFrame = CurrentFrameObject.GetComponent<UIFrame>();
            if (playerCamFrame != null)
            {
                playerCamFrame.panelFrameCentre = new Vector2(panelCamWidth * 0.5f, MainGridHeightProportion + (1.0f - MainGridHeightProportion) * 0.5f);
                playerCamFrame.sideLengthProportion = 1.0f - MainGridHeightProportion;
            }
            Camera playerCamera = PlayerCameraObject.GetComponent<Camera>();
            playerCamera.rect = new Rect(0.0f, 0.0f, panelCamWidth, 1.0f - MainGridHeightProportion);
            HideCamFeed(PlayerCameraObject);
        }
    }

    public void GridSetupComplete()
    {
        if (PlayerCameraObject != null)
        {
            Vector2 topLeftTileWorldPos = Grid.GetTileWorldCentre(new Vector2Int(0, 0));
            PlayerCameraObject.transform.position = new Vector3(topLeftTileWorldPos.x, topLeftTileWorldPos.y, -1.0f);
            Camera playerCamera = PlayerCameraObject.GetComponent<Camera>();
            ShowCamFeed(PlayerCameraObject);

            FrameTile playerFrameTile = PlayerCameraObject.GetComponentInChildren<FrameTile>();
            if (playerFrameTile != null)
            {
                playerFrameTile.Show();
            }

            UIFrame playerCamFrame = CurrentFrameObject.GetComponent<UIFrame>();
            if (playerCamFrame != null)
            {
                playerCamFrame.display = true;
            }
        }

        if (TargetCamObject != null)
        {
            Vector3 targetPos = ChooseTargetPosition();
            TargetCamObject.transform.position = targetPos;
            TargetCamObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationOptions[Random.Range(0, numRotationOptions)]);
            Camera targetCamera = TargetCamObject.GetComponent<Camera>();
            ShowCamFeed(TargetCamObject);

            UIFrame targetCamFrame = TargetFrameObject.GetComponent<UIFrame>();
            if (targetCamFrame != null)
            {
                targetCamFrame.display = true;
            }
        }

        if (InstructionsObject != null)
            InstructionsObject.GetComponent<Instructions>().display = true;

        setupComplete = true;
    }

    Vector3 ChooseTargetPosition()
    {
        Vector2Int targetTilePosition = new Vector2Int(Random.Range(0, Grid.GridWidth), Random.Range(0, Grid.GridHeight));
        Vector2 worldPos = Grid.GetTileWorldCentre(targetTilePosition);
        return new Vector3(worldPos.x, worldPos.y, -1.0f);
    }
}
