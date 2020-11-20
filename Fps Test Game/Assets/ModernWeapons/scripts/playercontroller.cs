using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class playercontroller : MonoBehaviour {

    
    public Transform mycamera;
	private Transform reference;

	public float jumpHeight = 2.0f;
	public float jumpinterval = 1.5f;
	private float nextjump = 1.2f;
	public float maxhitpoints = 100f;
	public float hitpoints = 100f;
    private float oldhitpoints;
    private float blendhud =0f;
    public Image painhud;
	public Text healthtext;
    public GameObject hearticon;
    public Color normalUiColor;
    public Color damageUiColor;
    private Color UiColor;
	public AudioClip[] hurtsounds;
	
	

	
	//float recoverSpeed = 1;//HitPoints per second
	public Transform recoilCamera;
	public float inputsmoothspeed = 1f;
	public float gravity = 20.0f;
	public float rotatespeed = 4.0f;
	private float speed;
	public float normalspeed = 4.0f;
	public float runspeed = 8.0f;
	public float crouchspeed = 1.0f;
	public float crouchHeight = 1;
	private bool crouching = false;
	public float normalHeight = 2.0f;
	public float camerahighposition = 1.75f;
	public float cameralowposition = 0.9f;
	private float cameranewpositionY;
	private Vector3 cameranewposition;
	private float cameranextposition;
	public float dampTime = 2.0f;



	private float moveAmount;
	public float smoothSpeed = 2.0f;

	private Vector3 forward = Vector3.forward;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 right;

	private float movespeed;
	public Vector3 localvelocity;


	public bool climbladder = false;
	public Quaternion ladderRotation;
	public Vector3 ladderposition;
	public Vector3 ladderforward;
	public Vector3 climbdirection;




	public float climbspeed = 2.0f;


	public bool canclimb = false;
	private Vector3 addVector = Vector3.zero;


	public bool running = false;
	bool canrun = true;

	public AudioSource myAudioSource;
	
	
	
	Vector3 targetDirection = Vector3.zero;
	//public Transform playermesh;
	//public Vector3 playermeshNormalPosition;
	//public Vector3 playermeshForwardPosition;
	//public Transform playerskinnedmesh;
	private bool canrun2 = true;
	//bool hideselectedweapon = false;
	Vector3 targetVelocity;
	public float falldamage;
	private float airTime;
	public float falltreshold = 2f;
	private bool prevGrounded;
	public Transform Deadplayer;
	public Transform weaponroot;
	public Transform head;
	//bool grounded;



	Animator weaponanimator;
	Animator headanimator;
	public LayerMask mask;
	CharacterController controller;
	playerrotate rotatescript;
	weaponselector inventory;
   
	
    private Animation heartanim;

    private float wantedspeed;
    
	void Awake ()
	{
		reference = new GameObject().transform;
		weaponanimator = weaponroot.GetComponent<Animator> ();
		headanimator = head.GetComponent<Animator> ();
		controller = GetComponent<CharacterController>();
		rotatescript = GetComponent<playerrotate>();
		inventory = GetComponent<weaponselector>();
        
        heartanim = hearticon.GetComponent<Animation>();
    }



	void Start () 
	{
		speed = normalspeed;
        UiColor = normalUiColor;
        healthtext.color = UiColor;
        hearticon.GetComponent<Image>().color = UiColor;
        cameranextposition = camerahighposition;
        oldhitpoints = hitpoints;
    }
	

	void Update () 
	{


      

        //Animator meshanimator = playermesh.GetComponent<Animator>();

        reference.eulerAngles = new Vector3(0, mycamera.eulerAngles.y, 0);
		forward = reference.forward;
		right = new Vector3(forward.z, 0, -forward.x);


		float hor = Input.GetAxisRaw("Horizontal");
		float ver = Input.GetAxisRaw("Vertical");



		Vector3 velocity = controller.velocity;
		localvelocity = transform.InverseTransformDirection(velocity);

		bool ismovingforward =localvelocity.z > .5f;



		if (climbladder) 
		{

           
            if (crouching)
            {
                crouching = !crouching;
            }
			
            airTime = 0f;


			crouching = false;

            
           
           
			Vector3 wantedPosition = (ladderposition - transform.position);

			if( wantedPosition.magnitude > 0.2f )
			{
				addVector = wantedPosition.normalized;
                
			}
			else
			{
				addVector = Vector3.zero;
			}
           
            
           
            if (mycamera.transform.localRotation.w  > 0)
            {

                targetDirection = (-ver * climbdirection);

            }
             else
            {

                targetDirection = (ver * climbdirection);

            }
            targetDirection = targetDirection.normalized;
            targetDirection += addVector;

            moveDirection = targetDirection * climbspeed;
            headanimator.SetBool("jump", false);
            headanimator.SetBool("grounded", true);
            headanimator.SetFloat("speed", (localvelocity.magnitude * 2.5f), dampTime, .8f);

        } 
		else 
		{

			
            //playermesh.transform.localPosition = Vector3.MoveTowards(playermesh.transform.localPosition,playermeshNormalPosition, Time.deltaTime * 2f);
            //meshanimator.SetBool ("climbladder", false);
            //playerskinnedmesh.GetComponent<Renderer>().material.SetFloat("_cutoff", 1f);
           

			targetDirection = (hor * right) + (ver * forward);
			targetDirection = targetDirection.normalized;
			targetVelocity = targetDirection;
			if (controller.isGrounded) 
			{
				//grounded = true;
				airTime = 0f;
				
				if(Input.GetButtonDown("Crouch")) 
				{ 
					crouchcheck ();

				}
				if (!crouching)
				{   
					//meshanimator.SetBool ("crouch", false);
					canrun = true;
					controller.center = new Vector3(0f,normalHeight / 2f,0f);
					controller.height = normalHeight;
					canrun2 = true;
					cameranextposition = camerahighposition;
					

				}	
				else if (crouching )
				{
					//meshanimator.SetBool ("crouch", true);
					canrun = false;
					controller.center = new Vector3(0f,crouchHeight / 2f,0f);
					controller.height = crouchHeight;
					canrun2 = false;
					cameranextposition = cameralowposition;
					

				}
				// Jump
				if (Input.GetButton ("Jump") && Time.time > nextjump)
				{
					nextjump = Time.time + jumpinterval;
					moveDirection.y = jumpHeight;

					headanimator.SetBool("jump",true);
					weaponanimator.SetBool("jump",true);
					//meshanimator.SetBool ("jump", true);

					if (crouching)
					{
						crouchcheck ();
					}
				} 
				else 
				{
					weaponanimator.SetBool("jump",false);
					headanimator.SetBool("jump",false);

					//meshanimator.SetBool ("jump", false);
					
				}  

					
			}
				

			
			else 
			{
				
				airTime += Time.deltaTime;
				moveDirection.y -= (gravity) * Time.deltaTime;
				nextjump = Time.time + jumpinterval;
				
				//grounded = false;
			}

			if (Input.GetButton ("Fire2") && canrun && canrun2 && ismovingforward) 
			{
				speed = runspeed;
				running = true;
				
			}
			else if(crouching)
			{
				speed = crouchspeed;
				running = false;
			}
			else
			{
				speed = normalspeed;
				running = false;
				
				
			}
          

            wantedspeed = Mathf.Lerp(wantedspeed, speed, 2f * Time.deltaTime);
			targetVelocity *= wantedspeed;
			moveDirection.z = Mathf.Lerp (moveDirection.z, targetVelocity.z, inputsmoothspeed);
				
			moveDirection.x =  Mathf.Lerp (moveDirection.x, targetVelocity.x, inputsmoothspeed);
            weaponanimator.SetBool("grounded", controller.isGrounded);
            weaponanimator.SetFloat("speed", (localvelocity.magnitude), dampTime, .8f);
            headanimator.SetBool("grounded", controller.isGrounded);
            headanimator.SetFloat("speed", (localvelocity.magnitude), dampTime, .8f);


        }
        



        cameranewpositionY = Mathf.Lerp(Camera.main.transform.localPosition.y,cameranextposition, Time.deltaTime * 4f);

			
		//meshanimator.SetBool ("grounded", controller.isGrounded);
						
		
		//meshanimator.SetFloat("hor",(localvelocity.x/speed) + (Input.GetAxis("Mouse X") /3f), dampTime , 0.2f);
		//meshanimator.SetFloat("ver",(localvelocity.z/ speed), dampTime , 0.8f);


		cameranewposition = new Vector3(Camera.main.transform.localPosition.x,cameranewpositionY,Camera.main.transform.localPosition.z);
		Camera.main.transform.localPosition = cameranewposition;


		controller.Move (moveDirection * Time.deltaTime);
		if (!prevGrounded && controller.isGrounded )
		{
			
			
			if (airTime > falltreshold)
			{
				Damage(falldamage * airTime * 2f);
                
			}

							
		}
		
		
		prevGrounded = controller.isGrounded;	
		
		
		string healthstring = (Mathf.Round(hitpoints)).ToString();
		healthtext.text= (healthstring);
        healthtext.color = Color.Lerp(UiColor, damageUiColor, 1f * (maxhitpoints - hitpoints) / maxhitpoints);
        hearticon.GetComponent<Image>().color = Color.Lerp(UiColor, damageUiColor, 1f * (maxhitpoints - hitpoints) / maxhitpoints);
        
       
        heartanim["heartpump"].speed = 1f + (2f * ((maxhitpoints - hitpoints) / maxhitpoints));
        if (oldhitpoints > hitpoints)
        {
            blendhud = 0.6f;


        }
        else if (hitpoints <= 0)
        {
            //die
            blendhud = 1f;
            inventory.disable();
            Instantiate(Deadplayer, transform.position, transform.rotation);
          
            Destroy(gameObject);
        }
        else if (blendhud > 0f)
        {
            blendhud -= 2f * Time.deltaTime;

        }
        Color tempcolor = painhud.color;
        tempcolor.a = blendhud;
        painhud.color = tempcolor;
        oldhitpoints = hitpoints;
        inventory.hideweapons = climbladder;
        inventory.canswitch = !climbladder;

    }
	void Damage (float damage)
	{


       

		hitpoints -= damage;
        hitpoints = Mathf.Clamp(hitpoints, 0f, maxhitpoints);
		camerarotate cameracontroller = recoilCamera.GetComponent<camerarotate> ();


		cameracontroller.SendMessage ("dorecoil", damage / 3f, SendMessageOptions.DontRequireReceiver);
		if (!myAudioSource.isPlaying && hitpoints >= 0) {

			int n = Random.Range (1, hurtsounds.Length);
			myAudioSource.clip = hurtsounds [n];
            myAudioSource.pitch = Random.Range(0.9f, 1.1f);
            myAudioSource.Play ();
			hurtsounds [n] = hurtsounds [0];
			hurtsounds [0] = myAudioSource.clip;
		}
		
	}
	void crouchcheck()
	{
		//check ceiling!
		Ray heightray = new Ray (transform.position, Vector3.up);
		RaycastHit ceilinghit = new RaycastHit();

		if (!Physics.Raycast (heightray, out ceilinghit, 2.2f, mask)) 
		{
			crouching = !crouching;

		}
	}


}

