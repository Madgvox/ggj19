using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {
	[SerializeField]
	ParticleSystem sys;

	Vector3 initialScale;

	private void Start () {
		initialScale = transform.localScale;
	}

	private void Update () {
		transform.localScale = new Vector3( Mathf.Sin( Time.time ), Mathf.Cos( Time.time ) );
	}

	private void OnTriggerEnter2D ( Collider2D collider ) {
		if( collider.CompareTag( "Player" ) ) {
			sys.transform.SetParent( null );
			sys.Play();

			Destroy( sys.gameObject, 2 );
			Destroy( gameObject );
		}
	}
}
