using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformController : MonoBehaviour
{
    public enum AnimationState
    {
        NoAnimation,
        Shaking,
        Rotating
    };

    public float rotationSpeedDegPerSec = 5.0f;
    float pointUpZRotation = 0.0f;
    float tiltLeftZRotation = 45.0f;
    float tiltRightZRotation = -45.0f;
    float targetRotation = 0.0f;
    float rotationAnimationT = 0.0f;
    float rotationAnimationStartValue = 0.0f;

    AnimationState animState = AnimationState.NoAnimation;
    public AnimationState PlayerAnimState
    {
        get { return animState; }
        set
        {
            animState = value;
            if (animState == AnimationState.Rotating)
            {
                rotationAnimationStartValue = gameObject.transform.rotation.eulerAngles.z;
                if (rotationAnimationStartValue > 180.0f)
                {
                    targetRotation += 360.0f;
                }
                rotationAnimationT = 0.0f;
            }
        }
    }

    public void PointUp()
    {
        targetRotation = pointUpZRotation;
        PlayerAnimState = AnimationState.Rotating;
    }

    public void TiltRight()
    {
        targetRotation = tiltRightZRotation;
        PlayerAnimState = AnimationState.Rotating;
    }

    public void TiltLeft()
    {
        targetRotation = tiltLeftZRotation;
        PlayerAnimState = AnimationState.Rotating;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (PlayerAnimState)
        {
            case AnimationState.NoAnimation: break;
            case AnimationState.Shaking: break;
            case AnimationState.Rotating:
                if (gameObject.transform.rotation.eulerAngles.z != targetRotation)
                {
                    rotationAnimationT += Time.deltaTime * rotationSpeedDegPerSec;
                    float animCurveCounter = (Mathf.Pow(2.0f, rotationAnimationT * 4.0f) - 1.0f) / 15.0f;
                    float newZRot = Mathf.Lerp(rotationAnimationStartValue, targetRotation, animCurveCounter);
                    Quaternion q = Quaternion.Euler(0.0f, 0.0f, newZRot);
                    gameObject.transform.rotation = q;
                    if (rotationAnimationT >= 1.0f)
                    {
                        PlayerAnimState = AnimationState.NoAnimation;
                    }
                }
                else
                {
                    PlayerAnimState = AnimationState.NoAnimation;
                }
                break;
            default: break;
        }
    }
}
