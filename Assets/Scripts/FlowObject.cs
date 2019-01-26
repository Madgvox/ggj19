using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Rigidbody2D ) )]
public class FlowObject : MonoBehaviour {

	public float flowMass = 1;
	public float flowQuality = 100;

	public float flowDrag = 0.3f;
	public Vector3 flowVelocity;
	Vector3 flowVelTarget;
	Vector3 flowTargetVel;

	private void Start () {

	}

	public void UpdateFlow () {
		var flow = FlowManager.SampleFlowField( transform.position, 100 );
		var flowTarget = Vector3.zero;

		if( flow.field != null ) {
			flowTarget = flow.field.GetStrength() * ( 1 / flowMass ) * flow.flowDir;
		}

		flowVelTarget = Vector3.SmoothDamp( flowVelTarget, flowTarget, ref flowTargetVel, 0.2f );
		flowVelocity = Vector3.RotateTowards( flowVelocity, flowVelTarget, 2f * Time.deltaTime, float.MaxValue );
		var flowMag = flowVelocity.magnitude;
		flowMag -= flowDrag * Time.deltaTime;
		if( flowMag < flowVelTarget.magnitude ) flowMag = flowVelTarget.magnitude;
		flowVelocity = flowVelocity.normalized * flowMag;
	}
}
