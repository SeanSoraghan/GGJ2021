using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;

	public float TargetReachedThreshold = 0.5f;
	public float RotationReachedThreshold = 5.0f;

	InputActionAsset inputActions;
	LevelController levelController;

	Vector2 pointerXY = Vector2.zero;

	private void OnDestroy()
	{
		InputActionMap actionMap = inputActions.actionMaps[0];
		actionMap.actions[0].performed -= OnUp;
		actionMap.actions[1].performed -= OnRight;
		actionMap.actions[2].performed -= OnFired;
		actionMap.actions[3].performed -= OnFireReleased;
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
		actionMap.actions[3].performed += OnFireReleased;
	}

	public void OnUp(InputAction.CallbackContext context)
	{
		PlayerTransformController controller = levelController.PlayerCameraObject.GetComponent<PlayerTransformController>();
		if (controller != null)
		{
			if (controller.PlayerControlState == PlayerTransformController.ControlState.NoRotation)
			{
				float pointerY = context.ReadValue<float>();
				pointerXY.y = pointerY;
				MovePlayerCamToCursor();
			}
		}
	}

	public void OnRight(InputAction.CallbackContext context)
	{
		PlayerTransformController controller = levelController.PlayerCameraObject.GetComponent<PlayerTransformController>();
		if (controller != null)
		{
			if (controller.PlayerControlState == PlayerTransformController.ControlState.NoRotation)
			{
				float pointerX = context.ReadValue<float>();
				pointerXY.x = pointerX;
				MovePlayerCamToCursor();
			}
			else if (controller.PlayerControlState == PlayerTransformController.ControlState.RotationByPlayer)
			{
				float pointerX = context.ReadValue<float>();
				controller.InputZRotation = pointerXY.x - pointerX;
				CheckWinState();
			}
		}
	}

	public void OnFired(InputAction.CallbackContext context)
	{
		if (levelController.setupComplete)
		{
			if (!CheckWinState())
			{
				PlayerTransformController controller = levelController.PlayerCameraObject.GetComponent<PlayerTransformController>();
				if (controller != null)
				{
					controller.PlayerControlState = PlayerTransformController.ControlState.RotationByPlayer;
				}
			}
		}
	}

	public void OnFireReleased(InputAction.CallbackContext context)
	{
		if (levelController.setupComplete)
		{
			PlayerTransformController controller = levelController.PlayerCameraObject.GetComponent<PlayerTransformController>();
			if (controller != null)
			{
				controller.PlayerControlState = PlayerTransformController.ControlState.RotatingBack;
			}
		}
	}

	bool CheckWinState()
	{
		Vector2 playerPos = levelController.PlayerCameraObject.transform.position;
		Vector2 targetPos = levelController.TargetCamObject.transform.position;
		float playerRotZ = levelController.PlayerCameraObject.transform.rotation.eulerAngles.z;
		if (playerRotZ < 0.0f)
			playerRotZ += 360.0f;
		float targetRotZ = levelController.TargetCamObject.transform.rotation.eulerAngles.z;
		if (targetRotZ < 0.0f)
			targetRotZ += 360.0f;
		float distFromTarget = (targetPos - playerPos).magnitude;
		float rotDist = Mathf.Abs(playerRotZ - targetRotZ);
		if (distFromTarget < TargetReachedThreshold && rotDist < RotationReachedThreshold)
		{
			levelController.ClearLevel();
			levelController.CreateLevel();
			return true;
		}
		return false;
	}

	void MovePlayerCamToCursor()
	{
		float playerZ = levelController.PlayerCameraObject.transform.position.z;
		Vector3 newPos = levelController.GridCameraObject.GetComponent<Camera>().ScreenToWorldPoint(pointerXY);
		levelController.PlayerCameraObject.transform.position = new Vector3(newPos.x, newPos.y, playerZ);
	}
}
