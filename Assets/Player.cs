using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float baseSpeed = 10f;

    public float pulseSpeed = 1f;
    public float drag = 0.3f;
    public float maxSpeed = 10f;
    public float minSpeed = 1f;

    float pulseTimer = 0;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        var h = Input.GetAxisRaw( "Horizontal" );
        var v = Input.GetAxisRaw( "Vertical" );

        var movement = new Vector3( h, v );

        movement.Normalize();

        pulseTimer -= Time.deltaTime;

        if( pulseTimer <= 0 && movement != Vector3.zero ) {
            pulseTimer = pulseSpeed;

            velocity += movement * baseSpeed;

            if( velocity.magnitude > maxSpeed ) {
                velocity = Vector3.ClampMagnitude( velocity, maxSpeed );
            }
        }

        var mag = velocity.magnitude;

        mag -= drag * Time.deltaTime;

        if( mag < minSpeed ) mag = minSpeed;

        velocity = velocity.normalized * mag;

        transform.position += velocity * Time.deltaTime;
    }
}
