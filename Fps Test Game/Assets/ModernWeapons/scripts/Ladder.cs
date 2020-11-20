using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {

	
	Quaternion myrotation;
	Vector3 direction;
   
    
    public Transform ladderTop;
	public Transform ladderBottom;

	Vector3 wantedLadderposition;
	float ControllerY;
    float middle;
	float lengthDiagonal;
	Vector3 delta;
	float lengthB;
	float wantedZ;
	float wantedX;
    bool checkgrounded;
    bool canclimb = false;
	void Start () 
	{
		myrotation = transform.rotation;
        //offset z
		direction = ladderTop.transform.position -  ladderBottom.transform.position;
		direction = direction.normalized;
        middle = ladderTop.transform.position.y - ladderBottom.transform.position.y;
        checkgrounded = false;

    }
    IEnumerator waitforgrounded()
    {
        
        yield return new WaitForSeconds(1f);
        checkgrounded = true;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            checkgrounded = false;
            canclimb = false;
            //check if controller is looking at ladder
            Vector3 forward = other.transform.TransformDirection(Vector3.right);
            Vector3 toOther = other.transform.position - transform.position;
            float angle = Vector3.Dot(forward, toOther);

            if (angle > -0.3 && angle < 0.3)
            {
                StartCoroutine(waitforgrounded());
                canclimb = true;
            }
               
            
        }
    }
    void OnTriggerStay (Collider other) 
	{

		if  (other.tag == "Player" && canclimb)
		{
			playercontroller controller = other.GetComponent<playercontroller>();
			ControllerY = other.transform.position.y;
           
           
            if (controller.GetComponent<CharacterController>().isGrounded && checkgrounded)
            {
                  controller.climbladder = false;
                    
            }
            else
            {
                  controller.climbladder = true;
                    
            }
                

           
           
           

            delta = ladderTop.position - ladderBottom.position;
			lengthDiagonal = Mathf.Pow((delta.x * delta.x) + (delta.z * delta.z), 0.5f);


			if (lengthDiagonal == 0f)
			{
				wantedZ = ladderBottom.position.z;
				wantedX = ladderBottom.position.x;
			}
			else
			{
				lengthB = lengthDiagonal * ((ControllerY - ladderBottom.position.y)/ (ladderTop.position.y - ladderBottom.position.y));
				wantedZ = ladderBottom.position.z + ((ladderTop.position.z - ladderBottom.position.z) * (lengthB / lengthDiagonal));
				wantedX = ladderBottom.position.x + ((ladderTop.position.x - ladderBottom.position.x) * (lengthB / lengthDiagonal));

			}


           
			wantedLadderposition = new Vector3(wantedX,ControllerY,wantedZ);
           
			controller.climbdirection = direction;
			
			controller.ladderposition = wantedLadderposition;
			controller.ladderforward = (-transform.forward);
			controller.ladderRotation = myrotation;

		}
	
		

		
	}
	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Player")
		{
			playercontroller controller = other.GetComponent<playercontroller>();

			controller.climbladder = false;
            canclimb = false;
        }
		
		
	}
}
