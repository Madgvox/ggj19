using UnityEngine;

public struct FlowSample {
	public FlowField field;
	public float t;
	public float width;
	public float distance;
	public Vector3 tangent;
	public Vector3 flowDir;
	public Vector3 point;
}

public class FlowManager : MonoBehaviour {
	public static FlowManager instance;

	public float globalPulseDelay = 1f;

	float pulseTime = 0;
	float currentPulseStrength;
	float currentPulseStrengthNormal;
	public float globalPulseStrengthMax = 1f;
	public float globalPulseStrengthMin = 0.4f;

	public FlowField[] allFlowFields;

	void Awake () {
		instance = this;

		allFlowFields = FindObjectsOfType<FlowField>();
	}

	void Update () {
		pulseTime -= Time.deltaTime;

		if( pulseTime < 0 ) {
			pulseTime += globalPulseDelay;
		}

		var p = pulseTime / globalPulseDelay;

		currentPulseStrengthNormal = p;
		currentPulseStrength = Mathf.Lerp( globalPulseStrengthMin, globalPulseStrengthMax, p );
	}

	public static float GetPulseStrength () {
		return instance.currentPulseStrength;
	}

	public static float GetPulseStrengthNormalized () {
		return instance.currentPulseStrengthNormal;
	}

	public static FlowSample SampleFlowField ( Vector3 atPoint, float quality = 100, bool sampleFlow = true, bool limitDistance = true, bool limitDot = true ) {
		FlowSample sample = default;
		sample.distance = float.MaxValue;

		foreach( var field in instance.allFlowFields ) {
			float t;
			var pt = field.spline.FindNearestPointTo( atPoint, out t, quality );


			var dist = ( atPoint - pt ).magnitude;
			var influenceDist = field.GetWidth( t );

			if( limitDistance && dist > influenceDist ) continue;

			var dot = Vector3.Dot( ( atPoint - pt ).normalized, field.spline.GetNormal( t ) );

			if( limitDot && Mathf.Abs( dot ) < 0.9f && dist > 0.2f ) {
				continue;
			}

			if( dist < sample.distance ) {
				sample.distance = dist;
				sample.width = influenceDist;
				sample.field = field;
				sample.t = t;
				sample.tangent = field.spline.GetTangent( t ).normalized;
				sample.point = pt;
			}
		}

		if( sample.field != null && sampleFlow ) {
			var lengthRatio = 1 / sample.field.spline.Length;
			float nextT = sample.t;
			var nextPoint = sample.field.spline.MoveAlongSpline( ref nextT, 1, 3 );
			var dir = 1;
			if( nextT > 1 ) {
				nextT = sample.t;
				nextPoint = sample.field.spline.MoveAlongSpline( ref nextT, -1, 3 );
				dir = -1;
			} 

			var nextWidth = sample.field.GetWidth( nextT );
			var nextNormal = sample.field.spline.GetNormal( nextT );


			if( Vector3.Dot( nextNormal, atPoint - sample.point ) < 0 ) {
				nextNormal = -nextNormal;
			}

			var distRatio = sample.distance / sample.width;

			var nextOffset = nextWidth * distRatio;
			var nextOffsetPoint = nextPoint + nextNormal * nextOffset;

			sample.flowDir = ( nextOffsetPoint - atPoint ).normalized * dir;
		}

		return sample;
	}
}
