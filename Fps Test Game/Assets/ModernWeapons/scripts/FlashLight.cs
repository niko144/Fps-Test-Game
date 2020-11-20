using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{

    //variables
    public Transform player;

    public Vector3 normalposition;

    public Vector3 retractPos;

    public float normalFOV = 65f;




    public GameObject spotlight;
    public AnimationClip[] fireAnims;


    public AudioClip fireSound;
    public AudioSource fireAudioSource;


    public AnimationClip readyAnim;
    public AnimationClip hideAnim;


    public AnimationClip ambientAnim;

    private float nextambient = 2f;

    public AudioSource myAudioSource;

    public AudioClip hideSound;

    public AudioClip readySound;






    private float inaccuracy = 0.02f;


    public float force = 500f;
    public float damage = 50f;
    public float range = 100f;





    public float runXrotation = 20f;
    public float runYrotation = 0f;
    public Vector3 runposition = Vector3.zero;



    //private

    private Vector3 wantedrotation;
    public Vector3 startrotation;

    private bool retract = false;

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

    }
    void Start()
    {


        myanimation.Stop();
        onstart();
    }

    void onstart()
    {
        spotlight.SetActive(false);
        myAudioSource.Stop();
        fireAudioSource.Stop();
        if (weaponfirer == null) weaponfirer = rayfirer.GetComponent<raycastfire>();
        weaponfirer.inaccuracy = inaccuracy;
        weaponfirer.damage = damage;
        weaponfirer.range = range;
        weaponfirer.force = force;




        myanimation.Stop();
        if (inventory == null)
        {
            inventory = player.GetComponent<weaponselector>();
            //Init the Current Weapon with ammo value
            inventory.UpdateCurrentWeaponAmmo(-1);
        }
        myAudioSource.clip = readySound;
        myAudioSource.loop = false;
        myAudioSource.volume = 1;
        myAudioSource.Play();
        StartCoroutine(lighton(myanimation[readyAnim.name].length * 0.4f));
        myanimation.Play(readyAnim.name);




    }
    void Update()
    {


        if (!myanimation.isPlaying || myanimation.IsPlaying(ambientAnim.name))
        {


            if ((Input.GetButton("Fire1") || Input.GetAxis("Fire1") > 0.1))
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


            else if (Time.time > nextambient && !myanimation.IsPlaying(ambientAnim.name))
            {

                nextambient = Time.time + Random.Range(4f, 12f);
                myanimation.Play(ambientAnim.name);

            }




        }

        float step = 2 * Time.deltaTime;


        float newField = Mathf.Lerp(Camera.main.fieldOfView, normalFOV, Time.deltaTime * 2);

        Camera.main.fieldOfView = newField;



        if (retract)
        {

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, step * 2f);

        }

        else if (playercontrol.running)
        {

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, runposition, step);
            wantedrotation = new Vector3(runXrotation, runYrotation, 0f);


        }
        else
        {

            wantedrotation = startrotation;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);

        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(wantedrotation), step * 3f);


    }


    void fire()
    {



        myanimation.clip = fireAnims[Random.Range(0, fireAnims.Length)];
        myanimation.Play();
        StartCoroutine(firedelayed(0.2f));

    }


    IEnumerator firedelayed(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        fireAudioSource.clip = fireSound;
        fireAudioSource.pitch = 0.9f + 0.1f * Random.value;
        fireAudioSource.Play();
        weaponfirer.fireMelee();
    }

    void doNormal()
    {
        onstart();
    }


    IEnumerator setThrowGrenade()
    {
        spotlight.SetActive(false);
        retract = true;
        grenadethrower.gameObject.SetActive(true);
        grenadethrower.gameObject.BroadcastMessage("throwstuff");
        Animation throwerAnimation = grenadethrower.GetComponent<Animation>();

        yield return new WaitForSeconds(throwerAnimation.clip.length);
        retract = false;
        grenadethrower.gameObject.SetActive(false);
        spotlight.SetActive(true);
    }
    void doRetract()
    {
        StartCoroutine(lighton(myanimation[hideAnim.name].length * 0.24f));
        myAudioSource.clip = hideSound;
        myAudioSource.loop = false;
        myAudioSource.volume = 1;
        myAudioSource.Play();
        myanimation.Play(hideAnim.name);
    }
    IEnumerator lighton (float waittime)
    {
        yield return new WaitForSeconds(waittime);
        spotlight.SetActive(true);
    }
    IEnumerator lightoff(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        spotlight.SetActive(false);
    }

}
