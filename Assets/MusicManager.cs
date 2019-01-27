using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    public AudioSource fastToSlow;
    // Start is called before the first frame update
    void Start()
    {
        fastToSlow.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
