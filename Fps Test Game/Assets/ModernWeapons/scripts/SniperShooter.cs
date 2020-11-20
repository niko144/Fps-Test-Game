using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperShooter : MonoBehaviour {

	//variables
	public Transform player; 

	public Vector3 normalposition;
	public Vector3 aimposition;	
	public Vector3 retractPos;

	public float aimFOV = 45f;
	public float normalFOV  = 65f;
    
    
    public GameObject scopeGUI;

    public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AudioClip fireSound;
	public AudioSource fireAudioSource;

	public AnimationClip reloadAnim;
	public AnimationClip boltAnim;
	public AnimationClip readyAnim;
	public AnimationClip hideAnim;

	public AnimationClip meleeAnim;

	public AnimationClip ambientAnim;
	private float nextambient = 2f;
	public AudioSource myAudioSource;

	public AudioClip emptySound;
	public AudioClip readySound;
	public AudioClip reloadSound;
	public AudioClip boltSound;
	public AudioClip meleeSound;



	public int projectilecount = 1;
	private float inaccuracy = 0.02f;
	public float spreadNormal = 0.08f;
	public float spreadAim = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 100f;



	public GameObject shell;
	public Transform shellPos;

	public Transform muzzle;
    public Transform particlesmuzzle;
    public Transform clipShell;


	private float recoil;
    public int weaponammoNumber;
    public int ammo = 200;
	public int currentammo= 20;
	public int clipSize = 20;


	
	public Transform recoilCamera;
	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;



	//private
	private float nextField;
	private float weaponnextfield;
	private Vector3 wantedrotation;
	private bool canaim = true;
    
	private bool canfire = true;
	private bool canreload = true;
	private bool retract = false;	
	private bool isreloading  = false;
	private bool isaiming = false;

	public Transform rayfirer;
	public Transform grenadethrower;
    
    //scriptreference
    raycastfire weaponfirer;
	playercontroller playercontrol ;
	weaponselector inventory;
	camerarotate cameracontroller;
	Animation myanimation;
    
    void Awake()
	{
		weaponfirer = rayfirer.GetComponent<raycastfire>();
		playercontrol = player.GetComponent<playercontroller>();
		myanimation = GetComponent<Animation>();
		cameracontroller = recoilCamera.GetComponent<camerarotate>();
        
    }
	void Start()
	{
		clipSize=currentammo;


		nextField = normalFOV ;
		
		myanimation.Stop();
		onstart();
	}

	void onstart()
	{
        scopeGUI.SetActive(false);
        StartCoroutine(waitforinput());
		myAudioSource.Stop();
		fireAudioSource.Stop();
		if(weaponfirer==null) weaponfirer = rayfirer.GetComponent<raycastfire>();
		weaponfirer.inaccuracy = inaccuracy;
		weaponfirer.damage = damage;
		weaponfirer.range = range;
		weaponfirer.force = force;
		weaponfirer.projectilecount = projectilecount;
		if(inventory==null){ 
			inventory = player.GetComponent<weaponselector>();
            ammo = inventory.WeaponsAmmo[weaponammoNumber];
        }
        
        inventory.showAIM(false);
       
        myanimation.Stop();
        if (isreloading)
        {
            isreloading = !isreloading;
            reload();
        }
        else 
		{

			clipShell.gameObject.SetActive (true);
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();
			myanimation.Play (readyAnim.name);
			
			canfire = true;
			inventory.showAIM(true);
		}

	}
    IEnumerator waitforinput()
    {
        canaim = false;
        yield return new WaitForSeconds(myanimation[readyAnim.name].length);
        canaim = true;
    }
	void Update ()
	{


		if (!myanimation.isPlaying || myanimation.IsPlaying(ambientAnim.name)) 
		{
            if (!isreloading)
            {
                
                if ((Input.GetButton("Fire1") || Input.GetAxis("Fire1") > 0.1) && currentammo > 0 && canfire)
                {

                    fire();


                }
                else if (Input.GetButton("ThrowGrenade") && inventory.grenade>0)
				{

					if(Time.timeSinceLevelLoad>(inventory.lastGrenade+1))
					{
						inventory.lastGrenade=Time.timeSinceLevelLoad;			
						StartCoroutine(setThrowGrenade());
					}


				}
				else if (Input.GetButton("Melee"))
				{

					StartCoroutine(setMelee (.19f));


				}
				else if(Input.GetButton("Reload") ){
					//Debug.Log("RELOAD");
					if (currentammo !=clipSize && ammo >0)
					{

						reload();
					}
				}
				else if (Time.time > nextambient && !myanimation.IsPlaying(ambientAnim.name) && !isaiming) 
				{

					nextambient = Time.time + Random.Range (4f, 12f);
					myanimation.Play(ambientAnim.name);

				}
				else if (currentammo  <= 0 )
				{	

					if (ammo <= 0)
					{
						canfire = false;
						canreload = false;
						if ((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1) && !myAudioSource.isPlaying)
						{
							if (!myAudioSource.isPlaying)
							{
								myAudioSource.PlayOneShot(emptySound);
							}
						}

					}
					else 
					{
						reload();
					}
				}


			}
			else 
			{
				inventory.showAIM (false);
			}



		} 

		float step = 2 * Time.deltaTime;

		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 3f);
		
		Camera.main.fieldOfView = newField;
		
		inventory.currentammo = currentammo;
        
       
        if (retract)
		{
			canfire = false;
			canaim = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, step *3f);
			
			nextField = normalFOV;
            
           
        }

		else if (playercontrol.running)
		{
			canfire = false;
            isaiming = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,runposition, step);
			wantedrotation = new Vector3(runXrotation,runYrotation,0f);
            scopeGUI.SetActive(false);
            nextField = normalFOV;
           


        }
		else
		{
			canfire = true;
			wantedrotation = Vector3.zero;
			if (((Input.GetButton("Aim") || Input.GetAxis("Aim") > 0.1)) && canaim )
			{
                
                 
                
                inventory.showAIM(false);
               
                isaiming = true;

                scopeGUI.SetActive(true);
                inaccuracy = spreadAim;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step*2f);
				
				nextField = aimFOV;
				recoil = .5f;
			}
			else
			{
                isaiming = false;
               
                
                inventory.showAIM(true);

                inaccuracy = spreadNormal;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
				
				nextField = normalFOV;
				recoil = 3f;
                scopeGUI.SetActive(false);

            }

		}
		weaponfirer.inaccuracy = inaccuracy;
		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);
       if(!isaiming)
        {
            Animation scopeanim = scopeGUI.GetComponentInChildren<Animation>();
            scopeanim.Play();
        }

    }

   
    void fire()
	{


		float randomZ = Random.Range (-0.05f,-0.01f);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y ,transform.localPosition.z + randomZ);

		StartCoroutine(setfire());


	}
	IEnumerator setfire()
	{
		if (currentammo > 1) {
			StartCoroutine(flashthemuzzle());
			
			weaponfirer.fire();
			cameracontroller.dorecoil(recoil);
			fireAudioSource.clip = fireSound;
            fireAudioSource.pitch = Random.Range(0.9f, 1.1f);
            fireAudioSource.Play ();


			myanimation.Play (fireAnim.name);
			currentammo -= 1;
			yield return new WaitForSeconds (myanimation[fireAnim.name].length);
			myAudioSource.clip = boltSound;
			myAudioSource.Play();
           

			myanimation.Play(boltAnim.name);
            scopeGUI.GetComponentInChildren<Animation>().Play();

            
           
                       
            
            
            
            
            
			StartCoroutine (ejectshell (myanimation[boltAnim.name].length * 0.35f));
			yield return new WaitForSeconds (myanimation[boltAnim.name].length);

           
        } 
		else if (currentammo <= 1) 
		{
			if (currentammo <= 0) {
				reload ();
			}
			cameracontroller.dorecoil(recoil);
			StartCoroutine(flashthemuzzle());
			weaponfirer.fire();
			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f * Random.value;
			fireAudioSource.Play ();

			myanimation.Play (fireAnim.name);

			currentammo -= 1;
			yield return new WaitForSeconds (myanimation[fireAnim.name].length);
		}


	}
	void reload()
	{
		if (canreload && !isreloading )
		{
			myAudioSource.pitch = 1f;
			StartCoroutine(setreload());
		}
	}

	void doNormal()
	{
		onstart();
	}

    IEnumerator flashthemuzzle()
    {
        ParticleSystem[] particleSystems;
        particleSystems = particlesmuzzle.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particleSystems)
        {
            particle.Play();
        }
        muzzle.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
        muzzle.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzle.gameObject.SetActive(false);
    }
    IEnumerator ejectshell(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		GameObject shellInstance;
		shellInstance = Instantiate(shell, shellPos.transform.position,shellPos.transform.rotation) as GameObject;

		yield return null;
		shellInstance.GetComponent<Rigidbody>().AddRelativeForce(60,70,0);
		shellInstance.GetComponent<Rigidbody>().AddRelativeTorque(500,20,800);
		shellInstance.transform.localRotation = transform.localRotation * Quaternion.Euler(0,Random.Range(-90f,90f), Random.Range(-90f, 90f));
	}
	IEnumerator deactivateShell(float waitTime)
	{
		clipShell.gameObject.SetActive (false);
		yield return new WaitForSeconds(waitTime);
		clipShell.gameObject.SetActive (true);
	}

	IEnumerator setreload()
	{
		playercontrol.canclimb = false;
		inventory.canswitch = false;
		int oldammo = currentammo;
		isreloading = true;

		canaim = false;


		if (currentammo == 0) 
		{
			StartCoroutine (deactivateShell (myanimation[reloadAnim.name].length * 0.5f)); 
			
			myAudioSource.clip = reloadSound;
			myAudioSource.Play ();
			myanimation.Play (reloadAnim.name);
			yield return new WaitForSeconds (myanimation [reloadAnim.name].length * .5f);
            currentammo = 0 + (Mathf.Clamp(clipSize, clipSize - oldammo, ammo));
            ammo -= Mathf.Clamp(ammo, 0, clipSize - oldammo);

            inventory.UpdateCurrentWeaponAmmo(ammo);



            yield return new WaitForSeconds (myanimation [reloadAnim.name].length * .5f);
			myanimation.Play (boltAnim.name);
			StartCoroutine (ejectshell (myanimation[boltAnim.name].length * 0.35f));
			myAudioSource.clip = boltSound;
			myAudioSource.Play ();

			yield return new WaitForSeconds (myanimation [boltAnim.name].length);
		} 
		else 
		{
			
			
			myAudioSource.clip = reloadSound;
			myAudioSource.Play ();
			myanimation.Play (reloadAnim.name);
			yield return new WaitForSeconds (myanimation [reloadAnim.name].length * .5f);
            currentammo = 0 + (Mathf.Clamp(clipSize, clipSize - oldammo, ammo));
            ammo -= Mathf.Clamp(ammo, 0, clipSize - oldammo);

            inventory.UpdateCurrentWeaponAmmo(ammo);


            yield return new WaitForSeconds (myanimation [reloadAnim.name].length * .5f);

		}	

		isreloading = false;
		canaim = true;
		inventory.canswitch = true;
		playercontrol.canclimb = true;

	}

	void pickAmmo(int inventoryAmmo){
		ammo=inventoryAmmo;
	}
	IEnumerator setMelee(float waittime)
	{
		myAudioSource.clip = meleeSound;
		myAudioSource.Play();
		myanimation.Play(meleeAnim.name);
		yield return new WaitForSeconds (waittime);
		weaponfirer.fireMelee();
	}
   
    IEnumerator setThrowGrenade()
	{
        canaim = false;
        canfire = false;
		retract = true;
        
        grenadethrower.gameObject.SetActive(true);
		grenadethrower.gameObject.BroadcastMessage("throwstuff");
		Animation throwerAnimation = grenadethrower.GetComponent<Animation> ();

		yield return new WaitForSeconds(throwerAnimation.clip.length);
		retract = false;
		canaim = true;
		canfire = true;
		grenadethrower.gameObject.SetActive(false);
	}
	void doRetract()
	{
		myanimation.Play(hideAnim.name);


	}

}