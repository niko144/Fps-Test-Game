using UnityEngine;
using System.Collections;

public class explosionForce : MonoBehaviour {
	public float radius = 5.0f;
	public float power = 200.0f;
	public float waitTime = 5.0f;
	public float damage = 150f;
	private AudioSource myaudio;
	public AudioClip explodeSound; 
	
	
	
	void Start () {

		
		myaudio = GetComponent<AudioSource>();

		Destroy (gameObject, 10f);

		myaudio.clip = explodeSound;
		myaudio.pitch = 0.8f + 0.2f *Random.value;
		myaudio.PlayOneShot(myaudio.clip);
		StartCoroutine (explodeforce (waitTime));

	
	}
	IEnumerator explodeforce(float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		Vector3 explosionPos = transform.position;
		//Instantiate(debris, transform.position + new Vector3(0f,1f,0f), transform.rotation);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);


		foreach (Collider hit in colliders) 
		{
            float proximity = (explosionPos - hit.transform.position).magnitude;
            float effect = 1 - (proximity / radius);
            hit.transform.SendMessageUpwards("Damage",Mathf.Abs(damage * effect), SendMessageOptions.DontRequireReceiver);
            if (hit.GetComponent<Rigidbody>() != null)
			{
				Rigidbody rb = hit.GetComponent<Rigidbody>();
				rb.AddExplosionForce(power, explosionPos, radius, 3.0f);

			}
           
            
		}
	}

	

}
