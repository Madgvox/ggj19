using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	[SerializeField]
	FlowObject obj;

	[SerializeField]
	SpriteRenderer sprite;

	[SerializeField]
	Rigidbody2D rigidbody;

	[SerializeField]
	AnimationCurve squishCurve;

	public float movementSpeed;

	Transform target;

	bool chasing;

	Vector3 initialScale;

	Vector3 velocity;

	float idleTimer;

	float chaseTimer;

	private void Start () {
		initialScale = sprite.transform.localScale;
	}

	void Update () {
		obj.UpdateFlow();

		if( chasing ) {
			if( target ) {
				var dir = target.position - transform.position;

				if( dir.magnitude > 8 ) {
					target = null;
					chasing = false;
				}

				velocity = dir.normalized * movementSpeed;
			} else {
				chasing = false;
				target = null;
			}
		} else {
			idleTimer -= Time.deltaTime;

			if( idleTimer < 0 ) {
				idleTimer += UnityEngine.Random.Range( 2, 5 );

				var targetPos = transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 5f;

				var dir = targetPos - transform.position;
				velocity = dir.normalized * movementSpeed;
			}
		}

		var mag = velocity.magnitude;
		mag -= 1f * Time.deltaTime;
		if( mag < 0 ) mag = 0;
		velocity = velocity.normalized * mag;

		if( velocity.magnitude > 0.001f ) {

			if( Vector3.Dot( velocity.normalized, Vector3.right ) > 0 ) {
				sprite.flipY = true;
			} else {
				sprite.flipY = false;
			}

			var rotation = Quaternion.LookRotation( velocity, Vector3.forward );
			rotation *= Quaternion.AngleAxis( 90, Vector3.right );
			rotation *= Quaternion.AngleAxis( 90, Vector3.forward );
			sprite.transform.rotation = rotation;
		}

		if( chasing ) {
			sprite.transform.localScale = new Vector3( initialScale.x,
				Mathf.Lerp( initialScale.y * 0.75f, initialScale.y, ( Mathf.Sin( Time.time * 8 ) + 1 ) * 0.5f ),
				initialScale.z );
		} else {
			sprite.transform.localScale = new Vector3( initialScale.x,
				Mathf.Lerp( initialScale.y * 0.75f, initialScale.y, squishCurve.Evaluate( 1 - ( velocity.magnitude / movementSpeed ) ) ),
				initialScale.z );
		}
	}

	private void FixedUpdate () {
		var newPos = (Vector3)rigidbody.position + velocity * Time.fixedDeltaTime + obj.flowVelocity * Time.fixedDeltaTime;
		rigidbody.MovePosition( newPos );
	}

	private void OnTriggerStay2D ( Collider2D collider ) {
		if( !chasing ) {
			if( chaseTimer <= 0 ) {
				if( collider.CompareTag( "Player" ) ) {
					chasing = true;
					target = collider.transform;
				}
			} else {
				chaseTimer -= Time.deltaTime;
			}
		}
	}

	internal void ForgetTarget () {
		chasing = false;
		target = null;
		chaseTimer = 4f;
	}
}
