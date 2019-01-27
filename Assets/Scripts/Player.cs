using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField]
	SpriteRenderer sprite;

	[SerializeField]
	Rigidbody2D rigidbody;

	[SerializeField]
	AnimationCurve squishCurve;

    public float baseSpeed = 10f;

    public float pulseRate = 1f;
    public float drag = 0.3f;
    public float knockbackDrag = 0.3f;
    public float flowDrag = 0.3f;
    public float maxSpeed = 10f;
    public float minSpeed = 1f;
    public float stuckSpeed = 5f;

	public float damageTimer = 0f;
	public bool damageFlip = false;

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

	Vector3 initialScale;

	Vector3 knockbackVelocity;
	public float knockbackPower;

    //sounds
    public AudioSource playerHurt;

	bool stuck;

	float squishTimer;

    // Start is called before the first frame update
    void Start () {
        initialScale = sprite.transform.localScale;
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

				var sp = baseSpeed;
				if( stuck ) sp = stuckSpeed;
				velocity = hv * sp;
				movementDir = velocity.normalized;
				initialDirection = movementDir;
				squishTimer = 1;
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

		if( stuck && mag > stuckSpeed ) mag = stuckSpeed;
		velocity = velocity.normalized * mag;

		var mag2 = knockbackVelocity.magnitude;
		mag2 -= drag * Time.deltaTime;
		if( mag2 < 0 ) mag2 = 0;
		knockbackVelocity = knockbackVelocity.normalized * mag2;

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
		} else {
		}
		if( squishTimer > 0 ) {
			squishTimer -= Time.deltaTime;

			sprite.transform.localScale = new Vector3( initialScale.x,
				Mathf.Lerp( initialScale.y, initialScale.y * 0.75f, squishCurve.Evaluate( 1 - squishTimer ) ),
				initialScale.z );
		} else {
			sprite.transform.localScale = initialScale;
		}

		if( damageTimer > 0 ) {
			damageTimer -= Time.deltaTime;
			sprite.enabled = damageFlip;
			damageFlip = !damageFlip;
		} else {
			sprite.enabled = true;
			damageFlip = false;
		}
	}

	private void FixedUpdate () {
		var newPos = (Vector3)rigidbody.position + velocity * Time.fixedDeltaTime + flowVelocity * Time.fixedDeltaTime + knockbackVelocity * Time.fixedDeltaTime;
		rigidbody.MovePosition( newPos );
	}


	ContactPoint2D[] contacts = new ContactPoint2D[ 1 ];

	private void OnCollisionEnter2D ( Collision2D collision ) {
		var count = collision.GetContacts( contacts );

		var collider = collision.collider;

		var enemy = collider.GetComponent<Enemy>();

		if( enemy != null && damageTimer <= 0 ) {
			damageTimer = 1.5f;
			enemy.ForgetTarget();
			knockbackVelocity = collision.GetContact( 0 ).normal * knockbackPower;
			playerHurt.Play();
		}
	}

	private void LateUpdate () {
		stuck = false;
	}

	private void OnTriggerStay2D ( Collider2D collider ) {
		if( collider.CompareTag( "Goo" ) ) {
			stuck = true;
		}
	}
}
