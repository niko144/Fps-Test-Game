using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LMGshooter : MonoBehaviour {

	//variables
	public Transform player; 

	public Vector3 normalposition;
	public Vector3 aimposition;	
	public Vector3 retractPos;

	public float aimFOV = 45f;
	public float normalFOV  = 65f;
	



	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AudioClip fireSound;
	public AudioSource fireAudioSource;

	public AnimationClip reloadAnim;
	public AnimationClip reloadBAnim;
	public AnimationClip readyAnim;
	public AnimationClip hideAnim;
    
    public AnimationClip meleeAnim;

	public AnimationClip ambientAnim;
	private float nextambient = 2f;
	public AudioSource myAudioSource;

	public AudioClip emptySound;
	public AudioClip readySound;
	public AudioClip reloadSound;
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
	public float shellejectdelay;
	public Transform muzzle;
    public Transform particlesmuzzle;

    public Transform bullet1;
	public Transform bullet2;
	public Transform bullet3;
	public Transform bullet4;
	public Transform bullet5;
	public Transform bullet6;
	public Transform bullet7;
	public Transform bullet8;
    public Transform bullet9;
    private Transform nextbullet;



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
			if (!isreloading) {
				if (isaiming)
					inventory.showAIM (false);
				else
					inventory.showAIM (true);
				if ((Input.GetButton ("Fire1") || Input.GetAxis ("Fire1") > 0.1) && currentammo > 0  && canfire) 
				{

					fire ();


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

					StartCoroutine(setMelee (.12f));


				}
				else if(Input.GetButton("Reload") ){
					//Debug.Log("RELOAD");
					if (currentammo !=clipSize && ammo >0)
					{

						reload();
					}
				}
				else if (Time.time > nextambient && !myanimation.IsPlaying(ambientAnim.name) && !isaiming )

                {

					nextambient = Time.time + Random.Range (4f, 12f);
                    myanimation.CrossFade(ambientAnim.name, .5f);

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

		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
		
		Camera.main.fieldOfView = newField;
		
		inventory.currentammo = currentammo;

		if (retract)
		{
			canfire = false;
			canaim = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, step *2f);
			
			nextField = normalFOV;
		}

		else if (playercontrol.running)
		{
			canfire = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,runposition, step);
			wantedrotation = new Vector3(runXrotation,runYrotation,0f);
			
			nextField = normalFOV;
          
		}
		else
		{

                          
            
			canfire = true;
			wantedrotation = Vector3.zero;
			if (((Input.GetButton("Aim") || Input.GetAxis("Aim") > 0.1)) && canaim && !playercontrol.running)
			{

				isaiming = true;
				inaccuracy = spreadAim;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);
				
				nextField = aimFOV;
				recoil = 2f;
			}
			else
			{


				isaiming = false;
				inaccuracy = spreadNormal;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
				
				nextField = normalFOV;
				recoil = 4f;


			}

		}
		weaponfirer.inaccuracy = inaccuracy;
		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);

		if (currentammo < 8)
		{

            if (currentammo ==7)
            {
                bullet9.gameObject.SetActive(false);

                nextbullet = bullet8;

            }
            if (currentammo == 6)
			{
                bullet9.gameObject.SetActive(false);
                bullet8.gameObject.SetActive(false);

				nextbullet = bullet7;

			}
			else if (currentammo == 5)
			{
                bullet9.gameObject.SetActive(false);
                bullet8.gameObject.SetActive(false);
                bullet7.gameObject.SetActive(false);

				nextbullet = bullet6;

			}
			else if (currentammo == 4)
			{
				nextbullet = bullet5;
                bullet9.gameObject.SetActive(false);
                bullet8.gameObject.SetActive(false);
                bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);

			}
			else if (currentammo == 3)
			{
				nextbullet = bullet4;
                bullet9.gameObject.SetActive(false);
                bullet8.gameObject.SetActive(false);
                bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);


			}
			else if (currentammo == 2)
			{
				nextbullet = bullet3;
                bullet9.gameObject.SetActive(false);
                bullet8.gameObject.SetActive(false);
                bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				bullet4.gameObject.SetActive(false);

			}
			else if (currentammo == 1)
			{
				nextbullet = bullet2;
                bullet9.gameObject.SetActive(false);
                bullet8.gameObject.SetActive(false);
                bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				bullet4.gameObject.SetActive(false);
				bullet3.gameObject.SetActive(false);

			}

			else if (currentammo == 0 || currentammo  <= 0 )
			{	
				nextbullet = bullet1;

                bullet9.gameObject.SetActive(false);
                bullet8.gameObject.SetActive(false);
                bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				bullet4.gameObject.SetActive(false);
				bullet3.gameObject.SetActive(false);
				bullet2.gameObject.SetActive(false);
				bullet1.gameObject.SetActive(false);

			}


		}
		else
		{
            bullet9.gameObject.SetActive(true);           
            bullet8.gameObject.SetActive(true);
			bullet7.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(true);
			bullet5.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(true);
			bullet1.gameObject.SetActive(true);
			nextbullet = null;
		}

	}


	void fire()
	{
		
		float randomZ = Random.Range (-0.05f,-0.01f);
		//float randomY = Random.Range (-0.1f,0.1f);

		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y ,transform.localPosition.z + randomZ);
		myanimation[fireAnim.name].speed = fireAnimSpeed;     
		myanimation.Play(fireAnim.name);
		cameracontroller.dorecoil(recoil);

		StartCoroutine(flashthemuzzle());

		weaponfirer.fire();
		fireAudioSource.clip = fireSound;
        myAudioSource.loop = false;
        fireAudioSource.pitch = Random.Range(0.9f, 1.1f);
        fireAudioSource.Play();


		StartCoroutine(ejectshell(shellejectdelay));
		if (nextbullet != null)
		{
			nextbullet.gameObject.SetActive (false);
		}
		currentammo -=1;
	}
	void reload()
	{

		if (canreload && !isreloading ) {

			if ((currentammo > 7 || currentammo <=0) && ammo >= 7)
			{
				myAudioSource.pitch = 1f;
				myAudioSource.clip = reloadSound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play();		

				myanimation.CrossFade(reloadAnim.name);
				StartCoroutine(setreload (myanimation[reloadAnim.name].length));
			}
			else
			{
				myAudioSource.pitch = 1f;	
				myAudioSource.clip = reloadSound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play();		

				myanimation.CrossFade(reloadBAnim.name);
				StartCoroutine(setreload (myanimation[reloadBAnim.name].length));
			}


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
        shellInstance.transform.localScale = new Vector3(0.2f, 0.2f, .2f);
        shellInstance.GetComponent<Rigidbody>().AddRelativeForce(60,70,0);
		shellInstance.GetComponent<Rigidbody>().AddRelativeTorque(500,20,800);
		shellInstance.transform.localRotation = transform.localRotation * Quaternion.Euler(0,Random.Range(-90f,90f), Random.Range(-90f, 90f));
	}


    IEnumerator setreload(float waitTime)
    {
        playercontrol.canclimb = false;
        inventory.canswitch = false;
        int oldammo = currentammo;
        isreloading = true;

        canaim = false;
        yield return new WaitForSeconds(waitTime * .5f);

        currentammo = 0 + (Mathf.Clamp(clipSize, clipSize - oldammo, ammo));
        ammo -= Mathf.Clamp(ammo, 0, clipSize - oldammo);

        inventory.UpdateCurrentWeaponAmmo(ammo);
        yield return new WaitForSeconds(waitTime * .5f);
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
        myAudioSource.loop = false;
        myAudioSource.Play();
		myanimation.Play(meleeAnim.name);
		yield return new WaitForSeconds (waittime);
		weaponfirer.fireMelee();
	}
    
    IEnumerator setThrowGrenade()
	{
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