using UnityEngine;
using System.Collections;

public class playerrotate : MonoBehaviour {

	private float  sensitivityX  = 6f;
	public float minimumX = -360f;
	public float maximumX  = 360f;
	private float rotationX = 0f;
	public float aimSens= 2f;
	public float normalSens= 6f;
	public bool climbing = false;
	public float smooth = 0.5f;
	bool stop=false;

	float offsetX = 0f;

	float totalOffsetX = 0f;

	float resetSpeed = 1f;
	float resetDelay = 0f;

	float maxKickback = 0f;

	float smoothFactor = 2f;

	private Quaternion tRotation;
	public Vector3 ladderforward;


	private Quaternion originalRotation;
	
	private Quaternion[] temp;
	private Quaternion smoothRotation;
	

	void Start()
	{
		originalRotation = transform.localRotation;
	}
	void Update () {



			
		if(Input.GetKey(KeyCode.Escape) ){
			stop=!stop;
		}
		if(stop){
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible=true;
		}
		else{
			Cursor.lockState = CursorLockMode.Locked;
		}

		if (Input.GetButton("Aim"))
		{
			sensitivityX = aimSens;
		}
		else
		{
			sensitivityX = normalSens;
		}



		rotationX += Input.GetAxis("Mouse X") * sensitivityX;

		float xDecrease;

		if(totalOffsetX > 0){
			xDecrease = Mathf.Clamp(resetSpeed*Time.deltaTime, 0, totalOffsetX);
		} 
		else 
		{
				
			xDecrease = Mathf.Clamp(resetSpeed*-Time.deltaTime, totalOffsetX, 0);

		}

		if(resetDelay > 0)
		{

			xDecrease = 0;

			resetDelay = Mathf.Clamp(resetDelay-Time.deltaTime, 0, resetDelay);

		}

		if(Random.value < .5)
				offsetX *= -1;

		if((totalOffsetX < maxKickback && totalOffsetX >= 0) || (totalOffsetX > -maxKickback && totalOffsetX <= 0))
		{

			totalOffsetX += offsetX;

		} 
		else 
		{


			resetDelay *= .5f;

		}

		rotationX = ClampAngle (rotationX, minimumX, maximumX)+ offsetX - xDecrease;

		if((Input.GetAxis("Mouse X") * sensitivityX) < 0)
		{

			totalOffsetX += Input.GetAxis("Mouse X") * sensitivityX;

		}



		totalOffsetX -= xDecrease;

		if(totalOffsetX < 0) 
		{

			totalOffsetX = 0;
		}





		
		Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
		tRotation =  originalRotation * xQuaternion;
		



		float offsetVal = Mathf.Clamp(totalOffsetX * smoothFactor,1, smoothFactor);

		transform.localRotation=Quaternion.Slerp(transform.localRotation,tRotation,Time.deltaTime*25/smoothFactor*offsetVal);


	}




	float ClampAngle(float angle,float min, float max)
	{

		if (angle < -360F)

			angle += 360F;

		if (angle > 360F)

			angle -= 360F;

		return Mathf.Clamp (angle, min, max);

	}
}

