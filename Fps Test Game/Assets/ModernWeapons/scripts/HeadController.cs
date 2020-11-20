using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour {
    public LayerMask mask;
    public AudioSource footaudiosource;
    public AudioClip[] footnormal;
    public AudioClip[] footwood;
    public AudioClip[] footmetal;
    public AudioClip[] footdirt;
    public void playfootsound()
    {



        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, 1.5f, mask))
        {

            if (hit.transform.tag == "Untagged")
            {
                int n = Random.Range(1, footnormal.Length);
                footaudiosource.clip = footnormal[n];
                footaudiosource.pitch = Random.Range(0.8f, 1.2f);
                footaudiosource.Play();
                footnormal[n] = footnormal[0];
                footnormal[0] = footaudiosource.clip;
            }
           else if (hit.transform.tag == "dirt")
            {
                int n = Random.Range(1, footdirt.Length);
                footaudiosource.clip = footdirt[n];
                footaudiosource.pitch = Random.Range(0.8f, 1.2f);
                footaudiosource.Play();
                footdirt[n] = footdirt[0];
                footdirt[0] = footaudiosource.clip;
            }
            else if (hit.transform.tag == "wood")
            {
                int n = Random.Range(1, footwood.Length);
                footaudiosource.clip = footwood[n];
                footaudiosource.pitch = Random.Range(0.8f, 1.2f);
                footaudiosource.Play();
                footwood[n] = footwood[0];
                footwood[0] = footaudiosource.clip;
            }
            else if (hit.transform.tag == "metal")
            {
                int n = Random.Range(1, footmetal.Length);
                footaudiosource.clip = footmetal[n];
                footaudiosource.pitch = Random.Range(0.8f, 1.2f);
                footaudiosource.Play();
                footmetal[n] = footmetal[0];
                footmetal[0] = footaudiosource.clip;
            }


        }
        else
        {
            int n = Random.Range(1, footnormal.Length);
            footaudiosource.clip = footnormal[n];
            footaudiosource.pitch = Random.Range(0.8f, 1.2f);
            footaudiosource.Play();
            footnormal[n] = footnormal[0];
            footnormal[0] = footaudiosource.clip;
        }
    }
}
