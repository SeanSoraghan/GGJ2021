using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformController : MonoBehaviour
{
    float defaultZRotation = 0.0f;
    float targetReturnRotation = 0.0f;
    float inputZRotation = 0.0f;
    public float InputZRotation
    {
        private get { return inputZRotation; }
        set
        {
            inputZRotation = (int)value % 360;
            if (inputZRotation < 0)
                inputZRotation += 360;
        }
    }
    float rotationAnimationT = 0.0f;
    float rotationAnimationStartValue = 0.0f;

    private void Start()
    {
        defaultZRotation = gameObject.transform.rotation.z;    
    }

    public enum ControlState
    {
        NoRotation = 0,
        RotationByPlayer,
        RotatingBack
    }

    ControlState controlState = ControlState.NoRotation;
    public ControlState PlayerControlState
    {
        get { return controlState; }
        set
        { 
            controlState = value;
            if (controlState == ControlState.RotatingBack)
            {
                rotationAnimationStartValue = gameObject.transform.rotation.eulerAngles.z;
                targetReturnRotation = defaultZRotation;
                if (rotationAnimationStartValue > 180.0f)
                {
                    targetReturnRotation += 360.0f;
                }
                rotationAnimationT = 0.0f;
                InputZRotation = 0.0f;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (PlayerControlState)
        {
            case ControlState.NoRotation: break;
            case ControlState.RotationByPlayer:
                gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, InputZRotation);
                break;
            case ControlState.RotatingBack:
                if (gameObject.transform.rotation.eulerAngles.z != defaultZRotation)
                {
                    rotationAnimationT += Time.deltaTime;
                    float animCurveCounter = (Mathf.Pow(2.0f, rotationAnimationT * 4.0f) - 1.0f) / 15.0f;
                    float newZRot = Mathf.Lerp(rotationAnimationStartValue, targetReturnRotation, animCurveCounter);
                    Quaternion q = Quaternion.Euler(0.0f, 0.0f, newZRot);
                    gameObject.transform.rotation = q;
                    if (rotationAnimationT >= 1.0f)
                    {
                        gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, defaultZRotation);
                        PlayerControlState = ControlState.NoRotation;
                    }
                }
                else
                {
                    PlayerControlState = ControlState.NoRotation;
                }
                break;
            default: break;
        }
    }
}
