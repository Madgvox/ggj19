using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flora : MonoBehaviour {
    [SerializeField]
    AnimationCurve curve;

	public float pulseStrength = 1;

    Vector3 initialScale;

    void Start () {
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update() {
        var scale = initialScale;
        scale.y = Mathf.Lerp( initialScale.y * 1.5f * pulseStrength, initialScale.y, curve.Evaluate( FlowManager.GetPulseStrengthNormalized() ) );
        transform.localScale = scale;
    }
}
