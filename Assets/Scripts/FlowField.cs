using System;
using BezierSolution;
using UnityEngine;

[RequireComponent( typeof( BezierSpline ))]
public class FlowField : MonoBehaviour {
	public BezierSpline spline;
	
	[SerializeField]
	float strength;

	public float GetStrength () {
		return strength * FlowManager.GetPulseStrength();
	}

	internal float GetWidth ( float t ) {
		return spline.influenceWidth.Evaluate( t ) * spline.baseWidth;
	}
}
