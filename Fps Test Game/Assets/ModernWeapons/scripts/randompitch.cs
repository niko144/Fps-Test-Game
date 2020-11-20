using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randompitch : MonoBehaviour {
    public AudioSource myaudio;
    public AudioClip clip;
   
	// Use this for initialization
	void Start ()
    {
       myaudio.clip = clip;
       myaudio.pitch = Random.Range(.9f, 1.1f);
       myaudio.Play();

    }
	
}
