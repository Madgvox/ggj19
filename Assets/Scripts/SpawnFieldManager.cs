using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpawnArea {
	public FlowField field;
	public float maxT;
	public float direction;
	public Vector3 point;
}

public class SpawnFieldManager : MonoBehaviour {
	public static SpawnFieldManager instance;

	public Transform target;
	public Camera camera;

	public Debris prefab;

	public Rect worldCameraRect;

	List<SpawnArea> aheadSpawns = new List<SpawnArea>();
	List<SpawnArea> behindSpawns = new List<SpawnArea>();

	float debrisTimer = 0f;

	private void Awake () {
		instance = this;
	}

	private void LateUpdate () {
		UpdateAreas();

		debrisTimer -= Time.deltaTime;

		if( debrisTimer < 0 ) {
			debrisTimer += Random.Range( 0.5f, 1f );
			foreach( var spawn in behindSpawns ) {
				SpawnDebris( spawn, 8f );
			}

			foreach( var spawn in aheadSpawns ) {
				SpawnDebris( spawn, 2f );
			}
		}

		foreach( var spawn in behindSpawns ) {
			Draw.Point( spawn.field.spline.GetPoint( spawn.maxT ) );
		}

		foreach( var spawn in aheadSpawns ) {
			Draw.Point( spawn.field.spline.GetPoint( spawn.maxT ), Color.green );
		}
	}

	private void SpawnDebris ( SpawnArea spawn, float destroyDelay ) {
		var obj = Instantiate( prefab );

		obj.passiveDeathTimer = destroyDelay;

		var pos = spawn.field.spline.GetPoint( spawn.maxT );
		var dir = Random.value > 0.5f ? 1 : -1;
		var offset = spawn.field.spline.GetNormal( spawn.maxT ) * spawn.field.GetWidth( spawn.maxT ) * 0.8f * dir * Random.value;

		obj.transform.position = pos + offset;
	}

	void UpdateAreas () {
		aheadSpawns.Clear();
		behindSpawns.Clear();

		worldCameraRect = new Rect();
		worldCameraRect.size = new Vector2( camera.orthographicSize * 2 * camera.aspect + 2f, camera.orthographicSize * 2 + 2f );
		worldCameraRect.center = camera.transform.position;

		foreach( var field in FlowManager.instance.allFlowFields ) {
			float nearestT;
			var nearestPt = field.spline.FindNearestPointTo( target.position, out nearestT, 100 );

			if( !worldCameraRect.Contains( nearestPt ) ) {
				continue;
			}

			var tan = field.spline.GetTangent( nearestT ).normalized;

			var dir = nearestPt - target.position;
			var dot = Vector3.Dot( tan, dir.normalized );

			var direction = 1;
			if( dot < 0 ) direction = -1;
			var area = RunField( field, nearestT, direction, worldCameraRect );

			if( area.field != null ) {
				if( direction > 0 ) {
					aheadSpawns.Add( area );
				} else {
					behindSpawns.Add( area );
				}
			}

			if( nearestT > 0 && nearestT < 1 ) {
				var area2 = RunField( field, nearestT, -direction, worldCameraRect );

				if( area2.field != null ) {
					if( -direction > 0 ) {
						aheadSpawns.Add( area2 );
					} else {
						behindSpawns.Add( area2 );
					}
				}
			}
		}

		//var initial = FlowManager.SampleFlowField( target.position, 100, false, false );

		//if( initial.field != null ) {
		//	beforeArea = RunField( initial.field, initial.t, -1 );
		//	//afterArea = RunFields( initial.field, initial.t, 1 );
		//}
	}

	SpawnArea RunField ( FlowField field, float t, float direction, Rect cameraRect ) {
		var stepSize = 1f;
		var maxSteps = 30f;

		var count = 0;

		//Debug.Log( direction );
		while( direction < 0 ? t > 0 : t < 1 ) {
			var pt = field.spline.MoveAlongSpline( ref t, stepSize * direction, 3 );
			//Draw.Point( pt );

			if( !cameraRect.Contains( pt ) ) {
				var t0 = pt + field.spline.GetNormal( t ) * field.GetWidth( t );
				var t1 = pt - field.spline.GetNormal( t ) * field.GetWidth( t );

				//Draw.Point( t0 );
				//Draw.Point( t1 );

				if( !( cameraRect.Contains( t0 ) || cameraRect.Contains( t1 ) ) ) {
					SpawnArea area = default;
					area.field = field;
					area.maxT = t;
					area.direction = direction;
					area.point = pt;
					return area;
				}
			}
			count += 1;
			if( count > maxSteps ) break;
		}

		return default;

		//while( !success && count < 5 ) {
		//	var innerCount = 0;
		//	var pt = lastPoint;

		//	var delta = field.spline.GetTangent( direction > 0 ? 1 : 0 ).normalized;

		//	if( direction < 0 ) delta *= -1;

		//	Draw.Ray( pt, delta, Color.green );

		//	var sample = FlowManager.SampleFlowField( pt + delta, 100, false, false, true );

		//	//Debug.Log( sample.point );
		//	Draw.Point( sample.point );

		//	if( sample.field == null ) {
		//		return default;
		//	}

		//	field = sample.field;
		//	t = sample.t;

		//	count += 1;
		//	if( count > 5 ) break;
		//}
	}
}
