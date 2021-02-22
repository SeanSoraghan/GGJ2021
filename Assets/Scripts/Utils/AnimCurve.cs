using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCurve
{
	public enum MotionType
	{
		Linear,
		Exponential,
		Logarithmic
	}

	public MotionType currentMotionType { private get; set; } = MotionType.Linear;

	float animationSpeed = 0.0f;
	float _animationTimeSeconds;
	public float animationTimeSeconds
	{
		private get { return _animationTimeSeconds; }
		set
		{
			_animationTimeSeconds = value;
			animationSpeed = _animationTimeSeconds == 0.0f ? 0.0f : 1.0f / _animationTimeSeconds;
		}
	}
	public float portionThisFrame { get; private set; } = 0.0f;
	public float animLinearCounter { get; private set; } = 0.0f;
	public float animCurveCounter { get; private set; } = 0.0f;
	float prevAnimCurveCounter = 0.0f;

	public void Reset()
	{
		animLinearCounter = 0.0f;
		prevAnimCurveCounter = 0.0f;
	}

	// Update is called once per frame
	public bool UpdateCurve(float deltaTime)
	{
		animLinearCounter += Time.deltaTime * animationSpeed;
		animCurveCounter = animLinearCounter;
		if (currentMotionType == MotionType.Exponential)
			animCurveCounter = (Mathf.Pow(2.0f, animLinearCounter * 4.0f) - 1.0f) / 15.0f;
		else if (currentMotionType == MotionType.Logarithmic)
			animCurveCounter = Mathf.Log10(9.0f * animLinearCounter + 1.0f);

		float remaining = 1.0f - prevAnimCurveCounter;
		portionThisFrame = (animCurveCounter - prevAnimCurveCounter) / remaining;

		prevAnimCurveCounter = animCurveCounter;

		return animLinearCounter >= 1.0f;
	}
}
