using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	[SerializeField]
	Rigidbody2D rigidbody;

    public float baseSpeed = 10f;

    public float pulseRate = 1f;
    public float drag = 0.3f;
    public float flowDrag = 0.3f;
    public float maxSpeed = 10f;
    public float minSpeed = 1f;

    float pulseTimer = 0;

	public float maxRotateVelocity = 2f;
	float rotateVelocity;

	float flowStrengthTarget;

    Vector3 velocity;
    Vector3 flowVelocity;
    Vector3 flowVelTarget;
    Vector3 flowTargetVel;

	Vector3 movementDir;

	Vector3 initialDirection;

    // Start is called before the first frame update
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        var h = Input.GetAxisRaw( "Horizontal" );
        var v = Input.GetAxisRaw( "Vertical" );

		var wantsMove = h != 0 || v != 0;
		var hv = new Vector3( h, v ).normalized;

		if( pulseTimer > 0 ) {
			pulseTimer -= Time.deltaTime;
		}

		if( wantsMove ) {
			if( pulseTimer <= 0 ) {
				pulseTimer += pulseRate;

				velocity = hv * baseSpeed;
				movementDir = velocity.normalized;
				initialDirection = movementDir;
			} else {
				movementDir = hv;
				rotateVelocity = maxRotateVelocity;
			}
		} else {
			rotateVelocity -= drag * Time.deltaTime;
			if( rotateVelocity < 0 ) rotateVelocity = 0;
		}

		movementDir = movementDir.normalized * velocity.magnitude;

		var dirDot = Vector3.Dot( movementDir.normalized, initialDirection );
		var velDot = Vector3.Dot( velocity.normalized, initialDirection );

		if( dirDot < 0 ) {
			movementDir = Vector3.Lerp( movementDir, Vector3.zero, dirDot * -1 * 0.5f );
		}

		var f = ( velocity.magnitude / maxSpeed );
		var speedMod = Mathf.Lerp( 2, 1, f * f * f );
		velocity = Vector3.RotateTowards( velocity, movementDir, rotateVelocity * speedMod * Time.deltaTime, velocity.magnitude * 2 * Time.deltaTime );

		var mag = velocity.magnitude;
		mag -= drag * Time.deltaTime;
		if( mag < 0 ) mag = 0;
		velocity = velocity.normalized * mag;

		var flow = FlowManager.SampleFlowField( transform.position, 400 );
		var flowTarget = Vector3.zero;

		if( flow.field != null ) {
			flowTarget = flow.field.GetStrength( !wantsMove ) * flow.flowDir;
		}

		flowVelTarget = Vector3.SmoothDamp( flowVelTarget, flowTarget, ref flowTargetVel, 0.1f );
		flowVelocity = Vector3.RotateTowards( flowVelocity, flowVelTarget, 2f * Time.deltaTime, float.MaxValue );
		var flowMag = flowVelocity.magnitude;
		flowMag -= flowDrag * Time.deltaTime;
		if( flowMag < flowVelTarget.magnitude ) flowMag = flowVelTarget.magnitude;
		flowVelocity = flowVelocity.normalized * flowMag;

		Debug.DrawRay( transform.position, flowVelTarget, Color.red );

		//Debug.DrawRay( transform.position, velocity, Color.red );
		//Debug.DrawRay( transform.position, initialDirection, Color.blue );
		//Debug.DrawRay( transform.position, movementDir, Color.green );
	}

	private void FixedUpdate () {
		var newPos = (Vector3)rigidbody.position + velocity * Time.fixedDeltaTime + flowVelocity * Time.fixedDeltaTime;
		rigidbody.MovePosition( newPos );
	}


	ContactPoint2D[] contacts = new ContactPoint2D[ 1 ];

	private void OnCollisionEnter2D ( Collision2D collision ) {
		var count = collision.GetContacts( contacts );

		var normal = contacts[ 0 ].normalImpulse;
	}
}
