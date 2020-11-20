using UnityEngine;
using System.Collections;

public class shell : MonoBehaviour {
	public float  waitTime = 3f;
	public AudioSource myAudioSource;
	public AudioClip[] shellsounds ;
	
	

	void OnCollisionEnter( Collision collision) 
	{
        GetComponent<MeshRenderer>().enabled = false;
		if (!myAudioSource.isPlaying)
		{
			
			myAudioSource.clip = shellsounds[Random.Range(0,shellsounds.Length)];
			myAudioSource.Play();
            StartCoroutine(waitanddestroy(myAudioSource.clip.length));
        }
	}
    IEnumerator waitanddestroy (float waittime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject, waitTime);
    }

}
