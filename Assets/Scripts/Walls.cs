using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Walls : MonoBehaviour {
	public SpriteShapeController shape;

	public AnimationCurve curve;

	public bool flipPulse;


	Vector3[] positions;


	private void Start () {
		shape.autoUpdateCollider = false;
		var spline = shape.spline;

		var count = spline.GetPointCount();
		positions = new Vector3[ count ];

		for( int i = 0; i < count; i++ ) {
			var pt = spline.GetPosition( i );
			positions[ i ] = pt;
		}
	}

	private void Update () {
		var spline = shape.spline;

		var count = spline.GetPointCount();

		for( int i = 0; i < count; i++ ) {
			var pt = positions[ i ];
			var tan = spline.GetRightTangent( i );
			var norm = Vector3.Cross( tan, Vector3.forward );

			var dir = flipPulse ? -1 : 1;
			pt += norm * curve.Evaluate( 1 - FlowManager.GetPulseStrengthNormalized() ) * 0.1f * dir;

			spline.SetPosition( i, pt );
		}
	}
}
