using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPointAnimator : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,
        PulseAttack,
        PulseRelease
    }

    public float PulseScaleProportion = 1.5f;

    bool pulseStarted = false;
    AnimationState animState = AnimationState.Idle;
    public AnimationState AnimState
    {
        get
        {
            return animState;
        }

        set
        {
            animState = value;
            if (animState == AnimationState.Idle)
            {
                gameObject.transform.localScale = defaultScale;
                if (pulseStarted)
                {
                    //pulse Ended
                }
                pulseStarted = false;
            }
            if (animState == AnimationState.PulseAttack)
            {
                pulseStarted = true;
                gameObject.transform.localScale = defaultScale;
                attackCurve.Reset();
            }
            else if (animState == AnimationState.PulseRelease)
            {
                gameObject.transform.localScale = defaultScale * PulseScaleProportion;
                releaseCurve.Reset();
            }
        }
    }

    AnimCurve attackCurve = new AnimCurve();
    AnimCurve releaseCurve = new AnimCurve();
    Vector3 defaultScale = Vector3.one;

    private void Awake()
    {
        attackCurve.currentMotionType = AnimCurve.MotionType.Logarithmic;
        attackCurve.animationTimeSeconds = 0.1f;

        releaseCurve.currentMotionType = AnimCurve.MotionType.Exponential;
        releaseCurve.animationTimeSeconds = 0.2f;

        defaultScale = gameObject.transform.localScale;
    }

    void Start()
    {
        attackCurve.Reset();
        releaseCurve.Reset();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (AnimState)
        {
            case AnimationState.Idle: break;
            case AnimationState.PulseAttack:
                bool attackComplete = attackCurve.UpdateCurve(Time.deltaTime);
                gameObject.transform.localScale = Vector3.Lerp(defaultScale, defaultScale * PulseScaleProportion, attackCurve.animCurveCounter);
                if (attackComplete)
                {
                    AnimState = AnimationState.PulseRelease;
                }
                break;
            case AnimationState.PulseRelease:
                bool releaseComplete = releaseCurve.UpdateCurve(Time.deltaTime);
                gameObject.transform.localScale = Vector3.Lerp(defaultScale * PulseScaleProportion, defaultScale, releaseCurve.animCurveCounter);
                if (releaseComplete)
                {
                    AnimState = AnimationState.Idle;
                }
                break;
        }
        
    }

    public void Pulse(float pulseScaleProportion)
    {
        PulseScaleProportion = pulseScaleProportion;
        AnimState = AnimationState.PulseAttack;
    }
}
