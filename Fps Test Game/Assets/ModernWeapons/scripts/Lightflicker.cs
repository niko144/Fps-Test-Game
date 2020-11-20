using UnityEngine;
using System.Collections;

public class Lightflicker : MonoBehaviour {

	public float minFlickerIntensity = 3f;
	public float maxFlickerIntensity = 5f;

	private Light mylight;
	private float randomintensity;
	void Start()
	{
		randomintensity = (Random.Range (0.0f,6f));
	}
	void Update()
	{
		float noise = Mathf.PerlinNoise(randomintensity,Time.time);
		mylight = GetComponentInChildren<Light>();
		mylight.range = Mathf.Lerp(minFlickerIntensity,maxFlickerIntensity,noise);
	}





}
