using System;
using BezierSolution;
using UnityEngine;

[RequireComponent( typeof( BezierSpline ))]
public class FlowField : MonoBehaviour {
	public BezierSpline spline;
	
	[SerializeField]
	float strength;

	public bool pulsing = true;

	public float GetStrength ( bool pulsed = true ) {
		var str = strength;
		if( pulsed && pulsing ) str *= FlowManager.GetPulseStrength();
		return str;
	}

	internal float GetWidth ( float t ) {
		return spline.influenceWidth.Evaluate( t ) * spline.baseWidth;
	}
}
