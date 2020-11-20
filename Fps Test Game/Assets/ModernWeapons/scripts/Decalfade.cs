using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
public class Decalfade : MonoBehaviour
{
    public float maxsize = .2f;
    public float minsize = .1f;
    private DecalProjectorComponent mydecal;
    private float starttime;
    
    private float fadespeed = .1f;
    
    private Vector3 randomsize;
    private void Awake()
    {
        mydecal = GetComponent<DecalProjectorComponent>();
        
    }
    void Start()
    {
        transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-360f, 360f));
        float randomscale = Random.Range(minsize, maxsize);
        randomsize = new Vector3(randomscale, randomscale, randomscale);
        mydecal.size =randomsize;
        starttime = Time.time;
    }

    
    void Update()
    {
        float step = fadespeed * Time.deltaTime;
        
        if (    Time.time > (starttime +8f))
        {

           
            Destroy(gameObject);
           



        }


    }
    
}
