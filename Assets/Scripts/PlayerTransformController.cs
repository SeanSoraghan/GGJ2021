using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerTransformController : MonoBehaviour
{
    public enum AnimationState
    {
        NoAnimation,
        Rotating,
        WinStateAnimation
    };

    public LevelController levelController;

    public float rotationSpeedDegPerSec = 5.0f;
    public float winRotationSpeedDegPerSec = 1.0f;
    float pointUpZRotation = 0.0f;
    float tiltLeftZRotation = 45.0f;
    float tiltRightZRotation = -45.0f;
    float targetRotation = 0.0f;
    float rotationAnimationT = 0.0f;
    float rotationAnimationStartValue = 0.0f;

    float postWinPauseSeconds = 0.5f;
    float postWinPauseT = 0.0f;

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
            AnimationState prevState = animState;
            animState = value;
            if (animState == AnimationState.NoAnimation)
            {
                if (prevState == AnimationState.WinStateAnimation)
                {
                    if (levelController.setupComplete)
                        levelController?.EraseAndCreateNewLevel();
                }
            }
            else if (animState == AnimationState.Rotating)
            {
                rotationAnimationStartValue = gameObject.transform.rotation.eulerAngles.z;
                if (rotationAnimationStartValue > 180.0f)
                {
                    targetRotation += 360.0f;
                }
                rotationAnimationT = 0.0f;
            }
            else if (animState == AnimationState.WinStateAnimation)
            {
                rotationAnimationStartValue = gameObject.transform.rotation.eulerAngles.z + 360.0f;
                rotationAnimationT = 0.0f;
                postWinPauseT = 0.0f;
            }
        }
    }

    public void PointUp(bool immediate = false)
    {
        targetRotation = pointUpZRotation;
        if (immediate)
        {
            Quaternion q = Quaternion.Euler(0.0f, 0.0f, targetRotation);
            gameObject.transform.rotation = q;
        }
        else
        {
            PlayerAnimState = AnimationState.Rotating;
        }
    }

    public void TiltRight(bool immediate = false)
    {
        targetRotation = tiltRightZRotation;
        if (immediate)
        {
            Quaternion q = Quaternion.Euler(0.0f, 0.0f, targetRotation);
            gameObject.transform.rotation = q;
        }
        else
        {
            PlayerAnimState = AnimationState.Rotating;
        }
    }

    public void TiltLeft(bool immediate = false)
    {
        targetRotation = tiltLeftZRotation;
        if (immediate)
        {
            Quaternion q = Quaternion.Euler(0.0f, 0.0f, targetRotation);
            gameObject.transform.rotation = q;
        }
        else
        {
            PlayerAnimState = AnimationState.Rotating;
        }
    }

    public void Shake()
    {
        Shaking = true;
    }
    
    public void Win()
    {
        targetRotation = gameObject.transform.rotation.eulerAngles.z;
        PlayerAnimState = AnimationState.WinStateAnimation;
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
                if (rotationAnimationT < 1.0f)
                {
                    rotationAnimationT += Time.deltaTime * rotationSpeedDegPerSec;
                    float animCurveCounter = (Mathf.Pow(2.0f, rotationAnimationT * 4.0f) - 1.0f) / 15.0f;
                    shakeCentreRotation = Mathf.Lerp(rotationAnimationStartValue, targetRotation, animCurveCounter);
                    Quaternion q = Quaternion.Euler(0.0f, 0.0f, shakeCentreRotation);
                    gameObject.transform.rotation = q;
                }
                else
                {
                    PlayerAnimState = AnimationState.NoAnimation;
                }
                break;
            case AnimationState.WinStateAnimation:
                if (rotationAnimationT < 1.0f)
                {
                    rotationAnimationT += Time.deltaTime * winRotationSpeedDegPerSec;
                    float animCurveCounter = Mathf.Log10(9.0f * rotationAnimationT + 1.0f);
                    float newZRot = Mathf.Lerp(rotationAnimationStartValue, targetRotation, animCurveCounter);
                    Quaternion q = Quaternion.Euler(0.0f, 0.0f, newZRot);
                    gameObject.transform.rotation = q;
                }
                else
                {
                    if (postWinPauseT < postWinPauseSeconds)
                        postWinPauseT += Time.deltaTime;
                    if (postWinPauseT >= postWinPauseSeconds)
                        PlayerAnimState = AnimationState.NoAnimation;
                }
                break;
            default: break;
        }

        if (Shaking)
        {
            Assert.AreNotEqual(PlayerAnimState, AnimationState.WinStateAnimation);
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
