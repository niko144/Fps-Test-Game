using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour {

	//variables
	public Transform player; 

	public Vector3 normalposition;
	public Vector3 aimposition;	
	public Vector3 retractPos;

	public float aimFOV = 45f;
	public float normalFOV  = 65f;
	

	public AnimationClip fireLoopAnim;


	public AudioClip fireSound;
	public AudioSource fireAudioSource;

	public AnimationClip reloadAnim;
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
	



	
	public Transform fireparticles;
	public int particleRate = 60;

    public int weaponammoNumber;
    public float ammo = 40f;
	public float currentammo= 40f;
	public int clipSize = 40;


	

	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;



	//private
	private float nextField;
	
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

    playercontroller playercontrol ;
	weaponselector inventory;
	
	raycastfire weaponfirer;
	Animation myanimation;

	void Awake()
	{
		weaponfirer = rayfirer.GetComponent<raycastfire>();
		playercontrol = player.GetComponent<playercontroller>();
		myanimation = GetComponent<Animation>();

	}
    void Start()
    {
        clipSize = Mathf.RoundToInt(currentammo);


        nextField = normalFOV;

        myanimation.Stop();
        onstart();
    }


    void onstart()
    {
        StartCoroutine(waitforinput());
        myAudioSource.Stop();
        fireAudioSource.Stop();
        if (weaponfirer == null) weaponfirer = rayfirer.GetComponent<raycastfire>();
        
        if (inventory == null)
        {
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
            myAudioSource.Play();
            myanimation.Play(readyAnim.name);

            canfire = true;
            inventory.showAIM(false);
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
                
               if (Input.GetButton("ThrowGrenade") && inventory.grenade > 0)
                {

                    if (Time.timeSinceLevelLoad > (inventory.lastGrenade + 1))
                    {
                        inventory.lastGrenade = Time.timeSinceLevelLoad;
                        StartCoroutine(setThrowGrenade());
                    }


                }

                else if (Input.GetButton("Melee"))
                {

                    StartCoroutine(setMelee(.12f));


                }
                else if (Input.GetButton("Reload"))
                {
                    //Debug.Log("RELOAD");
                    if (currentammo != clipSize && ammo > 0)
                    {

                        reload();
                    }
                }
                else if (Time.time > nextambient && !myanimation.IsPlaying(ambientAnim.name) && !isaiming)
                {

                    nextambient = Time.time + Random.Range(4f, 12f);
                    myanimation.Play(ambientAnim.name);

                }
                else if (currentammo <= 0)
                {

                    if (ammo <= 0)
                    {
                        canfire = false;
                        canreload = false;
                        if ((Input.GetButton("Fire1") || Input.GetAxis("Fire1") > 0.1) && !myAudioSource.isPlaying)
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
            

        }
        if ((Input.GetButton ("Fire1") || Input.GetAxis ("Fire1") > 0.1) && currentammo > 0 && canfire) {



			currentammo -= 5f * Time.deltaTime;
			ParticleSystem[] particleSystems;
			particleSystems = fireparticles.GetComponentsInChildren<ParticleSystem> ();
			foreach (ParticleSystem particle in particleSystems) {
				var em = particle.emission;
				em.rateOverTime = particleRate;
			}
			if (!fireAudioSource.isPlaying) {
				fireAudioSource.clip = fireSound;
				fireAudioSource.loop = true;
				fireAudioSource.Play ();
			}
			
			myanimation.CrossFade (fireLoopAnim.name,1f);

		}
		else 
		{
			
			ParticleSystem[] particleSystems;
			particleSystems = fireparticles.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var em = particle.emission;
				em.rateOverTime= 0f;
			}
			
			fireAudioSource.Stop();
			myanimation.Stop (fireLoopAnim.name);

		}
        float step = 2 * Time.deltaTime;

        float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);

        Camera.main.fieldOfView = newField;

        inventory.currentammo = Mathf.RoundToInt(currentammo);

        if (retract)
        {
            canfire = false;
            canaim = false;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, step * 2f);
           
            nextField = normalFOV;
        }
        else if (myanimation.IsPlaying(readyAnim.name)|| myanimation.IsPlaying(reloadAnim.name))
        {
            canfire = false;
        }

        else if (playercontrol.running)
        {
            canfire = false;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, runposition, step);
            wantedrotation = new Vector3(runXrotation, runYrotation, 0f);

            nextField = normalFOV;
            if (currentammo <= 0)
            {

                if (ammo <= 0)
                {
                    canfire = false;
                    canreload = false;
                    if ((Input.GetButton("Fire1") || Input.GetAxis("Fire1") > 0.1) && !myAudioSource.isPlaying)
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
            canfire = true;
            wantedrotation = Vector3.zero;
            if (((Input.GetButton("Aim") || Input.GetAxis("Aim") > 0.1)) && canaim && !playercontrol.running)
            {

                isaiming = true;
                
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);

                nextField = aimFOV;
               
            }
            else
            {


                isaiming = false;
                
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);

                nextField = normalFOV;
                


            }

        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);


	}

	void reload()
	{

		if (canreload && !isreloading ) {
			myAudioSource.pitch = 1f;
			StartCoroutine(setreload (myanimation[reloadAnim.name].length));
			myAudioSource.clip = reloadSound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		
			myanimation.Play(reloadAnim.name);

		}
	}

	void doNormal()
	{
		onstart();
	}



    IEnumerator setreload(float waitTime)
    {
        playercontrol.canclimb = false;
        inventory.canswitch = false;
        int oldammo = Mathf.RoundToInt(currentammo);
        isreloading = true;

        canaim = false;
        yield return new WaitForSeconds(waitTime * .5f);

        currentammo = 0 + (Mathf.Clamp(clipSize, clipSize - oldammo, ammo));
        ammo -= Mathf.Clamp(ammo, 0, clipSize - oldammo);

        inventory.UpdateCurrentWeaponAmmo(Mathf.RoundToInt(ammo));
        yield return new WaitForSeconds(waitTime * .5f);
        isreloading = false;
        canaim = true;
        inventory.canswitch = true;
        playercontrol.canclimb = true;

    }

    void pickAmmo(int inventoryAmmo)
    {
        ammo = inventoryAmmo;
    }
    IEnumerator setMelee(float waittime)
    {
        myAudioSource.clip = meleeSound;
        myAudioSource.Play();
        myanimation.Play(meleeAnim.name);
        yield return new WaitForSeconds(waittime);
        weaponfirer.fireMelee();
    }
    IEnumerator setThrowGrenade()
    {
        canfire = false;
        retract = true;
        grenadethrower.gameObject.SetActive(true);
        grenadethrower.gameObject.BroadcastMessage("throwstuff");
        Animation throwerAnimation = grenadethrower.GetComponent<Animation>();

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
