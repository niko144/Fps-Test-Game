using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bodypart : MonoBehaviour
{

    public Transform root;
    public Transform gibmesh;
    public GameObject limb;
    public GameObject limb2;
    public bool dismember = false;
    public bool dismemberparent = false;
    public float hitpoints = 100f;
    public float hitpointsbeforedismember = 400f;
    public Vector3 hitvector = Vector3.zero;
    public Vector3 hitpoint = Vector3.zero;
    public int[] randomvalue = new int[2];
    //private Animator rootanimator;
   
    private bool diddestroy = false;
    private Vector3 startposition;
    void Start()
    {
        
        startposition = transform.localPosition;
    }
    void Update()
    {
        if (!diddestroy)
        {


            if (hitpoints <= 0)
            {
                Destroy(root.GetComponent<UnityEngine.AI.NavMeshAgent>());
                if (GetComponent<AudioSource>()!= null)
                {
                    Destroy(GetComponent<AudioSource>());
                    
                    
                }
                   
                Destroy(root.GetComponent<Animator>());
                //Destroy(root.GetComponent<GhoulNPC>());
                Rigidbody[] bodies = root.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody mybody in bodies)
                {
                    mybody.isKinematic = false;

                }
                if (GetComponent<Rigidbody>() != null)
                    GetComponent<Rigidbody>().AddForceAtPosition(hitvector, hitpoint);
                if (dismember)
                {
                    if (hitpointsbeforedismember <= 0)
                    {
                        if (gibmesh != null)
                            gibmesh.gameObject.SetActive(true);


                        if (limb != null)
                        {
                            if (transform.GetChild(0).localScale != Vector3.zero && limb2 != null)
                            {
                                limb = Instantiate(limb, transform.position, transform.rotation) as GameObject;
                                limb2 = Instantiate(limb2, transform.GetChild(0).transform.position, transform.GetChild(0).transform.rotation) as GameObject;
                            }
                            else
                            {
                                limb = Instantiate(limb, transform.position, transform.rotation) as GameObject;
                            }
                        }

                        //Instantiate(gibs, transform.position, Quaternion.identity);
                        StartCoroutine(disable());

                        
                       
                        diddestroy = true;
                    }

                }
            }
        }
    }


    void Damage(float damage)
    {



        
        hitpoints = hitpoints - damage;
        hitpointsbeforedismember = hitpointsbeforedismember - damage;
        if (root.GetComponent<Animator>() != null)
        {
            StartCoroutine(hitanim());
        }




    }

   
    IEnumerator disable()
    {

        yield return null;

        GetComponent<Rigidbody>().isKinematic = true;

        //transform.gameObject.SetActive (false);
        Destroy(GetComponent<CharacterJoint>());
        Destroy(GetComponent<Rigidbody>());
        transform.localPosition = startposition;
        if (dismemberparent)
        {
            transform.parent.localScale = Vector3.zero;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
        
    }
    IEnumerator hitanim()
    {
        if (root.GetComponent<Animator>() != null)
        {
            int n = Random.Range(0, randomvalue.Length);
           
            root.GetComponent<Animator>().SetFloat("randomhit", randomvalue[n]);
            root.GetComponent<Animator>().SetBool("hit", true);

            yield return new WaitForSeconds(0.5f);
            if (root.GetComponent<Animator>() != null)
            {
                root.GetComponent<Animator>().SetBool("hit", false);
            }
        }

    }
}
