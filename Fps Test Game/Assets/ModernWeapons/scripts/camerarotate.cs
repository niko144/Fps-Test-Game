using UnityEngine;
using System.Collections;

public class camerarotate : MonoBehaviour {

	private float  sensitivityY  = 6f;
	public float minimumY = -70f;
	public float maximumY  = 70f;
	private float rotationY = 0f;
	public float aimSens= 2f;
	public float normalSens= 6f;

	public float smooth = 0.5f;
	bool stop=false;

	float offsetY = 0f;

	float totalOffsetY = 0f;

	float resetSpeed = 1f;
	float resetDelay = 0f;

	float maxKickback = 0f;

	float smoothFactor = 2f;

	private Quaternion tRotation;

	private float idleSway = 0.01f;
	


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
			sensitivityY = aimSens;
			idleSway = 0f;
		}
		else
		{
			sensitivityY = normalSens;
			idleSway = 0.005f;
		}

		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

		float yDecrease = Mathf.Clamp(resetSpeed*Time.deltaTime, 0, totalOffsetY);

		if(resetDelay > 0)
		{
			yDecrease = 0;
			resetDelay = Mathf.Clamp(resetDelay-Time.deltaTime, 0, resetDelay);

		}

		if(totalOffsetY < maxKickback)
		{
			totalOffsetY += offsetY;

		} 
		else
		{
			offsetY = 0;

			resetDelay *= .5f;

		}

		rotationY = ClampAngle (rotationY, minimumY, maximumY)+ offsetY - yDecrease;

		if((Input.GetAxis("Mouse Y") * sensitivityY) < 0){

			totalOffsetY += Input.GetAxis("Mouse Y") * sensitivityY;

		}

		rotationY+=Mathf.Sin(Time.time)*idleSway;

		totalOffsetY -= yDecrease;


		if(totalOffsetY < 0) 
		{
			totalOffsetY = 0;
		}

		Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);

		tRotation = originalRotation * yQuaternion;

		float offsetVal = Mathf.Clamp(totalOffsetY*smoothFactor,1, smoothFactor);

		//temp here
		float tempfloat = Quaternion.Slerp(transform.localRotation,tRotation,Time.deltaTime*25/smoothFactor*offsetVal).eulerAngles.x;

		Vector3 tempVector = new Vector3(tempfloat,transform.localEulerAngles.y, transform.localEulerAngles.z);
		transform.localEulerAngles = tempVector;



	}


	public void dorecoil(float recoil)
	{
		rotationY += recoil * Time.deltaTime * 20f;
		
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
