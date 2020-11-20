using UnityEngine;
using System.Collections;

public class gibs : MonoBehaviour {
	public AudioSource myaudio;
	public AudioClip[] dismemberSounds;

	public float radius = 3.0f;
	public float power = 100.0f;
	public float waittime = 6f;
	// Use this for initialization
	void Start () 
	{
		if (!myaudio.isPlaying)
		{
			int n = Random.Range(1,dismemberSounds.Length);
			myaudio.clip = dismemberSounds[n];
			myaudio.pitch = 0.9f + 0.1f *Random.value;
			myaudio.PlayOneShot(myaudio.clip);

			dismemberSounds[n] = dismemberSounds[0];
			dismemberSounds[0] = myaudio.clip;
		}
		StartCoroutine (addforces ());

	}

	// Update is called once per frame
	IEnumerator addforces()
	{
			
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) 
		{
			if (hit.GetComponent<Rigidbody>() != null)
			{
				Rigidbody rb = hit.GetComponent<Rigidbody>();
				rb.AddExplosionForce(power, explosionPos, radius, 3.0f);

			}
		}
		yield return new WaitForSeconds (waittime);

	}

}