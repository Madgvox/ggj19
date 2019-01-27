using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour {
	[SerializeField]
	FlowObject obj;
	[SerializeField]
	Rigidbody2D rigidbody;
	[SerializeField]
	SpriteRenderer sprite;

	public Sprite[] sprites;

	bool activeInScene;

	public float passiveDeathTimer = 2f;

	bool stuck = false;

	private void Start () {
		obj.flowMass = Random.Range( 0.3f, 1.5f );

		sprite.sprite = sprites[ Random.Range( 0, sprites.Length ) ];

		rigidbody.AddTorque( Random.Range( -40, 40f ) );
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

		var color = sprite.color;
		color.a = Mathf.Lerp( 0, 1, obj.flowVelocity.magnitude / 0.4f );
		sprite.color = color;
	}

	private void FixedUpdate () {
		var flow = obj.flowVelocity;

		if( stuck ) flow *= 0.25f;
		var newPos = (Vector3)rigidbody.position + flow * Time.fixedDeltaTime;
		rigidbody.MovePosition( newPos );
	}

	private void OnTriggerEnter2D ( Collider2D collision ) {
		if( collision.CompareTag( "Goo" ) ) {
			stuck = true;
		}
	}

	private void OnTriggerExit2D ( Collider2D collision ) {
		if( collision.CompareTag( "Goo" ) ) {
			stuck = false;
		}
	}
}
