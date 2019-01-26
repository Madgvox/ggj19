using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour {
	[SerializeField]
	FlowObject obj;
	[SerializeField]
	Rigidbody2D rigidbody;

	bool activeInScene;

	float passiveDeathTimer = 2f;

	private void Start () {
		obj.flowMass = Random.Range( 0.3f, 1.5f );
	}

	private void Update () {
		obj.UpdateFlow();

		var onScreen = SpawnFieldManager.instance.worldCameraRect.Contains( transform.position );

		if( onScreen ) {
			activeInScene = true;
		}

		if( !onScreen && !activeInScene ) {
			passiveDeathTimer -= Time.deltaTime;
		}

		if( obj.flowVelocity == Vector3.zero || 
			( !onScreen && activeInScene ) || 
			passiveDeathTimer < 0 ) {

			Destroy( gameObject );
		}
	}

	private void FixedUpdate () {
		var newPos = (Vector3)rigidbody.position + obj.flowVelocity * Time.fixedDeltaTime;
		rigidbody.MovePosition( newPos );
	}
}
