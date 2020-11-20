using UnityEngine;
using System.Collections;

public class ParticleDamage : MonoBehaviour {
	public float damage = 10f;

	void OnParticleCollision (GameObject other ) {
		other.SendMessageUpwards("Damage", damage,SendMessageOptions.DontRequireReceiver);
	}
}
