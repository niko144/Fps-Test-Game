using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadierSlug : MonoBehaviour {
	
	public GameObject explosion;
	public float waitTime = 5.0f;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, waitTime);

	}
	
	void OnCollisionEnter(){

        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject); // destroy the projectile

	}
}
