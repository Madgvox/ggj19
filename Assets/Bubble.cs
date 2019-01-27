﻿using System.Collections;
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
		var sin = ( Mathf.Sin( Time.time ) + 1 ) * 0.5f;
		var cos = ( Mathf.Cos( Time.time ) + 1 ) * 0.5f;
		transform.localScale = new Vector3( Mathf.Lerp( 0.8f, 1.2f, sin ) , Mathf.Lerp( 0.8f, 1.2f, cos ) );
	}

	private void OnTriggerEnter2D ( Collider2D collider ) {
		if( collider.CompareTag( "Player" ) ) {
			sys.transform.SetParent( null );
			sys.transform.localScale = Vector3.one;
			sys.Play();

			Destroy( sys.gameObject, 2 );
			Destroy( gameObject );
		}
	}
}
