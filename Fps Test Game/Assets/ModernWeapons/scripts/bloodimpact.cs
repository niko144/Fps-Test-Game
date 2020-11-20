using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloodimpact : MonoBehaviour {
   
    
    public AudioSource myaudio;
    public AudioClip clip;
    public float maxpitch = 0.8f;
    public float minpitch = 0.2f;
    
   
    public float waittodestroy = 4f;
    // Use this for initialization
    void Start()
    {
        
        
        myaudio.clip = clip;
        myaudio.pitch = Random.Range(minpitch, maxpitch);
        myaudio.Play();
        Destroy(gameObject, waittodestroy);
    }
   
   
}
