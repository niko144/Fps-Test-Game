using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunsway : MonoBehaviour 
{
	public Transform root;


	public float speed = 3f;
	float x;
	float y;
	//float xpos;
	//float ypos;
	//float oldYpos;
	//float oldXpos;

	private float oldY;
	private float oldX;


	void Start () 
	{
		
		oldX = root.transform.eulerAngles.x;
		oldY = root.transform.eulerAngles.y;
		//oldXpos = root.transform.localPosition.x;
		//oldYpos = root.transform.localPosition.y;
	}


	void FixedUpdate () 
	{
		//x = root.transform.localRotation.x - oldrotation.x;

		//x *= root.transform.localRotation.w * rotationAmount;

		x = -(Mathf.DeltaAngle(root.transform.eulerAngles.x, oldX) /150f);
		y = -(Mathf.DeltaAngle(root.transform.eulerAngles.y, oldY) /150f);
		//xpos = (root.transform.localPosition.x - oldXpos) ;
		//ypos = (root.transform.localPosition.y - oldYpos);




		//Vector3 temppos = new Vector3 (xpos, ypos, transform.localPosition.z); 
		Quaternion temp = new Quaternion (x,y,-y,transform.localRotation.w);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, temp,Time.deltaTime * speed );
		//transform.localPosition = Vector3.Slerp (transform.localPosition, temppos, Time.deltaTime * speed);
		oldY = root.transform.eulerAngles.y;
		oldX = root.transform.eulerAngles.x;
		//oldXpos = root.transform.localPosition.x;
		//oldYpos = root.transform.localPosition.y;

	}




}
