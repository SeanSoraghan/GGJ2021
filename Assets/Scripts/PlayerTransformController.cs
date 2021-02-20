using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformController : MonoBehaviour
{
    public enum AnimationState
    {
        NoAnimation,
        Rotating
    };

    public float rotationSpeedDegPerSec = 5.0f;
    float pointUpZRotation = 0.0f;
    float tiltLeftZRotation = 45.0f;
    float tiltRightZRotation = -45.0f;
    float targetRotation = 0.0f;
    float rotationAnimationT = 0.0f;
    float rotationAnimationStartValue = 0.0f;

    float shakeCentreRotation = 0.0f;
    float shakeAnimationT = 0.0f;
    [Range(10.0f, 25.0f)]
    float shakeArcDegrees = 15.0f;
    float shakeTarget = 0.0f;
    [Range(10.0f, 50.0f)]
    float shakeRotationSpeedDegPerSec = 20.0f;
    int shakeSegment = 0;
    float shakeSegmentStartRotation = 0.0f;
    [Range(3, 6)]
    public int numShakeSegments = 3;

    bool shaking = false;
    public bool Shaking
    { 
        get { return shaking; } 
        set 
        {
            shaking = value;
            if (shaking)
            {
                shakeSegment = 0;
                shakeAnimationT = 0.0f;
                shakeCentreRotation = gameObject.transform.rotation.eulerAngles.z;
                shakeSegmentStartRotation = shakeCentreRotation;
                shakeTarget = GetShakeTarget();
            }
            else
            {
                Quaternion q = Quaternion.Euler(0.0f, 0.0f, shakeCentreRotation);
                gameObject.transform.rotation = q;
            }
        } 
    }

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

    public void Shake()
    {
        Shaking = true;
    }
    
    float GetShakeTarget()
    {
        return shakeCentreRotation + shakeArcDegrees * ((shakeSegment % 2) * 2 - 1);
    }

    void FixedUpdate()
    {
        switch (PlayerAnimState)
        {
            case AnimationState.NoAnimation: break;
            case AnimationState.Rotating:
                if (/*gameObject.transform.rotation.eulerAngles.z*/shakeCentreRotation != targetRotation)
                {
                    rotationAnimationT += Time.deltaTime * rotationSpeedDegPerSec;
                    float animCurveCounter = (Mathf.Pow(2.0f, rotationAnimationT * 4.0f) - 1.0f) / 15.0f;
                    /*float newZRot*/
                    shakeCentreRotation = Mathf.Lerp(rotationAnimationStartValue, targetRotation, animCurveCounter);
                    Quaternion q = Quaternion.Euler(0.0f, 0.0f, /*newZRot*/shakeCentreRotation);
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

        if (Shaking)
        {
            shakeAnimationT += Time.deltaTime * shakeRotationSpeedDegPerSec;
            float animCurveCounter = (Mathf.Pow(2.0f, shakeAnimationT * 4.0f) - 1.0f) / 15.0f;
            float newZRot = Mathf.Lerp(shakeSegmentStartRotation, shakeTarget, animCurveCounter);
            Quaternion q = Quaternion.Euler(0.0f, 0.0f, newZRot);
            gameObject.transform.rotation = q;
            if (shakeAnimationT >= 1.0f)
            {
                ++shakeSegment;
                if (shakeSegment >= numShakeSegments)
                {
                    Shaking = false;
                }
                else
                {
                    shakeSegmentStartRotation = shakeTarget;
                    if (shakeSegment == numShakeSegments - 1)
                        shakeTarget = shakeCentreRotation;
                    else
                        shakeTarget = GetShakeTarget();
                    shakeAnimationT = 0.0f;
                }
            }
        }
    }
}
