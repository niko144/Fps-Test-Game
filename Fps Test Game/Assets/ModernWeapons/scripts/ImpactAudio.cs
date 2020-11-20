using UnityEngine;
using System.Collections;

public class ImpactAudio : MonoBehaviour {

	

	private AudioSource myaudio;
	public AudioClip impactSound;    	
	void Awake() {
		myaudio = GetComponent<AudioSource>();
	}
		
		
	void OnCollisionEnter(Collision collision) {

		// Play a sound if the colliding objects had a big impact.		
		if (collision.relativeVelocity.magnitude > .02)
			if (!myaudio.isPlaying && myaudio.clip != null)
		{
			
			myaudio.clip = impactSound;
			myaudio.pitch = 0.9f + 0.1f *Random.value;
			myaudio.PlayOneShot(myaudio.clip);
			
		}
			
		}
	}
