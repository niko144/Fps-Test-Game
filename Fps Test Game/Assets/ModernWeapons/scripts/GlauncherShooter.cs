using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlauncherShooter : MonoBehaviour
{

    //variables
    public Transform player;

    public Vector3 normalposition;
    public Vector3 aimposition;
    public Vector3 retractPos;

    public float aimFOV = 45f;
    public float normalFOV = 65f;




    public AnimationClip fireAnim;
    public float fireAnimSpeed = 1.1f;

    public AudioClip fireSound;
    public AudioSource fireAudioSource;

    public AnimationClip toreloadAnim;
    //public AnimationClip reloadonceAnim;
    public AnimationClip reloadB1Anim;
    public AnimationClip reloadB2Anim;
    public AnimationClip reloadB3Anim;
    public AnimationClip reloadB4Anim;
    public AnimationClip reloadB5Anim;
    public AnimationClip reloadB6Anim;


    public AnimationClip reloadlastAnim;
    public AnimationClip readyAnim;
    public AnimationClip hideAnim;

    public AnimationClip meleeAnim;

    public AnimationClip ambientAnim;
    private float nextambient = 2f;
    public AudioSource myAudioSource;

    public AudioClip emptySound;
    public AudioClip readySound;
    public AudioClip toreloadSound;
    public AudioClip reloadonceSound;
    public AudioClip reloadlastSound;
    public AudioClip meleeSound;




    public int projectilecount = 1;

    public Transform bullet1;
    public Transform bullet2;
    public Transform bullet3;
    public Transform bullet4;
    public Transform bullet5;
    public Transform bullet6;
    private int bulletactivator = 6;


    public GameObject projectile;
    public Transform projectilepos;
    public float projectileForce = 1500f;



    public GameObject shell;
    public Transform shellPos;
    public float shellejectdelay;
    public Transform muzzle;
    public Transform particlesmuzzle;



    private float recoil;
    public int weaponammoNumber;
    public int ammo = 200;
    public int currentammo = 20;
    public int clipSize = 20;



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
        if (bulletactivator == 0)
        {
            bullet1.gameObject.SetActive(false);
            bullet2.gameObject.SetActive(false);
            bullet3.gameObject.SetActive(false);
            bullet4.gameObject.SetActive(false);
            bullet5.gameObject.SetActive(false);
            bullet6.gameObject.SetActive(false);
        }
        else if (bulletactivator == 1)
        {
            bullet1.gameObject.SetActive(false);
            bullet2.gameObject.SetActive(false);
            bullet3.gameObject.SetActive(false);
            bullet4.gameObject.SetActive(false);
            bullet5.gameObject.SetActive(false);
            bullet6.gameObject.SetActive(true);
        }
        else if (bulletactivator == 2)
        {
            bullet1.gameObject.SetActive(false);
            bullet2.gameObject.SetActive(false);
            bullet3.gameObject.SetActive(false);
            bullet4.gameObject.SetActive(false);
            bullet5.gameObject.SetActive(true);
            bullet6.gameObject.SetActive(true);
        }
        else if (bulletactivator == 3)
        {
            bullet1.gameObject.SetActive(false);
            bullet2.gameObject.SetActive(false);
            bullet3.gameObject.SetActive(false);
            bullet4.gameObject.SetActive(true);
            bullet5.gameObject.SetActive(true);
            bullet6.gameObject.SetActive(true);
        }
        else if (bulletactivator == 4)
        {
            bullet1.gameObject.SetActive(false);
            bullet2.gameObject.SetActive(false);
            bullet3.gameObject.SetActive(true);
            bullet4.gameObject.SetActive(true);
            bullet5.gameObject.SetActive(true);
            bullet6.gameObject.SetActive(true);
        }
        else if (bulletactivator == 5)
        {
            bullet1.gameObject.SetActive(false);
            bullet2.gameObject.SetActive(true);
            bullet3.gameObject.SetActive(true);
            bullet4.gameObject.SetActive(true);
            bullet5.gameObject.SetActive(true);
            bullet6.gameObject.SetActive(true);
        }
        else if (bulletactivator == 6)
        {
            bullet1.gameObject.SetActive(true);
            bullet2.gameObject.SetActive(true);
            bullet3.gameObject.SetActive(true);
            bullet4.gameObject.SetActive(true);
            bullet5.gameObject.SetActive(true);
            bullet6.gameObject.SetActive(true);
        }


    }


    void fire()
    {

        float randomZ = Random.Range(-0.05f, -0.01f);
        //float randomY = Random.Range (-0.1f,0.1f);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + randomZ);

        cameracontroller.dorecoil(recoil);

        StartCoroutine(flashthemuzzle());
        StartCoroutine(launchSlug());
        fireAudioSource.clip = fireSound;
        fireAudioSource.pitch = 0.9f + 0.1f * Random.value;
        fireAudioSource.Play();
        myanimation[fireAnim.name].speed = fireAnimSpeed;
        myanimation.Play(fireAnim.name);
        currentammo -= 1;



    }
    IEnumerator launchSlug()
    {
        GameObject slugInstance = Instantiate(projectile, projectilepos.position, projectilepos.rotation) as GameObject;
        yield return new WaitForEndOfFrame();
        slugInstance.GetComponent<Rigidbody>().AddRelativeForce(0f, 0f, projectileForce);
    }
    void reload()
    {

        if (canreload && !isreloading)
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
        for (int i = 0; i < clipSize - currentammo; i++)
        {
            Instantiate(shell, shellPos.transform.position, shellPos.transform.rotation);
        }

    }


    IEnumerator setreload()
    {
        //ammoToReload = Mathf.Clamp (ammoToReload, ammoToReload, ammo);
        //reload first
        isreloading = true;
        canaim = false;
        playercontrol.canclimb = false;
       
        inventory.canswitch = false;
        myAudioSource.clip = toreloadSound;
        myAudioSource.Play();

        //GetComponent<Animation>()[reloadAnim.name].time = startTime;
        myanimation.Play(toreloadAnim.name);
        yield return new WaitForSeconds(myanimation[toreloadAnim.name].length * 0.55f);
        StartCoroutine(ejectshell(shellejectdelay));
        bulletactivator = currentammo;

        yield return new WaitForSeconds(myanimation[toreloadAnim.name].length * 0.45f);
        //reloadonce

        while (currentammo != clipSize && ammo >= 1)
        {


            if (bulletactivator == 0)
            {
                myanimation.Play(reloadB6Anim.name);
                myAudioSource.clip = reloadonceSound;
                myAudioSource.Play();
                bulletactivator += 1;
                yield return new WaitForSeconds(myanimation[reloadB6Anim.name].length);
                ammo -= 1;
                currentammo += 1;
                inventory.UpdateCurrentWeaponAmmo(ammo);

            }
            else if (bulletactivator == 1)
            {
                myanimation.Play(reloadB5Anim.name);
                myAudioSource.clip = reloadonceSound;
                myAudioSource.Play();
                bulletactivator += 1;
                yield return new WaitForSeconds(myanimation[reloadB5Anim.name].length);
                ammo -= 1;
                currentammo += 1;
                inventory.UpdateCurrentWeaponAmmo(ammo);

            }
            else if (bulletactivator == 2)
            {
                myanimation.Play(reloadB4Anim.name);
                myAudioSource.clip = reloadonceSound;
                myAudioSource.Play();
                bulletactivator += 1;
                yield return new WaitForSeconds(myanimation[reloadB4Anim.name].length);
                ammo -= 1;
                currentammo += 1;
                inventory.UpdateCurrentWeaponAmmo(ammo);

            }
            else if (bulletactivator == 3)
            {
                myanimation.Play(reloadB3Anim.name);
                myAudioSource.clip = reloadonceSound;
                myAudioSource.Play();
                bulletactivator += 1;
                yield return new WaitForSeconds(myanimation[reloadB3Anim.name].length);
                ammo -= 1;
                currentammo += 1;
                inventory.UpdateCurrentWeaponAmmo(ammo);
            }
            else if (bulletactivator == 4)
            {
                myanimation.Play(reloadB2Anim.name);
                myAudioSource.clip = reloadonceSound;
                myAudioSource.Play();
                bulletactivator += 1;
                yield return new WaitForSeconds(myanimation[reloadB2Anim.name].length);
                ammo -= 1;
                currentammo += 1;
                inventory.UpdateCurrentWeaponAmmo(ammo);

            }
            else if (bulletactivator == 5)
            {
                myanimation.Play(reloadB1Anim.name);
                myAudioSource.clip = reloadonceSound;
                myAudioSource.Play();
                bulletactivator += 1;
                yield return new WaitForSeconds(myanimation[reloadB1Anim.name].length);
                ammo -= 1;
                currentammo += 1;
                inventory.UpdateCurrentWeaponAmmo(ammo);

            }






        }

        //reloadlast
        myAudioSource.clip = reloadlastSound;
        myAudioSource.Play();
        //GetComponent<Animation>()[reloadAnim.name].time = startTime;
        myanimation.Play(reloadlastAnim.name);
        yield return new WaitForSeconds(myanimation[reloadlastAnim.name].length);

        playercontrol.canclimb = true;
        isreloading = false;
        canaim = true;
        //inventory.canswitch = true;

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