using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door1 : MonoBehaviour
{
    public Transform door;
 
    public bool canOpen = true;
    public AudioSource myAudioSource;
    public AudioClip opensound;
    private bool isopen = false;
    private GameObject player;
    weaponselector inventory;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<weaponselector>();


    }
    public void interact()
    {
        if (!isopen)
        {
            myAudioSource.PlayOneShot(opensound);
            door.GetComponent<Animation>().Play();
           
            Destroy(GetComponent<BoxCollider>());

            StartCoroutine(waitanddestroy());
            isopen = true;
        }
    }
    
    IEnumerator waitanddestroy()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
