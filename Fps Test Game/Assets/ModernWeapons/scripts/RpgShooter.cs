using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RpgShooter : MonoBehaviour
{

    //variables
    public Transform player;

    public Vector3 normalposition;
    public Vector3 aimposition;
    public Vector3 retractPos;

    public float aimFOV = 45f;
    public float normalFOV = 65f;




    public AnimationClip fireAnim;
    public float fireAnimSpeed = 1f;

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




   




    public GameObject rocket;
    public GameObject projectile;
    public Transform projectilepos;
    



   
    public Transform muzzle;
    public Transform particlesmuzzle;



    private float recoil;
    public int weaponammoNumber;
    public int ammo = 200;
    public int currentammo = 20;
    public int clipSize = 1;



    public Transform recoilCamera;
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
    private bool isreloading = false;
    private bool isaiming = false;

    public Transform rayfirer;
    public Transform grenadethrower;



    //scriptreference
    raycastfire weaponfirer;
    playercontroller playercontrol;
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
        clipSize = currentammo;
        nextField = normalFOV;

        myanimation.Stop();
        onstart();
    }

    void onstart()
    {
        myAudioSource.Stop();
        fireAudioSource.Stop();

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
            rocket.SetActive(true);
            myAudioSource.clip = readySound;
            myAudioSource.loop = false;
            myAudioSource.volume = 1;
            myAudioSource.Play();
            myanimation.Play(readyAnim.name);
            canaim = true;
            canfire = true;
            inventory.showAIM(true);
        }

    }
    void Update()
    {


        if (!myanimation.isPlaying || myanimation.IsPlaying(ambientAnim.name))
        {
            if (!isreloading)
            {
                if (isaiming)
                    inventory.showAIM(false);
                else
                    inventory.showAIM(true);
                if ((Input.GetButton("Fire1") || Input.GetAxis("Fire1") > 0.1) && currentammo > 0 && canfire)
                {

                    fire();


                }
                else if (Input.GetButton("ThrowGrenade") && inventory.grenade > 0)
                {

                    if (Time.timeSinceLevelLoad > (inventory.lastGrenade + 1))
                    {
                        inventory.lastGrenade = Time.timeSinceLevelLoad;
                        StartCoroutine(setThrowGrenade());
                    }


                }
                else if (Input.GetButton("Melee"))
                {

                    StartCoroutine(setMelee(.4f));


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
            else
            {
                inventory.showAIM(false);
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
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, step * 2f);

            nextField = normalFOV;
        }

        else if (playercontrol.running)
        {
            canfire = false;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, runposition, step);
            wantedrotation = new Vector3(runXrotation, runYrotation, 0f);

            nextField = normalFOV;

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
                recoil = .5f;
            }
            else
            {


                isaiming = false;

                transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);

                nextField = normalFOV;
                recoil = 3f;


            }

        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(wantedrotation), step * 3f);
        


    }


    void fire()
    {

        float randomZ = Random.Range(-0.05f, -0.01f);
        //float randomY = Random.Range (-0.1f,0.1f);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + randomZ);

        cameracontroller.dorecoil(recoil);

        StartCoroutine(flashthemuzzle());
        StartCoroutine(launchRocket( ));
        fireAudioSource.clip = fireSound;
        fireAudioSource.pitch = 0.9f + 0.1f * Random.value;
        fireAudioSource.Play();
        myanimation[fireAnim.name].speed = fireAnimSpeed;
        myanimation.Play(fireAnim.name);
        currentammo -= 1;
       


    }
    IEnumerator launchRocket()
    {

        yield return new WaitForSeconds(myanimation[fireAnim.name].length * 0.2f);
        rocket.SetActive(false);
        GameObject rocketInstance = Instantiate(projectile, projectilepos.position, projectilepos.rotation) as GameObject;
    }
    void reload()
    {

        if (canreload && !isreloading)
        {

           
            StartCoroutine(setreload(myanimation[reloadAnim.name].length));
            myAudioSource.pitch = 1f;
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



    IEnumerator setreload(float waitTime)
    {
        playercontrol.canclimb = false;
        inventory.canswitch = false;
        int oldammo = currentammo;
        isreloading = true;
        rocket.SetActive(true);
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