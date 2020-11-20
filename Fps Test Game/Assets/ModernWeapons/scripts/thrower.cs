using UnityEngine;
using System.Collections;

public class thrower : MonoBehaviour {
	public float throwforce = 200.0f;
	public float ejectdelay = 0.3f;
	float lastLaunch;
	public GameObject projectile;
	public AudioClip throwSound;
	public AudioSource myAudioSource;

	public GameObject player;
	weaponselector inventory;
	Animation myanimation;
	void Awake()
	{
		
		myanimation = GetComponent<Animation>();

	}
	void throwstuff () 
	{
		if(player==null) player=GameObject.Find("playerController");
		if(inventory==null) inventory = player.GetComponent<weaponselector>();
		if (!myanimation.isPlaying && inventory.grenade>0)
		{
			inventory.grenade--;
			StartCoroutine(throwprojectile(ejectdelay));
			myAudioSource.clip = throwSound;
			myAudioSource.pitch = 0.9f + 0.1f *Random.value;
			myAudioSource.Play();
			myanimation.Play("throwing");
		}
	}

	IEnumerator throwprojectile(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		GameObject grenadeInstance = Instantiate(projectile,( transform.position+ Vector3.forward * 0.4f),transform.rotation) as GameObject;
		yield return null;
		grenadeInstance.GetComponent<Rigidbody>().AddRelativeForce(0f,throwforce/ 4f,throwforce);
		grenadeInstance.GetComponent<Rigidbody>().AddRelativeTorque(500,20,800);
		grenadeInstance.transform.localRotation = transform.localRotation * Quaternion.Euler(0,Random.Range(-90f,90f),0);
	}
}
