using UnityEngine;
using System.Collections;

public class grenade : MonoBehaviour {
	public Transform explosion;
    
	public float waitTime = 2.0f;
	
	void Start() {
		StartCoroutine (waitanddestroy());
	}
	
	void explode() 
	{

        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy (gameObject);
	}
	IEnumerator waitanddestroy ()
	{
		yield return new WaitForSeconds (waitTime);
		explode ();
	}
    private void OnCollisionEnter(Collision collision)
    {
                
        transform.SetParent(collision.collider.transform);
      
    }


}