using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;
	
	[Range(0.0f, 1.0f)]
	public float movementSensitivity = 1.0f;
	float maxSensitivity = 0.1f;
	float sensitivity { get { return movementSensitivity * maxSensitivity; } }

	public float TargetReachedThreshold = 0.5f;
	public float RotationReachedThreshold = 5.0f;

	static string[] actionNames =
	{
		"MoveUp", "MoveRight", "Fire", "FireReleased", "Left", "Right", "LeftReleased", "RightReleased"
	};

	List<Action<InputAction.CallbackContext>> actionBindings = new List<Action<InputAction.CallbackContext>>();

	InputActionAsset inputActions;
	LevelController levelController;
	PlayerTransformController playerController;

	Vector2 pointerXY = Vector2.zero;

	bool leftButtonState = false;
	bool rightButtonState = false;

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

		actionBindings.Add(OnMouseUp);
		actionBindings.Add(OnMouseRight);
		actionBindings.Add(OnFired);
		actionBindings.Add(OnFireReleased);
		actionBindings.Add(OnLeftPressed);
		actionBindings.Add(OnRightPressed);
		actionBindings.Add(OnLeftButtonReleased);
		actionBindings.Add(OnRightButtonReleased);

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
		for (int i = 0; i < actionNames.Length; ++i)
		{
			InputAction action = actionMap.FindAction(actionNames[i]);
			if (action != null)
				action.performed += actionBindings[i];
		}

		playerController = levelController.PlayerCameraObject.GetComponent<PlayerTransformController>();
		Assert.IsNotNull(playerController);
	}

	private void OnDestroy()
	{
		InputActionMap actionMap = inputActions.actionMaps[0];
		for (int i = 0; i < actionNames.Length; ++i)
		{
			InputAction action = actionMap.FindAction(actionNames[i]);
			if (action != null)
				action.performed -= actionBindings[i];
		}
	}

	public void OnMouseUp(InputAction.CallbackContext context)
	{
		//float pointerY = context.ReadValue<float>();
		//pointerXY.y = pointerY;
		//MovePlayerCamToCursor();
		float deltaY = context.ReadValue<float>() * sensitivity;
		MovePlayerCamDelta(new Vector3(0.0f, deltaY, 0.0f));
	}

	public void OnMouseRight(InputAction.CallbackContext context)
	{

		//float pointerX = context.ReadValue<float>();
		//pointerXY.x = pointerX;
		//MovePlayerCamToCursor();
		float deltaX = context.ReadValue<float>() * sensitivity;
		MovePlayerCamDelta(new Vector3(deltaX, 0.0f, 0.0f));
	}

	public void OnFired(InputAction.CallbackContext context)
	{
		if (levelController.setupComplete)
		{
			CheckWinState();
		}
	}

	public void OnFireReleased(InputAction.CallbackContext context)
	{

	}

	public void OnLeftPressed(InputAction.CallbackContext context)
	{
		leftButtonState = true;
		playerController?.TiltLeft();
	}

	public void OnRightPressed(InputAction.CallbackContext context)
	{
		rightButtonState = true;
		playerController?.TiltRight();
	}

	public void OnLeftButtonReleased(InputAction.CallbackContext context)
	{
		leftButtonState = false;
		if (rightButtonState)
			playerController?.TiltRight();
		else
			playerController?.PointUp();
	}

	public void OnRightButtonReleased(InputAction.CallbackContext context)
	{
		rightButtonState = false;
		if (leftButtonState)
			playerController?.TiltLeft();
		else
			playerController?.PointUp();
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
			playerController.PlayerAnimState = PlayerTransformController.AnimationState.NoAnimation;
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

	void MovePlayerCamDelta(Vector3 delta)
	{
		Vector3 newPos = levelController.PlayerCameraObject.transform.position + delta;
		Debug.Log(delta);
		levelController.PlayerCameraObject.transform.position = newPos;
	}
}
