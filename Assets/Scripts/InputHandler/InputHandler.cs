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
		Assert.IsTrue(actionMap.actions.Count == 2);
		actionMap.actions[0].performed -= OnUp;
		actionMap.actions[1].performed -= OnRight;
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
		Assert.IsTrue(actionMap.actions.Count == 2);
		actionMap.actions[0].performed += OnUp;
		actionMap.actions[1].performed += OnRight;
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

	void MovePlayerCamToCursor()
	{
		levelController.PlayerCameraObject.transform.position = levelController.MainCameraObject.GetComponent<Camera>().ScreenToWorldPoint(pointerXY);
		Vector2 playerPos = levelController.PlayerCameraObject.transform.position;
		Vector2 targetPos = levelController.TargetCamObject.transform.position;
		float distFromTarget = (targetPos - playerPos).magnitude;
		if (distFromTarget < TargetReachedThreshold)
		{
			levelController.CreateLevel();
		}
	}
}
