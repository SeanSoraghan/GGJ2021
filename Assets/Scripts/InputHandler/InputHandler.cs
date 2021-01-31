using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;

	public float TargetReachedThreshold = 0.5f;

	InputActionAsset inputActions;
	LevelController levelController;

	Vector2 pointerXY = Vector2.zero;

	private void OnDestroy()
	{
		InputActionMap actionMap = inputActions.actionMaps[0];
		actionMap.actions[0].performed -= OnUp;
		actionMap.actions[1].performed -= OnRight;
		actionMap.actions[2].performed -= OnFired;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}

		PlayerInput input = GetComponent<PlayerInput>();
		Assert.IsNotNull(input);
		if (input != null)
			inputActions = input.actions;
		Assert.IsNotNull(inputActions);

		levelController = GetComponent<LevelController>();
		Assert.IsNotNull(levelController);
	}

	void Start()
	{
		InputActionMap actionMap = inputActions.actionMaps[0];
		actionMap.actions[0].performed += OnUp;
		actionMap.actions[1].performed += OnRight;
		actionMap.actions[2].performed += OnFired;
	}

	public void OnUp(InputAction.CallbackContext context)
	{
		float pointerY = context.ReadValue<float>();
		pointerXY.y = pointerY;
		MovePlayerCamToCursor();
	}

	public void OnRight(InputAction.CallbackContext context)
	{
		float pointerX = context.ReadValue<float>();
		pointerXY.x = pointerX;
		MovePlayerCamToCursor();
	}

	public void OnFired(InputAction.CallbackContext context)
	{
		if (levelController.setupComplete)
		{
			Vector2 playerPos = levelController.PlayerCameraObject.transform.position;
			Vector2 targetPos = levelController.TargetCamObject.transform.position;
			float distFromTarget = (targetPos - playerPos).magnitude;
			if (distFromTarget < TargetReachedThreshold)
			{
				levelController.ClearLevel();
				levelController.CreateLevel();
			}
		}
	}

	void MovePlayerCamToCursor()
	{
		float playerZ = levelController.PlayerCameraObject.transform.position.z;
		Vector3 newPos = levelController.GridCameraObject.GetComponent<Camera>().ScreenToWorldPoint(pointerXY);
		levelController.PlayerCameraObject.transform.position = new Vector3(newPos.x, newPos.y, playerZ);
		
	}
}
